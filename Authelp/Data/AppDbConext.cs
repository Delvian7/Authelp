using Microsoft.EntityFrameworkCore;
using Authelp.Models;

namespace Authelp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Child> Children { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<LoginLog> LoginLogs { get; set; } = null!;
        public DbSet<Testimonial> Testimonials { get; set;} =null!;
        




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Children)
                .WithOne(c => c.Parent)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
