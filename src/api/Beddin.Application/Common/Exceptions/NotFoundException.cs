// <copyright file="NotFoundException.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="resource">The name of the resource that was not found.</param>
        /// <param name="key">The key of the resource that was not found.</param>
        public NotFoundException(string resource, object key)
            : base($"{resource} with key '{key}' was not found.")
        {
        }
    }
}
