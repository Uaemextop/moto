using System;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Detects error patterns in command responses.
    /// </summary>
    public static class ErrorDetector
    {
        public const string ANTI_ROLLBACK_PATTERN = "STATUS_SEC_VIOLATE_ANTI_ROLLBACK";

        /// <summary>
        /// Checks whether a response string indicates an error.
        /// </summary>
        public static bool HasError(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            string lower = response.ToLower();
            return lower.Contains("error") || lower.Contains("fail");
        }

        /// <summary>
        /// Checks whether a response indicates an anti-rollback violation.
        /// </summary>
        public static bool HasAntiRollbackViolation(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            return response.Contains(ANTI_ROLLBACK_PATTERN, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines the Result based on the response content.
        /// </summary>
        public static Result EvaluateResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return Result.FAILED;

            if (HasAntiRollbackViolation(response))
                return Result.FASTBOOT_DEGRADE_QUIT;

            if (HasError(response))
                return Result.FAILED;

            return Result.PASSED;
        }
    }
}
