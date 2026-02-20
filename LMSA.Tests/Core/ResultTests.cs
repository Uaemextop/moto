using FluentAssertions;
using lenovo.mbg.service.framework.common;
using Xunit;

namespace LMSA.Tests.Core
{
    public class ResultTests
    {
        [Fact]
        public void Result_PassedValue_Exists()
        {
            Result.PASSED.Should().Be(Result.PASSED);
        }

        [Fact]
        public void Result_FailedValue_Exists()
        {
            Result.FAILED.Should().Be(Result.FAILED);
        }

        [Fact]
        public void Result_QuitValue_Exists()
        {
            Result.QUIT.Should().Be(Result.QUIT);
        }

        [Theory]
        [InlineData(Result.PASSED)]
        [InlineData(Result.FAILED)]
        [InlineData(Result.QUIT)]
        [InlineData(Result.FASTBOOT_FLASH_FAILED)]
        [InlineData(Result.FASTBOOT_SHELL_FAILED)]
        [InlineData(Result.FASTBOOT_DEGRADE_QUIT)]
        public void Result_AllValuesAreDefined(Result result)
        {
            System.Enum.IsDefined(typeof(Result), result).Should().BeTrue();
        }

        [Fact]
        public void Log_AddResult_PassedDoesNotThrow()
        {
            // Arrange
            var step = new object();

            // Act & Assert
            FluentActions.Invoking(() => Log.AddResult(step, Result.PASSED, null))
                .Should().NotThrow();
        }

        [Fact]
        public void Log_AddResult_FailedDoesNotThrow()
        {
            var step = new object();
            FluentActions.Invoking(() => Log.AddResult(step, Result.FAILED, "Something went wrong"))
                .Should().NotThrow();
        }

        [Fact]
        public void Log_AddLog_DoesNotThrow()
        {
            FluentActions.Invoking(() => Log.AddLog("Test log message", upload: false))
                .Should().NotThrow();
        }

        [Fact]
        public void Log_AddLog_WithUpload_DoesNotThrow()
        {
            FluentActions.Invoking(() => Log.AddLog("Upload message", upload: true))
                .Should().NotThrow();
        }
    }
}
