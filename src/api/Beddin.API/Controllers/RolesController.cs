// <copyright file="RolesController.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Features.Users.Commands.CreateRole;
using Beddin.Application.Features.Users.Queries.GetRoleById;
using Beddin.Application.Features.Users.Queries.GetRoles;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Beddin.API.Controllers
{
    /// <summary>
    /// The <c>RolesController</c> class provides API endpoints for managing user roles, including creating new roles and retrieving existing roles. It utilizes MediatR to handle incoming requests and delegate the processing to the appropriate handlers. The controller includes methods for creating a role with specified details, fetching all roles with optional filtering and pagination, and retrieving a single role by its unique identifier. Each method returns appropriate HTTP responses based on the success or failure of the operations, ensuring that clients receive clear feedback on their requests.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance used for handling requests.</param>
        public RolesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Creates a new role based on the provided payload, which includes the role's name, description, and other relevant details. The method processes the creation request and returns an appropriate response indicating the success or failure of the operation. If the role is created successfully, it returns an HTTP 200 OK response with the details of the newly created role. If there are any validation errors or issues during the creation process, it returns an HTTP 400 Bad Request response with the corresponding error messages.
        /// </summary>
        /// <param name="payload">The payload containing the details for creating a new role.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateRole(
            [FromBody] CreateRoleCommand payload)
        {
            var cmd = new CreateRoleCommand(
                payload.Name,
                payload.Description,
                payload.IpAddress,
                payload.UserAgent);

            var result = await this.mediator.Send(cmd);

            if (!result.Success)
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Retrieves a list of all roles based on the provided query parameters, allowing for filtering and pagination. The query parameters can include criteria such as role name, description, or other relevant attributes to narrow down the results. The method returns an HTTP 200 OK response with the list of roles that match the specified criteria. If there are any issues with the query parameters or if an error occurs during retrieval, it will return an appropriate error response.
        /// </summary>
        /// <param name="query">The query parameters for retrieving roles.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllRoles(
                [FromQuery] GetRolesQuery query,
                CancellationToken ct)
        {
            var result = await this.mediator.Send(query, ct);

            return this.Ok(result);
        }

        /// <summary>
        /// Retrieves the details of a single role based on its unique identifier (ID). The method accepts the role ID as a query parameter and processes the request to fetch the corresponding role information. If the role with the specified ID is found, it returns an HTTP 200 OK response with the role details. If no role is found with the given ID, it returns an HTTP 404 Not Found response indicating that the requested resource does not exist. Additionally, if there are any issues with the request or if an error occurs during retrieval, it will return an appropriate error response.
        /// </summary>
        /// <param name="id">The unique identifier of the role to retrieve.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("Id")]
        public async Task<IActionResult> GetSingleRole(
                [FromQuery] Guid id,
                CancellationToken ct)
        {
            var query = new GetRoleByIdQuery(new RoleId(id));

            var result = await this.mediator.Send(query, ct);

            if (!result.Success)
            {
                return this.NotFound(result);
            }

            return this.Ok(result);
        }
    }
}
