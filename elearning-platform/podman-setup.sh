# ============================================================================
# E-LEARNING PLATFORM - PODMAN SETUP SCRIPT
# ============================================================================

set -e  # Exit on error

echo "========================================="
echo "E-Learning Platform - Podman Setup"
echo "========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored messages
print_message() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Podman is installed
if ! command -v podman &> /dev/null; then
    print_error "Podman is not installed. Please install Podman first."
    echo "Visit: https://podman.io/getting-started/installation"
    exit 1
fi

# Check if podman-compose is installed
if ! command -v podman-compose &> /dev/null; then
    print_warning "podman-compose is not installed. Installing..."
    pip3 install podman-compose
fi

print_message "Podman version: $(podman --version)"
print_message "Podman Compose version: $(podman-compose --version)"

# Create network if it doesn't exist
print_message "Creating Podman network..."
podman network exists elearning-network || podman network create elearning-network

# Create volumes
print_message "Creating volumes..."
# podman volume create sqlserver-data 2>/dev/null || true
podman volume create rabbitmq-data 2>/dev/null || true
podman volume create redis-data 2>/dev/null || true

# Stop and remove existing containers
print_message "Stopping existing containers..."
podman-compose down 2>/dev/null || true

# Build and start services
print_message "Building and starting services..."
podman-compose up -d --build

# Wait for SQL Server to be ready
print_message "Waiting for SQL Server to be ready..."
RETRY_COUNT=0
MAX_RETRIES=30

until podman exec elearning-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ojkiyd,kd8iy[" -Q "SELECT 1" &>/dev/null || [ $RETRY_COUNT -eq $MAX_RETRIES ]; do
    RETRY_COUNT=$((RETRY_COUNT+1))
    print_warning "Waiting for SQL Server... (Attempt $RETRY_COUNT/$MAX_RETRIES)"
    sleep 5
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    print_error "SQL Server failed to start"
    exit 1
fi

print_message "SQL Server is ready!"

# Initialize databases
print_message "Initializing databases..."
cd sql-scripts

# Create databases
print_message "Creating databases..."
podman exec -i elearning-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ojkiyd,kd8iy[" < 01-create-databases.sql

# User Service Schema
print_message "Creating User Service schema..."
podman exec -i elearning-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ojkiyd,kd8iy[" -d UserServiceDb < 02-user-service-schema.sql

# Course Service Schema
print_message "Creating Course Service schema..."
podman exec -i elearning-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ojkiyd,kd8iy[" -d CourseServiceDb < 03-course-service-schema.sql

# Subscription Service Schema
print_message "Creating Subscription Service schema..."
podman exec -i elearning-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ojkiyd,kd8iy[" -d SubscriptionServiceDb < 04-subscription-service-schema.sql

# Payment Service Schema
print_message "Creating Payment Service schema..."
podman exec -i elearning-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ojkiyd,kd8iy[" -d PaymentServiceDb < 05-payment-service-schema.sql

# Certificate Service Schema
print_message "Creating Certificate Service schema..."
podman exec -i elearning-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ojkiyd,kd8iy[" -d CertificateServiceDb < 06-certificate-service-schema.sql

# Seed initial data
print_message "Seeding initial data..."
podman exec -i elearning-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ojkiyd,kd8iy[" -d UserServiceDb < 07-seed-data.sql

cd ..

print_message "========================================="
print_message "Setup completed successfully!"
print_message "========================================="
echo ""
print_message "Services running:"
echo "  - SQL Server:          localhost:1433"
echo "  - RabbitMQ:            localhost:5672"
echo "  - RabbitMQ Management: http://localhost:15672 (admin/admin123)"
echo "  - Redis:               localhost:6379"
echo "  - API Gateway:         http://localhost:5000"
echo "  - User Service:        http://localhost:5001"
echo "  - Course Service:      http://localhost:5002"
echo "  - Subscription Service: http://localhost:5003"
echo "  - Payment Service:     http://localhost:5004"
echo "  - Certificate Service: http://localhost:5005"
echo ""
print_message "To view logs: podman-compose logs -f [service-name]"
print_message "To stop services: podman-compose down"
print_message "To restart services: podman-compose restart"
echo ""
print_message "========================================="