# ============================================================================
# E-LEARNING PLATFORM - .NET 8 COMPATIBLE SETUP SCRIPT (FULL VERSION)
# ============================================================================

set -e  # Exit on error

echo "========================================="
echo "Setting up E-Learning Platform (.NET 8)"
echo "========================================="

# ============================================================================
# ROOT STRUCTURE
# ============================================================================
mkdir -p elearning-platform
cd elearning-platform

# ============================================================================
# INFRASTRUCTURE
# ============================================================================
echo "Creating infrastructure directory..."
mkdir -p infrastructure/sql-scripts

# ============================================================================
# BACKEND SETUP
# ============================================================================
echo "Creating backend structure..."
mkdir -p backend
cd backend

# Create solution
dotnet new sln -n ELearningPlatform

# ============================================================================
# SHARED LIBRARIES
# ============================================================================
echo "Creating Shared libraries..."

# Shared.Common
mkdir -p Shared/Shared.Common
cd Shared/Shared.Common
dotnet new classlib -f net8.0
cd ../..

# Shared.MessageQueue
mkdir -p Shared/Shared.MessageQueue
cd Shared/Shared.MessageQueue
dotnet new classlib -f net8.0
dotnet add package RabbitMQ.Client --version 6.8.1
dotnet add package Newtonsoft.Json --version 13.0.3
cd ../..

# ============================================================================
# USER SERVICE
# ============================================================================
echo "Creating User Service..."
mkdir -p Services/UserService

cd Services/UserService
dotnet new classlib -n UserService.Core -f net8.0
dotnet new classlib -n UserService.Infrastructure -f net8.0
dotnet new webapi -n UserService.API -f net8.0
cd ../..

echo "Adding packages to UserService..."

cd Services/UserService/UserService.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
cd ../../..

cd Services/UserService/UserService.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package BCrypt.Net-Next --version 4.0.3
cd ../../..

# Add project references
cd Services/UserService/UserService.Core
dotnet add reference ../../../Shared/Shared.Common/Shared.Common.csproj
cd ../../..

cd Services/UserService/UserService.Infrastructure
dotnet add reference ../UserService.Core/UserService.Core.csproj
dotnet add reference ../../../Shared/Shared.Common/Shared.Common.csproj
cd ../../..

cd Services/UserService/UserService.API
dotnet add reference ../UserService.Core/UserService.Core.csproj
dotnet add reference ../UserService.Infrastructure/UserService.Infrastructure.csproj
dotnet add reference ../../../Shared/Shared.Common/Shared.Common.csproj
dotnet add reference ../../../Shared/Shared.MessageQueue/Shared.MessageQueue.csproj
cd ../../..

# ============================================================================
# COURSE SERVICE
# ============================================================================
echo "Creating Course Service..."
mkdir -p Services/CourseService

cd Services/CourseService
dotnet new classlib -n CourseService.Core -f net8.0
dotnet new classlib -n CourseService.Infrastructure -f net8.0
dotnet new webapi -n CourseService.API -f net8.0
cd ../..

cd Services/CourseService/CourseService.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
cd ../../..

cd Services/CourseService/CourseService.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
cd ../../..

cd Services/CourseService/CourseService.Core
dotnet add reference ../../../Shared/Shared.Common/Shared.Common.csproj
cd ../../..

cd Services/CourseService/CourseService.Infrastructure
dotnet add reference ../CourseService.Core/CourseService.Core.csproj
cd ../../..

cd Services/CourseService/CourseService.API
dotnet add reference ../CourseService.Core/CourseService.Core.csproj
dotnet add reference ../CourseService.Infrastructure/CourseService.Infrastructure.csproj
dotnet add reference ../../../Shared/Shared.MessageQueue/Shared.MessageQueue.csproj
cd ../../..

# ============================================================================
# SUBSCRIPTION SERVICE
# ============================================================================
echo "Creating Subscription Service..."
mkdir -p Services/SubscriptionService

cd Services/SubscriptionService
dotnet new classlib -n SubscriptionService.Core -f net8.0
dotnet new classlib -n SubscriptionService.Infrastructure -f net8.0
dotnet new webapi -n SubscriptionService.API -f net8.0
cd ../..

cd Services/SubscriptionService/SubscriptionService.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
cd ../../..

cd Services/SubscriptionService/SubscriptionService.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
cd ../../..

cd Services/SubscriptionService/SubscriptionService.Core
dotnet add reference ../../../Shared/Shared.Common/Shared.Common.csproj
cd ../../..

cd Services/SubscriptionService/SubscriptionService.Infrastructure
dotnet add reference ../SubscriptionService.Core/SubscriptionService.Core.csproj
cd ../../..

cd Services/SubscriptionService/SubscriptionService.API
dotnet add reference ../SubscriptionService.Core/SubscriptionService.Core.csproj
dotnet add reference ../SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj
dotnet add reference ../../../Shared/Shared.MessageQueue/Shared.MessageQueue.csproj
cd ../../..

# ============================================================================
# PAYMENT SERVICE
# ============================================================================
echo "Creating Payment Service..."
mkdir -p Services/PaymentService

cd Services/PaymentService
dotnet new classlib -n PaymentService.Core -f net8.0
dotnet new classlib -n PaymentService.Infrastructure -f net8.0
dotnet new webapi -n PaymentService.API -f net8.0
cd ../..

cd Services/PaymentService/PaymentService.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
cd ../../..

