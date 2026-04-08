// <copyright file="ValidationException.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when validation failures occur.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="failures">The collection of validation failures.</param>
        public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
            : base("One or more validation failures occurred.")
        {
            this.Errors = failures
                .GroupBy(f => f.PropertyName)
                .Select(g => new ValidationError(
                    g.Key,
                    g.Select(f => f.ErrorMessage).ToArray()))
                .ToList();
        }

        /// <summary>
        /// Gets the collection of validation errors.
        /// </summary>
        public IEnumerable<ValidationError> Errors { get; }
    }

    /// <summary>
    /// Represents a validation error for a specific field.
    /// </summary>
    /// <param name="Field">The field name.</param>
    /// <param name="Messages">The error messages for the field.</param>
#pragma warning disable SA1402 // File may only contain a single type
    public record ValidationError(string Field, string[] Messages);
#pragma warning restore SA1402 // File may only contain a single type
}
