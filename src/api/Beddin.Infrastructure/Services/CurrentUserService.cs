using Beddin.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
                var value = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        public string? IpAddress =>
            _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        public string? UserAgent =>
          _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

        public string? Role =>
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

        public Guid? SessionId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue("session_id");
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        public string? Name =>
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        public string? Email =>
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    }
}
