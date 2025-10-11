using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CertificateService.Core.Entities;

namespace CertificateService.Infrastructure.Services
{
    /// <summary>
    /// Certificate PDF generator using QuestPDF
    /// </summary>
    public class CertificateGenerator : ICertificateGenerator
    {
        public byte[] GenerateCertificatePdf(Certificate certificate)
        {
            // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content().Column(column =>
                    {
                        // Border
                        column.Item().Border(3).BorderColor(Colors.Blue.Darken2).Padding(30).Column(innerColumn =>
                        {
                            // Header
                            innerColumn.Item().AlignCenter().Text("CERTIFICATE OF COMPLETION")
                                .FontSize(32)
                                .FontColor(Colors.Blue.Darken3)
                                .Bold();

                            innerColumn.Item().PaddingVertical(10);

                            // Subtitle
                            innerColumn.Item().AlignCenter().Text("This is to certify that")
                                .FontSize(16)
                                .FontColor(Colors.Grey.Darken2);

                            innerColumn.Item().PaddingVertical(10);

                            // Recipient name
                            innerColumn.Item().AlignCenter().Text(certificate.UserFullName)
                                .FontSize(28)
                                .FontColor(Colors.Blue.Darken3)
                                .Bold();

                            innerColumn.Item().PaddingVertical(10);

                            // Course completion text
                            innerColumn.Item().AlignCenter().Text("has successfully completed the course")
                                .FontSize(16)
                                .FontColor(Colors.Grey.Darken2);

                            innerColumn.Item().PaddingVertical(10);

                            // Course name
                            innerColumn.Item().AlignCenter().Text(certificate.CourseTitle)
                                .FontSize(24)
                                .FontColor(Colors.Blue.Darken3)
                                .Bold();

                            innerColumn.Item().PaddingVertical(15);

                            // Course details
                            innerColumn.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().AlignCenter().Text($"Duration: {certificate.TotalHours} hours")
                                        .FontSize(12)
                                        .FontColor(Colors.Grey.Darken1);
                                    
                                    if (certificate.FinalScore.HasValue)
                                    {
                                        col.Item().PaddingTop(5).AlignCenter().Text($"Score: {certificate.FinalScore.Value:F2}%")
                                            .FontSize(12)
                                            .FontColor(Colors.Grey.Darken1);
                                    }
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().AlignCenter().Text($"Completion Date: {certificate.CompletionDate:MMMM dd, yyyy}")
                                        .FontSize(12)
                                        .FontColor(Colors.Grey.Darken1);
                                    
                                    if (!string.IsNullOrEmpty(certificate.Grade))
                                    {
                                        col.Item().PaddingTop(5).AlignCenter().Text($"Grade: {certificate.Grade}")
                                            .FontSize(12)
                                            .FontColor(Colors.Grey.Darken1);
                                    }
                                });
                            });

                            innerColumn.Item().PaddingVertical(20);

                            // Instructor signature section
                            innerColumn.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().BorderTop(1).BorderColor(Colors.Grey.Medium).PaddingTop(5)
                                        .AlignCenter().Text(certificate.InstructorName)
                                        .FontSize(12)
                                        .FontColor(Colors.Grey.Darken2);
                                    
                                    col.Item().AlignCenter().Text("Instructor")
                                        .FontSize(10)
                                        .FontColor(Colors.Grey.Darken1);
                                });

                                row.ConstantItem(100); // Spacer

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().BorderTop(1).BorderColor(Colors.Grey.Medium).PaddingTop(5)
                                        .AlignCenter().Text("E-Learning Platform")
                                        .FontSize(12)
                                        .FontColor(Colors.Grey.Darken2);
                                    
                                    col.Item().AlignCenter().Text("Platform Administrator")
                                        .FontSize(10)
                                        .FontColor(Colors.Grey.Darken1);
                                });
                            });

                            innerColumn.Item().PaddingVertical(15);

                            // Certificate number and verification code
                            innerColumn.Item().AlignCenter().Column(col =>
                            {
                                col.Item().Text($"Certificate No: {certificate.CertificateNumber}")
                                    .FontSize(10)
                                    .FontColor(Colors.Grey.Darken1);
                                
                                col.Item().PaddingTop(3).Text($"Verification Code: {certificate.VerificationCode}")
                                    .FontSize(10)
                                    .FontColor(Colors.Grey.Darken1);
                                
                                col.Item().PaddingTop(3).Text("Verify at: https://elearning.com/verify")
                                    .FontSize(10)
                                    .FontColor(Colors.Blue.Medium);
                            });
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Issued on: ").FontSize(10).FontColor(Colors.Grey.Darken1);
                        text.Span($"{certificate.IssueDate:MMMM dd, yyyy}").FontSize(10).FontColor(Colors.Grey.Darken2).Bold();
                    });
                });
            });

            // Generate PDF bytes
            return document.GeneratePdf();
        }

        public string SaveCertificatePdf(Certificate certificate, string savePath)
        {
            var pdfBytes = GenerateCertificatePdf(certificate);
            var fileName = $"certificate_{certificate.CertificateNumber}.pdf";
            var fullPath = Path.Combine(savePath, fileName);

            // Ensure directory exists
            Directory.CreateDirectory(savePath);

            // Save to file
            File.WriteAllBytes(fullPath, pdfBytes);

            return fullPath;
        }
    }

    public interface ICertificateGenerator
    {
        byte[] GenerateCertificatePdf(Certificate certificate);
        string SaveCertificatePdf(Certificate certificate, string savePath);
    }
}