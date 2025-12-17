using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Authelp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Role { get; set; } = null!;  

        [Required]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string? Phone { get; set; }
        public string? PasswordPlain { get; set; }
        public string? Contact { get; set; }
        public string? ProfileImage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Child> Children { get; set; } = new List<Child>();

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        
    }
}
