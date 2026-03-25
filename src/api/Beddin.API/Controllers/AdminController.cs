using Azure.Core;
using Beddin.Application.Features.Admin.Commands.ActivateUser;
using Beddin.Application.Features.Admin.Commands.DeactivateUser;
using Beddin.Application.Features.Admin.Queries.GetSessions;
using Beddin.Application.Features.Admin.Queries.GetUsers;
using Beddin.Application.Features.Users.Commands.CreateRole;
using Beddin.Application.Features.Users.Commands.Login;
using Beddin.Application.Features.Users.Commands.ResetPassword;
using Beddin.Application.Features.Users.Queries.GetRoleById;
using Beddin.Application.Features.Users.Queries.GetRoles;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beddin.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
            
        }

        [HttpPost("Create")]
        //[Authorize(Roles = $"{UserRoles.SystemAdminInitiator},{UserRoles.SystemAdminAuthorizer}")]
        public async Task<IActionResult> CreateRole(
            [FromBody] CreateRoleCommand payload)
        {
            var cmd = new CreateRoleCommand(
                payload.Name,
                payload.Description,
                payload.IpAddress,
                payload.UserAgent
            );

            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllRoles(
                [FromQuery] GetRolesQuery query,
                CancellationToken ct)
        {
            var result = await _mediator.Send(query, ct);

            return Ok(result);
        }

        [HttpGet("Id")]
        public async Task<IActionResult> GetSingleRole(
                [FromQuery] Guid id,
                CancellationToken ct)
        {
            var query = new GetRoleByIdQuery(new RoleId(id));

            var result = await _mediator.Send(query, ct);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public SessionsController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSessions(
            [FromQuery] string? userId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetSessionsQuery(userId, page, pageSize));

            return Ok(result);
        }

        [HttpPost("MockLogin")]
        public async Task<IActionResult> MockLogin(
            [FromQuery] MockRoles role)
        {
            //request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            //request.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var userType = role.ToString();

            var mockUserSection = _configuration.GetSection($"MockUsers:{userType}");

            if (!mockUserSection.Exists())
            {
                return BadRequest($"Mock user configuration not found for user type: {userType}");
            }

            var email = mockUserSection["Email"]
                ?? throw new InvalidOperationException($"Mock {userType} Email is not configured");

            var password = mockUserSection["Password"]
                ?? throw new InvalidOperationException($"Mock {userType} Password is not configured");

            var result = await _mediator.Send(new LoginCommand(email, password));

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        public enum MockRoles
        {
            Admin,
            Buyer,
            Owner
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;

        }

        [HttpGet]
        //[Authorize(Roles = $"{UserRoles.SystemAdminInitiator},{UserRoles.SystemAdminAuthorizer}")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] Guid? userId,
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? email,
            [FromQuery] Guid? role,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20

            )
        {
            var cmd = new GetUsersQuery(
                userId, firstName, lastName, email, role, page, pageSize
            );

            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Activate")]
        //[Authorize(Roles = $"{UserRoles.SystemAdminInitiator},{UserRoles.SystemAdminAuthorizer}")]
        public async Task<IActionResult> ActivateUser(
            [FromBody] ActivateUserCommand cmd)
        {

            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Deactivate")]
        //[Authorize(Roles = $"{UserRoles.SystemAdminInitiator},{UserRoles.SystemAdminAuthorizer}")]
        public async Task<IActionResult> DeactivateUser(
            [FromBody] DeactivateUserCommand cmd)
        {

            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
