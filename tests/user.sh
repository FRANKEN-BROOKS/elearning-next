#!/bin/bash

# ============================================
# E-Learning Platform - User Service API Tests
# ============================================
# Base URL through API Gateway
BASE_URL="http://localhost:5016"

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Variables (will be set after registration/login)
ACCESS_TOKEN=""
USER_ID=""
REFRESH_TOKEN=""

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Testing User Service API${NC}"
echo -e "${BLUE}========================================${NC}\n"

# ============================================
# 1. AUTH ENDPOINTS
# ============================================

echo -e "\n${GREEN}=== AUTH ENDPOINTS ===${NC}\n"

# 1.1 Register
echo -e "${BLUE}1. POST /api/auth/register${NC}"
REGISTER_RESPONSE=$(curl -s -X POST "$BASE_URL/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@examplexx.com",
    "password": "Test@1234",
    "firstName": "Johnx",
    "lastName": "Doex",
    "phoneNumber": "+1234567899"
  }')
echo "$REGISTER_RESPONSE" | jq '.'

# Extract tokens and user ID from response
ACCESS_TOKEN=$(echo "$REGISTER_RESPONSE" | jq -r '.data.accessToken // empty')
REFRESH_TOKEN=$(echo "$REGISTER_RESPONSE" | jq -r '.data.refreshToken // empty')
USER_ID=$(echo "$REGISTER_RESPONSE" | jq -r '.data.user.id // empty')

echo -e "\nExtracted TOKEN: ${ACCESS_TOKEN:0:20}..."
echo -e "Extracted USER_ID: $USER_ID\n"

# 1.2 Login
echo -e "\n${BLUE}2. POST /api/auth/login${NC}"
LOGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@1234"
  }')
echo "$LOGIN_RESPONSE" | jq '.'

# Update tokens from login
ACCESS_TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.data.accessToken // empty')
REFRESH_TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.data.refreshToken // empty')

# 1.3 Refresh Token
echo -e "\n${BLUE}3. POST /api/auth/refresh-token${NC}"
curl -s -X POST "$BASE_URL/api/auth/refresh-token" \
  -H "Content-Type: application/json" \
  -d "{
    \"refreshToken\": \"$REFRESH_TOKEN\"
  }" | jq '.'

# 1.4 Change Password
echo -e "\n${BLUE}4. POST /api/auth/change-password${NC}"
curl -s -X POST "$BASE_URL/api/auth/change-password" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "currentPassword": "Test@1234",
    "newPassword": "NewTest@1234"
  }' | jq '.'

# 1.5 Forgot Password
echo -e "\n${BLUE}5. POST /api/auth/forgot-password${NC}"
curl -s -X POST "$BASE_URL/api/auth/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com"
  }' | jq '.'

# 1.6 Reset Password
echo -e "\n${BLUE}6. POST /api/auth/reset-password${NC}"
curl -s -X POST "$BASE_URL/api/auth/reset-password" \
  -H "Content-Type: application/json" \
  -d '{
    "token": "your-reset-token-here",
    "newPassword": "NewPassword@1234"
  }' | jq '.'

# 1.7 Verify Email
echo -e "\n${BLUE}7. GET /api/auth/verify-email${NC}"
curl -s -X GET "$BASE_URL/api/auth/verify-email?token=your-verification-token" \
  -H "Content-Type: application/json" | jq '.'

# 1.8 Logout
echo -e "\n${BLUE}8. POST /api/auth/logout${NC}"
curl -s -X POST "$BASE_URL/api/auth/logout" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# ============================================
# 2. USERS ENDPOINTS
# ============================================

echo -e "\n${GREEN}=== USERS ENDPOINTS ===${NC}\n"

# 2.1 Get All Users (with pagination)
echo -e "\n${BLUE}9. GET /api/users (All Users)${NC}"
curl -s -X GET "$BASE_URL/api/users?PageNumber=1&PageSize=10&SearchTerm=&SortBy=createdAt&SortOrder=desc" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# 2.2 Get Current User (Me)
echo -e "\n${BLUE}10. GET /api/users/me${NC}"
curl -s -X GET "$BASE_URL/api/users/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# 2.3 Get User by ID
echo -e "\n${BLUE}11. GET /api/users/{id}${NC}"
curl -s -X GET "$BASE_URL/api/users/$USER_ID" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# 2.4 Update User
echo -e "\n${BLUE}12. PUT /api/users/{id}${NC}"
curl -s -X PUT "$BASE_URL/api/users/$USER_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "firstName": "John",
    "lastName": "Smith",
    "phoneNumber": "+1234567890",
    "dateOfBirth": "1990-01-01T00:00:00Z",
    "profileImageUrl": "https://example.com/avatar.jpg"
  }' | jq '.'

# 2.5 Update User Profile
echo -e "\n${BLUE}13. PUT /api/users/{id}/profile${NC}"
curl -s -X PUT "$BASE_URL/api/users/$USER_ID/profile" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "bio": "Software Developer with 5 years of experience",
    "address": "123 Main St",
    "city": "New York",
    "province": "NY",
    "postalCode": "10001",
    "country": "USA",
    "linkedInUrl": "https://linkedin.com/in/johndoe",
    "websiteUrl": "https://johndoe.com",
    "occupation": "Software Developer",
    "company": "Tech Corp"
  }' | jq '.'

# 2.6 Get User Roles
echo -e "\n${BLUE}14. GET /api/users/{id}/roles${NC}"
curl -s -X GET "$BASE_URL/api/users/$USER_ID/roles" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# 2.7 Get User Permissions
echo -e "\n${BLUE}15. GET /api/users/{id}/permissions${NC}"
curl -s -X GET "$BASE_URL/api/users/$USER_ID/permissions" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# 2.8 Assign Role
echo -e "\n${BLUE}16. POST /api/users/assign-role${NC}"
curl -s -X POST "$BASE_URL/api/users/assign-role" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "userId": "'"$USER_ID"'",
    "roleId": "role-uuid-here"
  }' | jq '.'

# 2.9 Remove Role
echo -e "\n${BLUE}17. DELETE /api/users/{userId}/roles/{roleId}${NC}"
curl -s -X DELETE "$BASE_URL/api/users/$USER_ID/roles/role-uuid-here" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# 2.10 Activate User
echo -e "\n${BLUE}18. POST /api/users/{id}/activate${NC}"
curl -s -X POST "$BASE_URL/api/users/$USER_ID/activate" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# 2.11 Deactivate User
echo -e "\n${BLUE}19. POST /api/users/{id}/deactivate${NC}"
curl -s -X POST "$BASE_URL/api/users/$USER_ID/deactivate" \
  -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

# 2.12 Delete User (Run this last)
echo -e "\n${BLUE}20. DELETE /api/users/{id}${NC}"
echo -e "${RED}Skipping delete to preserve test user${NC}"
# Uncomment to actually delete:
# curl -s -X DELETE "$BASE_URL/api/users/$USER_ID" \
#   -H "Authorization: Bearer $ACCESS_TOKEN" | jq '.'

echo -e "\n${GREEN}========================================${NC}"
echo -e "${GREEN}All tests completed!${NC}"
echo -e "${GREEN}========================================${NC}\n"

# Save tokens for future use
echo -e "Tokens saved for manual testing:"
echo -e "ACCESS_TOKEN=$ACCESS_TOKEN"
echo -e "REFRESH_TOKEN=$REFRESH_TOKEN"
echo -e "USER_ID=$USER_ID"