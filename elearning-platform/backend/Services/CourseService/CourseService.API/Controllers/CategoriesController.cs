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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IconUrl = c.IconUrl,
                    CourseCount = c.Courses?.Count ?? 0
                }).ToList();

                return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(categoryDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CategoryDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(ApiResponse<CategoryDto>.ErrorResponse("Category not found", 404));
                }

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IconUrl = category.IconUrl
                };

                return Ok(ApiResponse<CategoryDto>.SuccessResponse(categoryDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CategoryDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] Category request)
        {
            try
            {
                var category = await _categoryRepository.CreateAsync(request);
                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IconUrl = category.IconUrl
                };

                return CreatedAtAction(nameof(GetById), new { id = categoryDto.Id },
                    ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Category created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CategoryDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, [FromBody] Category request)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(ApiResponse<CategoryDto>.ErrorResponse("Category not found", 404));
                }

                category.Name = request.Name;
                category.Description = request.Description;
                category.IconUrl = request.IconUrl;
                category.DisplayOrder = request.DisplayOrder;

                var updatedCategory = await _categoryRepository.UpdateAsync(category);
                var categoryDto = new CategoryDto
                {
                    Id = updatedCategory.Id,
                    Name = updatedCategory.Name,
                    Description = updatedCategory.Description,
                    IconUrl = updatedCategory.IconUrl
                };

                return Ok(ApiResponse<CategoryDto>.SuccessResponse(categoryDto, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CategoryDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                await _categoryRepository.DeleteAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TopicsController : ControllerBase
    {
        private readonly ICourseTopicRepository _topicRepository;

        public TopicsController(ICourseTopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseTopicDto>>> GetById(Guid id)
        {
            try
            {
                var topic = await _topicRepository.GetByIdWithLessonsAsync(id);
                if (topic == null)
                {
                    return NotFound(ApiResponse<CourseTopicDto>.ErrorResponse("Topic not found", 404));
                }

                var topicDto = MapToTopicDto(topic);
                return Ok(ApiResponse<CourseTopicDto>.SuccessResponse(topicDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CourseTopicDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CourseTopicDto>>>> GetByCourse(Guid courseId)
        {
            try
            {
                var topics = await _topicRepository.GetByCourseIdAsync(courseId);
                var topicDtos = topics.Select(MapToTopicDto).ToList();
                return Ok(ApiResponse<List<CourseTopicDto>>.SuccessResponse(topicDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CourseTopicDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseTopicDto>>> Create([FromBody] CreateTopicRequestDto request)
        {
            try
            {
                var topic = new CourseTopic
                {
                    CourseId = request.CourseId,
                    Title = request.Title,
                    Description = request.Description,
                    DisplayOrder = request.DisplayOrder
                };

                var createdTopic = await _topicRepository.CreateAsync(topic);
                var topicDto = MapToTopicDto(createdTopic);

                return CreatedAtAction(nameof(GetById), new { id = topicDto.Id },
                    ApiResponse<CourseTopicDto>.SuccessResponse(topicDto, "Topic created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CourseTopicDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CourseTopicDto>>> Update(Guid id, [FromBody] CreateTopicRequestDto request)
        {
            try
            {
                var topic = await _topicRepository.GetByIdAsync(id);
                if (topic == null)
                {
                    return NotFound(ApiResponse<CourseTopicDto>.ErrorResponse("Topic not found", 404));
                }

                topic.Title = request.Title;
                topic.Description = request.Description;
                topic.DisplayOrder = request.DisplayOrder;

                var updatedTopic = await _topicRepository.UpdateAsync(topic);
                var topicDto = MapToTopicDto(updatedTopic);

                return Ok(ApiResponse<CourseTopicDto>.SuccessResponse(topicDto, "Topic updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CourseTopicDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                await _topicRepository.DeleteAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Topic deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        private CourseTopicDto MapToTopicDto(CourseTopic topic)
        {
            return new CourseTopicDto
            {
                Id = topic.Id,
                CourseId = topic.CourseId,
                Title = topic.Title,
                Description = topic.Description,
                DisplayOrder = topic.DisplayOrder,
                Duration = topic.Duration,
                IsActive = topic.IsActive,
                Lessons = topic.Lessons?.Select(l => new LessonDto
                {
                    Id = l.Id,
                    TopicId = l.TopicId,
                    Title = l.Title,
                    Content = l.Content,
                    VideoUrl = l.VideoUrl,
                    Duration = l.Duration,
                    DisplayOrder = l.DisplayOrder,
                    IsFree = l.IsFree,
                    IsActive = l.IsActive
                }).ToList()
            };
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonsController(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LessonDto>>> GetById(Guid id)
        {
            try
            {
                var lesson = await _lessonRepository.GetByIdWithResourcesAsync(id);
                if (lesson == null)
                {
                    return NotFound(ApiResponse<LessonDto>.ErrorResponse("Lesson not found", 404));
                }

                var lessonDto = MapToLessonDto(lesson);
                return Ok(ApiResponse<LessonDto>.SuccessResponse(lessonDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LessonDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("topic/{topicId}")]
        public async Task<ActionResult<ApiResponse<List<LessonDto>>>> GetByTopic(Guid topicId)
        {
            try
            {
                var lessons = await _lessonRepository.GetByTopicIdAsync(topicId);
                var lessonDtos = lessons.Select(MapToLessonDto).ToList();
                return Ok(ApiResponse<List<LessonDto>>.SuccessResponse(lessonDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<LessonDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<LessonDto>>> Create([FromBody] CreateLessonRequestDto request)
        {
            try
            {
                var lesson = new Lesson
                {
                    TopicId = request.TopicId,
                    Title = request.Title,
                    Content = request.Content,
                    VideoUrl = request.VideoUrl,
                    Duration = request.Duration,
                    DisplayOrder = request.DisplayOrder,
                    IsFree = request.IsFree
                };

                var createdLesson = await _lessonRepository.CreateAsync(lesson);
                var lessonDto = MapToLessonDto(createdLesson);

                return CreatedAtAction(nameof(GetById), new { id = lessonDto.Id },
                    ApiResponse<LessonDto>.SuccessResponse(lessonDto, "Lesson created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LessonDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<LessonDto>>> Update(Guid id, [FromBody] CreateLessonRequestDto request)
        {
            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(id);
                if (lesson == null)
                {
                    return NotFound(ApiResponse<LessonDto>.ErrorResponse("Lesson not found", 404));
                }

                lesson.Title = request.Title;
                lesson.Content = request.Content;
                lesson.VideoUrl = request.VideoUrl;
                lesson.Duration = request.Duration;
                lesson.DisplayOrder = request.DisplayOrder;
                lesson.IsFree = request.IsFree;

                var updatedLesson = await _lessonRepository.UpdateAsync(lesson);
                var lessonDto = MapToLessonDto(updatedLesson);

                return Ok(ApiResponse<LessonDto>.SuccessResponse(lessonDto, "Lesson updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LessonDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                await _lessonRepository.DeleteAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Lesson deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        private LessonDto MapToLessonDto(Lesson lesson)
        {
            return new LessonDto
            {
                Id = lesson.Id,
                TopicId = lesson.TopicId,
                Title = lesson.Title,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                Duration = lesson.Duration,
                DisplayOrder = lesson.DisplayOrder,
                IsFree = lesson.IsFree,
                IsActive = lesson.IsActive,
                Resources = lesson.Resources?.Select(r => new LessonResourceDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    ResourceType = r.ResourceType,
                    ResourceUrl = r.ResourceUrl,
                    FileSize = r.FileSize
                }).ToList()
            };
        }
    }
}