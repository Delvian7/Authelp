using System;
using System.ComponentModel.DataAnnotations;

namespace Authelp.Models
{
    public class Testimonial
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        

        
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
