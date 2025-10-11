using Microsoft.EntityFrameworkCore;
using PaymentService.Core.Entities;

namespace PaymentService.Infrastructure.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<PaymentWebhook> PaymentWebhooks { get; set; }
        public DbSet<PaymentTransactionLog> PaymentTransactionLogs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("THB");
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.HasIndex(e => e.TransactionId);
                entity.HasIndex(e => e.UserId);

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SubtotalThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.DiscountThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TaxThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalThb).HasColumnType("decimal(10,2)");
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.UserId);

                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPriceThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.DiscountThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalPriceThb).HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethods");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<Refund>(entity =>
            {
                entity.ToTable("Refunds");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RefundAmount).HasColumnType("decimal(10,2)");
                
                entity.HasOne(e => e.Payment)
                    .WithMany()
                    .HasForeignKey(e => e.PaymentId);
            });

            modelBuilder.Entity<PaymentWebhook>(entity =>
            {
                entity.ToTable("PaymentWebhooks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventId).IsRequired();
                entity.HasIndex(e => e.EventId).IsUnique();
                
                entity.HasOne(e => e.Payment)
                    .WithMany()
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<PaymentTransactionLog>(entity =>
            {
                entity.ToTable("PaymentTransactionLogs");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Payment)
                    .WithMany()
                    .HasForeignKey(e => e.PaymentId);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SubtotalThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TaxThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalThb).HasColumnType("decimal(10,2)");
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId);
                
                entity.HasOne(e => e.Payment)
                    .WithMany()
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}