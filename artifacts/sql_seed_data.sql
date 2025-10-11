-- ============================================================================
-- SEED INITIAL DATA
-- ============================================================================

USE UserServiceDb;
GO

-- Insert Roles
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @InstructorRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @StudentRoleId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Roles (Id, Name, Description, IsActive, CreatedAt, UpdatedAt)
VALUES 
    (@AdminRoleId, 'Admin', 'System Administrator with full access', 1, GETUTCDATE(), GETUTCDATE()),
    (@InstructorRoleId, 'Instructor', 'Course creator and instructor', 1, GETUTCDATE(), GETUTCDATE()),
    (@StudentRoleId, 'Student', 'Student with access to enrolled courses', 1, GETUTCDATE(), GETUTCDATE());
GO

-- Insert Permissions
DECLARE @PermUserManage UNIQUEIDENTIFIER = NEWID();
DECLARE @PermUserView UNIQUEIDENTIFIER = NEWID();
DECLARE @PermCourseCreate UNIQUEIDENTIFIER = NEWID();
DECLARE @PermCourseEdit UNIQUEIDENTIFIER = NEWID();
DECLARE @PermCourseDelete UNIQUEIDENTIFIER = NEWID();
DECLARE @PermCourseView UNIQUEIDENTIFIER = NEWID();
DECLARE @PermEnrollmentManage UNIQUEIDENTIFIER = NEWID();
DECLARE @PermPaymentView UNIQUEIDENTIFIER = NEWID();
DECLARE @PermCertificateIssue UNIQUEIDENTIFIER = NEWID();
DECLARE @PermReportView UNIQUEIDENTIFIER = NEWID();

INSERT INTO Permissions (Id, Name, Description, Category, CreatedAt)
VALUES 
    -- User Management
    (@PermUserManage, 'users.manage', 'Create, update, and delete users', 'User Management', GETUTCDATE()),
    (@PermUserView, 'users.view', 'View user information', 'User Management', GETUTCDATE()),
    
    -- Course Management
    (@PermCourseCreate, 'courses.create', 'Create new courses', 'Course Management', GETUTCDATE()),
    (@PermCourseEdit, 'courses.edit', 'Edit existing courses', 'Course Management', GETUTCDATE()),
    (@PermCourseDelete, 'courses.delete', 'Delete courses', 'Course Management', GETUTCDATE()),
    (@PermCourseView, 'courses.view', 'View courses', 'Course Management', GETUTCDATE()),
    
    -- Enrollment Management
    (@PermEnrollmentManage, 'enrollments.manage', 'Manage student enrollments', 'Enrollment Management', GETUTCDATE()),
    
    -- Payment Management
    (@PermPaymentView, 'payments.view', 'View payment information', 'Payment Management', GETUTCDATE()),
    
    -- Certificate Management
    (@PermCertificateIssue, 'certificates.issue', 'Issue certificates', 'Certificate Management', GETUTCDATE()),
    
    -- Reports
    (@PermReportView, 'reports.view', 'View system reports', 'Reports', GETUTCDATE());
GO

-- Get Role IDs
DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT Id FROM Roles WHERE Name = 'Admin');
DECLARE @InstructorRoleId UNIQUEIDENTIFIER = (SELECT Id FROM Roles WHERE Name = 'Instructor');
DECLARE @StudentRoleId UNIQUEIDENTIFIER = (SELECT Id FROM Roles WHERE Name = 'Student');

-- Get Permission IDs
DECLARE @PermUserManage UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'users.manage');
DECLARE @PermUserView UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'users.view');
DECLARE @PermCourseCreate UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'courses.create');
DECLARE @PermCourseEdit UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'courses.edit');
DECLARE @PermCourseDelete UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'courses.delete');
DECLARE @PermCourseView UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'courses.view');
DECLARE @PermEnrollmentManage UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'enrollments.manage');
DECLARE @PermPaymentView UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'payments.view');
DECLARE @PermCertificateIssue UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'certificates.issue');
DECLARE @PermReportView UNIQUEIDENTIFIER = (SELECT Id FROM Permissions WHERE Name = 'reports.view');

-- Assign Permissions to Admin Role (All permissions)
INSERT INTO RolePermissions (RoleId, PermissionId, GrantedAt)
VALUES 
    (@AdminRoleId, @PermUserManage, GETUTCDATE()),
    (@AdminRoleId, @PermUserView, GETUTCDATE()),
    (@AdminRoleId, @PermCourseCreate, GETUTCDATE()),
    (@AdminRoleId, @PermCourseEdit, GETUTCDATE()),
    (@AdminRoleId, @PermCourseDelete, GETUTCDATE()),
    (@AdminRoleId, @PermCourseView, GETUTCDATE()),
    (@AdminRoleId, @PermEnrollmentManage, GETUTCDATE()),
    (@AdminRoleId, @PermPaymentView, GETUTCDATE()),
    (@AdminRoleId, @PermCertificateIssue, GETUTCDATE()),
    (@AdminRoleId, @PermReportView, GETUTCDATE());

