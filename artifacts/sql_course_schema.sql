-- ============================================================================
-- COURSE SERVICE DATABASE SCHEMA
-- ============================================================================

USE CourseServiceDb;
GO

-- Categories Table
CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IconUrl NVARCHAR(500) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Courses Table
CREATE TABLE Courses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(255) NOT NULL UNIQUE,
    ShortDescription NVARCHAR(500) NULL,
    Description NVARCHAR(MAX) NULL,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    InstructorId UNIQUEIDENTIFIER NOT NULL,
    ThumbnailUrl NVARCHAR(500) NULL,
    PreviewVideoUrl NVARCHAR(500) NULL,
    Level NVARCHAR(20) NOT NULL DEFAULT 'Beginner', -- Beginner, Intermediate, Advanced
    Language NVARCHAR(50) NOT NULL DEFAULT 'Thai',
    PriceThb DECIMAL(10, 2) NOT NULL DEFAULT 0,
    DiscountPriceThb DECIMAL(10, 2) NULL,
    Duration INT NOT NULL DEFAULT 0, -- in minutes
    IsPublished BIT NOT NULL DEFAULT 0,
    PublishedAt DATETIME2 NULL,
    IsFeatured BIT NOT NULL DEFAULT 0,
    MetaTitle NVARCHAR(255) NULL,
    MetaDescription NVARCHAR(500) NULL,
    MetaKeywords NVARCHAR(500) NULL,
    EnrollmentCount INT NOT NULL DEFAULT 0,
    AverageRating DECIMAL(3, 2) NOT NULL DEFAULT 0,
    TotalRatings INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
GO

-- CourseTopics Table
CREATE TABLE CourseTopics (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(1000) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    Duration INT NOT NULL DEFAULT 0, -- in minutes
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
GO

-- Lessons Table
CREATE TABLE Lessons (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TopicId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Content NVARCHAR(MAX) NULL,
    VideoUrl NVARCHAR(500) NULL,
    Duration INT NOT NULL DEFAULT 0, -- in minutes
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsFree BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (TopicId) REFERENCES CourseTopics(Id) ON DELETE CASCADE
);
GO

-- LessonResources Table
CREATE TABLE LessonResources (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LessonId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(500) NULL,
    ResourceType NVARCHAR(50) NOT NULL, -- PDF, Video, Link, Document
    ResourceUrl NVARCHAR(500) NOT NULL,
    FileSize BIGINT NULL, -- in bytes
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (LessonId) REFERENCES Lessons(Id) ON DELETE CASCADE
);
GO

-- Quizzes Table
CREATE TABLE Quizzes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    TopicId UNIQUEIDENTIFIER NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(1000) NULL,
    PassingScore DECIMAL(5, 2) NOT NULL DEFAULT 70.00,
    TimeLimit INT NULL, -- in minutes
    MaxAttempts INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE,
    FOREIGN KEY (TopicId) REFERENCES CourseTopics(Id)
);
GO

-- Questions Table
CREATE TABLE Questions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    QuizId UNIQUEIDENTIFIER NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionType NVARCHAR(50) NOT NULL, -- MultipleChoice, TrueFalse, ShortAnswer
    Points INT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    Explanation NVARCHAR(MAX) NULL,
    ImageUrl NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (QuizId) REFERENCES Quizzes(Id) ON DELETE CASCADE
);
GO

-- QuestionOptions Table
CREATE TABLE QuestionOptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    OptionText NVARCHAR(MAX) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (QuestionId) REFERENCES Questions(Id) ON DELETE CASCADE
);
GO

-- UserProgress Table
CREATE TABLE UserProgress (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    LessonId UNIQUEIDENTIFIER NULL,
    CompletionPercentage DECIMAL(5, 2) NOT NULL DEFAULT 0,
    LastAccessedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsCompleted BIT NOT NULL DEFAULT 0,
    CompletedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE,
    FOREIGN KEY (LessonId) REFERENCES Lessons(Id)
);
GO

-- QuizAttempts Table
CREATE TABLE QuizAttempts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    QuizId UNIQUEIDENTIFIER NOT NULL,
    Score DECIMAL(5, 2) NOT NULL DEFAULT 0,
    TotalQuestions INT NOT NULL,
    CorrectAnswers INT NOT NULL,
    IsPassed BIT NOT NULL DEFAULT 0,
    StartedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2 NULL,
    TimeSpent INT NULL, -- in seconds
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (QuizId) REFERENCES Quizzes(Id)
);
GO

-- QuizAnswers Table
CREATE TABLE QuizAnswers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AttemptId UNIQUEIDENTIFIER NOT NULL,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    SelectedOptionId UNIQUEIDENTIFIER NULL,
    AnswerText NVARCHAR(MAX) NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    PointsAwarded INT NOT NULL DEFAULT 0,
    AnsweredAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (AttemptId) REFERENCES QuizAttempts(Id) ON DELETE CASCADE,
    FOREIGN KEY (QuestionId) REFERENCES Questions(Id),
    FOREIGN KEY (SelectedOptionId) REFERENCES QuestionOptions(Id)
);
GO

-- CourseReviews Table
CREATE TABLE CourseReviews (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CourseId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(1000) NULL,
    IsApproved BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
GO

-- Indexes for performance
CREATE INDEX IX_Courses_CategoryId ON Courses(CategoryId);
CREATE INDEX IX_Courses_InstructorId ON Courses(InstructorId);
CREATE INDEX IX_Courses_Slug ON Courses(Slug);
CREATE INDEX IX_Courses_IsPublished ON Courses(IsPublished);
CREATE INDEX IX_Courses_IsFeatured ON Courses(IsFeatured);
CREATE INDEX IX_CourseTopics_CourseId ON CourseTopics(CourseId);
CREATE INDEX IX_Lessons_TopicId ON Lessons(TopicId);
CREATE INDEX IX_LessonResources_LessonId ON LessonResources(LessonId);
CREATE INDEX IX_Quizzes_CourseId ON Quizzes(CourseId);
CREATE INDEX IX_Quizzes_TopicId ON Quizzes(TopicId);
CREATE INDEX IX_Questions_QuizId ON Questions(QuizId);
CREATE INDEX IX_QuestionOptions_QuestionId ON QuestionOptions(QuestionId);
CREATE INDEX IX_UserProgress_UserId ON UserProgress(UserId);
CREATE INDEX IX_UserProgress_CourseId ON UserProgress(CourseId);
CREATE INDEX IX_QuizAttempts_UserId ON QuizAttempts(UserId);
CREATE INDEX IX_QuizAttempts_QuizId ON QuizAttempts(QuizId);
CREATE INDEX IX_QuizAnswers_AttemptId ON QuizAnswers(AttemptId);
CREATE INDEX IX_CourseReviews_CourseId ON CourseReviews(CourseId);
CREATE INDEX IX_CourseReviews_UserId ON CourseReviews(UserId);
GO

PRINT 'Course Service schema created successfully!';
GO