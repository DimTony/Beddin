//using System;
//using StackExchange.Redis;
//using Beddin.Application.Common.Interfaces;

//namespace Beddin.Infrastructure.Services
//{
//    public sealed class RedisRateLimitService : IRateLimitService
//    {
//        private readonly IConnectionMultiplexer _redis;

//        public RedisRateLimitService(IConnectionMultiplexer redis) => _redis = redis;

//        public async Task<bool> IsAllowedAsync(string key, int maxAttempts, int windowSeconds)
//        {
//            var db = _redis.GetDatabase();
//            var current = await db.StringIncrementAsync(key);

//            if (current == 1)
//                await db.KeyExpireAsync(key, TimeSpan.FromSeconds(windowSeconds));

//            return current <= maxAttempts;
//        }
//    }
//}
