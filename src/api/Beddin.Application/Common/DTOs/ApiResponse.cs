// <copyright file="ApiResponse.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// Non-generic version for responses with no data payload.
    /// Used for commands that return only success/failure — e.g. Approve, Suspend.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Gets an optional message describing the result.
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// Gets a collection of error messages, if any.
        /// </summary>
        public IEnumerable<string> Errors { get; init; } = [];

        /// <summary>
        /// Gets the UTC timestamp when the response was created.
        /// </summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a successful <see cref="ApiResponse"/> instance.
        /// </summary>
        /// <param name="message">An optional message describing the result.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success.</returns>
        public static ApiResponse Ok(string? message = null)
        {
            return new()
            {
                Success = true,
                Message = message,
            };
        }

        /// <summary>
        /// Creates a failed <see cref="ApiResponse"/> instance with a single error message.
        /// </summary>
        /// <param name="error">The error message describing the failure.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating failure.</returns>
        public static ApiResponse Fail(string error) =>
            new()
            {
                Success = false,
                Errors = [error],
            };

        /// <summary>
        /// Creates a failed <see cref="ApiResponse"/> instance with multiple error messages.
        /// </summary>
        /// <param name="errors">A collection of error messages describing the failure.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating failure.</returns>
        public static ApiResponse Fail(IEnumerable<string> errors) =>
            new()
            {
                Success = false,
                Errors = errors,
            };
    }
}
