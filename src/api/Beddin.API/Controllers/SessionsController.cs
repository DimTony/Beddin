// <copyright file="SessionsController.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Features.Admin.Queries.GetSessions;
using Beddin.Application.Features.Users.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beddin.API.Controllers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests and responses.</param>
    /// <param name="configuration">The configuration instance for accessing application settings.</param>
    [ApiController]
    [Route("[controller]")]
    public class SessionsController(IMediator mediator, IConfiguration configuration) : ControllerBase
    {
        private readonly IMediator mediator = mediator;
        private readonly IConfiguration configuration = configuration;

        /// <summary>
        /// Represents the available mock roles for testing login functionality.
        /// </summary>
        public enum MockRoles
        {
            /// <summary>
            /// Represents an administrator user role.
            /// </summary>
            Admin,

            /// <summary>
            /// Represents a buyer user role.
            /// </summary>
            Buyer,

            /// <summary>
            /// Represents an owner user role.
            /// </summary>
            Owner,
        }

        /// <summary>
        /// Retrieves a list of user sessions based on the specified query parameters, allowing for filtering by user ID and pagination. The method accepts optional query parameters such as userId, page, and pageSize to customize the retrieval of sessions. If a userId is provided, it will return sessions specific to that user; otherwise, it will return sessions for all users. The response includes the session details along with pagination information, enabling clients to navigate through large sets of session data efficiently.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose sessions are to be retrieved. If null, sessions for all users will be retrieved.</param>
        /// <param name="page">The page number for pagination. Defaults to 1.</param>
        /// <param name="pageSize">The number of sessions to retrieve per page. Defaults to 20.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSessions(
            [FromQuery] string? userId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await this.mediator.Send(new GetSessionsQuery(userId, page, pageSize));

            return this.Ok(result);
        }

        /// <summary>
        /// Simulates a login process for a mock user based on the specified role. This method allows developers to test the login functionality of the application without needing real user credentials. By providing a role (e.g., Admin, Buyer, Owner), the method retrieves the corresponding mock user credentials from the configuration and attempts to log in using those credentials. The response indicates whether the login was successful or if there were any issues during the process, such as missing configuration or invalid credentials.
        /// </summary>
        /// <param name="role">The role of the mock user to log in as.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("MockLogin")]
        public async Task<IActionResult> MockLogin(
            [FromQuery] MockRoles role)
        {
            // request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            // request.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var userType = role.ToString();

            var mockUserSection = this.configuration.GetSection($"MockUsers:{userType}");

            if (!mockUserSection.Exists())
            {
                return this.BadRequest($"Mock user configuration not found for user type: {userType}");
            }

            var email = mockUserSection["Email"]
                ?? throw new InvalidOperationException($"Mock {userType} Email is not configured");

            var password = mockUserSection["Password"]
                ?? throw new InvalidOperationException($"Mock {userType} Password is not configured");

            var result = await this.mediator.Send(new LoginCommand(email, password));

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }
    }
}
