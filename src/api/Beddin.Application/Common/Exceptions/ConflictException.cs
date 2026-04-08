// <copyright file="ConflictException.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when an operation conflicts with current system state.
    /// e.g. duplicate membership number, duplicate journal entry number.
    /// Maps to HTTP 409.
    /// </summary>
    public class ConflictException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}
