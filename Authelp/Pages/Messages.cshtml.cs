using System.Security.Claims;
using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Authelp.Pages
{
    public class MessagesModel : PageModel
    {
        private readonly AppDbContext _db;
        public MessagesModel(AppDbContext db) => _db = db;

        
        [BindProperty(SupportsGet = true)]
        public int? ChatWithId { get; set; }

        
        [BindProperty]
        public string MessageText { get; set; } = string.Empty;

        
        public List<Message> Conversation { get; set; } = new();

        
        public List<User> UserList { get; set; } = new();

        public int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

       
        public async Task OnGetAsync()
        {
            
            UserList = await _db.Users
                .Where(u => u.Id != CurrentUserId)
                .ToListAsync();

            if (ChatWithId.HasValue)
            {
               
                Conversation = await _db.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .Where(m =>
                        (m.SenderId == CurrentUserId && m.ReceiverId == ChatWithId.Value) ||
                        (m.SenderId == ChatWithId.Value && m.ReceiverId == CurrentUserId)
                    )
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

             
                var unread = Conversation
                    .Where(m => m.ReceiverId == CurrentUserId && !m.IsRead)
                    .ToList();

                if (unread.Any())
                {
                    foreach (var msg in unread)
                        msg.IsRead = true;

                    await _db.SaveChangesAsync();
                }
            }
        }

       
        public async Task<IActionResult> OnPostSendAsync()
        {
            if (!ChatWithId.HasValue || string.IsNullOrWhiteSpace(MessageText))
                return RedirectToPage(new { ChatWithId });

            var msg = new Message
            {
                SenderId = CurrentUserId,
                ReceiverId = ChatWithId.Value,
                Text = MessageText,
                SentAt = DateTime.UtcNow,
                IsRead = false 
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            return RedirectToPage(new { ChatWithId });
        }
    }
}
