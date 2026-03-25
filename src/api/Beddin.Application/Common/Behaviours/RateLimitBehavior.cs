//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Beddin.Application.Common.Interfaces;

//namespace Beddin.Application.Common.Behaviours
//{
//    public sealed class RateLimitingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//        where TRequest : ILimitable  // marker interface (see below)
//        where TResponse : class
//    {
//        private readonly IRateLimitService _rateLimitService;

//        public RateLimitingBehaviour(IRateLimitService rateLimitService)
//        {
//            _rateLimitService = rateLimitService;
//        }

//        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//        {
//            var key = request.RateLimitKey; // e.g. "login:192.168.1.1" or "login:user@email.com"
//            var isAllowed = await _rateLimitService.IsAllowedAsync(key, request.MaxAttempts, request.WindowSeconds);

//            if (!isAllowed)
//            {
//                // Construct a failure response dynamically
//                var failMethod = typeof(TResponse).GetMethod("Fail", new[] { typeof(string) });
//                return (TResponse)failMethod!.Invoke(null, ["Too many attempts. Please try again later."])!;
//            }

//            return await next();
//        }
//    }
//}
