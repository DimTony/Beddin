using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// Standard envelope for all API responses.
    /// Every endpoint returns this shape — success or failure.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public IEnumerable<string> Errors { get; init; } = [];
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new()
            {
                Success = true,
                Message = message,
                Data = data,
            };

        public static ApiResponse<T> Fail(string error) =>
            new()
            {
                Success = false,
                Errors = [error],
            };

        public static ApiResponse<T> Fail(IEnumerable<string> errors) =>
            new()
            {
                Success = false,
                Errors = errors,
            };

        public static ApiResponse<T> Created(T data, string? message = null) =>
            new()
            {
                Success = true,
                Message = message ?? "Resource created successfully",
                Data = data,
            };
    }

    /// <summary>
    /// Non-generic version for responses with no data payload.
    /// Used for commands that return only success/failure — e.g. Approve, Suspend.
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public IEnumerable<string> Errors { get; init; } = [];
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        public static ApiResponse Ok(string? message = null) =>
            new()
            {
                Success = true,
                Message = message,
            };

        public static ApiResponse Fail(string error) =>
            new()
            {
                Success = false,
                Errors = [error],
            };

        public static ApiResponse Fail(IEnumerable<string> errors) =>
            new()
            {
                Success = false,
                Errors = errors,
            };
    }
}
