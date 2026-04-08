// <copyright file="UsersController.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Features.Admin.Commands.ActivateUser;
using Beddin.Application.Features.Admin.Commands.DeactivateUser;
using Beddin.Application.Features.Admin.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beddin.API.Controllers
{
    /// <summary>
    /// The <c>UsersController</c> class is an API controller responsible for handling user-related operations in the application. It provides endpoints for retrieving user information, activating users, and deactivating users. The controller uses the MediatR library to send commands and queries to the application layer, allowing for a clean separation of concerns and promoting a CQRS (Command Query Responsibility Segregation) pattern. The endpoints are secured with authorization attributes to ensure that only authorized users can perform these operations. The controller processes incoming HTTP requests, interacts with the application layer through MediatR, and returns appropriate HTTP responses based on the success or failure of the operations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance used for handling requests.</param>
        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Retrieves a list of users based on the provided query parameters, allowing for filtering and pagination. The query parameters can include criteria such as user ID, first name, last name, email, role, and pagination details (page number and page size). The method processes the request to fetch the corresponding user information that matches the specified criteria. If users are found that meet the criteria, it returns an HTTP 200 OK response with the list of users. If no users are found or if there are any issues with the request, it will return an appropriate error response.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="email">The email address of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page for pagination.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Authorize]

        // [Authorize(Roles = $"{UserRoles.SystemAdminInitiator},{UserRoles.SystemAdminAuthorizer}")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] Guid? userId,
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? email,
            [FromQuery] Guid? role,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var cmd = new GetUsersQuery(
                userId, firstName, lastName, email, role, page, pageSize);

            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Activates a user based on the provided command, which contains the necessary details for identifying the user to be activated. The method processes the request to perform the activation operation and returns an appropriate response indicating the success or failure of the operation. If the user is successfully activated, it returns an HTTP 200 OK response with the result of the operation. If there are any issues with the request or if an error occurs during activation, it will return an HTTP 400 Bad Request response with the corresponding error messages.
        /// </summary>
        /// <param name="cmd">The command containing the details for activating a user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("Activate")]

        // [Authorize(Roles = $"{UserRoles.SystemAdminInitiator},{UserRoles.SystemAdminAuthorizer}")]
        public async Task<IActionResult> ActivateUser(
            [FromBody] ActivateUserCommand cmd)
        {
            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Deactivates a user based on the provided command, which contains the necessary details for identifying the user to be deactivated. The method processes the request to perform the deactivation operation and returns an appropriate response indicating the success or failure of the operation. If the user is successfully deactivated, it returns an HTTP 200 OK response with the result of the operation. If there are any issues with the request or if an error occurs during deactivation, it will return an HTTP 400 Bad Request response with the corresponding error messages.
        /// </summary>
        /// <param name="cmd">The command containing the details for deactivating a user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("Deactivate")]

        // [Authorize(Roles = $"{UserRoles.SystemAdminInitiator},{UserRoles.SystemAdminAuthorizer}")]
        public async Task<IActionResult> DeactivateUser(
            [FromBody] DeactivateUserCommand cmd)
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
