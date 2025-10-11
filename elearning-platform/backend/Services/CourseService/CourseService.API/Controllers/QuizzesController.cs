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
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuizAttemptRepository _attemptRepository;

        public QuizzesController(
            IQuizRepository quizRepository,
            IQuestionRepository questionRepository,
            IQuizAttemptRepository attemptRepository)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _attemptRepository = attemptRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuizDto>>> GetById(Guid id)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdWithQuestionsAsync(id);
                if (quiz == null)
                {
                    return NotFound(ApiResponse<QuizDto>.ErrorResponse("Quiz not found", 404));
                }

                var quizDto = MapToQuizDto(quiz);
                return Ok(ApiResponse<QuizDto>.SuccessResponse(quizDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<QuizDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<QuizDto>>>> GetByCourse(Guid courseId)
        {
            try
            {
                var quizzes = await _quizRepository.GetByCourseIdAsync(courseId);
                var quizDtos = quizzes.Select(MapToQuizDto).ToList();
                return Ok(ApiResponse<List<QuizDto>>.SuccessResponse(quizDtos));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<QuizDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QuizDto>>> Create([FromBody] CreateQuizRequestDto request)
        {
            try
            {
                var quiz = new Quiz
                {
                    CourseId = request.CourseId,
                    TopicId = request.TopicId,
                    Title = request.Title,
                    Description = request.Description,
                    PassingScore = request.PassingScore,
                    TimeLimit = request.TimeLimit,
                    MaxAttempts = request.MaxAttempts
                };

                var createdQuiz = await _quizRepository.CreateAsync(quiz);
                var quizDto = MapToQuizDto(createdQuiz);

                return CreatedAtAction(nameof(GetById), new { id = quizDto.Id },
                    ApiResponse<QuizDto>.SuccessResponse(quizDto, "Quiz created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<QuizDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("questions")]
        public async Task<ActionResult<ApiResponse<QuestionDto>>> CreateQuestion([FromBody] CreateQuestionRequestDto request)
        {
            try
            {
                var question = new Question
                {
                    QuizId = request.QuizId,
                    QuestionText = request.QuestionText,
                    QuestionType = request.QuestionType,
                    Points = request.Points,
                    Explanation = request.Explanation,
                    ImageUrl = request.ImageUrl,
                    Options = request.Options.Select(o => new QuestionOption
                    {
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect,
                        DisplayOrder = o.DisplayOrder
                    }).ToList()
                };

                var createdQuestion = await _questionRepository.CreateAsync(question);
                var questionDto = MapToQuestionDto(createdQuestion);

                return Ok(ApiResponse<QuestionDto>.SuccessResponse(questionDto, "Question created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<QuestionDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{id}/submit")]
        public async Task<ActionResult<ApiResponse<QuizResultDto>>> SubmitQuiz(Guid id, [FromBody] SubmitQuizAnswerRequestDto request)
        {
            try
            {
                // Get quiz with questions
                var quiz = await _quizRepository.GetByIdWithQuestionsAsync(id);
                if (quiz == null)
                {
                    return NotFound(ApiResponse<QuizResultDto>.ErrorResponse("Quiz not found", 404));
                }

                // Create quiz attempt
                var attempt = new QuizAttempt
                {
                    UserId = Guid.Parse("00000000-0000-0000-0000-000000000000"), // TODO: Get from JWT
                    QuizId = id,
                    StartedAt = DateTime.UtcNow
                };

                // Grade answers
                int correctAnswers = 0;
                int totalPoints = 0;
                int earnedPoints = 0;
                var questionResults = new List<QuestionResultDto>();

                foreach (var question in quiz.Questions)
                {
                    totalPoints += question.Points;
                    var userAnswer = request.Answers.FirstOrDefault(a => a.QuestionId == question.Id);

                    if (userAnswer != null)
                    {
                        bool isCorrect = false;
                        int pointsAwarded = 0;
                        string correctAnswer = "";

                        if (question.QuestionType == "MultipleChoice")
                        {
                            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                            var selectedOption = question.Options.FirstOrDefault(o => o.Id == userAnswer.SelectedOptionId);
                            
                            isCorrect = selectedOption?.IsCorrect ?? false;
                            correctAnswer = correctOption?.OptionText ?? "";
                            
                            if (isCorrect)
                            {
                                correctAnswers++;
                                pointsAwarded = question.Points;
                                earnedPoints += pointsAwarded;
                            }
                        }

                        // Save answer
                        var quizAnswer = new QuizAnswer
                        {
                            AttemptId = attempt.Id,
                            QuestionId = question.Id,
                            SelectedOptionId = userAnswer.SelectedOptionId,
                            AnswerText = userAnswer.AnswerText,
                            IsCorrect = isCorrect,
                            PointsAwarded = pointsAwarded
                        };

                        if (attempt.Answers == null)
                            attempt.Answers = new List<QuizAnswer>();
                        
                        attempt.Answers.Add(quizAnswer);

                        questionResults.Add(new QuestionResultDto
                        {
                            QuestionId = question.Id,
                            QuestionText = question.QuestionText,
                            IsCorrect = isCorrect,
                            PointsAwarded = pointsAwarded,
                            YourAnswer = question.Options.FirstOrDefault(o => o.Id == userAnswer.SelectedOptionId)?.OptionText,
                            CorrectAnswer = correctAnswer,
                            Explanation = question.Explanation
                        });
                    }
                }

                // Calculate score
                attempt.TotalQuestions = quiz.Questions.Count;
                attempt.CorrectAnswers = correctAnswers;
                attempt.Score = totalPoints > 0 ? (decimal)earnedPoints / totalPoints * 100 : 0;
                attempt.IsPassed = attempt.Score >= quiz.PassingScore;
                attempt.CompletedAt = DateTime.UtcNow;
                attempt.TimeSpent = (int)(attempt.CompletedAt.Value - attempt.StartedAt).TotalSeconds;

                // Save attempt
                var savedAttempt = await _attemptRepository.CreateAsync(attempt);

                var result = new QuizResultDto
                {
                    AttemptId = savedAttempt.Id,
                    Score = savedAttempt.Score,
                    TotalQuestions = savedAttempt.TotalQuestions,
                    CorrectAnswers = savedAttempt.CorrectAnswers,
                    IsPassed = savedAttempt.IsPassed,
                    TimeSpent = savedAttempt.TimeSpent ?? 0,
                    QuestionResults = questionResults
                };

                return Ok(ApiResponse<QuizResultDto>.SuccessResponse(result, "Quiz submitted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<QuizResultDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}/attempts/{userId}")]
        public async Task<ActionResult<ApiResponse<List<QuizAttempt>>>> GetAttempts(Guid id, Guid userId)
        {
            try
            {
                var attempts = await _attemptRepository.GetByQuizIdAsync(id, userId);
                return Ok(ApiResponse<List<QuizAttempt>>.SuccessResponse(attempts));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<QuizAttempt>>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                await _quizRepository.DeleteAsync(id);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Quiz deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        private QuizDto MapToQuizDto(Quiz quiz)
        {
            return new QuizDto
            {
                Id = quiz.Id,
                CourseId = quiz.CourseId,
                TopicId = quiz.TopicId,
                Title = quiz.Title,
                Description = quiz.Description,
                PassingScore = quiz.PassingScore,
                TimeLimit = quiz.TimeLimit,
                MaxAttempts = quiz.MaxAttempts,
                IsActive = quiz.IsActive,
                Questions = quiz.Questions?.Select(MapToQuestionDto).ToList()
            };
        }

        private QuestionDto MapToQuestionDto(Question question)
        {
            return new QuestionDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType,
                Points = question.Points,
                Explanation = question.Explanation,
                ImageUrl = question.ImageUrl,
                Options = question.Options?.Select(o => new QuestionOptionDto
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect
                }).ToList()
            };
        }
    }
}