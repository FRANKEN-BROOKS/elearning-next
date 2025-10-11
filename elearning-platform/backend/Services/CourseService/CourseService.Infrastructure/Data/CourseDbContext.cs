using Microsoft.EntityFrameworkCore;
using CourseService.Core.Entities;

namespace CourseService.Infrastructure.Data
{
    /// <summary>
    /// Course service database context
    /// </summary>
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTopic> CourseTopics { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonResource> LessonResources { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<UserProgress> UserProgress { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Course configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Level).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Language).HasMaxLength(50);
                entity.Property(e => e.PriceThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.DiscountPriceThb).HasColumnType("decimal(10,2)");
                entity.Property(e => e.AverageRating).HasColumnType("decimal(3,2)");
                entity.HasIndex(e => e.Slug).IsUnique();

                entity.HasOne(e => e.Category)
                    .WithMany(e => e.Courses)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Topics)
                    .WithOne(e => e.Course)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CourseTopic configuration
            modelBuilder.Entity<CourseTopic>(entity =>
            {
                entity.ToTable("CourseTopics");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);

                entity.HasMany(e => e.Lessons)
                    .WithOne(e => e.Topic)
                    .HasForeignKey(e => e.TopicId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Lesson configuration
            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("Lessons");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.VideoUrl).HasMaxLength(500);

                entity.HasMany(e => e.Resources)
                    .WithOne(e => e.Lesson)
                    .HasForeignKey(e => e.LessonId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // LessonResource configuration
            modelBuilder.Entity<LessonResource>(entity =>
            {
                entity.ToTable("LessonResources");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ResourceType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ResourceUrl).IsRequired().HasMaxLength(500);
            });

            // Quiz configuration
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.ToTable("Quizzes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PassingScore).HasColumnType("decimal(5,2)");

                entity.HasOne(e => e.Course)
                    .WithMany(e => e.Quizzes)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Topic)
                    .WithMany(e => e.Quizzes)
                    .HasForeignKey(e => e.TopicId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.Questions)
                    .WithOne(e => e.Quiz)
                    .HasForeignKey(e => e.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Question configuration
            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Questions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuestionType).IsRequired().HasMaxLength(50);

                entity.HasMany(e => e.Options)
                    .WithOne(e => e.Question)
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // QuestionOption configuration
            modelBuilder.Entity<QuestionOption>(entity =>
            {
                entity.ToTable("QuestionOptions");
                entity.HasKey(e => e.Id);
            });

            // UserProgress configuration
            modelBuilder.Entity<UserProgress>(entity =>
            {
                entity.ToTable("UserProgress");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompletionPercentage).HasColumnType("decimal(5,2)");

                entity.HasOne(e => e.Course)
                    .WithMany()
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Lesson)
                    .WithMany()
                    .HasForeignKey(e => e.LessonId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // QuizAttempt configuration
            modelBuilder.Entity<QuizAttempt>(entity =>
            {
                entity.ToTable("QuizAttempts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Score).HasColumnType("decimal(5,2)");

                entity.HasOne(e => e.Quiz)
                    .WithMany()
                    .HasForeignKey(e => e.QuizId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Answers)
                    .WithOne(e => e.Attempt)
                    .HasForeignKey(e => e.AttemptId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // QuizAnswer configuration
            modelBuilder.Entity<QuizAnswer>(entity =>
            {
                entity.ToTable("QuizAnswers");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Question)
                    .WithMany()
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SelectedOption)
                    .WithMany()
                    .HasForeignKey(e => e.SelectedOptionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CourseReview configuration
            modelBuilder.Entity<CourseReview>(entity =>
            {
                entity.ToTable("CourseReviews");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Course)
                    .WithMany(e => e.Reviews)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}