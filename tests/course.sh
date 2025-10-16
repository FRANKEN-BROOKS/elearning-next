#!/bin/bash

# ==============================================================================
# Course Service API Test Script
# E-Learning Platform - Course Management Service
# ==============================================================================

# Configuration
BASE_URL="http://localhost:5002"  # Change this to your Course API base URL
USER_API_URL="http://localhost:5000"  # User Service API for authentication
API_BASE="${BASE_URL}/api"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Test data
TEST_EMAIL="instructor_$(date +%s)@example.com"
TEST_PASSWORD="InstructorPass123!"

# Variables to store IDs and tokens
ACCESS_TOKEN=""
USER_ID=""
CATEGORY_ID=""
COURSE_ID=""
TOPIC_ID=""
LESSON_ID=""
QUIZ_ID=""
QUESTION_ID=""
OPTION_ID=""

# ==============================================================================
# Helper Functions
# ==============================================================================

print_header() {
    echo -e "\n${BLUE}================================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}================================================${NC}\n"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ $1${NC}"
}

print_step() {
    echo -e "${CYAN}→ $1${NC}"
}

print_response() {
    echo "$1" | jq '.' 2>/dev/null || echo "$1"
}

# ==============================================================================
# Authentication Functions
# ==============================================================================

setup_authentication() {
    print_header "SETUP: Authentication"
    
    print_step "Registering instructor user..."
    
    RESPONSE=$(curl -s -X POST "${USER_API_URL}/api/Auth/register" \
        -H "Content-Type: application/json" \
        -d "{
            \"email\": \"${TEST_EMAIL}\",
            \"password\": \"${TEST_PASSWORD}\",
            \"firstName\": \"Test\",
            \"lastName\": \"Instructor\",
            \"phoneNumber\": \"+1234567890\"
        }")
    
    ACCESS_TOKEN=$(echo "$RESPONSE" | jq -r '.data.accessToken // empty')
    USER_ID=$(echo "$RESPONSE" | jq -r '.data.user.id // empty')
    
    if [ -n "$ACCESS_TOKEN" ] && [ -n "$USER_ID" ]; then
        print_success "Authentication successful! Instructor ID: $USER_ID"
        return 0
    else
        print_error "Authentication failed!"
        print_response "$RESPONSE"
        return 1
    fi
}

# ==============================================================================
# Category Tests
# ==============================================================================

test_create_category() {
    print_header "TEST 1: Create Category"
    
    RESPONSE=$(curl -s -X POST "${API_BASE}/categories" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"name\": \"Web Development\",
            \"description\": \"Learn modern web development technologies\",
            \"iconUrl\": \"https://example.com/icons/web-dev.png\",
            \"displayOrder\": 1,
            \"isActive\": true
        }")
    
    print_response "$RESPONSE"
    
    CATEGORY_ID=$(echo "$RESPONSE" | jq -r '.data.id // empty')
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ] && [ -n "$CATEGORY_ID" ]; then
        print_success "Category created! ID: $CATEGORY_ID"
    else
        print_error "Failed to create category"
        return 1
    fi
}

test_get_all_categories() {
    print_header "TEST 2: Get All Categories"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/categories" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved all categories"
    else
        print_error "Failed to retrieve categories"
    fi
}

