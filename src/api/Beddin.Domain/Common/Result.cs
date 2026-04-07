// <copyright file="Result.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Domain.Common
{
    /// <summary>
    /// Represents the result of an operation, indicating success or failure.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="isSuccess">Whether the operation succeeded.</param>
        /// <param name="error">The error message if the operation failed.</param>
        protected Result(bool isSuccess, string error)
        {
            this.IsSuccess = isSuccess;
            this.Error = error;
        }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !this.IsSuccess;

        /// <summary>
        /// Gets the error message if the operation failed.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>A successful <see cref="Result"/>.</returns>
        public static Result Success() => new(true, string.Empty);

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns>A failed <see cref="Result"/>.</returns>
        public static Result Failure(string error) => new(false, error);

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A successful <see cref="Result{T}"/>.</returns>
        public static Result<T> Success<T>(T value) => new(value, true, string.Empty);

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="error">The error message.</param>
        /// <returns>A failed <see cref="Result{T}"/>.</returns>
        public static Result<T> Failure<T>(string error) => new(default!, false, error);
    }

    /// <summary>
    /// Represents the result of an operation with a value, indicating success or failure.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
#pragma warning disable SA1402 // File may only contain a single type
    public class Result<T> : Result
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isSuccess">Whether the operation succeeded.</param>
        /// <param name="error">The error message if the operation failed.</param>
        protected internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the result.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Implicitly converts a value to a successful result.
        /// </summary>
        /// <param name="value">The value.</param>
        public static implicit operator Result<T>(T value) => Success(value);
    }
}
