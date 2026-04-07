// <copyright file="ForbiddenException.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a user attempts to perform an action that they do not have permission to perform.
    /// </summary>
    public class ForbiddenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ForbiddenException(string? message = null)
            : base(message ?? "You do not have permission to perform this action.")
        {
        }
    }
}
