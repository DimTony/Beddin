// <copyright file="ApiResponse{T}.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// Standard envelope for all API responses.
    /// Every endpoint returns this shape — success or failure.
    /// </summary>
    /// <typeparam name="T">The type of the data returned in the API response.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets a value indicating whether the API response indicates success.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Gets an optional message providing additional information about the response.
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// Gets the data returned in the API response.
        /// </summary>
        public T? Data { get; init; }

        /// <summary>
        /// Gets the collection of error messages if the response indicates failure.
        /// </summary>
        public IEnumerable<string> Errors { get; init; } = [];

        /// <summary>
        /// Gets the UTC timestamp when the response was created.
        /// </summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a successful <see cref="ApiResponse{T}"/> with the specified data and optional message.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">An optional message providing additional information about the response.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> indicating success.</returns>
        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new()
            {
                Success = true,
                Message = message,
                Data = data,
            };

        /// <summary>
        /// Creates a failed <see cref="ApiResponse{T}"/> with a single error message.
        /// </summary>
        /// <param name="error">The error message to include in the response.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> indicating failure.</returns>
        public static ApiResponse<T> Fail(string error) =>
            new()
            {
                Success = false,
                Errors = [error],
            };

        /// <summary>
        /// Creates a failed <see cref="ApiResponse{T}"/> with a collection of error messages.
        /// </summary>
        /// <param name="errors">The collection of error messages to include in the response.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> indicating failure.</returns>
        public static ApiResponse<T> Fail(IEnumerable<string> errors) =>
            new()
            {
                Success = false,
                Errors = errors,
            };

        /// <summary>
        /// Creates a successful <see cref="ApiResponse{T}"/> indicating a resource was created.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">An optional message providing additional information about the response.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> indicating resource creation success.</returns>
        public static ApiResponse<T> Created(T data, string? message = null) =>
            new()
            {
                Success = true,
                Message = message ?? "Resource created successfully",
                Data = data,
            };
    }
}
