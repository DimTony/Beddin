// <copyright file="CurrentUserService.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Security.Claims;
using Beddin.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Beddin.Infrastructure.Services
{
    /// <summary>
    /// Provides information about the current user from the HTTP context.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor used to retrieve information about the current user.</param>
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public Guid? UserId
        {
            get
            {
                // Prefer the raw JWT "sub" claim — works regardless of MapInboundClaims
                var value = this.httpContextAccessor.HttpContext?.User
                    .FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? this.httpContextAccessor.HttpContext?.User
                       .FindFirstValue(ClaimTypes.NameIdentifier); // fallback for mapped tokens

                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        /// <inheritdoc/>
        public string? IpAddress =>
            this.httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        /// <inheritdoc/>
        public string? UserAgent =>
          this.httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

        /// <inheritdoc/>
        public Guid? SessionId
        {
            get
            {
                var value = this.httpContextAccessor.HttpContext?
                    .User.FindFirstValue("session_id");
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        /// <inheritdoc/>
        public string? Role =>
            this.httpContextAccessor.HttpContext?.User
                .FindFirstValue("role")
                ?? this.httpContextAccessor.HttpContext?.User
                   .FindFirstValue(ClaimTypes.Role);

        /// <inheritdoc/>
        public string? Name =>
            this.httpContextAccessor.HttpContext?.User
                .FindFirstValue(JwtRegisteredClaimNames.Name)
                ?? this.httpContextAccessor.HttpContext?.User
                   .FindFirstValue(ClaimTypes.Name);

        /// <inheritdoc/>
        public string? Email =>
            this.httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    }
}
