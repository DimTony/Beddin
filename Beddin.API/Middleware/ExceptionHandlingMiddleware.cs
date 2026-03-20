using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using Beddin.Application.Common.Exceptions;
using Beddin.Application.Common.DTOs;
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
    ///   Everything else      → 500 (logged, detail hidden from client)
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // Client disconnected — not an error, don't log as one
                _logger.LogInformation(
                    "Request cancelled by client: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                context.Response.StatusCode = 499;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(
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
                _logger.LogWarning(
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
                _logger.LogWarning(
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
                _logger.LogWarning(
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
                _logger.LogError(
                    ex,
                    "Unhandled exception on {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                var message = _environment.IsDevelopment()
                    ? ex.Message          // show detail locally
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
