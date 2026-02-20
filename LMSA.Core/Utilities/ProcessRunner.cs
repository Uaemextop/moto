using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.common.utilities;

public class ProcessRunner
{
    public static string ProcessString(string exe, string command, int timeout)
    {
        List<string> list = ProcessList(exe, command, timeout);
        if (list != null && list.Count > 0)
        {
            string text = string.Join("\r\n", list);
            if (text.EndsWith("\r\n"))
            {
                text = text.Remove(text.LastIndexOf("\r\n"));
            }
            return text;
        }
        return string.Empty;
    }

    public static string ProcessString(string exe, string command, int timeout, Action<Process> action, int period, out long exitCode, string? workingDirectory = null)
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
                process.Exited += (sender, e) =>
                {
                    processExited = true;
                    try
                    {
                        inputTimer?.Dispose();
                    }
                    catch
                    {
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (action != null)
                {
                    inputTimer = new Timer(_ =>
                    {
                        try
                        {
                            if (!processExited && !process.HasExited)
                            {
                                action?.Invoke(process);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }, null, TimeSpan.FromMilliseconds(period), TimeSpan.FromMilliseconds(period));
                }
                if (!process.WaitForExit(timeout))
                {
                    response.Add($"execute error, commnad timeout: {timeout}");
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                response.Add($"execute occur an error, {ex}");
            }
            exitCode = process.ExitCode;
            return string.Join("\r\n", response);
        }
        finally
        {
            process.Dispose();
        }
    }

    public static string ProcessString(string exe, string command, int timeout, Action<Process> action, int period, string? workingDirectory = null)
    {
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
                process.Exited += (sender, e) =>
                {
                    processExited = true;
                    try
                    {
                        inputTimer?.Dispose();
                    }
                    catch
                    {
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (action != null)
                {
                    inputTimer = new Timer(_ =>
                    {
                        try
                        {
                            if (!processExited && !process.HasExited)
                            {
                                action?.Invoke(process);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }, null, TimeSpan.FromMilliseconds(period), TimeSpan.FromMilliseconds(period));
                }
                if (!process.WaitForExit(timeout))
                {
                    response.Add($"execute error, commnad timeout: {timeout}");
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                response.Add($"execute occur an error, {ex}");
            }
            return string.Join("\r\n", response);
        }
        finally
        {
            process.Dispose();
        }
    }

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
            if (!string.IsNullOrEmpty(e.Data))
            {
                output.Add(e.Data);
            }
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                error.Add(e.Data);
            }
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
            if (output.Count > 0)
            {
                list.AddRange(output);
            }
            if (error.Count > 0)
            {
                list.AddRange(error);
            }
            return list;
        }
        catch (Exception ex)
        {
            list.Add($"execute occur an error, {ex}");
            return list;
        }
    }

    public static List<string> ProcessListKeyWord(string exe, string command, int timeout, List<string> keyword, string? workingDirectory = null)
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
            if (!string.IsNullOrEmpty(e.Data))
            {
                output.Add(e.Data);
            }
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                error.Add(e.Data);
            }
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
                    string[] outputSnapshot = output.ToArray().Where(p => !p.ToLower().Contains("waiting for")).ToArray();
                    string[] errorSnapshot = error.ToArray().Where(p => !p.ToLower().Contains("waiting for")).ToArray();
                    if (error.Count <= 1 && output.Count <= 1)
                    {
                        list.Add($"execute error, commnad timeout: {timeout}");
                        LogHelper.LogInstance.Debug("execute commnad timeout, command: " + command);
                        if (!process.HasExited)
                        {
                            process.Kill();
                        }
                        return list;
                    }
                    bool hasKeyword = false;
                    if (keyword != null && keyword.Count > 0)
                    {
                        hasKeyword = keyword.Any(item => outputSnapshot.Any(p => p.Contains(item)))
                                  || keyword.Any(item => errorSnapshot.Any(p => p.Contains(item)));
                    }
                    if (hasKeyword)
                    {
                        LogHelper.LogInstance.Debug($"TestLog--execute timeout ,but has data,try sleep:{retryCount * 500} milliseconds");
                        break;
                    }
                    Thread.Sleep(500);
                }
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            if (output.Count > 0)
            {
                list.AddRange(output);
            }
            if (error.Count > 0)
            {
                list.AddRange(error);
            }
            if (retryCount == 4)
            {
                list.Add($"execute error, commnad timeout: {timeout}");
                LogHelper.LogInstance.Debug("execute commnad timeout,buffer data residue， command: " + command);
                if (!process.HasExited)
                {
                    process.Kill();
                }
                return list;
            }
            return list;
        }
        catch (InvalidOperationException) when (process.HasExited)
        {
            LogHelper.LogInstance.Debug("test---result-exited--error");
            if (output.Count > 0)
            {
                list.AddRange(output);
            }
            if (error.Count > 0)
            {
                list.AddRange(error);
            }
            return list;
        }
        catch (Exception ex)
        {
            list.Add($"execute occur an error, {ex}");
            return list;
        }
    }

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

    public static string CmdExcuteWithExit(string cmdStr, string? workdir = null, int timeOut = 60000)
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
                return string.Empty;
            }
            string text = string.Empty;
            StreamReader? standardOutput = process.StandardOutput;
            StreamReader? standardError = process.StandardError;
            if (standardOutput != null)
            {
                text = standardOutput.ReadToEnd();
            }
            if (standardError != null)
            {
                text += standardError.ReadToEnd();
            }
            LogHelper.LogInstance.Info("CmdHelper.Excute END - Ouput:[" + text + "]");
            return text;
        }
        catch
        {
            return string.Empty;
        }
    }
}
