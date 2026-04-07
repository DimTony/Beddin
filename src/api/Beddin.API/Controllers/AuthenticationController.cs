// <copyright file="AuthenticationController.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Features.Users.Commands.ChangePassword;
using Beddin.Application.Features.Users.Commands.ConfirmEmail;
using Beddin.Application.Features.Users.Commands.Login;
using Beddin.Application.Features.Users.Commands.Logout;
using Beddin.Application.Features.Users.Commands.RefreshToken;
using Beddin.Application.Features.Users.Commands.RegisterUser;
using Beddin.Application.Features.Users.Commands.ResendEmail;
using Beddin.Application.Features.Users.Commands.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beddin.API.Controllers
{
    /// <summary>
    /// Provides authentication-related endpoints such as registration, login, password management, and email confirmation.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for handling requests.</param>
        public AuthenticationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="cmd">The registration command.</param>
        /// <returns>The result of the registration operation.</returns>
        [HttpPost("Register")]
        public async Task<IActionResult> SubmitUserRegistration(
            [FromBody] RegisterCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Resends the confirmation email to the user.
        /// </summary>
        /// <param name="cmd">The resend confirmation email command.</param>
        /// <returns>The result of the resend operation.</returns>
        [HttpPost("ResendConfirmation")]
        public async Task<IActionResult> ResendConfirmationEmail(
            [FromBody] ResendConfirmationEmailCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Confirms the user's email address.
        /// </summary>
        /// <param name="cmd">The confirm email command.</param>
        /// <returns>The result of the confirmation operation.</returns>
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(
            [FromBody] ConfirmEmailCommand cmd)
        {
            var ip = this.HttpContext.Connection.RemoteIpAddress?.ToString()
             ?? this.Request.Headers["X-Forwarded-For"].FirstOrDefault()
             ?? string.Empty;

            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Authenticates a user and returns a token.
        /// </summary>
        /// <param name="cmd">The login command.</param>
        /// <returns>The result of the login operation.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Refreshes the authentication token.
        /// </summary>
        /// <param name="cmd">The refresh token command.</param>
        /// <returns>The result of the refresh operation.</returns>
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="cmd">The change password command.</param>
        /// <returns>The result of the change password operation.</returns>
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Initiates a password reset request.
        /// </summary>
        /// <param name="cmd">The request password reset command.</param>
        /// <returns>The result of the reset request operation.</returns>
        [HttpPost("Reset")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] RequestPasswordResetCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Sets a new password for the user.
        /// </summary>
        /// <param name="cmd">The reset password command.</param>
        /// <returns>The result of the set password operation.</returns>
        [HttpPost("SetPassword")]
        public async Task<IActionResult> SetPassword(
            [FromBody] ResetPasswordCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Logs out the authenticated user.
        /// </summary>
        /// <param name="cmd">The logout command.</param>
        /// <returns>The result of the logout operation.</returns>
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(
            [FromBody] LogoutCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }
    }
}
