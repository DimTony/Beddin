using Beddin.Application.Common.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Beddin.Application.Common.Behaviours
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            using var activity = BeddinActivitySource.Instance
                .StartActivity(requestName, ActivityKind.Internal);

            activity?.SetTag("request.type", requestName);

            _logger.LogInformation("Handling {Request}", requestName);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation(
                    "Handled {Request} in {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                activity?.SetStatus(ActivityStatusCode.Ok);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "Error handling {Request} after {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.AddException(ex);

                throw;
            }
        }
    }
}