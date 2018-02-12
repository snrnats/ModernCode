using System;
using System.Threading.Tasks;
using ModernCode.Network.Exceptions;
using Newtonsoft.Json;

namespace ModernCode.Network.Facade
{
    public static class CommonFacades
    {
        public static async Task<T> Retry<T, TException>(Func<Task<T>> thingToTry, int attempts)
            where TException : Exception
        {
            // Start at 1 instead of 0 to allow the final attempt
            for (var i = 1; i < attempts; i++)
            {
                try
                {
                    return await thingToTry();
                }
                catch (TException)
                {
                }
            }

            // Final attempt, let exception bubble up
            return await thingToTry();
        }

        public static async Task<T> Handle<T, TError>(Func<Task<T>> thingToTry, Action<ServerException, TError> handler)
        {
            try
            {
                return await thingToTry();
            }
            catch (ServerException e)
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<TError>(e.Response);
                    handler(e, errorResponse);
                }
                catch (JsonException)
                {
                }

                throw;
            }
        }

        public static async Task<T> RetryWithDelay<T, TException>(Func<Task<T>> thingToTry, int attempts, TimeSpan delay)
            where TException : Exception
        {
            // Start at 1 instead of 0 to allow the final attempt
            for (var i = 1; i < attempts; i++)
            {
                try
                {
                    return await thingToTry();
                }
                catch (TException)
                {
                    await Task.Delay(delay);
                }
            }

            // Final attempt, let exception bubble up
            return await thingToTry();
        }
    }
}