-- Assign Permissions to Instructor Role
INSERT INTO RolePermissions (RoleId, PermissionId, GrantedAt)
VALUES 
    (@InstructorRoleId, @PermCourseCreate, GETUTCDATE()),
    (@InstructorRoleId, @PermCourseEdit, GETUTCDATE()),
    (@InstructorRoleId, @PermCourseView, GETUTCDATE()),
    (@InstructorRoleId, @PermUserView, GETUTCDATE());

-- Assign Permissions to Student Role
INSERT INTO RolePermissions (RoleId, PermissionId, GrantedAt)
VALUES 
    (@StudentRoleId, @PermCourseView, GETUTCDATE());
GO

-- Create Default Admin User
-- Password: Admin@123 (BCrypt hashed)
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT Id FROM Roles WHERE Name = 'Admin');

INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, PhoneNumber, IsActive, IsEmailVerified, CreatedAt, UpdatedAt)
VALUES 
    (@AdminUserId, 'admin@elearning.com', '$2a$11$qvXHkQQjqRJXN6fQRZN6TuN4zC.wz3VT9WBm0Hy8bJWO8Yj3PXX7m', 'System', 'Administrator', '0812345678', 1, 1, GETUTCDATE(), GETUTCDATE());

-- Assign Admin Role to Admin User
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (@AdminUserId, @AdminRoleId, GETUTCDATE());

-- Create Admin Profile
INSERT INTO UserProfiles (Id, UserId, Bio, Country, CreatedAt, UpdatedAt)
VALUES (NEWID(), @AdminUserId, 'System Administrator', 'Thailand', GETUTCDATE(), GETUTCDATE());
GO

-- Create Demo Instructor User
-- Password: Instructor@123 (BCrypt hashed)
DECLARE @InstructorUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @InstructorRoleId UNIQUEIDENTIFIER = (SELECT Id FROM Roles WHERE Name = 'Instructor');

INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, PhoneNumber, IsActive, IsEmailVerified, CreatedAt, UpdatedAt)
VALUES 
    (@InstructorUserId, 'instructor@elearning.com', '$2a$11$qvXHkQQjqRJXN6fQRZN6TuN4zC.wz3VT9WBm0Hy8bJWO8Yj3PXX7m', 'Demo', 'Instructor', '0823456789', 1, 1, GETUTCDATE(), GETUTCDATE());

-- Assign Instructor Role
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (@InstructorUserId, @InstructorRoleId, GETUTCDATE());

-- Create Instructor Profile
INSERT INTO UserProfiles (Id, UserId, Bio, Country, Occupation, CreatedAt, UpdatedAt)
VALUES (NEWID(), @InstructorUserId, 'Experienced instructor with 10+ years of teaching', 'Thailand', 'Senior Developer', GETUTCDATE(), GETUTCDATE());
GO

-- Create Demo Student User
-- Password: Student@123 (BCrypt hashed)
DECLARE @StudentUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @StudentRoleId UNIQUEIDENTIFIER = (SELECT Id FROM Roles WHERE Name = 'Student');

INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, PhoneNumber, IsActive, IsEmailVerified, CreatedAt, UpdatedAt)
VALUES 
    (@StudentUserId, 'student@elearning.com', '$2a$11$qvXHkQQjqRJXN6fQRZN6TuN4zC.wz3VT9WBm0Hy8bJWO8Yj3PXX7m', 'Demo', 'Student', '0834567890', 1, 1, GETUTCDATE(), GETUTCDATE());

-- Assign Student Role
INSERT INTO UserRoles (UserId, RoleId, AssignedAt)
VALUES (@StudentUserId, @StudentRoleId, GETUTCDATE());

-- Create Student Profile
INSERT INTO UserProfiles (Id, UserId, Bio, Country, CreatedAt, UpdatedAt)
VALUES (NEWID(), @StudentUserId, 'Enthusiastic learner', 'Thailand', GETUTCDATE(), GETUTCDATE());
GO

PRINT 'Seed data inserted successfully!';
PRINT 'Default users created:';
PRINT '  Admin: admin@elearning.com / Admin@123';
PRINT '  Instructor: instructor@elearning.com / Instructor@123';
PRINT '  Student: student@elearning.com / Student@123';
GO

-- Seed Course Service Data
USE CourseServiceDb;
GO

-- Insert Categories
INSERT INTO Categories (Id, Name, Description, DisplayOrder, IsActive, CreatedAt, UpdatedAt)
VALUES 
    (NEWID(), 'Programming', 'Learn programming languages and software development', 1, 1, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'Design', 'Graphic design, UI/UX, and creative skills', 2, 1, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'Business', 'Business management, marketing, and entrepreneurship', 3, 1, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'Data Science', 'Data analysis, machine learning, and AI', 4, 1, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'Language', 'Foreign language learning', 5, 1, GETUTCDATE(), GETUTCDATE());
GO

PRINT 'Course categories created successfully!';
GO