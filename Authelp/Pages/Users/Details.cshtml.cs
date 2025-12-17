using System.Security.Claims;
using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Authelp.Pages.Users
{
    [Authorize] 
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _db;
        public DetailsModel(AppDbContext db) => _db = db;

        public User? CurrentUser { get; set; }

        [BindProperty]
        public string MessageText { get; set; } = string.Empty;

        public List<Message> Conversation { get; set; } = new();
        public List<Message> UnreadMessages { get; set; } = new();

        public string? SuccessMessage { get; set; }

        
        public int LoggedInUserId
        {
            get
            {
                var id = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(id, out var num) ? num : 0;
            }
        }

        
        public async Task<IActionResult> OnGetAsync(int id)
        {
            
            CurrentUser = await _db.Users
                .Include(u => u.Children)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (CurrentUser == null) return NotFound();

            
            Conversation = await _db.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m =>
                    (m.SenderId == LoggedInUserId && m.ReceiverId == id) ||
                    (m.SenderId == id && m.ReceiverId == LoggedInUserId)
                )
                .OrderBy(m => m.SentAt)
                .ToListAsync();

           
            var newMessages = await _db.Messages
                .Where(m => m.SenderId == id && m.ReceiverId == LoggedInUserId && !m.IsRead)
                .ToListAsync();

            foreach (var m in newMessages) m.IsRead = true;
            await _db.SaveChangesAsync();

            
            UnreadMessages = await _db.Messages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == LoggedInUserId && !m.IsRead)
                .OrderByDescending(m => m.SentAt)
                .Take(5)
                .ToListAsync();

            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync(int id)
        {
            CurrentUser = await _db.Users
                .Include(u => u.Children)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (CurrentUser == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(MessageText))
            {
                var msg = new Message
                {
                    SenderId = LoggedInUserId,
                    ReceiverId = CurrentUser.Id,
                    Text = MessageText,
                    SentAt = DateTime.UtcNow
                };

                _db.Messages.Add(msg);
                await _db.SaveChangesAsync();

                SuccessMessage = "Message sent!";
                MessageText = string.Empty;
            }

            return await OnGetAsync(id);
        }
    }
}
