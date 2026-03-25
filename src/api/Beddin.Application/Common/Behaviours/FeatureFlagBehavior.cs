using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beddin.Application.Common.Behaviours
{
    public class FeatureFlagBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IConfiguration _config;

        public FeatureFlagBehavior(IConfiguration config)
        {
            _config = config;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            if (request is not IRequiresFeature flagged)
                return await next();

            var isEnabled = _config.GetValue<bool>(flagged.FeatureFlag);

            if (!isEnabled)
            {
                var resultType = typeof(TResponse);

                if (resultType == typeof(Result))
                    return (TResponse)(object)Result.Failure(
                        $"This feature is currently disabled: {flagged.FeatureFlag}");

             

                if (resultType.IsGenericType &&
                    resultType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var innerType = resultType.GetGenericArguments()[0];
                    var failureMethod = typeof(Result)
                        .GetMethod(nameof(Result.Failure), 1, new[] { typeof(string) })!
                        .MakeGenericMethod(innerType);

                    return (TResponse)failureMethod.Invoke(null, new object[] {
                        $"This feature is currently disabled: {flagged.FeatureFlag}"
                    })!;
                }

                if (resultType.IsGenericType &&
                    resultType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    var innerType = resultType.GetGenericArguments()[0];

                    var constructedType = typeof(ApiResponse<>).MakeGenericType(innerType);

                    var method = constructedType.GetMethod(
                        nameof(ApiResponse<object>.Fail),
                        new[] { typeof(string) });

                    if (method == null)
                        throw new InvalidOperationException("Fail(string) not found on ApiResponse<T>");

                    return (TResponse)method.Invoke(null, new object[]
                    {
                         $"This feature is currently disabled: {flagged.FeatureFlag}"
                    })!;
                }

                throw new InvalidOperationException(
                    $"Feature '{flagged.FeatureFlag}' is disabled and response type " +
                    $"'{resultType.Name}' does not support failure results.");
            }

            return await next();
        }
    }
}
