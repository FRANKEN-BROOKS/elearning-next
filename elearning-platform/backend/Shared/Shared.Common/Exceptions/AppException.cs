using System;

namespace Shared.Common.Exceptions
{
    /// <summary>
    /// Base exception for the application
    /// </summary>
    public abstract class AppException : Exception
    {
        public int StatusCode { get; set; }

        protected AppException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }

        protected AppException(string message, Exception innerException, int statusCode = 400) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Exception thrown when entity is not found
    /// </summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, 404)
        {
        }

        public NotFoundException(string entityName, object key) 
            : base($"{entityName} with id '{key}' was not found.", 404)
        {
        }
    }

    /// <summary>
    /// Exception thrown when validation fails
    /// </summary>
    public class ValidationException : AppException
    {
        public ValidationException(string message) : base(message, 400)
        {
        }
    }

    /// <summary>
    /// Exception thrown when unauthorized access is attempted
    /// </summary>
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized access") : base(message, 401)
        {
        }
    }

    /// <summary>
    /// Exception thrown when forbidden access is attempted
    /// </summary>
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Forbidden access") : base(message, 403)
        {
        }
    }

    /// <summary>
    /// Exception thrown when business rule is violated
    /// </summary>
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message) : base(message, 422)
        {
        }
    }

    /// <summary>
    /// Exception thrown when conflict occurs
    /// </summary>
    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message, 409)
        {
        }
    }
}