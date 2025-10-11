using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Common.DTOs;
using Shared.MessageQueue;
using Shared.MessageQueue.Events;
using CertificateService.Core.Entities;
using CertificateService.Infrastructure.Data;
using CertificateService.Infrastructure.Services;

namespace CertificateService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificatesController : ControllerBase
    {
        private readonly CertificateDbContext _context;
        private readonly ICertificateGenerator _certificateGenerator;
        private readonly IMessageQueueService _messageQueue;

        public CertificatesController(
            CertificateDbContext context,
            ICertificateGenerator certificateGenerator,
            IMessageQueueService messageQueue)
        {
            _context = context;
            _certificateGenerator = certificateGenerator;
            _messageQueue = messageQueue;

            // Subscribe to course completed events
            _messageQueue.Subscribe<CourseCompletedEvent>("course.completed", OnCourseCompleted);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<Certificate>>>> GetUserCertificates(Guid userId)
        {
            try
            {
                var certificates = await _context.Certificates
                    .Where(c => c.UserId == userId && c.Status == "Active")
                    .OrderByDescending(c => c.IssueDate)
                    .ToListAsync();

                return Ok(ApiResponse<List<Certificate>>.SuccessResponse(certificates));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<Certificate>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Certificate>>> GetById(Guid id)
        {
            try
            {
                var certificate = await _context.Certificates.FindAsync(id);
                if (certificate == null)
                {
                    return NotFound(ApiResponse<Certificate>.ErrorResponse("Certificate not found", 404));
                }

                return Ok(ApiResponse<Certificate>.SuccessResponse(certificate));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Certificate>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Certificate>>> IssueCertificate([FromBody] IssueCertificateRequest request)
        {
            try
            {
                // Check if certificate already exists
                var existing = await _context.Certificates
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.CourseId == request.CourseId);

                if (existing != null)
                {
                    return BadRequest(ApiResponse<Certificate>.ErrorResponse("Certificate already issued"));
                }

                // Create certificate
                var certificate = new Certificate
                {
                    CertificateNumber = GenerateCertificateNumber(),
                    UserId = request.UserId,
                    CourseId = request.CourseId,
                    UserFullName = request.UserFullName,
                    CourseTitle = request.CourseTitle,
                    InstructorName = request.InstructorName,
                    CompletionDate = request.CompletionDate,
                    FinalScore = request.FinalScore,
                    Grade = CalculateGrade(request.FinalScore),
                    TotalHours = request.TotalHours,
                    VerificationCode = GenerateVerificationCode(),
                    Status = "Active"
                };

                _context.Certificates.Add(certificate);
                await _context.SaveChangesAsync();

                // Generate PDF
                var pdfBytes = _certificateGenerator.GenerateCertificatePdf(certificate);
                var pdfPath = await SaveCertificatePdf(certificate.Id, pdfBytes);
                
                certificate.PdfUrl = pdfPath;
                await _context.SaveChangesAsync();

                // Publish certificate issued event
                await _messageQueue.PublishAsync("certificate.issued", new CertificateIssuedEvent
                {
                    CertificateId = certificate.Id,
                    UserId = certificate.UserId,
                    CourseId = certificate.CourseId,
                    CertificateNumber = certificate.CertificateNumber,
                    PdfUrl = certificate.PdfUrl
                });

                return Ok(ApiResponse<Certificate>.SuccessResponse(certificate, "Certificate issued successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<Certificate>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadCertificate(Guid id)
        {
            try
            {
                var certificate = await _context.Certificates.FindAsync(id);
                if (certificate == null)
                {
                    return NotFound("Certificate not found");
                }

                var pdfBytes = _certificateGenerator.GenerateCertificatePdf(certificate);
                return File(pdfBytes, "application/pdf", $"certificate_{certificate.CertificateNumber}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("verify/{code}")]
        public async Task<ActionResult<ApiResponse<CertificateVerificationResult>>> VerifyCertificate(string code)
        {
            try
            {
                var certificate = await _context.Certificates
                    .FirstOrDefaultAsync(c => c.VerificationCode == code);

                if (certificate == null)
                {
                    return NotFound(ApiResponse<CertificateVerificationResult>.ErrorResponse(
                        "Certificate not found or invalid verification code", 404));
                }

                // Log verification
                var verification = new CertificateVerification
                {
                    CertificateId = certificate.Id,
                    VerificationCode = code,
                    VerifiedBy = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                _context.CertificateVerifications.Add(verification);
                await _context.SaveChangesAsync();

                var result = new CertificateVerificationResult
                {
                    IsValid = certificate.Status == "Active",
                    CertificateNumber = certificate.CertificateNumber,
                    UserFullName = certificate.UserFullName,
                    CourseTitle = certificate.CourseTitle,
                    IssueDate = certificate.IssueDate,
                    ExpiryDate = certificate.ExpiryDate,
                    Status = certificate.Status
                };

                return Ok(ApiResponse<CertificateVerificationResult>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CertificateVerificationResult>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{id}/share")]
        public async Task<ActionResult<ApiResponse<bool>>> ShareCertificate(Guid id, [FromBody] ShareCertificateRequest request)
        {
            try
            {
                var share = new CertificateShare
                {
                    CertificateId = id,
                    Platform = request.Platform
                };

                _context.CertificateShares.Add(share);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Certificate shared successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        private void OnCourseCompleted(CourseCompletedEvent evt)
        {
            // Handle course completion and issue certificate
            // This would typically be done via a background job
            var request = new IssueCertificateRequest
            {
                UserId = evt.UserId,
                CourseId = evt.CourseId,
                UserFullName = evt.UserFullName,
                CourseTitle = evt.CourseTitle,
                InstructorName = "Instructor", // Get from course data
                CompletionDate = DateTime.UtcNow,
                FinalScore = evt.FinalScore,
                TotalHours = evt.TotalHours
            };

            // Issue certificate asynchronously
            _ = IssueCertificate(request);
        }

        private string GenerateCertificateNumber()
        {
            return $"CERT{DateTime.UtcNow:yyyyMMdd}{new Random().Next(10000, 99999)}";
        }

        private string GenerateVerificationCode()
        {
            return Guid.NewGuid().ToString("N").ToUpper().Substring(0, 12);
        }

        private string CalculateGrade(decimal? score)
        {
            if (!score.HasValue) return "Pass";

            return score.Value switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _ => "F"
            };
        }

        private async Task<string> SaveCertificatePdf(Guid certificateId, byte[] pdfBytes)
        {
            var fileName = $"certificate_{certificateId}.pdf";
            var uploadPath = Path.Combine("uploads", "certificates");
            Directory.CreateDirectory(uploadPath);
            
            var filePath = Path.Combine(uploadPath, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
            
            return $"/uploads/certificates/{fileName}";
        }
    }

    // Request/Response DTOs
    public class IssueCertificateRequest
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string UserFullName { get; set; }
        public string CourseTitle { get; set; }
        public string InstructorName { get; set; }
        public DateTime CompletionDate { get; set; }
        public decimal? FinalScore { get; set; }
        public int TotalHours { get; set; }
    }

    public class CertificateVerificationResult
    {
        public bool IsValid { get; set; }
        public string CertificateNumber { get; set; }
        public string UserFullName { get; set; }
        public string CourseTitle { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; }
    }

    public class ShareCertificateRequest
    {
        public string Platform { get; set; }
    }
}