using FluentAssertions;
using lenovo.mbg.service.framework.devicemgt;
using System.Collections.Generic;
using Xunit;

namespace LMSA.Tests.DeviceManagement
{
    public class FastbootOperatorTests
    {
        [Fact]
        public void IsAntiRollbackViolation_WithViolationString_ReturnsTrue()
        {
            // Arrange
            string response = "FAILED (remote: STATUS_SEC_VIOLATE_ANTI_ROLLBACK)";

            // Act
            bool result = FastbootOperator.IsAntiRollbackViolation(response);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsAntiRollbackViolation_WithNormalResponse_ReturnsFalse()
        {
            // Arrange
            string response = "OKAY [  0.000s]";

            // Act
            bool result = FastbootOperator.IsAntiRollbackViolation(response);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsAntiRollbackViolation_WithNull_ReturnsFalse()
        {
            FastbootOperator.IsAntiRollbackViolation(null!).Should().BeFalse();
        }

        [Fact]
        public void IsError_WithErrorKeyword_ReturnsTrue()
        {
            FastbootOperator.IsError("FAILED error occurred").Should().BeTrue();
        }

        [Fact]
        public void IsError_WithFailKeyword_ReturnsTrue()
        {
            FastbootOperator.IsError("operation failed").Should().BeTrue();
        }

        [Fact]
        public void IsError_WithSuccessResponse_ReturnsFalse()
        {
            FastbootOperator.IsError("OKAY [  0.022s]").Should().BeFalse();
        }

        [Fact]
        public void IsError_WithNull_ReturnsTrue()
        {
            // Null/empty response is treated as an error
            FastbootOperator.IsError(null!).Should().BeTrue();
        }

        [Fact]
        public void IsError_WithEmptyString_ReturnsTrue()
        {
            FastbootOperator.IsError(string.Empty).Should().BeTrue();
        }
    }
}
