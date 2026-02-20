using System;
using System.Collections.Generic;
using System.Text;

namespace lenovo.mbg.service.framework.common
{
    /// <summary>
    /// Abstraction for executing external processes.
    /// Allows mocking in tests and enables dependency injection.
    /// </summary>
    public interface IProcessRunner
    {
        /// <summary>
        /// Executes a process and returns the combined stdout/stderr as a string.
        /// </summary>
        /// <param name="executablePath">Path to the executable.</param>
        /// <param name="arguments">Command-line arguments.</param>
        /// <param name="timeout">Timeout in milliseconds. -1 for no timeout.</param>
        /// <returns>Combined output string.</returns>
        string ProcessString(string executablePath, string arguments, int timeout = -1);

        /// <summary>
        /// Executes a process and returns stdout/stderr as a list of lines.
        /// </summary>
        /// <param name="executablePath">Path to the executable.</param>
        /// <param name="arguments">Command-line arguments.</param>
        /// <param name="timeout">Timeout in milliseconds. -1 for no timeout.</param>
        /// <returns>List of output lines.</returns>
        List<string> ProcessList(string executablePath, string arguments, int timeout = -1);

        /// <summary>
        /// Gets the exit code from the last executed process.
        /// </summary>
        int LastExitCode { get; }
    }
}
