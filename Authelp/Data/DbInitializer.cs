using Authelp.Models;

namespace Authelp.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Users.Any()) return;

            
            var parent = new User
            {
                Name = "Jane Parent",
                Email = "parent@example.com",
                Phone = "555-0101",
                Role = "parent",
                PasswordPlain = "password",
                Contact = "phone"
            };

            
            var helper = new User
            {
                Name = "John Helper",
                Email = "helper@example.com",
                Phone = "555-0202",
                Role = "helper",
                PasswordPlain = "password",
                Contact = "email"
            };

            db.Users.AddRange(parent, helper);
            db.SaveChanges(); 

            
            db.Children.Add(new Child
            {
                ParentId = parent.Id,
                ChildName = "Sam",
                ChildCondition = "Autism"
            });
            db.SaveChanges();

            
            db.Messages.Add(new Message
            {
                SenderId = helper.Id,
                ReceiverId = parent.Id,
                Text = "Hello, I can help with care.",
                SentAt = DateTime.UtcNow.AddMinutes(-30)
            });

            
            db.LoginLogs.Add(new LoginLog
            {
                UserId = parent.Id,
                LoginTime = DateTime.UtcNow.AddDays(-1)
            });

            db.SaveChanges();
        }
    }
}
