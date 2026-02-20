using lenovo.mbg.service.framework.common;
using Xunit;
using FluentAssertions;

namespace LMSA.Tests.Core
{
    public class ResultTests
    {
        [Fact]
        public void Result_HasExpectedValues()
        {
            Result.PASSED.Should().BeDefined();
            Result.FAILED.Should().BeDefined();
            Result.QUIT.Should().BeDefined();
            Result.FASTBOOT_FLASH_FAILED.Should().BeDefined();
            Result.FASTBOOT_SHELL_FAILED.Should().BeDefined();
            Result.FASTBOOT_DEGRADE_QUIT.Should().BeDefined();
        }
    }

    public class DevicePhysicalStateExTests
    {
        [Fact]
        public void DevicePhysicalStateEx_HasExpectedValues()
        {
            DevicePhysicalStateEx.Offline.Should().BeDefined();
            DevicePhysicalStateEx.Online.Should().BeDefined();
            DevicePhysicalStateEx.Fastboot.Should().BeDefined();
            DevicePhysicalStateEx.Recovery.Should().BeDefined();
            DevicePhysicalStateEx.EDL.Should().BeDefined();
            DevicePhysicalStateEx.Unknown.Should().BeDefined();
        }
    }

    public class ErrorDetectorTests
    {
        [Theory]
        [InlineData("OKAY", false)]
        [InlineData("error: cannot connect", true)]
        [InlineData("FAILED (remote failure)", true)]
        [InlineData("Operation failed", true)]
        [InlineData("ERROR in command", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void HasError_DetectsErrorPatterns(string response, bool expected)
        {
            ErrorDetector.HasError(response).Should().Be(expected);
        }

        [Theory]
        [InlineData("STATUS_SEC_VIOLATE_ANTI_ROLLBACK", true)]
        [InlineData("OKAY", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void HasAntiRollbackViolation_DetectsPattern(string response, bool expected)
        {
            ErrorDetector.HasAntiRollbackViolation(response).Should().Be(expected);
        }

        [Fact]
        public void EvaluateResponse_ReturnsPassedForOkay()
        {
            ErrorDetector.EvaluateResponse("OKAY").Should().Be(Result.PASSED);
        }

        [Fact]
        public void EvaluateResponse_ReturnsFailedForError()
        {
            ErrorDetector.EvaluateResponse("error: device not found").Should().Be(Result.FAILED);
        }

        [Fact]
        public void EvaluateResponse_ReturnsFailedForEmpty()
        {
            ErrorDetector.EvaluateResponse("").Should().Be(Result.FAILED);
        }

        [Fact]
        public void EvaluateResponse_ReturnsFailedForNull()
        {
            ErrorDetector.EvaluateResponse(null).Should().Be(Result.FAILED);
        }

        [Fact]
        public void EvaluateResponse_ReturnsDegradeQuitForAntiRollback()
        {
            ErrorDetector.EvaluateResponse("STATUS_SEC_VIOLATE_ANTI_ROLLBACK")
                .Should().Be(Result.FASTBOOT_DEGRADE_QUIT);
        }
    }

    public class RetryHelperTests
    {
        [Fact]
        public void Execute_ReturnsResultOnFirstSuccess()
        {
            int callCount = 0;
            var result = RetryHelper.Execute(
                () => { callCount++; return "success"; },
                3,
                r => r != "success");

            result.Should().Be("success");
            callCount.Should().Be(1);
        }

        [Fact]
        public void Execute_RetriesOnFailure()
        {
            int callCount = 0;
            var result = RetryHelper.Execute(
                () => { callCount++; return callCount >= 3 ? "success" : "fail"; },
                3,
                r => r != "success");

            result.Should().Be("success");
            callCount.Should().Be(3);
        }

        [Fact]
        public void Execute_StopsAfterMaxRetries()
        {
            int callCount = 0;
            var result = RetryHelper.Execute(
                () => { callCount++; return "fail"; },
                3,
                r => r == "fail");

            result.Should().Be("fail");
            callCount.Should().Be(3);
        }

        [Fact]
        public void Execute_ThrowsOnNullAction()
        {
            var act = () => RetryHelper.Execute<string>(null!, 3);
            act.Should().Throw<System.ArgumentNullException>();
        }

        [Fact]
        public void Execute_ThrowsOnInvalidRetryCount()
        {
            var act = () => RetryHelper.Execute(() => "ok", 0);
            act.Should().Throw<System.ArgumentOutOfRangeException>();
        }
    }
}
