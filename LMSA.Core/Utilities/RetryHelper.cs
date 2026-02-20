using System;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Provides retry logic for operations that may fail transiently.
    /// </summary>
    public static class RetryHelper
    {
        /// <summary>
        /// Executes a function with retry logic.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="action">The function to execute.</param>
        /// <param name="maxRetries">Maximum number of attempts.</param>
        /// <param name="shouldRetry">Predicate that determines whether to retry based on the result.</param>
        /// <returns>The result of the last attempt.</returns>
        public static T Execute<T>(Func<T> action, int maxRetries = 3, Func<T, bool>? shouldRetry = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (maxRetries < 1) throw new ArgumentOutOfRangeException(nameof(maxRetries), "Must be at least 1.");

            T? result = default;
            int retries = maxRetries;

            do
            {
                result = action();
                if (shouldRetry == null || !shouldRetry(result))
                    break;
            } while (--retries > 0);

            return result;
        }
    }
}
