using System.Security.Claims;
using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Authelp.Pages.Dashboard
{
    public class MessagesModel : PageModel
    {
        private readonly AppDbContext _db;
        public MessagesModel(AppDbContext db) => _db = db;

        
        public int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        
        public List<ConversationPreview> Conversations { get; set; } = new();

        public class ConversationPreview
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string LastMessage { get; set; } = string.Empty;
            public DateTime LastTime { get; set; }
            public int UnreadCount { get; set; }
        }

        public async Task OnGetAsync()
        {
           
            var msgs = await _db.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == CurrentUserId || m.ReceiverId == CurrentUserId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

           
            Conversations = msgs
                .GroupBy(m => m.SenderId == CurrentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => new ConversationPreview
                {
                    UserId = g.Key,
                    UserName = g.First().SenderId == g.Key
                        ? g.First().Sender?.Name ?? "Unknown"
                        : g.First().Receiver?.Name ?? "Unknown",
                    LastMessage = g.First().Text ?? string.Empty,
                    LastTime = g.First().SentAt,
                    UnreadCount = g.Count(m => m.ReceiverId == CurrentUserId && !m.IsRead)
                })
                .OrderByDescending(c => c.LastTime)
                .ToList();
        }
    }
}
