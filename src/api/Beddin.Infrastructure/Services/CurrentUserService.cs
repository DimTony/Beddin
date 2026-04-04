using Beddin.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Beddin.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                // Prefer the raw JWT "sub" claim — works regardless of MapInboundClaims
                var value = _httpContextAccessor.HttpContext?.User
                    .FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? _httpContextAccessor.HttpContext?.User
                       .FindFirstValue(ClaimTypes.NameIdentifier); // fallback for mapped tokens

                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        public string? IpAddress =>
            _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        public string? UserAgent =>
          _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();


        public Guid? SessionId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue("session_id");
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        public string? Role =>
    _httpContextAccessor.HttpContext?.User
        .FindFirstValue("role")                          
        ?? _httpContextAccessor.HttpContext?.User
           .FindFirstValue(ClaimTypes.Role);             

        public string? Name =>
            _httpContextAccessor.HttpContext?.User
                .FindFirstValue(JwtRegisteredClaimNames.Name) 
                ?? _httpContextAccessor.HttpContext?.User
                   .FindFirstValue(ClaimTypes.Name);
        public string? Email =>
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    }
}
