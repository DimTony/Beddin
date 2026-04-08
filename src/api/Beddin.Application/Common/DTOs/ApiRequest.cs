// <copyright file="ApiRequest.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

namespace Beddin.Application.Common.DTOs
{
    /// <summary>
    /// Represents a base class for API request DTOs, containing common properties such as IP address and user agent.
    /// </summary>
    public abstract record ApiRequest
    {
        private string ipAddress = null!;
        private string userAgent = null!;

        /// <summary>
        /// Gets or sets the IP address of the client making the API request.
        /// </summary>
        public string IpAddress
        {
            get => this.ipAddress;
            set => this.ipAddress = value;
        }

        /// <summary>
        /// Gets or sets the user agent string of the client making the API request.
        /// </summary>
        public string UserAgent
        {
            get => this.userAgent;
            set => this.userAgent = value;
        }
    }
}
