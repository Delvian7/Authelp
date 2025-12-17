using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Authelp.Pages.Testimonials
{
    [ValidateAntiForgeryToken]  
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<TestimonialDisplay> Testimonials { get; set; } = new();

        public class TestimonialDisplay
        {
            public int Id { get; set; } 
            public string UserName { get; set; } = "Anonymous";
            public string ProfileImage { get; set; } = "/images/default-user.png";
            public string Text { get; set; } = "";
            public string CreatedAt { get; set; } = "";
            public int UserId { get; set; } 
        }

        public async Task OnGetAsync()
        {
            var testimonials = await _context.Testimonials
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            Testimonials = testimonials.Select(t => new TestimonialDisplay
            {
                Id = t.Id,
                UserId = t.UserId,
                UserName = t.User?.Name ?? "Anonymous",
                ProfileImage = !string.IsNullOrEmpty(t.User?.ProfileImage) ? t.User.ProfileImage : "/images/default-user.png",
                Text = t.Text,
                CreatedAt = t.CreatedAt.ToString("g")
            }).ToList();
        }

        public class TestimonialInput
        {
            public string Text { get; set; } = "";
        }

        
        public async Task<JsonResult> OnPostAsync([FromBody] TestimonialInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Text))
                return new JsonResult(new { error = "Empty text" }) { StatusCode = 400 };

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return new JsonResult(new { error = "User not logged in or invalid ID." }) { StatusCode = 400 };

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new JsonResult(new { error = "User not found." }) { StatusCode = 400 };

            var testimonial = new Testimonial
            {
                UserId = user.Id,
                Text = input.Text.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();

            var result = new TestimonialDisplay
            {
                Id = testimonial.Id,
                UserId = user.Id,
                UserName = user.Name,
                ProfileImage = !string.IsNullOrEmpty(user.ProfileImage) ? user.ProfileImage : "/images/default-user.png",
                Text = testimonial.Text,
                CreatedAt = testimonial.CreatedAt.ToString("g")
            };

            return new JsonResult(result);
        }

        
        public async Task<JsonResult> OnPostDeleteAsync([FromBody] int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
                return new JsonResult(new { error = "Testimonial not found." }) { StatusCode = 404 };

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return new JsonResult(new { error = "User not logged in or invalid ID." }) { StatusCode = 400 };

            if (testimonial.UserId != userId)
                return new JsonResult(new { error = "You can only delete your own testimonial." }) { StatusCode = 403 };

            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        
        public class TestimonialUpdateInput
        {
            public int Id { get; set; }
            public string Text { get; set; } = "";
        }

        public async Task<JsonResult> OnPostUpdateAsync([FromBody] TestimonialUpdateInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Text))
                return new JsonResult(new { error = "Text cannot be empty." }) { StatusCode = 400 };

            var testimonial = await _context.Testimonials.FindAsync(input.Id);
            if (testimonial == null)
                return new JsonResult(new { error = "Testimonial not found." }) { StatusCode = 404 };

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return new JsonResult(new { error = "User not logged in or invalid ID." }) { StatusCode = 400 };

            if (testimonial.UserId != userId)
                return new JsonResult(new { error = "You can only update your own testimonial." }) { StatusCode = 403 };

            testimonial.Text = input.Text.Trim();
            _context.Testimonials.Update(testimonial);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, text = testimonial.Text });
        }
    }
}
