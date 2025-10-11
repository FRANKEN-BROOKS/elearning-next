-- ============================================================================
-- E-LEARNING PLATFORM - CREATE DATABASES
-- ============================================================================

USE master;
GO

-- Drop existing databases if they exist
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'UserServiceDb')
BEGIN
    ALTER DATABASE UserServiceDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE UserServiceDb;
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'CourseServiceDb')
BEGIN
    ALTER DATABASE CourseServiceDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CourseServiceDb;
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'SubscriptionServiceDb')
BEGIN
    ALTER DATABASE SubscriptionServiceDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SubscriptionServiceDb;
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PaymentServiceDb')
BEGIN
    ALTER DATABASE PaymentServiceDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PaymentServiceDb;
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'CertificateServiceDb')
BEGIN
    ALTER DATABASE CertificateServiceDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CertificateServiceDb;
END
GO

-- Create databases
CREATE DATABASE UserServiceDb;
GO

CREATE DATABASE CourseServiceDb;
GO

CREATE DATABASE SubscriptionServiceDb;
GO

CREATE DATABASE PaymentServiceDb;
GO

CREATE DATABASE CertificateServiceDb;
GO

PRINT 'All databases created successfully!';
GO