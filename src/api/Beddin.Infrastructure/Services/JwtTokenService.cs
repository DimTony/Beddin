// <copyright file="JwtTokenService.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Beddin.Infrastructure.Services
{
    /// <summary>
    /// Implements the <see cref="ITokenService"/> interface to provide functionality for generating and validating JWT access tokens and refresh tokens. This service uses configuration settings for the JWT secret key, issuer, audience, and token expiration time. It also includes robust error handling and logging for token validation failures, ensuring that issues such as expired tokens, invalid signatures, and other token-related errors are properly logged and handled without exposing sensitive information.
    /// </summary>
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<JwtTokenService> logger;
        private readonly string secretKey;
        private readonly string issuer;
        private readonly string audience;
        private readonly int expirationMinutes;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="logger">The logger instance.</param>
        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.secretKey = this.configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            this.issuer = this.configuration["Jwt:Issuer"] ?? "BeddinAPI";
            this.audience = this.configuration["Jwt:Audience"] ?? "BeddinClient";
            this.expirationMinutes = int.Parse(this.configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        /// <inheritdoc/>
        public string GenerateAccessToken(User user, Guid sessionId, IEnumerable<Claim>? additionalClaims = null)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim("role", user.RoleId.ToString()),
                new Claim("session_id", sessionId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (user.MustChangePassword)
            {
                claims.Add(new Claim("must_change_password", "true"));
            }

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: this.issuer,
                audience: this.audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(this.expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <inheritdoc/>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <inheritdoc/>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(this.secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = this.issuer,
                    ValidateAudience = true,
                    ValidAudience = this.audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                // Expected — token simply expired. Info level, no alarm.
                this.logger.LogInformation("Token validation failed: token expired at {Expiry}", ex.Expires);
                return null;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                var sessionId = this.TryReadSessionIdUnsafe(token);
                this.logger.LogWarning(
                    ex,
                    "Invalid signature on token. SessionId (unverified): {SessionId}. Prefix: {Prefix}",
                    sessionId,
                    SafeTokenPrefix(token));
                return null;
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                this.logger.LogWarning(
                    ex,
                    "Token validation failed: invalid issuer '{Issuer}'",
                    ex.InvalidIssuer);
                return null;
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                this.logger.LogWarning(
                    ex,
                    "Token validation failed: invalid audience '{Audience}'",
                    ex.InvalidAudience);
                return null;
            }
            catch (SecurityTokenException ex)
            {
                // Catch-all for other token-specific issues (malformed, not-yet-valid, etc.)
                this.logger.LogWarning(ex, "Token validation failed: {Reason}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                // Truly unexpected — infrastructure failure, key parsing error, etc.
                this.logger.LogError(ex, "Unexpected error during token validation");
                return null;
            }
        }

        /// <inheritdoc/>
        public DateTime GetTokenExpiration(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (expClaim != null && long.TryParse(expClaim, out var exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            }

            return DateTime.MinValue;
        }

        /// <inheritdoc/>
        public Guid? GetSessionIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var sessionIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "session_id")?.Value;

                if (Guid.TryParse(sessionIdClaim, out var sessionId))
                {
                    return sessionId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string SafeTokenPrefix(string token) =>
            token.Length > 20 ? token[..20] + "…" : "[short token]";

        private string? TryReadSessionIdUnsafe(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(c => c.Type == "session_id")?.Value;
            }
            catch
            {
                return null; // Truly unreadable — not loggable
            }
        }
    }
}
