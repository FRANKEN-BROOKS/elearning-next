-- ============================================================================
-- CERTIFICATE SERVICE DATABASE SCHEMA
-- ============================================================================

USE CertificateServiceDb;
GO

-- Certificates Table
CREATE TABLE Certificates (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CertificateNumber NVARCHAR(50) NOT NULL UNIQUE,
    UserId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    UserFullName NVARCHAR(255) NOT NULL,
    CourseTitle NVARCHAR(255) NOT NULL,
    InstructorName NVARCHAR(255) NOT NULL,
    CompletionDate DATETIME2 NOT NULL,
    IssueDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ExpiryDate DATETIME2 NULL,
    FinalScore DECIMAL(5, 2) NULL,
    Grade NVARCHAR(10) NULL, -- A, B, C, D, F or Pass/Fail
    TotalHours INT NOT NULL,
    VerificationCode NVARCHAR(100) NOT NULL UNIQUE,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Active, Revoked, Expired
    PdfUrl NVARCHAR(500) NULL,
    ThumbnailUrl NVARCHAR(500) NULL,
    IsPublic BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    RevokedAt DATETIME2 NULL,
    RevokedBy UNIQUEIDENTIFIER NULL,
    RevokedReason NVARCHAR(500) NULL
);
GO

-- CertificateTemplates Table
CREATE TABLE CertificateTemplates (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    TemplateType NVARCHAR(50) NOT NULL, -- Course, Achievement, Participation
    BackgroundImageUrl NVARCHAR(500) NULL,
    LogoUrl NVARCHAR(500) NULL,
    SignatureUrl NVARCHAR(500) NULL,
    Layout NVARCHAR(MAX) NULL, -- JSON configuration for layout
    IsDefault BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER NOT NULL
);
GO

-- CertificateVerifications Table (Track verification requests)
CREATE TABLE CertificateVerifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CertificateId UNIQUEIDENTIFIER NOT NULL,
    VerificationCode NVARCHAR(100) NOT NULL,
    VerifiedBy NVARCHAR(255) NULL, -- IP address or user identifier
    VerifiedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CertificateId) REFERENCES Certificates(Id)
);
GO

-- CertificateBadges Table
CREATE TABLE CertificateBadges (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    BadgeType NVARCHAR(50) NOT NULL, -- Achievement, Skill, Milestone
    IconUrl NVARCHAR(500) NULL,
    Criteria NVARCHAR(MAX) NULL, -- JSON describing how to earn the badge
    Points INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- UserBadges Table
CREATE TABLE UserBadges (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    BadgeId UNIQUEIDENTIFIER NOT NULL,
    EarnedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDisplayed BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (BadgeId) REFERENCES CertificateBadges(Id),
    CONSTRAINT UQ_UserBadge UNIQUE (UserId, BadgeId)
);
GO

-- CertificateSkills Table (Skills earned through courses)
CREATE TABLE CertificateSkills (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CertificateId UNIQUEIDENTIFIER NOT NULL,
    SkillName NVARCHAR(100) NOT NULL,
    SkillLevel NVARCHAR(50) NULL, -- Beginner, Intermediate, Advanced
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CertificateId) REFERENCES Certificates(Id) ON DELETE CASCADE
);
GO

-- CertificateShares Table (Track social sharing)
CREATE TABLE CertificateShares (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CertificateId UNIQUEIDENTIFIER NOT NULL,
    Platform NVARCHAR(50) NOT NULL, -- LinkedIn, Facebook, Twitter, Email
    SharedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CertificateId) REFERENCES Certificates(Id)
);
GO

-- CertificateAchievements Table (Milestones and special achievements)
CREATE TABLE CertificateAchievements (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    AchievementType NVARCHAR(50) NOT NULL, -- FirstCourse, TenCourses, FastLearner, etc.
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(500) NULL,
    IconUrl NVARCHAR(500) NULL,
    EarnedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Indexes for performance
CREATE INDEX IX_Certificates_UserId ON Certificates(UserId);
CREATE INDEX IX_Certificates_CourseId ON Certificates(CourseId);
CREATE INDEX IX_Certificates_CertificateNumber ON Certificates(CertificateNumber);
CREATE INDEX IX_Certificates_VerificationCode ON Certificates(VerificationCode);
CREATE INDEX IX_Certificates_Status ON Certificates(Status);
CREATE INDEX IX_Certificates_IssueDate ON Certificates(IssueDate);
CREATE INDEX IX_CertificateTemplates_IsDefault ON CertificateTemplates(IsDefault);
CREATE INDEX IX_CertificateTemplates_IsActive ON CertificateTemplates(IsActive);
CREATE INDEX IX_CertificateVerifications_CertificateId ON CertificateVerifications(CertificateId);
CREATE INDEX IX_CertificateVerifications_VerificationCode ON CertificateVerifications(VerificationCode);
CREATE INDEX IX_UserBadges_UserId ON UserBadges(UserId);
CREATE INDEX IX_UserBadges_BadgeId ON UserBadges(BadgeId);
CREATE INDEX IX_CertificateSkills_CertificateId ON CertificateSkills(CertificateId);
CREATE INDEX IX_CertificateShares_CertificateId ON CertificateShares(CertificateId);
CREATE INDEX IX_CertificateAchievements_UserId ON CertificateAchievements(UserId);
GO

PRINT 'Certificate Service schema created successfully!';
GO