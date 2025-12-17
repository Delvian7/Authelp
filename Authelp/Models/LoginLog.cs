using System;

namespace Authelp.Models
{
    public class LoginLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; } 

        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public DateTime? LogoutTime { get; set; }
    }
}
