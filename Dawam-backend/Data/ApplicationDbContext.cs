using Dawam_backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dawam_backend.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships and constraints
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.SubscriptionPlan)
                .WithMany()
                .HasForeignKey(u => u.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Job>()
                .HasOne(j => j.Category)
                .WithMany()
                .HasForeignKey(j => j.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Job>()
                .HasOne(j => j.PostedByUser)
                .WithMany()
                .HasForeignKey(j => j.PostedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications )
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Application>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.SubscriptionPlan)
                .WithMany()
                .HasForeignKey(p => p.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

