// <copyright file="ExceptionHandlingMiddleware.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Net;
using System.Text.Json;
using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Beddin.Application.Common.Exceptions.ValidationException;

namespace Beddin.API.Middleware
{
    /// <summary>
    /// Global exception handler — sits at the top of the pipeline.
    /// Catches anything that escapes handlers and maps it to a
    /// consistent ApiResponse envelope with the correct HTTP status.
    ///
    /// What it handles:
    ///   ValidationException  → 400 with field-level errors
    ///   NotFoundException    → 404
    ///   ForbiddenException   → 403
    ///   ConflictException    → 409
    ///   OperationCancelledEx → 499 (client disconnected — not logged as error)
    ///   Everything else      → 500 (logged, detail hidden from client).
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        private readonly IHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger instance for logging exceptions.</param>
        /// <param name="environment">The hosting environment.</param>
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            this.next = next;
            this.logger = logger;
            this.environment = environment;
        }

        /// <summary>
        /// Invokes the middleware to catch unhandled exceptions, log them appropriately, and return a consistent JSON response with the correct HTTP status code based on the type of exception. It handles specific exceptions like validation failures, not found, forbidden access, conflicts, and client cancellations, while logging unexpected errors without exposing details to the client in production environments.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // Client disconnected — not an error, don't log as one
                this.logger.LogInformation(
                    "Request cancelled by client: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                context.Response.StatusCode = 499;
            }
            catch (ValidationException ex)
            {
                this.logger.LogWarning(
                    "Validation failure on {Method} {Path}: {Errors}",
                    context.Request.Method,
                    context.Request.Path,
                    string.Join(", ", ex.Errors.Select(e => $"{e.Field}: {string.Join(", ", e.Messages)}")));

                await WriteResponseAsync(
                    context,
                    HttpStatusCode.BadRequest,
                    ApiResponse<ValidationProblemDetails>.Fail(
                        ex.Errors
                          .SelectMany(e => e.Messages.Select(m => $"{e.Field}: {m}"))
                          .ToList()));
            }
            catch (NotFoundException ex)
            {
                this.logger.LogWarning(
                    "Resource not found on {Method} {Path}: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message);

                await WriteResponseAsync(
                    context,
                    HttpStatusCode.NotFound,
                    ApiResponse.Fail(ex.Message));
            }
            catch (ForbiddenException ex)
            {
                this.logger.LogWarning(
                    "Forbidden access on {Method} {Path} by user {UserId}: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    context.User.FindFirst("sub")?.Value ?? "anonymous",
                    ex.Message);

                await WriteResponseAsync(
                    context,
                    HttpStatusCode.Forbidden,
                    ApiResponse.Fail(ex.Message));
            }
            catch (ConflictException ex)
            {
                this.logger.LogWarning(
                    "Conflict on {Method} {Path}: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message);

                await WriteResponseAsync(
                    context,
                    HttpStatusCode.Conflict,
                    ApiResponse.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                // Unknown exception — log full details but hide from client
                this.logger.LogError(
                    ex,
                    "Unhandled exception on {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                var message = this.environment.IsDevelopment()
                    ? ex.Message // show detail locally
                    : "An unexpected error occurred. Please try again or contact support.";

                await WriteResponseAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    ApiResponse.Fail(message));
            }
        }

        private static async Task WriteResponseAsync<T>(
            HttpContext context,
            HttpStatusCode statusCode,
            T response)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            await context.Response.WriteAsync(json);
        }
    }
}