test_get_category_by_id() {
    print_header "TEST 3: Get Category By ID"
    
    print_info "Getting category: $CATEGORY_ID"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/categories/${CATEGORY_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved category by ID"
    else
        print_error "Failed to retrieve category"
    fi
}

test_update_category() {
    print_header "TEST 4: Update Category"
    
    RESPONSE=$(curl -s -X PUT "${API_BASE}/categories/${CATEGORY_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"name\": \"Web Development (Updated)\",
            \"description\": \"Master modern web development with HTML, CSS, JavaScript and more\",
            \"iconUrl\": \"https://example.com/icons/web-dev-updated.png\",
            \"displayOrder\": 1,
            \"isActive\": true
        }")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Category updated successfully"
    else
        print_error "Failed to update category"
    fi
}

# ==============================================================================
# Course Tests
# ==============================================================================

test_create_course() {
    print_header "TEST 5: Create Course"
    
    RESPONSE=$(curl -s -X POST "${API_BASE}/courses" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"title\": \"Complete JavaScript Course\",
            \"shortDescription\": \"Learn JavaScript from scratch to advanced\",
            \"description\": \"This comprehensive course covers everything you need to know about JavaScript, from basic syntax to advanced concepts like closures, promises, and async/await.\",
            \"categoryId\": \"${CATEGORY_ID}\",
            \"instructorId\": \"${USER_ID}\",
            \"thumbnailUrl\": \"https://example.com/courses/js-thumbnail.jpg\",
            \"previewVideoUrl\": \"https://example.com/courses/js-preview.mp4\",
            \"level\": \"Beginner\",
            \"language\": \"English\",
            \"priceThb\": 1999.00,
            \"discountPriceThb\": 999.00
        }")
    
    print_response "$RESPONSE"
    
    COURSE_ID=$(echo "$RESPONSE" | jq -r '.data.id // empty')
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ] && [ -n "$COURSE_ID" ]; then
        print_success "Course created! ID: $COURSE_ID"
    else
        print_error "Failed to create course"
        return 1
    fi
}

test_get_all_courses() {
    print_header "TEST 6: Get All Courses (Paginated)"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/courses?PageNumber=1&PageSize=10&SortBy=createdAt&SortOrder=desc" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved courses list"
    else
        print_error "Failed to retrieve courses"
    fi
}

