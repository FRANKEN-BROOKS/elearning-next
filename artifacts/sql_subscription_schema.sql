-- ============================================================================
-- SUBSCRIPTION SERVICE DATABASE SCHEMA
-- ============================================================================

USE SubscriptionServiceDb;
GO

-- Enrollments Table
CREATE TABLE Enrollments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    EnrollmentDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ExpiryDate DATETIME2 NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Active, Expired, Cancelled, Suspended
    PaymentStatus NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Completed, Failed, Refunded
    PriceThb DECIMAL(10, 2) NOT NULL,
    PaymentMethod NVARCHAR(50) NULL,
    TransactionId NVARCHAR(255) NULL,
    LastAccessedAt DATETIME2 NULL,
    CompletionPercentage DECIMAL(5, 2) NOT NULL DEFAULT 0,
    IsCompleted BIT NOT NULL DEFAULT 0,
    CompletedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CancelledAt DATETIME2 NULL,
    CancelledBy UNIQUEIDENTIFIER NULL,
    CancellationReason NVARCHAR(500) NULL
);
GO

-- EnrollmentHistory Table (Track enrollment status changes)
CREATE TABLE EnrollmentHistory (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EnrollmentId UNIQUEIDENTIFIER NOT NULL,
    OldStatus NVARCHAR(50) NOT NULL,
    NewStatus NVARCHAR(50) NOT NULL,
    ChangedBy UNIQUEIDENTIFIER NULL,
    Reason NVARCHAR(500) NULL,
    ChangedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(Id) ON DELETE CASCADE
);
GO

-- Wishlists Table
CREATE TABLE Wishlists (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    AddedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Notes NVARCHAR(500) NULL,
    CONSTRAINT UQ_Wishlist_UserCourse UNIQUE (UserId, CourseId)
);
GO

-- CourseNotifications Table
CREATE TABLE CourseNotifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    CourseId UNIQUEIDENTIFIER NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL, -- NewLesson, PriceChange, CourseUpdate
    Title NVARCHAR(255) NOT NULL,
    Message NVARCHAR(1000) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    ReadAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- SubscriptionPlans Table (for future platform-wide subscriptions)
CREATE TABLE SubscriptionPlans (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    PriceThb DECIMAL(10, 2) NOT NULL,
    BillingCycle NVARCHAR(50) NOT NULL, -- Monthly, Quarterly, Yearly
    Features NVARCHAR(MAX) NULL, -- JSON array of features
    MaxCourses INT NULL, -- NULL for unlimited
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- UserSubscriptions Table (for platform-wide subscriptions)
CREATE TABLE UserSubscriptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    PlanId UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Active, Cancelled, Expired, Suspended
    StartDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    EndDate DATETIME2 NOT NULL,
    NextBillingDate DATETIME2 NULL,
    AutoRenew BIT NOT NULL DEFAULT 1,
    PaymentMethod NVARCHAR(50) NULL,
    TransactionId NVARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CancelledAt DATETIME2 NULL,
    CancellationReason NVARCHAR(500) NULL,
    FOREIGN KEY (PlanId) REFERENCES SubscriptionPlans(Id)
);
GO

-- EnrollmentCoupons Table
CREATE TABLE EnrollmentCoupons (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EnrollmentId UNIQUEIDENTIFIER NOT NULL,
    CouponCode NVARCHAR(50) NOT NULL,
    DiscountType NVARCHAR(20) NOT NULL, -- Percentage, FixedAmount
    DiscountValue DECIMAL(10, 2) NOT NULL,
    AppliedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(Id) ON DELETE CASCADE
);
GO

-- Coupons Table
CREATE TABLE Coupons (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    DiscountType NVARCHAR(20) NOT NULL, -- Percentage, FixedAmount
    DiscountValue DECIMAL(10, 2) NOT NULL,
    MinimumPurchase DECIMAL(10, 2) NULL,
    MaximumDiscount DECIMAL(10, 2) NULL,
    UsageLimit INT NULL, -- NULL for unlimited
    UsageCount INT NOT NULL DEFAULT 0,
    ValidFrom DATETIME2 NOT NULL,
    ValidTo DATETIME2 NOT NULL,
    ApplicableCourseIds NVARCHAR(MAX) NULL, -- JSON array, NULL for all courses
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER NOT NULL
);
GO

-- UserCoupons Table (Track user-specific coupon usage)
CREATE TABLE UserCoupons (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    CouponId UNIQUEIDENTIFIER NOT NULL,
    UsedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    EnrollmentId UNIQUEIDENTIFIER NULL,
    DiscountAmount DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (CouponId) REFERENCES Coupons(Id),
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(Id)
);
GO

-- Indexes for performance
CREATE INDEX IX_Enrollments_UserId ON Enrollments(UserId);
CREATE INDEX IX_Enrollments_CourseId ON Enrollments(CourseId);
CREATE INDEX IX_Enrollments_Status ON Enrollments(Status);
CREATE INDEX IX_Enrollments_EnrollmentDate ON Enrollments(EnrollmentDate);
CREATE INDEX IX_EnrollmentHistory_EnrollmentId ON EnrollmentHistory(EnrollmentId);
CREATE INDEX IX_Wishlists_UserId ON Wishlists(UserId);
CREATE INDEX IX_Wishlists_CourseId ON Wishlists(CourseId);
CREATE INDEX IX_CourseNotifications_UserId ON CourseNotifications(UserId);
CREATE INDEX IX_CourseNotifications_IsRead ON CourseNotifications(IsRead);
CREATE INDEX IX_UserSubscriptions_UserId ON UserSubscriptions(UserId);
CREATE INDEX IX_UserSubscriptions_Status ON UserSubscriptions(Status);
CREATE INDEX IX_EnrollmentCoupons_EnrollmentId ON EnrollmentCoupons(EnrollmentId);
CREATE INDEX IX_Coupons_Code ON Coupons(Code);
CREATE INDEX IX_Coupons_ValidFrom ON Coupons(ValidFrom);
CREATE INDEX IX_Coupons_ValidTo ON Coupons(ValidTo);
CREATE INDEX IX_UserCoupons_UserId ON UserCoupons(UserId);
CREATE INDEX IX_UserCoupons_CouponId ON UserCoupons(CouponId);
GO

PRINT 'Subscription Service schema created successfully!';
GO