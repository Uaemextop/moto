using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.common.utilities;

/// <summary>
/// Executes external processes and captures their output.
/// </summary>
public class ProcessRunner
{
    /// <summary>
    /// Executes an external process and returns its combined output as a string.
    /// </summary>
    public static string ProcessString(string exe, string command, int timeout)
    {
        List<string> list = ProcessList(exe, command, timeout);
        if (list != null && list.Count > 0)
        {
            string text = string.Join("\r\n", list);
            if (text.EndsWith("\r\n"))
            {
                text = text.Remove(text.LastIndexOf("\r\n", StringComparison.Ordinal));
            }
            return text;
        }
        return string.Empty;
    }

    /// <summary>
    /// Executes an external process with a periodic action callback, returning combined output.
    /// </summary>
    public static string ProcessString(string exe, string command, int timeout, Action<Process>? action, int period, out long exitCode, string? workingDirectory = null)
    {
        exitCode = 0L;
        List<string> response = new List<string>();
        Process process = new Process();
        try
        {
            process.StartInfo.FileName = exe;
            process.StartInfo.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
            process.StartInfo.Arguments = command;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.EnableRaisingEvents = true;
            process.StartInfo.CreateNoWindow = true;
            Timer? inputTimer = null;
            bool processExited = false;
            try
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        response.Add(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        response.Add(e.Data);
                    }
                };
                process.Exited += delegate
                {
                    processExited = true;
                    try { inputTimer?.Dispose(); } catch { }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (action != null)
                {
                    inputTimer = new Timer(delegate
                    {
                        try
                        {
                            if (!processExited && !process.HasExited)
                            {
                                action?.Invoke(process);
                            }
                        }
                        catch (Exception) { }
                    }, null, TimeSpan.FromMilliseconds(period), TimeSpan.FromMilliseconds(period));
                }
                if (!process.WaitForExit(timeout))
                {
                    response.Add($"execute error, commnad timeout: {timeout}");
                    process.Kill();
                }
            }
            catch (Exception arg)
            {
                response.Add($"execute occur an error, {arg}");
            }
            exitCode = process.ExitCode;
            return string.Join("\r\n", response);
        }
        finally
        {
            process.Dispose();
        }
    }

    /// <summary>
    /// Executes an external process with a periodic action callback.
    /// </summary>
    public static string ProcessString(string exe, string command, int timeout, Action<Process>? action, int period, string? workingDirectory = null)
    {
        return ProcessString(exe, command, timeout, action, period, out _, workingDirectory);
    }

    /// <summary>
    /// Executes an external process and returns its output as a list of lines.
    /// </summary>
    public static List<string> ProcessList(string exe, string command, int timeout, string? workingDirectory = null)
    {
        List<string> list = new List<string>();
        List<string> output = new List<string>();
        List<string> error = new List<string>();
        using Process process = new Process();
        process.StartInfo.FileName = exe;
        process.StartInfo.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
        process.StartInfo.Arguments = command;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.EnableRaisingEvents = true;
        process.StartInfo.CreateNoWindow = true;
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) output.Add(e.Data);
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) error.Add(e.Data);
        };
        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!process.WaitForExit(timeout))
            {
                list.Add($"execute error, commnad timeout: {timeout}");
                LogHelper.LogInstance.Debug("execute commnad timeout, command: " + command);
                process.Kill();
                return list;
            }
            if (output.Count > 0) list.AddRange(output);
            if (error.Count > 0) list.AddRange(error);
            return list;
        }
        catch (Exception arg)
        {
            list.Add($"execute occur an error, {arg}");
            return list;
        }
    }

    /// <summary>
    /// Executes a process and waits for specific keyword output before returning.
    /// </summary>
    public static List<string> ProcessListKeyWord(string exe, string command, int timeout, List<string>? keyword, string? workingDirectory = null)
    {
        List<string> list = new List<string>();
        List<string> output = new List<string>();
        List<string> error = new List<string>();
        using Process process = new Process();
        process.StartInfo.FileName = exe;
        process.StartInfo.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
        process.StartInfo.Arguments = command;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.EnableRaisingEvents = true;
        process.StartInfo.CreateNoWindow = true;
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) output.Add(e.Data);
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) error.Add(e.Data);
        };
        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            int retryCount = 0;
            if (!process.WaitForExit(timeout))
            {
                for (; retryCount < 4; retryCount++)
                {
                    string[] outputSnapshot = output.ToArray();
                    string[] errorSnapshot = error.ToArray();
                    if (error.Count <= 1 && output.Count <= 1)
                    {
                        list.Add($"execute error, commnad timeout: {timeout}");
                        LogHelper.LogInstance.Debug("execute commnad timeout, command: " + command);
                        if (!process.HasExited) process.Kill();
                        return list;
                    }
                    bool hasKeyword = false;
                    if (keyword != null && keyword.Count > 0)
                    {
                        hasKeyword = keyword.Exists(item => Array.Exists(outputSnapshot, p => p.Contains(item))) ||
                                     keyword.Exists(item => Array.Exists(errorSnapshot, p => p.Contains(item)));
                    }
                    if (hasKeyword)
                    {
                        LogHelper.LogInstance.Debug($"execute timeout but has data, try sleep: {retryCount * 500} milliseconds");
                        break;
                    }
                    Thread.Sleep(500);
                }
                if (!process.HasExited) process.Kill();
            }
            if (output.Count > 0) list.AddRange(output);
            if (error.Count > 0) list.AddRange(error);
            if (retryCount == 4)
            {
                list.Add($"execute error, commnad timeout: {timeout}");
                LogHelper.LogInstance.Debug("execute commnad timeout, buffer data residue, command: " + command);
                if (!process.HasExited) process.Kill();
            }
            return list;
        }
        catch (InvalidOperationException) when (process.HasExited)
        {
            if (output.Count > 0) list.AddRange(output);
            if (error.Count > 0) list.AddRange(error);
            return list;
        }
        catch (Exception arg)
        {
            list.Add($"execute occur an error, {arg}");
            return list;
        }
    }

    /// <summary>
    /// Executes a command via cmd.exe shell and returns the output.
    /// </summary>
    public static string Shell(string command, string executeExe = "cmd.exe", int timeout = 2000)
    {
        Process process = new Process();
        process.StartInfo.FileName = executeExe;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.StandardInput.WriteLine(command + " &exit");
        process.StandardInput.AutoFlush = true;
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit(timeout);
        process.Close();
        return result;
    }

    /// <summary>
    /// Executes a cmd command and waits for exit.
    /// </summary>
    public static string? CmdExcuteWithExit(string cmdStr, string? workdir = null, int timeOut = 60000)
    {
        try
        {
            using Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            if (!string.IsNullOrEmpty(workdir))
            {
                process.StartInfo.WorkingDirectory = workdir;
            }
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.StandardInput.AutoFlush = true;
            process.StandardInput.WriteLine(cmdStr);
            process.StandardInput.WriteLine("exit");
            if (!process.WaitForExit(timeOut))
            {
                process.Kill();
                return null;
            }
            string text = string.Empty;
            StreamReader standardOutput = process.StandardOutput;
            StreamReader standardError = process.StandardError;
            if (standardOutput != null)
            {
                text = standardOutput.ReadToEnd();
            }
            if (standardError != null)
            {
                text += standardError.ReadToEnd();
            }
            LogHelper.LogInstance.Info("CmdHelper.Excute END - Output:[" + text + "]");
            return text;
        }
        catch
        {
            return null;
        }
    }
}
