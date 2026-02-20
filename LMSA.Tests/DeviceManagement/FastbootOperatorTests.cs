using System.Collections.Generic;
using lenovo.mbg.service.framework.common;
using lenovo.mbg.service.framework.devicemgt;
using Moq;
using Xunit;
using FluentAssertions;

namespace LMSA.Tests.DeviceManagement
{
    public class FastbootOperatorTests
    {
        private readonly Mock<IProcessRunner> _mockRunner;
        private readonly FastbootOperator _operator;

        public FastbootOperatorTests()
        {
            _mockRunner = new Mock<IProcessRunner>();
            _operator = new FastbootOperator(_mockRunner.Object, "/fake/fastboot.exe");
        }

        [Fact]
        public void Command_WithoutDevice_ExecutesDirectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s == "getvar all"), It.IsAny<int>()))
                       .Returns("(bootloader) product: moto");

            var result = _operator.Command("getvar all");

            result.Should().Contain("moto");
        }

        [Fact]
        public void Command_WithDeviceID_IncludesDeviceFlag()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("-s DEV123")), It.IsAny<int>()))
                       .Returns("OKAY");

            var result = _operator.Command("reboot", -1, "DEV123");

            result.Should().Be("OKAY");
        }

        [Fact]
        public void SetTargetDevice_AffectsSubsequentCommands()
        {
            _operator.SetTargetDevice("TARGET_DEVICE");

            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("-s TARGET_DEVICE reboot")), It.IsAny<int>()))
                       .Returns("OKAY");

            var result = _operator.Command("reboot");

            result.Should().Be("OKAY");
        }

        [Fact]
        public void Flash_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("flash boot")), It.IsAny<int>()))
                       .Returns("OKAY");

            var result = _operator.Flash("boot", "/path/to/boot.img");

            result.Should().Be("OKAY");
        }

        [Fact]
        public void Erase_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("erase userdata")), It.IsAny<int>()))
                       .Returns("OKAY");

            var result = _operator.Erase("userdata");

            result.Should().Be("OKAY");
        }

        [Fact]
        public void Format_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("format metadata")), It.IsAny<int>()))
                       .Returns("OKAY");

            var result = _operator.Format("metadata");

            result.Should().Be("OKAY");
        }

        [Fact]
        public void GetVarAll_ReturnsListOfLines()
        {
            _mockRunner.Setup(r => r.ProcessList(It.IsAny<string>(), It.Is<string>(s => s.Contains("getvar all")), It.IsAny<int>()))
                       .Returns(new List<string> { "(bootloader) product: moto", "(bootloader) serialno: ABC123" });

            var result = _operator.GetVarAll();

            result.Should().HaveCount(2);
        }

        [Fact]
        public void OemReadSv_CallsCorrectCommand()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("oem read_sv")), It.IsAny<int>()))
                       .Returns("sv: 1.0");

            var result = _operator.OemReadSv();

            result.Should().Contain("sv:");
        }

        [Fact]
        public void OemPartition_CallsCorrectCommand()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("oem partition")), It.IsAny<int>()))
                       .Returns("partition info");

            var result = _operator.OemPartition();

            result.Should().Contain("partition info");
        }

        [Fact]
        public void Reboot_CallsCorrectCommand()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("reboot")), It.IsAny<int>()))
                       .Returns("OKAY");

            _operator.Reboot();

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("reboot")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void RebootBootloader_CallsCorrectCommand()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("reboot-bootloader")), It.IsAny<int>()))
                       .Returns("OKAY");

            _operator.RebootBootloader();

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("reboot-bootloader")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Continue_CallsCorrectCommand()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("continue")), It.IsAny<int>()))
                       .Returns("OKAY");

            _operator.Continue();

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("continue")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void CheckAntiRollback_DetectsViolation()
        {
            _operator.CheckAntiRollback("STATUS_SEC_VIOLATE_ANTI_ROLLBACK").Should().BeTrue();
        }

        [Fact]
        public void CheckAntiRollback_ReturnsFalseForNormal()
        {
            _operator.CheckAntiRollback("OKAY").Should().BeFalse();
        }

        [Fact]
        public void FlashWithVerification_ReturnsPassed()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                       .Returns("OKAY");

            var result = _operator.FlashWithVerification("boot", "/path/boot.img");

            result.Should().Be(Result.PASSED);
        }

        [Fact]
        public void FlashWithVerification_ReturnsFailed()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                       .Returns("error: cannot flash");

            var result = _operator.FlashWithVerification("boot", "/path/boot.img");

            result.Should().Be(Result.FAILED);
        }

        [Fact]
        public void FlashWithVerification_ReturnsDegradeQuitOnAntiRollback()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                       .Returns("STATUS_SEC_VIOLATE_ANTI_ROLLBACK");

            var result = _operator.FlashWithVerification("boot", "/path/boot.img", retries: 1);

            result.Should().Be(Result.FASTBOOT_DEGRADE_QUIT);
        }

        [Fact]
        public void OperationToTimeout_HasCorrectMappings()
        {
            FastbootOperator.OperationToTimeout["flash"].Should().Be(300000);
            FastbootOperator.OperationToTimeout["erase"].Should().Be(60000);
            FastbootOperator.OperationToTimeout["flashall"].Should().Be(600000);
            FastbootOperator.OperationToTimeout["reboot"].Should().Be(20000);
            FastbootOperator.OperationToTimeout["continue"].Should().Be(10000);
        }

        [Fact]
        public void OemFbModeSet_CallsCorrectCommand()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("oem fb_mode_set")), It.IsAny<int>()))
                       .Returns("OKAY");

            _operator.OemFbModeSet();

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("oem fb_mode_set")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void OemFbModeClear_CallsCorrectCommand()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("oem fb_mode_clear")), It.IsAny<int>()))
                       .Returns("OKAY");

            _operator.OemFbModeClear();

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("oem fb_mode_clear")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Constructor_ThrowsOnNullProcessRunner()
        {
            var act = () => new FastbootOperator(null!);
            act.Should().Throw<System.ArgumentNullException>();
        }

        [Fact]
        public void EncapsulationFastbootCommand_WithNoDevice_ReturnsCommandAsIs()
        {
            var result = _operator.EncapsulationFastbootCommand("getvar all");
            result.Should().Be("getvar all");
        }

        [Fact]
        public void EncapsulationFastbootCommand_WithDevice_PrependsDeviceFlag()
        {
            _operator.SetTargetDevice("DEVICE_ID");
            var result = _operator.EncapsulationFastbootCommand("getvar all");
            result.Should().Be("-s DEVICE_ID getvar all");
        }
    }
}
