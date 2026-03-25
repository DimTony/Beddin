using Beddin.Application.Features.Users.Commands.ConfirmEmail;
using Beddin.Application.Features.Users.Commands.Login;
using Beddin.Application.Features.Users.Commands.RegisterUser;
using Beddin.Application.Features.Users.Commands.ResendEmail;
using Beddin.Application.Features.Users.Commands.RefreshToken;
using Beddin.Application.Features.Users.Commands.ChangePassword;
using Beddin.Application.Features.Users.Commands.ResetPassword;
using Beddin.Application.Features.Users.Commands.Logout;
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
            [FromBody] RegisterCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ResendConfirmation")]
        public async Task<IActionResult> ResendConfirmationEmail(
            [FromBody] ResendConfirmationEmailCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(
            [FromBody] ConfirmEmailCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Reset")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] RequestPasswordResetCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("SetPassword")]
        public async Task<IActionResult> SetPassword(
            [FromBody] ResetPasswordCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(
            [FromBody] LogoutCommand cmd)
        {
            var result = await _mediator.Send(cmd);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
