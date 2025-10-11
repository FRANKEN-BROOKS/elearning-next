using System.Collections.Generic;

namespace Shared.Common.DTOs
{
    /// <summary>
    /// Standard API response wrapper
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = 200
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = new List<string> { message }
            };
        }

        public static ApiResponse<T> ErrorResponse(List<string> errors, string message = "Error occurred", int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Paginated API response
    /// </summary>
    public class PaginatedResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public static PaginatedResponse<T> Create(List<T> data, int currentPage, int pageSize, int totalRecords)
        {
            var totalPages = (int)System.Math.Ceiling(totalRecords / (double)pageSize);

            return new PaginatedResponse<T>
            {
                Success = true,
                Message = "Success",
                Data = data,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords,
                HasNextPage = currentPage < totalPages,
                HasPreviousPage = currentPage > 1
            };
        }
    }
}