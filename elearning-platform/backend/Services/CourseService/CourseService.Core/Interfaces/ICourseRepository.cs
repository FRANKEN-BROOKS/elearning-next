using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CourseService.Core.Entities;

namespace CourseService.Core.Interfaces
{
    /// <summary>
    /// Course repository interface
    /// </summary>
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(Guid id);
        Task<Course> GetByIdWithDetailsAsync(Guid id);
        Task<Course> GetBySlugAsync(string slug);
        Task<List<Course>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null, Guid? categoryId = null, string level = null);
        Task<int> GetTotalCountAsync(string searchTerm = null, Guid? categoryId = null, string level = null);
        Task<List<Course>> GetFeaturedCoursesAsync(int count);
        Task<List<Course>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<Course> CreateAsync(Course course);
        Task<Course> UpdateAsync(Course course);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }

    /// <summary>
    /// Category repository interface
    /// </summary>
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(Guid id);
        Task<List<Category>> GetAllAsync();
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(Guid id);
    }

    /// <summary>
    /// Course topic repository interface
    /// </summary>
    public interface ICourseTopicRepository
    {
        Task<CourseTopic> GetByIdAsync(Guid id);
        Task<CourseTopic> GetByIdWithLessonsAsync(Guid id);
        Task<List<CourseTopic>> GetByCourseIdAsync(Guid courseId);
        Task<CourseTopic> CreateAsync(CourseTopic topic);
        Task<CourseTopic> UpdateAsync(CourseTopic topic);
        Task DeleteAsync(Guid id);
    }

    /// <summary>
    /// Lesson repository interface
    /// </summary>
    public interface ILessonRepository
    {
        Task<Lesson> GetByIdAsync(Guid id);
        Task<Lesson> GetByIdWithResourcesAsync(Guid id);
        Task<List<Lesson>> GetByTopicIdAsync(Guid topicId);
        Task<Lesson> CreateAsync(Lesson lesson);
        Task<Lesson> UpdateAsync(Lesson lesson);
        Task DeleteAsync(Guid id);
    }

    /// <summary>
    /// Quiz repository interface
    /// </summary>
    public interface IQuizRepository
    {
        Task<Quiz> GetByIdAsync(Guid id);
        Task<Quiz> GetByIdWithQuestionsAsync(Guid id);
        Task<List<Quiz>> GetByCourseIdAsync(Guid courseId);
        Task<Quiz> CreateAsync(Quiz quiz);
        Task<Quiz> UpdateAsync(Quiz quiz);
        Task DeleteAsync(Guid id);
    }

    /// <summary>
    /// Question repository interface
    /// </summary>
    public interface IQuestionRepository
    {
        Task<Question> GetByIdAsync(Guid id);
        Task<Question> GetByIdWithOptionsAsync(Guid id);
        Task<List<Question>> GetByQuizIdAsync(Guid quizId);
        Task<Question> CreateAsync(Question question);
        Task<Question> UpdateAsync(Question question);
        Task DeleteAsync(Guid id);
    }

    /// <summary>
    /// Quiz attempt repository interface
    /// </summary>
    public interface IQuizAttemptRepository
    {
        Task<QuizAttempt> GetByIdAsync(Guid id);
        Task<QuizAttempt> GetByIdWithAnswersAsync(Guid id);
        Task<List<QuizAttempt>> GetByUserIdAsync(Guid userId);
        Task<List<QuizAttempt>> GetByQuizIdAsync(Guid quizId, Guid userId);
        Task<QuizAttempt> CreateAsync(QuizAttempt attempt);
        Task<QuizAttempt> UpdateAsync(QuizAttempt attempt);
    }

    /// <summary>
    /// User progress repository interface
    /// </summary>
    public interface IUserProgressRepository
    {
        Task<UserProgress> GetByUserAndCourseAsync(Guid userId, Guid courseId);
        Task<List<UserProgress>> GetByUserIdAsync(Guid userId);
        Task<UserProgress> CreateAsync(UserProgress progress);
        Task<UserProgress> UpdateAsync(UserProgress progress);
        Task UpdateProgressAsync(Guid userId, Guid courseId, Guid lessonId);
    }
}