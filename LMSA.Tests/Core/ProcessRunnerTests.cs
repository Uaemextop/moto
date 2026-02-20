using FluentAssertions;
using lenovo.mbg.service.framework.common;
using System;
using System.Collections.Generic;
using Xunit;

namespace LMSA.Tests.Core
{
    public class ProcessRunnerTests
    {
        private static readonly bool IsWindows =
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows);

        [Fact]
        public void ProcessString_EchoCommand_ReturnsOutput()
        {
            // Arrange & Act - use a cross-platform command
            string exe = IsWindows ? "cmd.exe" : "/bin/sh";
            string args = IsWindows ? "/c echo hello" : "-c echo\\ hello";

            string result = ProcessRunner.ProcessString(exe, args, 5000);

            // Assert
            result.Should().Contain("hello");
        }

        [Fact]
        public void ProcessString_NonExistentExecutable_ReturnsEmpty()
        {
            // Arrange & Act
            string result = ProcessRunner.ProcessString("nonexistent_tool_xyz.exe", "args", 5000);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void ProcessString_WithTimeout_ReturnsWithinTime()
        {
            // Arrange
            string exe = IsWindows ? "cmd.exe" : "/bin/sh";
            string args = IsWindows ? "/c echo test" : "-c echo\\ test";

            // Act
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string result = ProcessRunner.ProcessString(exe, args, 10000);
            sw.Stop();

            // Assert
            sw.ElapsedMilliseconds.Should().BeLessThan(9000);
            result.Should().Contain("test");
        }

        [Fact]
        public void ProcessList_EchoCommand_ReturnsList()
        {
            // Arrange & Act
            string exe = IsWindows ? "cmd.exe" : "/bin/sh";
            string args = IsWindows ? "/c echo hello" : "-c echo\\ hello";

            var result = ProcessRunner.ProcessList(exe, args, 5000);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle(l => l.Contains("hello"));
        }
    }
}
