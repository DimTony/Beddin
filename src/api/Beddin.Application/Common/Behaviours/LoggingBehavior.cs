// <copyright file="LoggingBehavior.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Diagnostics;
using Beddin.Application.Common.Options;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Beddin.Application.Common.Behaviours
{
    /// <summary>
    /// Pipeline behavior that logs request handling and tracks performance.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            using var activity = BeddinActivitySource.Instance
                .StartActivity(requestName, ActivityKind.Internal);

            activity?.SetTag("request.type", requestName);

            this.logger.LogInformation("Handling {Request}", requestName);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();
                stopwatch.Stop();

                this.logger.LogInformation(
                    "Handled {Request} in {ElapsedMilliseconds}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                activity?.SetStatus(ActivityStatusCode.Ok);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                this.logger.LogError(
                    ex,
                    "Error handling {Request} after {ElapsedMilliseconds}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.AddException(ex);

                throw;
            }
        }
    }
}
