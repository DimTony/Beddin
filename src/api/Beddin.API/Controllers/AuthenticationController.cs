using Beddin.Application.Features.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Beddin.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController: ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> SubmitUserRegistration(
        [FromBody] UserRegistrationPayload payload)
        {
            var cmd = new RegisterCommand(
                payload.FirstName,
                payload.LastName,
                payload.Email,
                payload.Password,
                payload.Role
            );

            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
