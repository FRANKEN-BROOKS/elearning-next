-- ============================================================================
-- PAYMENT SERVICE DATABASE SCHEMA
-- ============================================================================

USE PaymentServiceDb;
GO

-- Payments Table
CREATE TABLE Payments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    TransactionId NVARCHAR(255) NULL, -- Omise charge ID
    PaymentMethod NVARCHAR(50) NOT NULL, -- CreditCard, DebitCard, PromptPay, TrueMoney, etc.
    PaymentProvider NVARCHAR(50) NOT NULL DEFAULT 'Omise',
    Amount DECIMAL(10, 2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL DEFAULT 'THB',
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Processing, Successful, Failed, Cancelled, Refunded
    Description NVARCHAR(500) NULL,
    FailureReason NVARCHAR(500) NULL,
    PaymentDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Orders Table
CREATE TABLE Orders (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    OrderType NVARCHAR(50) NOT NULL, -- CourseEnrollment, Subscription
    ReferenceId UNIQUEIDENTIFIER NOT NULL, -- CourseId or SubscriptionPlanId
    SubtotalThb DECIMAL(10, 2) NOT NULL,
    DiscountThb DECIMAL(10, 2) NOT NULL DEFAULT 0,
    TaxThb DECIMAL(10, 2) NOT NULL DEFAULT 0,
    TotalThb DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Confirmed, Cancelled, Refunded
    CouponCode NVARCHAR(50) NULL,
    Notes NVARCHAR(1000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- OrderItems Table
CREATE TABLE OrderItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OrderId UNIQUEIDENTIFIER NOT NULL,
    ItemType NVARCHAR(50) NOT NULL, -- Course, Subscription
    ItemId UNIQUEIDENTIFIER NOT NULL, -- CourseId or PlanId
    ItemName NVARCHAR(255) NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    UnitPriceThb DECIMAL(10, 2) NOT NULL,
    DiscountThb DECIMAL(10, 2) NOT NULL DEFAULT 0,
    TotalPriceThb DECIMAL(10, 2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
);
GO

-- PaymentMethods Table (Stored payment methods for users)
CREATE TABLE PaymentMethods (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    MethodType NVARCHAR(50) NOT NULL, -- CreditCard, DebitCard
    CardBrand NVARCHAR(50) NULL, -- Visa, Mastercard, etc.
    LastFourDigits NVARCHAR(4) NULL,
    ExpiryMonth INT NULL,
    ExpiryYear INT NULL,
    CardHolderName NVARCHAR(255) NULL,
    OmiseCardId NVARCHAR(255) NULL,
    IsDefault BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Refunds Table
CREATE TABLE Refunds (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PaymentId UNIQUEIDENTIFIER NOT NULL,
    RefundAmount DECIMAL(10, 2) NOT NULL,
    Reason NVARCHAR(500) NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected, Completed
    OmiseRefundId NVARCHAR(255) NULL,
    ProcessedBy UNIQUEIDENTIFIER NULL,
    RequestedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ProcessedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);
GO

-- PaymentWebhooks Table (Store webhook events from Omise)
CREATE TABLE PaymentWebhooks (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId NVARCHAR(255) NOT NULL UNIQUE,
    EventType NVARCHAR(100) NOT NULL,
    PaymentId UNIQUEIDENTIFIER NULL,
    PayloadJson NVARCHAR(MAX) NOT NULL,
    IsProcessed BIT NOT NULL DEFAULT 0,
    ProcessedAt DATETIME2 NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    RetryCount INT NOT NULL DEFAULT 0,
    ReceivedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);
GO

-- PaymentTransactionLogs Table
CREATE TABLE PaymentTransactionLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PaymentId UNIQUEIDENTIFIER NOT NULL,
    Action NVARCHAR(100) NOT NULL, -- CreateCharge, CaptureCharge, RefundCharge, etc.
    RequestData NVARCHAR(MAX) NULL,
    ResponseData NVARCHAR(MAX) NULL,
    StatusCode INT NULL,
    IsSuccess BIT NOT NULL DEFAULT 0,
    ErrorMessage NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);
GO

-- Invoices Table
CREATE TABLE Invoices (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OrderId UNIQUEIDENTIFIER NOT NULL,
    PaymentId UNIQUEIDENTIFIER NULL,
    InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
    IssueDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DueDate DATETIME2 NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, Sent, Paid, Overdue, Cancelled
    SubtotalThb DECIMAL(10, 2) NOT NULL,
    TaxThb DECIMAL(10, 2) NOT NULL DEFAULT 0,
    TotalThb DECIMAL(10, 2) NOT NULL,
    PdfUrl NVARCHAR(500) NULL,
    SentToEmail NVARCHAR(255) NULL,
    SentAt DATETIME2 NULL,
    PaidAt DATETIME2 NULL,
    Notes NVARCHAR(1000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);
GO

-- BillingAddresses Table
CREATE TABLE BillingAddresses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    FullName NVARCHAR(255) NOT NULL,
    CompanyName NVARCHAR(255) NULL,
    TaxId NVARCHAR(50) NULL,
    AddressLine1 NVARCHAR(255) NOT NULL,
    AddressLine2 NVARCHAR(255) NULL,
    City NVARCHAR(100) NOT NULL,
    Province NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL DEFAULT 'Thailand',
    PhoneNumber NVARCHAR(20) NULL,
    IsDefault BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Indexes for performance
CREATE INDEX IX_Payments_UserId ON Payments(UserId);
CREATE INDEX IX_Payments_OrderId ON Payments(OrderId);
CREATE INDEX IX_Payments_TransactionId ON Payments(TransactionId);
CREATE INDEX IX_Payments_Status ON Payments(Status);
CREATE INDEX IX_Payments_CreatedAt ON Payments(CreatedAt);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Orders_OrderNumber ON Orders(OrderNumber);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_PaymentMethods_UserId ON PaymentMethods(UserId);
CREATE INDEX IX_Refunds_PaymentId ON Refunds(PaymentId);
CREATE INDEX IX_Refunds_Status ON Refunds(Status);
CREATE INDEX IX_PaymentWebhooks_EventId ON PaymentWebhooks(EventId);
CREATE INDEX IX_PaymentWebhooks_IsProcessed ON PaymentWebhooks(IsProcessed);
CREATE INDEX IX_PaymentTransactionLogs_PaymentId ON PaymentTransactionLogs(PaymentId);
CREATE INDEX IX_Invoices_OrderId ON Invoices(OrderId);
CREATE INDEX IX_Invoices_InvoiceNumber ON Invoices(InvoiceNumber);
CREATE INDEX IX_BillingAddresses_UserId ON BillingAddresses(UserId);
GO

PRINT 'Payment Service schema created successfully!';
GO