using Microsoft.EntityFrameworkCore;
using CertificateService.Core.Entities;

namespace CertificateService.Infrastructure.Data
{
    public class CertificateDbContext : DbContext
    {
        public CertificateDbContext(DbContextOptions<CertificateDbContext> options) : base(options)
        {
        }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateTemplate> CertificateTemplates { get; set; }
        public DbSet<CertificateVerification> CertificateVerifications { get; set; }
        public DbSet<CertificateBadge> CertificateBadges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<CertificateSkill> CertificateSkills { get; set; }
        public DbSet<CertificateShare> CertificateShares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.ToTable("Certificates");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CertificateNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.VerificationCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FinalScore).HasColumnType("decimal(5,2)");
                entity.HasIndex(e => e.CertificateNumber).IsUnique();
                entity.HasIndex(e => e.VerificationCode).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CourseId);
            });

            modelBuilder.Entity<CertificateTemplate>(entity =>
            {
                entity.ToTable("CertificateTemplates");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<CertificateVerification>(entity =>
            {
                entity.ToTable("CertificateVerifications");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.VerificationCode);
                
                entity.HasOne(e => e.Certificate)
                    .WithMany()
                    .HasForeignKey(e => e.CertificateId);
            });

            modelBuilder.Entity<CertificateBadge>(entity =>
            {
                entity.ToTable("CertificateBadges");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<UserBadge>(entity =>
            {
                entity.ToTable("UserBadges");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.BadgeId }).IsUnique();
                
                entity.HasOne(e => e.Badge)
                    .WithMany()
                    .HasForeignKey(e => e.BadgeId);
            });

            modelBuilder.Entity<CertificateSkill>(entity =>
            {
                entity.ToTable("CertificateSkills");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Certificate)
                    .WithMany()
                    .HasForeignKey(e => e.CertificateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CertificateShare>(entity =>
            {
                entity.ToTable("CertificateShares");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Certificate)
                    .WithMany()
                    .HasForeignKey(e => e.CertificateId);
            });
        }
    }
}