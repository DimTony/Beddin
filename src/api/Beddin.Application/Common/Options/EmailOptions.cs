// <copyright file="EmailOptions.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.Options
{
    /// <summary>
    /// Represents configuration options for email functionality.
    /// </summary>
    public class EmailOptions
    {
        /// <summary>
        /// Gets the configuration section name for email options.
        /// </summary>
        public const string SectionName = "EmailOptions";

        /// <summary>
        /// Gets or sets the base URL used in email templates.
        /// </summary>
        public string BaseUrl { get; set; } = default!;

        /// <summary>
        /// Gets or sets the secret used for test JWT generation.
        /// </summary>
        public string TestJwtSecret { get; set; } = default!;

        /// <summary>
        /// Gets the SMTP server address.
        /// </summary>
        public string SmtpServer { get; init; } = default!;

        /// <summary>
        /// Gets the port number for the SMTP server.
        /// </summary>
        public int Port { get; init; } = default!;

        /// <summary>
        /// Gets the sender email address.
        /// </summary>
        public string FromEmail { get; init; } = default!;

        /// <summary>
        /// Gets the sender display name.
        /// </summary>
        public string FromName { get; init; } = default!;

        /// <summary>
        /// Gets the username for SMTP authentication.
        /// </summary>
        public string Username { get; init; } = default!;

        /// <summary>
        /// Gets the password for SMTP authentication.
        /// </summary>
        public string Password { get; init; } = default!;
    }
}
