using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CourseService.Core.Entities;
using CourseService.Core.Interfaces;
using CourseService.Infrastructure.Data;

namespace CourseService.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly CourseDbContext _context;

        public CourseRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<Course> GetByIdAsync(Guid id)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Topics)
                    .ThenInclude(t => t.Lessons)
                        .ThenInclude(l => l.Resources)
                .Include(c => c.Quizzes)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(q => q.Options)
                .Include(c => c.Reviews)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course> GetBySlugAsync(string slug)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task<List<Course>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null, Guid? categoryId = null, string level = null)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Where(c => c.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(searchTerm) ||
                    c.Description.ToLower().Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                query = query.Where(c => c.Level == level);
            }

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string searchTerm = null, Guid? categoryId = null, string level = null)
        {
            var query = _context.Courses
                .Where(c => c.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(searchTerm) ||
                    c.Description.ToLower().Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(level))
            {
                query = query.Where(c => c.Level == level);
            }

            return await query.CountAsync();
        }

        public async Task<List<Course>> GetFeaturedCoursesAsync(int count)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Where(c => c.IsPublished && c.IsFeatured)
                .OrderByDescending(c => c.AverageRating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByInstructorAsync(Guid instructorId)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Where(c => c.InstructorId == instructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Course> CreateAsync(Course course)
        {
            // Generate slug from title
            course.Slug = GenerateSlug(course.Title);
            
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            course.UpdatedAt = DateTime.UtcNow;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task DeleteAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Courses.AnyAsync(c => c.Id == id);
        }

        private string GenerateSlug(string title)
        {
            var slug = title.ToLower()
                .Replace(" ", "-")
                .Replace("ä", "a")
                .Replace("ö", "o")
                .Replace("ü", "u")
                .Replace("ß", "ss");

            // Remove invalid characters
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Ensure uniqueness
            var baseSlug = slug;
            var counter = 1;
            while (_context.Courses.Any(c => c.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }

    public class CategoryRepository : ICategoryRepository
    {
        private readonly CourseDbContext _context;

        public CategoryRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class CourseTopicRepository : ICourseTopicRepository
    {
        private readonly CourseDbContext _context;

        public CourseTopicRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<CourseTopic> GetByIdAsync(Guid id)
        {
            return await _context.CourseTopics.FindAsync(id);
        }

        public async Task<CourseTopic> GetByIdWithLessonsAsync(Guid id)
        {
            return await _context.CourseTopics
                .Include(t => t.Lessons)
                    .ThenInclude(l => l.Resources)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<CourseTopic>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.CourseTopics
                .Where(t => t.CourseId == courseId && t.IsActive)
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();
        }

        public async Task<CourseTopic> CreateAsync(CourseTopic topic)
        {
            _context.CourseTopics.Add(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task<CourseTopic> UpdateAsync(CourseTopic topic)
        {
            topic.UpdatedAt = DateTime.UtcNow;
            _context.CourseTopics.Update(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task DeleteAsync(Guid id)
        {
            var topic = await _context.CourseTopics.FindAsync(id);
            if (topic != null)
            {
                _context.CourseTopics.Remove(topic);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class LessonRepository : ILessonRepository
    {
        private readonly CourseDbContext _context;

        public LessonRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<Lesson> GetByIdAsync(Guid id)
        {
            return await _context.Lessons.FindAsync(id);
        }

        public async Task<Lesson> GetByIdWithResourcesAsync(Guid id)
        {
            return await _context.Lessons
                .Include(l => l.Resources)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Lesson>> GetByTopicIdAsync(Guid topicId)
        {
            return await _context.Lessons
                .Where(l => l.TopicId == topicId && l.IsActive)
                .OrderBy(l => l.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Lesson> CreateAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<Lesson> UpdateAsync(Lesson lesson)
        {
            lesson.UpdatedAt = DateTime.UtcNow;
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task DeleteAsync(Guid id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class QuizRepository : IQuizRepository
    {
        private readonly CourseDbContext _context;

        public QuizRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<Quiz> GetByIdAsync(Guid id)
        {
            return await _context.Quizzes.FindAsync(id);
        }

        public async Task<Quiz> GetByIdWithQuestionsAsync(Guid id)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<List<Quiz>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Quizzes
                .Where(q => q.CourseId == courseId && q.IsActive)
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Quiz> CreateAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<Quiz> UpdateAsync(Quiz quiz)
        {
            quiz.UpdatedAt = DateTime.UtcNow;
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task DeleteAsync(Guid id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class QuestionRepository : IQuestionRepository
    {
        private readonly CourseDbContext _context;

        public QuestionRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<Question> GetByIdAsync(Guid id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public async Task<Question> GetByIdWithOptionsAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<List<Question>> GetByQuizIdAsync(Guid quizId)
        {
            return await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.QuizId == quizId && q.IsActive)
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Question> CreateAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateAsync(Question question)
        {
            question.UpdatedAt = DateTime.UtcNow;
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task DeleteAsync(Guid id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class QuizAttemptRepository : IQuizAttemptRepository
    {
        private readonly CourseDbContext _context;

        public QuizAttemptRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<QuizAttempt> GetByIdAsync(Guid id)
        {
            return await _context.QuizAttempts.FindAsync(id);
        }

        public async Task<QuizAttempt> GetByIdWithAnswersAsync(Guid id)
        {
            return await _context.QuizAttempts
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<QuizAttempt>> GetByUserIdAsync(Guid userId)
        {
            return await _context.QuizAttempts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<QuizAttempt>> GetByQuizIdAsync(Guid quizId, Guid userId)
        {
            return await _context.QuizAttempts
                .Where(a => a.QuizId == quizId && a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<QuizAttempt> CreateAsync(QuizAttempt attempt)
        {
            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<QuizAttempt> UpdateAsync(QuizAttempt attempt)
        {
            _context.QuizAttempts.Update(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }
    }

    public class UserProgressRepository : IUserProgressRepository
    {
        private readonly CourseDbContext _context;

        public UserProgressRepository(CourseDbContext context)
        {
            _context = context;
        }

        public async Task<UserProgress> GetByUserAndCourseAsync(Guid userId, Guid courseId)
        {
            return await _context.UserProgress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);
        }

        public async Task<List<UserProgress>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserProgress
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<UserProgress> CreateAsync(UserProgress progress)
        {
            _context.UserProgress.Add(progress);
            await _context.SaveChangesAsync();
            return progress;
        }

        public async Task<UserProgress> UpdateAsync(UserProgress progress)
        {
            progress.UpdatedAt = DateTime.UtcNow;
            _context.UserProgress.Update(progress);
            await _context.SaveChangesAsync();
            return progress;
        }

        public async Task UpdateProgressAsync(Guid userId, Guid courseId, Guid lessonId)
        {
            var progress = await GetByUserAndCourseAsync(userId, courseId);
            
            if (progress == null)
            {
                progress = new UserProgress
                {
                    UserId = userId,
                    CourseId = courseId,
                    LessonId = lessonId,
                    CompletionPercentage = 0
                };
                await CreateAsync(progress);
            }
            else
            {
                progress.LessonId = lessonId;
                progress.LastAccessedAt = DateTime.UtcNow;
                await UpdateAsync(progress);
            }
        }
    }
}