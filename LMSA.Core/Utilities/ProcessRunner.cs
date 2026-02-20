using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public static List<string> ProcessList(string exe, string command, int timeout, string workingDirectory = null)
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
        catch (Exception arg)
        {
            list.Add($"execute occur an error, {arg}");
            return list;
        }
    }

    public static List<string> ProcessListKeyWord(string exe, string command, int timeout, List<string> keyWords)
    {
        List<string> list = new List<string>();
        List<string> output = new List<string>();
        List<string> error = new List<string>();
        using Process process = new Process();
        process.StartInfo.FileName = exe;
        process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
        process.StartInfo.Arguments = command;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.EnableRaisingEvents = true;
        process.StartInfo.CreateNoWindow = true;
        bool found = false;
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                output.Add(e.Data);
                if (keyWords != null && keyWords.Exists(k => e.Data.Contains(k)))
                {
                    found = true;
                }
            }
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                error.Add(e.Data);
                if (keyWords != null && keyWords.Exists(k => e.Data.Contains(k)))
                {
                    found = true;
                }
            }
        };
        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            DateTime startTime = DateTime.Now;
            while (!found && (DateTime.Now - startTime).TotalMilliseconds < timeout)
            {
                if (process.WaitForExit(100))
                {
                    break;
                }
            }
            if (!process.HasExited)
            {
                if (!found)
                {
                    list.Add($"execute error, commnad timeout: {timeout}");
                    LogHelper.LogInstance.Debug("execute commnad timeout, command: " + command);
                }
                try { process.Kill(); } catch { }
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
        catch (Exception arg)
        {
            list.Add($"execute occur an error, {arg}");
            return list;
        }
    }

    public static void KillProcess(string processName)
    {
        try
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process p in processes)
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(3000);
                }
                catch (Exception ex)
                {
                    LogHelper.LogInstance.Error($"Failed to kill process {processName}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.LogInstance.Error($"Error getting processes for {processName}: {ex.Message}");
        }
    }

    public static string Shell(string command, string excuteExe = "cmd.exe", int timeout = 2000)
    {
        Process process = new Process();
        process.StartInfo.FileName = excuteExe;
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
}