test_get_course_by_id() {
    print_header "TEST 7: Get Course By ID"
    
    print_info "Getting course: $COURSE_ID"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/courses/${COURSE_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved course by ID"
    else
        print_error "Failed to retrieve course"
    fi
}

test_update_course() {
    print_header "TEST 8: Update Course"
    
    RESPONSE=$(curl -s -X PUT "${API_BASE}/courses/${COURSE_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"title\": \"Complete JavaScript Masterclass 2025\",
            \"shortDescription\": \"Master JavaScript from zero to hero\",
            \"description\": \"Updated comprehensive course with latest JavaScript features including ES2024 updates.\",
            \"categoryId\": \"${CATEGORY_ID}\",
            \"level\": \"All Levels\",
            \"language\": \"English\",
            \"priceThb\": 2499.00,
            \"discountPriceThb\": 1299.00,
            \"metaTitle\": \"Learn JavaScript - Complete Course 2025\",
            \"metaDescription\": \"Comprehensive JavaScript course for all skill levels\",
            \"metaKeywords\": \"javascript, programming, web development\"
        }")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Course updated successfully"
    else
        print_error "Failed to update course"
    fi
}

test_get_courses_by_category() {
    print_header "TEST 9: Get Courses By Category"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/courses?categoryId=${CATEGORY_ID}&PageNumber=1&PageSize=10" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved courses by category"
    else
        print_error "Failed to retrieve courses by category"
    fi
}

test_get_courses_by_instructor() {
    print_header "TEST 10: Get Courses By Instructor"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/courses/instructor/${USER_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved courses by instructor"
    else
        print_error "Failed to retrieve courses by instructor"
    fi
}

test_get_featured_courses() {
    print_header "TEST 11: Get Featured Courses"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/courses/featured?count=5" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved featured courses"
    else
        print_error "Failed to retrieve featured courses"
    fi
}

# ==============================================================================
# Topic Tests
# ==============================================================================

test_create_topic() {
    print_header "TEST 12: Create Course Topic"
    
    RESPONSE=$(curl -s -X POST "${API_BASE}/topics" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"courseId\": \"${COURSE_ID}\",
            \"title\": \"Introduction to JavaScript\",
            \"description\": \"Learn the basics of JavaScript programming\",
            \"displayOrder\": 1
        }")
    
    print_response "$RESPONSE"
    
    TOPIC_ID=$(echo "$RESPONSE" | jq -r '.data.id // empty')
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ] && [ -n "$TOPIC_ID" ]; then
        print_success "Topic created! ID: $TOPIC_ID"
    else
        print_error "Failed to create topic"
        return 1
    fi
}

test_get_topics_by_course() {
    print_header "TEST 13: Get Topics By Course"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/topics/course/${COURSE_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved topics by course"
    else
        print_error "Failed to retrieve topics"
    fi
}

test_get_topic_by_id() {
    print_header "TEST 14: Get Topic By ID"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/topics/${TOPIC_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved topic by ID"
    else
        print_error "Failed to retrieve topic"
    fi
}

test_update_topic() {
    print_header "TEST 15: Update Topic"
    
    RESPONSE=$(curl -s -X PUT "${API_BASE}/topics/${TOPIC_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"courseId\": \"${COURSE_ID}\",
            \"title\": \"JavaScript Fundamentals\",
            \"description\": \"Master the fundamental concepts of JavaScript programming\",
            \"displayOrder\": 1
        }")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Topic updated successfully"
    else
        print_error "Failed to update topic"
    fi
}

# ==============================================================================
# Lesson Tests
# ==============================================================================

test_create_lesson() {
    print_header "TEST 16: Create Lesson"
    
    RESPONSE=$(curl -s -X POST "${API_BASE}/lessons" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"topicId\": \"${TOPIC_ID}\",
            \"title\": \"Variables and Data Types\",
            \"content\": \"In this lesson, we'll learn about JavaScript variables and different data types.\",
            \"videoUrl\": \"https://example.com/videos/js-variables.mp4\",
            \"duration\": 15,
            \"displayOrder\": 1,
            \"isFree\": true
        }")
    
    print_response "$RESPONSE"
    
    LESSON_ID=$(echo "$RESPONSE" | jq -r '.data.id // empty')
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ] && [ -n "$LESSON_ID" ]; then
        print_success "Lesson created! ID: $LESSON_ID"
    else
        print_error "Failed to create lesson"
        return 1
    fi
}

test_get_lessons_by_topic() {
    print_header "TEST 17: Get Lessons By Topic"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/lessons/topic/${TOPIC_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved lessons by topic"
    else
        print_error "Failed to retrieve lessons"
    fi
}

test_get_lesson_by_id() {
    print_header "TEST 18: Get Lesson By ID"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/lessons/${LESSON_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved lesson by ID"
    else
        print_error "Failed to retrieve lesson"
    fi
}

test_update_lesson() {
    print_header "TEST 19: Update Lesson"
    
    RESPONSE=$(curl -s -X PUT "${API_BASE}/lessons/${LESSON_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"topicId\": \"${TOPIC_ID}\",
            \"title\": \"JavaScript Variables, Constants and Data Types\",
            \"content\": \"Updated lesson content with more details about var, let, const and all data types.\",
            \"videoUrl\": \"https://example.com/videos/js-variables-updated.mp4\",
            \"duration\": 20,
            \"displayOrder\": 1,
            \"isFree\": true
        }")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Lesson updated successfully"
    else
        print_error "Failed to update lesson"
    fi
}

# ==============================================================================
# Quiz Tests
# ==============================================================================

test_create_quiz() {
    print_header "TEST 20: Create Quiz"
    
    RESPONSE=$(curl -s -X POST "${API_BASE}/quizzes" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"courseId\": \"${COURSE_ID}\",
            \"topicId\": \"${TOPIC_ID}\",
            \"title\": \"JavaScript Fundamentals Quiz\",
            \"description\": \"Test your knowledge of JavaScript basics\",
            \"passingScore\": 70.0,
            \"timeLimit\": 30,
            \"maxAttempts\": 3
        }")
    
    print_response "$RESPONSE"
    
    QUIZ_ID=$(echo "$RESPONSE" | jq -r '.data.id // empty')
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ] && [ -n "$QUIZ_ID" ]; then
        print_success "Quiz created! ID: $QUIZ_ID"
    else
        print_error "Failed to create quiz"
        return 1
    fi
}

test_create_quiz_question() {
    print_header "TEST 21: Create Quiz Question"
    
    RESPONSE=$(curl -s -X POST "${API_BASE}/quizzes/questions" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"quizId\": \"${QUIZ_ID}\",
            \"questionText\": \"What keyword is used to declare a variable in JavaScript?\",
            \"questionType\": \"MultipleChoice\",
            \"points\": 10,
            \"explanation\": \"The 'var', 'let', and 'const' keywords are used to declare variables in JavaScript.\",
            \"options\": [
                {
                    \"optionText\": \"var\",
                    \"isCorrect\": true,
                    \"displayOrder\": 1
                },
                {
                    \"optionText\": \"variable\",
                    \"isCorrect\": false,
                    \"displayOrder\": 2
                },
                {
                    \"optionText\": \"int\",
                    \"isCorrect\": false,
                    \"displayOrder\": 3
                },
                {
                    \"optionText\": \"string\",
                    \"isCorrect\": false,
                    \"displayOrder\": 4
                }
            ]
        }")
    
    print_response "$RESPONSE"
    
    QUESTION_ID=$(echo "$RESPONSE" | jq -r '.data.id // empty')
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ] && [ -n "$QUESTION_ID" ]; then
        OPTION_ID=$(echo "$RESPONSE" | jq -r '.data.options[0].id // empty')
        print_success "Question created! ID: $QUESTION_ID"
    else
        print_error "Failed to create question"
    fi
}

test_get_quiz_by_id() {
    print_header "TEST 22: Get Quiz By ID"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/quizzes/${QUIZ_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved quiz by ID"
    else
        print_error "Failed to retrieve quiz"
    fi
}

test_get_quizzes_by_course() {
    print_header "TEST 23: Get Quizzes By Course"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/quizzes/course/${COURSE_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved quizzes by course"
    else
        print_error "Failed to retrieve quizzes"
    fi
}

test_submit_quiz() {
    print_header "TEST 24: Submit Quiz Answers"
    
    print_info "Submitting quiz with correct answer"
    
    RESPONSE=$(curl -s -X POST "${API_BASE}/quizzes/${QUIZ_ID}/submit" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"quizId\": \"${QUIZ_ID}\",
            \"answers\": [
                {
                    \"questionId\": \"${QUESTION_ID}\",
                    \"selectedOptionId\": \"${OPTION_ID}\"
                }
            ]
        }")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    SCORE=$(echo "$RESPONSE" | jq -r '.data.score // 0')
    IS_PASSED=$(echo "$RESPONSE" | jq -r '.data.isPassed // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Quiz submitted! Score: $SCORE%, Passed: $IS_PASSED"
    else
        print_error "Failed to submit quiz"
    fi
}

test_get_quiz_attempts() {
    print_header "TEST 25: Get Quiz Attempts"
    
    RESPONSE=$(curl -s -X GET "${API_BASE}/quizzes/${QUIZ_ID}/attempts/${USER_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    
    if [ "$SUCCESS" = "true" ]; then
        print_success "Retrieved quiz attempts"
    else
        print_error "Failed to retrieve quiz attempts"
    fi
}

test_publish_course() {
    print_header "TEST 26: Publish Course"
    
    RESPONSE=$(curl -s -X POST "${API_BASE}/courses/${COURSE_ID}/publish" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    print_response "$RESPONSE"
    
    SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
    IS_PUBLISHED=$(echo "$RESPONSE" | jq -r '.data.isPublished // false')
    
    if [ "$SUCCESS" = "true" ] && [ "$IS_PUBLISHED" = "true" ]; then
        print_success "Course published successfully"
    else
        print_error "Failed to publish course"
    fi
}

test_get_course_by_slug() {
    print_header "TEST 27: Get Course By Slug"
    
    # First get the slug from the course
    SLUG_RESPONSE=$(curl -s -X GET "${API_BASE}/courses/${COURSE_ID}" \
        -H "Authorization: Bearer ${ACCESS_TOKEN}")
    
    SLUG=$(echo "$SLUG_RESPONSE" | jq -r '.data.slug // empty')
    
    if [ -n "$SLUG" ]; then
        print_info "Using slug: $SLUG"
        
        RESPONSE=$(curl -s -X GET "${API_BASE}/courses/slug/${SLUG}" \
            -H "Authorization: Bearer ${ACCESS_TOKEN}")
        
        print_response "$RESPONSE"
        
        SUCCESS=$(echo "$RESPONSE" | jq -r '.success // false')
        
        if [ "$SUCCESS" = "true" ]; then
            print_success "Retrieved course by slug"
        else
            print_error "Failed to retrieve course by slug"
        fi
    else
        print_error "Could not retrieve course slug"
    fi
}

# ==============================================================================
# Cleanup Tests (Optional)
# ==============================================================================

test_delete_lesson() {
    print_header "TEST 28: Delete Lesson (Optional)"
    
    print_info "This test is commented out to preserve data"
    print_info "Uncomment to test deletion: DELETE ${API_BASE}/lessons/${LESSON_ID}"
    
    # Uncomment to test:
    # RESPONSE=$(curl -s -X DELETE "${API_BASE}/lessons/${LESSON_ID}" \
    #     -H "Authorization: Bearer ${ACCESS_TOKEN}")
    # print_response "$RESPONSE"
}

test_delete_quiz() {
    print_header "TEST 29: Delete Quiz (Optional)"
    
    print_info "This test is commented out to preserve data"
    print_info "Uncomment to test deletion: DELETE ${API_BASE}/quizzes/${QUIZ_ID}"
}

test_delete_topic() {
    print_header "TEST 30: Delete Topic (Optional)"
    
    print_info "This test is commented out to preserve data"
    print_info "Uncomment to test deletion: DELETE ${API_BASE}/topics/${TOPIC_ID}"
}

test_delete_course() {
    print_header "TEST 31: Delete Course (Optional)"
    
    print_info "This test is commented out to preserve data"
    print_info "Uncomment to test deletion: DELETE ${API_BASE}/courses/${COURSE_ID}"
}

test_delete_category() {
    print_header "TEST 32: Delete Category (Optional)"
    
    print_info "This test is commented out to preserve data"
    print_info "Uncomment to test deletion: DELETE ${API_BASE}/categories/${CATEGORY_ID}"
}

# ==============================================================================
# Main Execution
# ==============================================================================

main() {
    echo -e "${BLUE}"
    echo "╔════════════════════════════════════════════════════════════════╗"
    echo "║          Course Service API Testing Script                    ║"
    echo "║          E-Learning Platform                                   ║"
    echo "╚════════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
    
    print_info "Course API URL: $BASE_URL"
    print_info "User API URL: $USER_API_URL"
    
    # Check if jq is installed
    if ! command -v jq &> /dev/null; then
        print_error "jq is not installed. Please install jq for JSON formatting."
        print_info "Installation: sudo apt-get install jq (Ubuntu/Debian) or brew install jq (Mac)"
        exit 1
    fi
    
    # Setup authentication
    setup_authentication || exit 1
    sleep 1
    
    # Category Tests
    test_create_category || exit 1
    sleep 1
    test_get_all_categories
    sleep 1
    test_get_category_by_id
    sleep 1
    test_update_category
    sleep 1
    
    # Course Tests
    test_create_course || exit 1
    sleep 1
    test_get_all_courses
    sleep 1
    test_get_course_by_id
    sleep 1
    test_update_course
    sleep 1
    test_get_courses_by_category
    sleep 1
    test_get_courses_by_instructor
    sleep 1
    test_get_featured_courses
    sleep 1
    
    # Topic Tests
    test_create_topic || exit 1
    sleep 1
    test_get_topics_by_course
    sleep 1
    test_get_topic_by_id
    sleep 1
    test_update_topic
    sleep 1
    
    # Lesson Tests
    test_create_lesson || exit 1
    sleep 1
    test_get_lessons_by_topic
    sleep 1
    test_get_lesson_by_id
    sleep 1
    test_update_lesson
    sleep 1
    
    # Quiz Tests
    test_create_quiz || exit 1
    sleep 1
    test_create_quiz_question
    sleep 1
    test_get_quiz_by_id
    sleep 1
    test_get_quizzes_by_course
    sleep 1
    test_submit_quiz
    sleep 1
    test_get_quiz_attempts
    sleep 1
    
    # Publishing
    test_publish_course
    sleep 1
    test_get_course_by_slug
    sleep 1
    
    # Cleanup tests (commented out by default)
    test_delete_lesson
    test_delete_quiz
    test_delete_topic
    test_delete_course
    test_delete_category
    
    # Summary
    print_header "TEST SUMMARY"
    print_success "All tests completed!"
    echo ""
    print_info "Created Resources:"
    echo "  - Category ID: $CATEGORY_ID"
    echo "  - Course ID: $COURSE_ID"
    echo "  - Topic ID: $TOPIC_ID"
    echo "  - Lesson ID: $LESSON_ID"
    echo "  - Quiz ID: $QUIZ_ID"
    echo "  - Question ID: $QUESTION_ID"
    echo "  - Instructor ID: $USER_ID"
}

# Run main function
main