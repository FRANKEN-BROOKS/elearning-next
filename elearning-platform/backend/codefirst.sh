#!/bin/bash
set -e  # Exit on any error

echo "⚠️  WARNING: This script will:"
echo "   - Delete all existing migration files"
echo "   - Drop all databases"
echo "   - Create fresh migrations"
echo "   - Recreate databases"
echo ""
read -p "Are you sure you want to continue? (yes/no): " confirmation

if [ "$confirmation" != "yes" ]; then
    echo "Migration cancelled."
    exit 0
fi

# Store the starting directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Check if dotnet-ef tool is installed
if ! command -v dotnet-ef &> /dev/null; then
    echo "❌ Error: dotnet-ef tool not found."
    echo "Install with: dotnet tool install --global dotnet-ef"
    exit 1
fi

echo ""
echo "Starting clean migration process for all services..."
echo "=================================================="

# Function to clean migrate a service
clean_migrate_service() {
    local service_name=$1
    local context_name=$2
    local api_path=$3
    local infra_project=$4
    
    echo ""
    echo "Processing ${service_name}..."
    echo "-----------------------------------"
    
    # Navigate to API project directory
    cd "${SCRIPT_DIR}/${api_path}"
    
    if [ ! -f "${service_name}.API.csproj" ]; then
        echo "❌ Error: ${service_name}.API.csproj not found in $(pwd)"
        return 1
    fi
    
    # Delete existing migrations folder
    MIGRATIONS_PATH="../${infra_project}/Data/Migrations"
    if [ -d "$MIGRATIONS_PATH" ]; then
        echo "🗑️  Deleting existing migrations folder..."
        rm -rf "$MIGRATIONS_PATH"
        echo "✓ Migrations folder deleted"
    else
        echo "ℹ️  No existing migrations folder found"
    fi
    
    # Drop existing database
    echo "🗑️  Dropping existing database..."
    dotnet ef database drop \
        --project ../${infra_project}/${infra_project}.csproj \
        --startup-project ${service_name}.API.csproj \
        --context ${context_name} \
        --force 2>/dev/null || echo "⚠️  Database didn't exist or couldn't be dropped (this is normal for first run)"
    
    # Create new migration
    echo "📝 Creating new migration..."
    if ! dotnet ef migrations add InitialCreate${context_name} \
        --project ../${infra_project}/${infra_project}.csproj \
        --startup-project ${service_name}.API.csproj \
        --context ${context_name} \
        --output-dir Data/Migrations; then
        echo "❌ Failed to create migration for ${service_name}"
        return 1
    fi
    
    # Update database
    echo "🔄 Updating database..."
    if ! dotnet ef database update \
        --project ../${infra_project}/${infra_project}.csproj \
        --startup-project ${service_name}.API.csproj \
        --context ${context_name}; then
        echo "❌ Failed to update database for ${service_name}"
        return 1
    fi
    
    echo "✅ ${service_name} completed successfully!"
    
    # Return to script directory
    cd "${SCRIPT_DIR}"
}

# Track success/failure
SUCCESS_COUNT=0
FAILURE_COUNT=0
FAILED_SERVICES=()

# User Service
echo ""
echo "=================================================="
echo "1/5 - User Service"
echo "=================================================="
if clean_migrate_service \
    "UserService" \
    "UserDbContext" \
    "../backend/Services/UserService/UserService.API" \
    "UserService.Infrastructure"; then
    ((SUCCESS_COUNT++))
else
    ((FAILURE_COUNT++))
    FAILED_SERVICES+=("UserService")
fi

# Course Service
echo ""
echo "=================================================="
echo "2/5 - Course Service"
echo "=================================================="
if clean_migrate_service \
    "CourseService" \
    "CourseDbContext" \
    "../backend/Services/CourseService/CourseService.API" \
    "CourseService.Infrastructure"; then
    ((SUCCESS_COUNT++))
else
    ((FAILURE_COUNT++))
    FAILED_SERVICES+=("CourseService")
fi

# Subscription Service
echo ""
echo "=================================================="
echo "3/5 - Subscription Service"
echo "=================================================="
if clean_migrate_service \
    "SubscriptionService" \
    "SubscriptionDbContext" \
    "../backend/Services/SubscriptionService/SubscriptionService.API" \
    "SubscriptionService.Infrastructure"; then
    ((SUCCESS_COUNT++))
else
    ((FAILURE_COUNT++))
    FAILED_SERVICES+=("SubscriptionService")
fi

# Payment Service
echo ""
echo "=================================================="
echo "4/5 - Payment Service"
echo "=================================================="
if clean_migrate_service \
    "PaymentService" \
    "PaymentDbContext" \
    "../backend/Services/PaymentService/PaymentService.API" \
    "PaymentService.Infrastructure"; then
    ((SUCCESS_COUNT++))
else
    ((FAILURE_COUNT++))
    FAILED_SERVICES+=("PaymentService")
fi

# Certificate Service
echo ""
echo "=================================================="
echo "5/5 - Certificate Service"
echo "=================================================="
if clean_migrate_service \
    "CertificateService" \
    "CertificateDbContext" \
    "../backend/Services/CertificateService/CertificateService.API" \
    "CertificateService.Infrastructure"; then
    ((SUCCESS_COUNT++))
else
    ((FAILURE_COUNT++))
    FAILED_SERVICES+=("CertificateService")
fi

# Final Summary
echo ""
echo "=================================================="
echo "📊 MIGRATION SUMMARY"
echo "=================================================="
echo "✅ Successful: ${SUCCESS_COUNT}/5"
echo "❌ Failed: ${FAILURE_COUNT}/5"

if [ $FAILURE_COUNT -gt 0 ]; then
    echo ""
    echo "Failed services:"
    for service in "${FAILED_SERVICES[@]}"; do
        echo "  - ${service}"
    done
    echo ""
    echo "❌ Migration completed with errors!"
    exit 1
else
    echo ""
    echo "✅ All migrations completed successfully!"
    echo "=================================================="
fi