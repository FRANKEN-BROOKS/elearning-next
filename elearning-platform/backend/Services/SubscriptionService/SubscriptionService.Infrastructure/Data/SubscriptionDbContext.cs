using Microsoft.EntityFrameworkCore;
using SubscriptionService.Core.Entities;

namespace SubscriptionService.Infrastructure.Data
{
    public class SubscriptionDbContext : DbContext
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : base(options)
        {
        }

        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<EnrollmentHistory> EnrollmentHistories { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<CourseNotification> CourseNotifications { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PaymentStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PriceThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.CompletionPercentage).HasColumnType("decimal(5,2)");
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CourseId);
            });

            modelBuilder.Entity<EnrollmentHistory>(entity =>
            {
                entity.ToTable("EnrollmentHistory");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Enrollment)
                    .WithMany()
                    .HasForeignKey(e => e.EnrollmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.ToTable("Wishlists");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
            });

            modelBuilder.Entity<CourseNotification>(entity =>
            {
                entity.ToTable("CourseNotifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NotificationType).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.ToTable("Coupons");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DiscountType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(10,2)");
                entity.HasIndex(e => e.Code).IsUnique();
            });

            modelBuilder.Entity<UserCoupon>(entity =>
            {
                entity.ToTable("UserCoupons");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.Coupon)
                    .WithMany()
                    .HasForeignKey(e => e.CouponId);
                entity.HasOne(e => e.Enrollment)
                    .WithMany()
                    .HasForeignKey(e => e.EnrollmentId);
            });
        }
    }
}