cd Services/PaymentService/PaymentService.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
cd ../../..

cd Services/PaymentService/PaymentService.Core
dotnet add reference ../../../Shared/Shared.Common/Shared.Common.csproj
cd ../../..

cd Services/PaymentService/PaymentService.Infrastructure
dotnet add reference ../PaymentService.Core/PaymentService.Core.csproj
cd ../../..

cd Services/PaymentService/PaymentService.API
dotnet add reference ../PaymentService.Core/PaymentService.Core.csproj
dotnet add reference ../PaymentService.Infrastructure/PaymentService.Infrastructure.csproj
dotnet add reference ../../../Shared/Shared.MessageQueue/Shared.MessageQueue.csproj
cd ../../..

# ============================================================================
# CERTIFICATE SERVICE
# ============================================================================
echo "Creating Certificate Service..."
mkdir -p Services/CertificateService

cd Services/CertificateService
dotnet new classlib -n CertificateService.Core -f net8.0
dotnet new classlib -n CertificateService.Infrastructure -f net8.0
dotnet new webapi -n CertificateService.API -f net8.0
cd ../..

cd Services/CertificateService/CertificateService.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package QuestPDF --version 2024.9.2
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
cd ../../..

cd Services/CertificateService/CertificateService.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package QuestPDF --version 2024.9.2
cd ../../..

cd Services/CertificateService/CertificateService.Core
dotnet add reference ../../../Shared/Shared.Common/Shared.Common.csproj
cd ../../..

cd Services/CertificateService/CertificateService.Infrastructure
dotnet add reference ../CertificateService.Core/CertificateService.Core.csproj
cd ../../..

cd Services/CertificateService/CertificateService.API
dotnet add reference ../CertificateService.Core/CertificateService.Core.csproj
dotnet add reference ../CertificateService.Infrastructure/CertificateService.Infrastructure.csproj
dotnet add reference ../../../Shared/Shared.MessageQueue/Shared.MessageQueue.csproj
cd ../../..

# ============================================================================
# API GATEWAY
# ============================================================================
echo "Creating API Gateway..."
mkdir -p ApiGateway
cd ApiGateway
dotnet new webapi -f net8.0
dotnet add package Ocelot --version 23.3.3
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11
cd ..

# ============================================================================
# ADD PROJECTS TO SOLUTION
# ============================================================================
echo "Adding projects to solution..."

dotnet sln add Shared/Shared.Common/Shared.Common.csproj
dotnet sln add Shared/Shared.MessageQueue/Shared.MessageQueue.csproj

dotnet sln add Services/UserService/UserService.Core/UserService.Core.csproj
dotnet sln add Services/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj
dotnet sln add Services/UserService/UserService.API/UserService.API.csproj

dotnet sln add Services/CourseService/CourseService.Core/CourseService.Core.csproj
dotnet sln add Services/CourseService/CourseService.Infrastructure/CourseService.Infrastructure.csproj
dotnet sln add Services/CourseService/CourseService.API/CourseService.API.csproj

dotnet sln add Services/SubscriptionService/SubscriptionService.Core/SubscriptionService.Core.csproj
dotnet sln add Services/SubscriptionService/SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj
dotnet sln add Services/SubscriptionService/SubscriptionService.API/SubscriptionService.API.csproj

dotnet sln add Services/PaymentService/PaymentService.Core/PaymentService.Core.csproj
dotnet sln add Services/PaymentService/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj
dotnet sln add Services/PaymentService/PaymentService.API/PaymentService.API.csproj

dotnet sln add Services/CertificateService/CertificateService.Core/CertificateService.Core.csproj
dotnet sln add Services/CertificateService/CertificateService.Infrastructure/CertificateService.Infrastructure.csproj
dotnet sln add Services/CertificateService/CertificateService.API/CertificateService.API.csproj

dotnet sln add ApiGateway/ApiGateway.csproj

# ============================================================================
# BUILD SOLUTION TO VERIFY
# ============================================================================
echo "Building solution to verify setup..."
dotnet restore
dotnet build

cd ..

# ============================================================================
# FRONTEND SETUP
# ============================================================================
echo "Setting up frontend..."
mkdir -p frontend
cd frontend

npx create-next-app@latest . --typescript --tailwind --app --no-src-dir --import-alias "@/*" --use-npm

mkdir -p app/\(auth\)/login
mkdir -p app/\(auth\)/register
mkdir -p app/\(student\)/dashboard
mkdir -p app/\(student\)/courses
mkdir -p app/\(admin\)/dashboard
mkdir -p app/\(admin\)/users
mkdir -p components/ui
mkdir -p components/layout
mkdir -p lib/api
mkdir -p lib/stores
mkdir -p types

npm install axios zustand react-hook-form zod @hookform/resolvers date-fns clsx tailwind-merge lucide-react

cd ..

# ============================================================================
# DONE
# ============================================================================
echo "========================================="
echo "Setup Complete!"
echo "========================================="
echo ""
echo "Project created at: $(pwd)"
echo ""
echo "Next steps:"
echo "1. Copy infrastructure files (docker-compose.yml, podman-setup.sh, SQL scripts)"
echo "2. Initialize DBs"
echo "3. Run: cd backend && dotnet build"
echo "4. Start services with: dotnet run"
echo "5. Run frontend: cd frontend && npm run dev"
echo ""
echo "========================================="
