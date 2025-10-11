# E-Learning Platform MVP - Backend

A comprehensive microservices-based e-learning platform built with .NET 8, inspired by Udemy.

## 📋 Project Overview

### **Features**
- **User Management**: Registration, authentication (JWT), role-based access control
- **Course Management**: Create, update, and publish courses with topics, lessons, and quizzes
- **Subscription System**: Course enrollment and subscription management
- **Payment Integration**: Omise payment gateway integration for Thai Baht transactions
- **Certificate Generation**: Automatic PDF certificate generation upon course completion
- **Admin Dashboard**: User permission management, course approval, analytics

### **Technology Stack**
- **Backend**: .NET 8 (C#), ASP.NET Core Web API
- **Database**: SQL Server
- **Message Queue**: RabbitMQ
- **Caching**: Redis
- **Container**: Podman
- **API Gateway**: Ocelot
- **Authentication**: JWT Bearer Tokens
- **PDF Generation**: QuestPDF
- **Payment**: Omise API

## 🏗️ Architecture

```
┌─────────────────┐
│  API Gateway    │
│   (Port 5000)   │
└────────┬────────┘
         │
    ┌────┴────────────────────────────┐
    │                                 │
┌───▼──────┐  ┌──────────┐  ┌───────▼────┐
│  User    │  │  Course  │  │Subscription│
│ Service  │  │ Service  │  │  Service   │
│(Port 5001)  │(Port 5002)  │ (Port 5003)│
└──────────┘  └──────────┘  └────────────┘
    │              │              │
┌───▼────┐    ┌───▼─────┐   ┌───▼────────┐
│Payment │    │Certificate   │ Message    │
│Service │    │  Service │   │  Queue     │
│(5004)  │    │  (5005)  │   │(RabbitMQ)  │
└────────┘    └──────────┘   └────────────┘
    │              │              │
    └──────────┬───┴──────────────┘
               │
        ┌──────▼──────┐
        │  SQL Server │
        │  (Port 1433)│
        └─────────────┘
```

## 📅 Development Timeline (5 Weeks)

### **Week 1: Infrastructure & Shared Components (Days 1-5)**
- **Day 1**: Project setup, Docker/Podman configuration, Database design
  - [x] Create project structure
  - [x] Setup docker-compose.yml
  - [x] Setup podman-setup.sh
  - [x] Create all SQL schemas

- **Day 2**: Shared libraries implementation
  - [x] Shared.Common (BaseEntity, DTOs, Exceptions)
  - [x] Shared.MessageQueue (RabbitMQ service, Events)

- **Day 3**: Database initialization
  - [x] Run SQL scripts
  - [x] Seed initial data (roles, permissions, admin user)
  - [x] Test database connections

- **Day 4**: API Gateway setup
  - [x] Ocelot configuration
  - [x] Route configuration
  - [x] CORS setup

- **Day 5**: Test infrastructure connectivity
  - [ ] Verify all services can connect to database
  - [ ] Verify RabbitMQ connectivity
  - [ ] Test API Gateway routing

### **Week 2: User Service (Days 6-10)**
- **Day 6-7**: User Service Core & Infrastructure
  - [x] Entities (User, Role, Permission, RefreshToken)
  - [x] DTOs
  - [x] Repository interfaces
  - [x] Repository implementations
  - [x] DbContext

- **Day 8-9**: Authentication & Authorization
  - [x] JWT Token Service
  - [x] Auth Service (Register, Login, Refresh Token)
  - [x] Password hashing (BCrypt)
  - [x] Role-based authorization

- **Day 10**: User Service API
  - [x] AuthController
  - [x] UsersController
  - [x] Program.cs configuration
  - [x] Swagger setup
  - [ ] Integration testing

### **Week 3: Course Service (Days 11-15)**
- **Day 11-12**: Course Service Core
  - [x] Entities (Course, Category, Topic, Lesson, Quiz, Question)
  - [x] DTOs
  - [x] Repository interfaces
  - [x] DbContext

- **Day 13-14**: Course Service Infrastructure
  - [ ] Repository implementations
  - [ ] Business logic services
  - [ ] SEO-friendly slug generation
  - [ ] Course search and filtering

- **Day 15**: Course Service API
  - [ ] CoursesController
  - [ ] CategoriesController
  - [ ] TopicsController
  - [ ] LessonsController
  - [ ] QuizzesController
  - [ ] Program.cs configuration

### **Week 4: Subscription & Payment Services (Days 16-20)**
- **Day 16-17**: Subscription Service
  - [ ] Core entities (Enrollment, Coupon, Wishlist)
  - [ ] DTOs
  - [ ] Repository implementations
  - [ ] Enrollment logic
  - [ ] Coupon validation
  - [ ] API controllers

- **Day 18-19**: Payment Service with Omise
  - [ ] Core entities (Payment, Order, Invoice)
  - [ ] DTOs
  - [ ] Omise SDK integration
  - [ ] Payment processing logic
  - [ ] Webhook handling
  - [ ] API controllers

- **Day 20**: Message Queue Integration
  - [ ] Event publishers
  - [ ] Event subscribers
  - [ ] Cross-service communication testing

### **Week 5: Certificate Service & Testing (Days 21-25)**
- **Day 21-22**: Certificate Service
  - [ ] Core entities (Certificate, Template, Badge)
  - [ ] DTOs
  - [ ] QuestPDF integration
  - [ ] Certificate generation logic
  - [ ] Verification system
  - [ ] API controllers

- **Day 23-24**: Integration Testing
  - [ ] End-to-end user registration flow
  - [ ] Course creation and enrollment flow
  - [ ] Payment processing flow
  - [ ] Certificate generation flow
  - [ ] API Gateway testing

- **Day 25**: Performance & Security
  - [ ] Load testing
  - [ ] Security audit
  - [ ] Performance optimization
  - [ ] Documentation

## 🚀 Getting Started

### **Prerequisites**
```bash
# Required software
- .NET 8 SDK
- SQL Server 2022
- Node.js 18+ (for frontend)
- Podman or Docker
- Git
```

### **Installation Steps**

#### **1. Clone Repository**
```bash
git clone <repository-url>
cd elearning-platform
```

#### **2. Run Setup Script**
```bash
chmod +x setup-project.sh
./setup-project.sh
```

#### **3. Start Infrastructure**
```bash
cd infrastructure
chmod +x podman-setup.sh
./podman-setup.sh
```

This will:
- Start SQL Server, RabbitMQ, and Redis containers
- Create all databases
- Run SQL migrations
- Seed initial data

#### **4. Configure Services**

Update connection strings in each service's `appsettings.json`:
- User Service: `backend/Services/UserService/UserService.API/appsettings.json`
- Course Service: `backend/Services/CourseService/CourseService.API/appsettings.json`
- Subscription Service: `backend/Services/SubscriptionService/SubscriptionService.API/appsettings.json`
- Payment Service: `backend/Services/PaymentService/PaymentService.API/appsettings.json`
- Certificate Service: `backend/Services/CertificateService/CertificateService.API/appsettings.json`

#### **5. Configure Omise API Keys**

In `PaymentService.API/appsettings.json`:
```json
{
  "Omise": {
    "PublicKey": "your-omise-public-key",
    "SecretKey": "your-omise-secret-key"
  }
}
```

#### **6. Run Services**

**Option A: Run all services with Docker/Podman**
```bash
cd infrastructure
podman-compose up -d
```

**Option B: Run services individually (for development)**
```bash
# Terminal 1 - API Gateway
cd backend/ApiGateway
dotnet run

# Terminal 2 - User Service
cd backend/Services/UserService/UserService.API
dotnet run

# Terminal 3 - Course Service
cd backend/Services/CourseService/CourseService.API
dotnet run

# Terminal 4 - Subscription Service
cd backend/Services/SubscriptionService/SubscriptionService.API
dotnet run

# Terminal 5 - Payment Service
cd backend/Services/PaymentService/PaymentService.API
dotnet run

# Terminal 6 - Certificate Service
cd backend/Services/CertificateService/CertificateService.API
dotnet run
```

## 📡 API Endpoints

### **Base URL**: `http://localhost:5000`

### **User Service** (`/api/auth`, `/api/users`)
```
POST   /api/auth/register          - Register new user
POST   /api/auth/login             - Login
POST   /api/auth/refresh-token     - Refresh access token
POST   /api/auth/logout            - Logout
POST   /api/auth/change-password   - Change password
POST   /api/auth/forgot-password   - Request password reset
POST   /api/auth/reset-password    - Reset password
GET    /api/auth/verify-email      - Verify email

GET    /api/users                  - Get all users (Admin)
GET    /api/users/{id}             - Get user by ID
GET    /api/users/me               - Get current user
PUT    /api/users/{id}             - Update user
PUT    /api/users/{id}/profile     - Update profile
DELETE /api/users/{id}             - Delete user (Admin)
POST   /api/users/assign-role      - Assign role (Admin)
GET    /api/users/{id}/roles       - Get user roles
```

### **Course Service** (`/api/courses`, `/api/categories`)
```
GET    /api/courses                - Get all courses
GET    /api/courses/{id}           - Get course by ID
POST   /api/courses                - Create course (Instructor/Admin)
PUT    /api/courses/{id}           - Update course
DELETE /api/courses/{id}           - Delete course
POST   /api/courses/{id}/publish   - Publish course

GET    /api/categories             - Get all categories
POST   /api/categories             - Create category (Admin)

POST   /api/topics                 - Create topic
PUT    /api/topics/{id}            - Update topic
DELETE /api/topics/{id}            - Delete topic

POST   /api/lessons                - Create lesson
PUT    /api/lessons/{id}           - Update lesson
DELETE /api/lessons/{id}           - Delete lesson

POST   /api/quizzes                - Create quiz
POST   /api/quizzes/{id}/submit    - Submit quiz answers
GET    /api/quizzes/{id}/attempts  - Get quiz attempts
```

### **Subscription Service** (`/api/enrollments`)
```
GET    /api/enrollments            - Get user enrollments
POST   /api/enrollments            - Enroll in course
GET    /api/enrollments/{id}       - Get enrollment details
POST   /api/enrollments/{id}/cancel - Cancel enrollment
```

### **Payment Service** (`/api/payments`, `/api/orders`)
```
POST   /api/payments/create-charge - Create payment charge
GET    /api/payments/{id}          - Get payment details
POST   /api/payments/webhook       - Omise webhook handler

GET    /api/orders                 - Get user orders
GET    /api/orders/{id}            - Get order details
POST   /api/orders                 - Create order
```

### **Certificate Service** (`/api/certificates`)
```
GET    /api/certificates           - Get user certificates
GET    /api/certificates/{id}      - Get certificate details
GET    /api/certificates/{id}/download - Download certificate PDF
GET    /api/certificates/verify/{code} - Verify certificate
```

## 🔐 Default Credentials

After running the seed script:

```
Admin:
Email: admin@elearning.com
Password: Admin@123

Instructor:
Email: instructor@elearning.com
Password: Instructor@123

Student:
Email: student@elearning.com
Password: Student@123
```

## 🗂️ Project Structure

```
elearning-platform/
├── infrastructure/
│   ├── docker-compose.yml
│   ├── podman-setup.sh
│   └── sql-scripts/
│       ├── 01-create-databases.sql
│       ├── 02-user-service-schema.sql
│       ├── 03-course-service-schema.sql
│       ├── 04-subscription-service-schema.sql
│       ├── 05-payment-service-schema.sql
│       ├── 06-certificate-service-schema.sql
│       └── 07-seed-data.sql
├── backend/
│   ├── Shared/
│   │   ├── Shared.Common/
│   │   └── Shared.MessageQueue/
│   ├── Services/
│   │   ├── UserService/
│   │   │   ├── UserService.Core/
│   │   │   ├── UserService.Infrastructure/
│   │   │   └── UserService.API/
│   │   ├── CourseService/
│   │   ├── SubscriptionService/
│   │   ├── PaymentService/
│   │   └── CertificateService/
│   └── ApiGateway/
└── frontend/ (Next.js - separate implementation)
```

## 📝 Testing

```bash
# Run unit tests
dotnet test

# Run integration tests
cd backend
dotnet test --filter Category=Integration

# Test API Gateway
curl http://localhost:5000/health

# Test User Service
curl http://localhost:5001/health

# Test Course Service
curl http://localhost:5002/health
```

## 🐛 Troubleshooting

### **SQL Server Connection Issues**
```bash
# Check if SQL Server container is running
podman ps | grep sqlserver

# View SQL Server logs
podman logs elearning-sqlserver

# Test connection
podman exec -it elearning-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "ojkiyd,kd8iy[" -Q "SELECT @@VERSION"
```

### **RabbitMQ Connection Issues**
```bash
# Check RabbitMQ status
podman ps | grep rabbitmq

# Access RabbitMQ Management UI
# http://localhost:15672 (admin/admin123)
```

### **Service Not Starting**
```bash
# Check logs
cd backend/Services/[ServiceName]/[ServiceName].API
dotnet run --verbosity detailed

# Check port conflicts
netstat -ano | findstr :5001
```

## 📚 Additional Resources

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Ocelot Documentation](https://ocelot.readthedocs.io/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [Omise API Documentation](https://www.omise.co/docs)
- [QuestPDF Documentation](https://www.questpdf.com/)

## 📄 License

This project is licensed under the MIT License.

## 👥 Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## 📧 Support

For support, email support@elearning.com or create an issue in the repository.
