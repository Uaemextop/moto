using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.framework.devicemgt.DeviceOperator;

public class ProcessHelper
{
    private static ProcessHelper m_instance;

    public static ProcessHelper Instance
    {
        get
        {
            if (m_instance != null)
            {
                return m_instance;
            }
            return m_instance = new ProcessHelper();
        }
    }

    public string Do(string exe, string command, int timeout)
    {
        StringBuilder output = new StringBuilder();
        try
        {
            using Process process = new Process();
            process.StartInfo.Arguments = command;
            process.StartInfo.FileName = exe;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            using AutoResetEvent outputWaitHandler = new AutoResetEvent(initialState: false);
            using AutoResetEvent errorWaitHandler = new AutoResetEvent(initialState: false);
            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null)
                {
                    if (!outputWaitHandler.SafeWaitHandle.IsClosed)
                    {
                        outputWaitHandler.Set();
                    }
                }
                else
                {
                    output.AppendLine(e.Data);
                }
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data == null)
                {
                    if (!errorWaitHandler.SafeWaitHandle.IsClosed)
                    {
                        errorWaitHandler.Set();
                    }
                }
                else
                {
                    output.AppendLine(e.Data);
                }
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (process.WaitForExit(timeout) && outputWaitHandler.WaitOne(timeout))
            {
                errorWaitHandler.WaitOne(timeout);
            }
        }
        catch (Exception ex)
        {
            LogHelper.LogInstance.Error(ex.ToString());
        }
        return output.ToString();
    }

    private void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            dataList.Add(e.Data);
        }
    }
}
