using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.DTOs;
using CourseService.Core.DTOs;
using CourseService.Core.Entities;
using CourseService.Core.Interfaces;

namespace CourseService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CoursesController(
            ICourseRepository courseRepository,
            ICategoryRepository categoryRepository)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<CourseDto>>> GetAll([FromQuery] PaginationRequest request, [FromQuery] Guid? categoryId = null, [FromQuery] string level = null)
        {
            try
            {
                var courses = await _courseRepository.GetAllAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.SearchTerm,
                    categoryId,
                    level
                );

                var totalCount = await _courseRepository.GetTotalCountAsync(
                    request.SearchTerm,
                    categoryId,
                    level
                );

                var courseDtos = courses.Select(MapToCourseDto).ToList();

                return Ok(PaginatedResponse<CourseDto>.Create(
                    courseDtos,
                    request.PageNumber,
                    request.PageSize,
                    totalCount
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CourseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> GetById(Guid id)
        {
            try
            {
                var course = await _courseRepository.GetByIdWithDetailsAsync(id);
                if (course == null)
                {
                    return NotFound(ApiResponse<CourseDto>.ErrorResponse("Course not found", 404));
                }

                var courseDto = MapToCourseDtoWithDetails(course);
                return Ok(ApiResponse<CourseDto>.SuccessResponse(courseDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CourseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> GetBySlug(string slug)
        {
            try
            {
                var course = await _courseRepository.GetBySlugAsync(slug);
                if (course == null)
                {
                    return NotFound(ApiResponse<CourseDto>.ErrorResponse("Course not found", 404));
                }

                var detailedCourse = await _courseRepository.GetByIdWithDetailsAsync(course.Id);
                var courseDto = MapToCourseDtoWithDetails(detailedCourse);
                return Ok(ApiResponse<CourseDto>.SuccessResponse(courseDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CourseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("featured")]
        public async Task<ActionResult<ApiResponse<List<CourseDto>>>> GetFeatured([FromQuery] int count = 10)
        {
            try
            {
                var courses = await _courseRepository.GetFeaturedCoursesAsync(count);
                var courseDtos = courses.Select(MapToCourseDto).ToList();
                return Ok(ApiResponse<List<CourseDto>>.SuccessResponse(courseDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CourseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("instructor/{instructorId}")]
        public async Task<ActionResult<ApiResponse<List<CourseDto>>>> GetByInstructor(Guid instructorId)
        {
            try
            {
                var courses = await _courseRepository.GetCoursesByInstructorAsync(instructorId);
                var courseDtos = courses.Select(MapToCourseDto).ToList();
                return Ok(ApiResponse<List<CourseDto>>.SuccessResponse(courseDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CourseDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Create([FromBody] CreateCourseRequestDto request)
        {
            try
            {
                var course = new Course
                {
                    Title = request.Title,
                    ShortDescription = request.ShortDescription,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    InstructorId = request.InstructorId,
                    ThumbnailUrl = request.ThumbnailUrl,
                    PreviewVideoUrl = request.PreviewVideoUrl,
                    Level = request.Level,
                    Language = request.Language,
                    PriceThb = request.PriceThb,
                    DiscountPriceThb = request.DiscountPriceThb,
                    IsPublished = false
                };

                var createdCourse = await _courseRepository.CreateAsync(course);
                var courseDto = MapToCourseDto(createdCourse);

                return CreatedAtAction(nameof(GetById), new { id = courseDto.Id }, 
                    ApiResponse<CourseDto>.SuccessResponse(courseDto, "Course created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CourseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Update(Guid id, [FromBody] UpdateCourseRequestDto request)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null)
                {
                    return NotFound(ApiResponse<CourseDto>.ErrorResponse("Course not found", 404));
                }

                course.Title = request.Title;
                course.ShortDescription = request.ShortDescription;
                course.Description = request.Description;
                course.CategoryId = request.CategoryId;
                course.ThumbnailUrl = request.ThumbnailUrl;
                course.PreviewVideoUrl = request.PreviewVideoUrl;
                course.Level = request.Level;
                course.Language = request.Language;
                course.PriceThb = request.PriceThb;
                course.DiscountPriceThb = request.DiscountPriceThb;
                course.MetaTitle = request.MetaTitle;
                course.MetaDescription = request.MetaDescription;
                course.MetaKeywords = request.MetaKeywords;

                var updatedCourse = await _courseRepository.UpdateAsync(course);
                var courseDto = MapToCourseDto(updatedCourse);

                return Ok(ApiResponse<CourseDto>.SuccessResponse(courseDto, "Course updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CourseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{id}/publish")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Publish(Guid id)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null)
                {
                    return NotFound(ApiResponse<CourseDto>.ErrorResponse("Course not found", 404));
                }

                course.IsPublished = true;
                course.PublishedAt = DateTime.UtcNow;

                var updatedCourse = await _courseRepository.UpdateAsync(course);
                var courseDto = MapToCourseDto(updatedCourse);

                return Ok(ApiResponse<CourseDto>.SuccessResponse(courseDto, "Course published successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CourseDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                var exists = await _courseRepository.ExistsAsync(id);
                if (!exists)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("Course not found", 404));
                }

                await _courseRepository.DeleteAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Course deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        private CourseDto MapToCourseDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Slug = course.Slug,
                ShortDescription = course.ShortDescription,
                Description = course.Description,
                CategoryId = course.CategoryId,
                CategoryName = course.Category?.Name,
                InstructorId = course.InstructorId,
                ThumbnailUrl = course.ThumbnailUrl,
                PreviewVideoUrl = course.PreviewVideoUrl,
                Level = course.Level,
                Language = course.Language,
                PriceThb = course.PriceThb,
                DiscountPriceThb = course.DiscountPriceThb,
                EffectivePrice = course.EffectivePrice,
                HasDiscount = course.HasDiscount,
                Duration = course.Duration,
                IsPublished = course.IsPublished,
                PublishedAt = course.PublishedAt,
                IsFeatured = course.IsFeatured,
                EnrollmentCount = course.EnrollmentCount,
                AverageRating = course.AverageRating,
                TotalRatings = course.TotalRatings,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            };
        }

        private CourseDto MapToCourseDtoWithDetails(Course course)
        {
            var dto = MapToCourseDto(course);
            
            if (course.Topics != null)
            {
                dto.Topics = course.Topics.Select(t => new CourseTopicDto
                {
                    Id = t.Id,
                    CourseId = t.CourseId,
                    Title = t.Title,
                    Description = t.Description,
                    DisplayOrder = t.DisplayOrder,
                    Duration = t.Duration,
                    IsActive = t.IsActive,
                    Lessons = t.Lessons?.Select(l => new LessonDto
                    {
                        Id = l.Id,
                        TopicId = l.TopicId,
                        Title = l.Title,
                        Content = l.Content,
                        VideoUrl = l.VideoUrl,
                        Duration = l.Duration,
                        DisplayOrder = l.DisplayOrder,
                        IsFree = l.IsFree,
                        IsActive = l.IsActive,
                        Resources = l.Resources?.Select(r => new LessonResourceDto
                        {
                            Id = r.Id,
                            Title = r.Title,
                            Description = r.Description,
                            ResourceType = r.ResourceType,
                            ResourceUrl = r.ResourceUrl,
                            FileSize = r.FileSize
                        }).ToList()
                    }).ToList()
                }).ToList();
            }

            return dto;
        }
    }
}