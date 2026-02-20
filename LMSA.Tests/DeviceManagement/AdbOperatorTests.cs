using System.Collections.Generic;
using lenovo.mbg.service.framework.common;
using lenovo.mbg.service.framework.devicemgt;
using Moq;
using Xunit;
using FluentAssertions;

namespace LMSA.Tests.DeviceManagement
{
    public class AdbOperatorTests
    {
        private readonly Mock<IProcessRunner> _mockRunner;
        private readonly AdbOperator _operator;

        public AdbOperatorTests()
        {
            _mockRunner = new Mock<IProcessRunner>();
            _operator = new AdbOperator(_mockRunner.Object, "/fake/adb.exe");
        }

        [Fact]
        public void Command_WithDeviceID_IncludesDeviceFlag()
        {
            _mockRunner.Setup(r => r.ProcessString("/fake/adb.exe", "-s ABC123 shell getprop", -1))
                       .Returns("ro.build=test");

            var result = _operator.Command("shell getprop", -1, "ABC123");

            result.Should().Be("ro.build=test");
            _mockRunner.Verify(r => r.ProcessString("/fake/adb.exe", "-s ABC123 shell getprop", -1), Times.Once);
        }

        [Fact]
        public void Command_WithoutDeviceID_OmitsDeviceFlag()
        {
            _mockRunner.Setup(r => r.ProcessString("/fake/adb.exe", "devices", -1))
                       .Returns("List of devices attached");

            var result = _operator.Command("devices");

            result.Should().Contain("List of devices");
        }

        [Fact]
        public void Shell_PrependsShellKeyword()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("shell getprop")), It.IsAny<int>()))
                       .Returns("[ro.build]: [test]");

            var result = _operator.Shell("getprop");

            result.Should().Contain("[ro.build]");
        }

        [Fact]
        public void GetProperties_CallsShellGetprop()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                       .Returns("[ro.product.model]: [moto g]");

            var result = _operator.GetProperties();

            result.Should().Contain("moto g");
        }

        [Fact]
        public void InstallPackage_WithReinstall_IncludesFlag()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("install -r")), It.IsAny<int>()))
                       .Returns("Success");

            var result = _operator.InstallPackage("/path/to/app.apk", reinstall: true);

            result.Should().Be("Success");
        }

        [Fact]
        public void InstallPackage_WithoutReinstall_OmitsFlag()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("install \"/path")), It.IsAny<int>()))
                       .Returns("Success");

            var result = _operator.InstallPackage("/path/to/app.apk", reinstall: false);

            result.Should().Be("Success");
        }

        [Fact]
        public void UninstallPackage_CallsCorrectCommand()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("uninstall com.test.app")), It.IsAny<int>()))
                       .Returns("Success");

            var result = _operator.UninstallPackage("com.test.app");

            result.Should().Be("Success");
        }

        [Fact]
        public void Push_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("push")), It.IsAny<int>()))
                       .Returns("1 file pushed");

            var result = _operator.Push("/local/file.txt", "/sdcard/file.txt");

            result.Should().Contain("file pushed");
        }

        [Fact]
        public void Pull_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("pull")), It.IsAny<int>()))
                       .Returns("1 file pulled");

            var result = _operator.Pull("/sdcard/file.txt", "/local/file.txt");

            result.Should().Contain("file pulled");
        }

        [Fact]
        public void RebootBootloader_CallsCorrectMode()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("reboot bootloader")), It.IsAny<int>()))
                       .Returns("");

            _operator.RebootBootloader();

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("reboot bootloader")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void GetDevices_ParsesDeviceList()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("devices")), It.IsAny<int>()))
                       .Returns("List of devices attached\nABC123\tdevice\nDEF456\tdevice\n");

            var devices = _operator.GetDevices();

            devices.Should().HaveCount(2);
            devices.Should().Contain("ABC123");
            devices.Should().Contain("DEF456");
        }

        [Fact]
        public void GetDevices_ReturnsEmptyListWhenNoDevices()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                       .Returns("List of devices attached\n");

            var devices = _operator.GetDevices();

            devices.Should().BeEmpty();
        }

        [Fact]
        public void CreateForward_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("forward tcp:5000 tcp:6000")), It.IsAny<int>()))
                       .Returns("");

            _operator.CreateForward("5000", "6000");

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("forward tcp:5000 tcp:6000")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Connect_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("connect 192.168.1.100:5555")), It.IsAny<int>()))
                       .Returns("connected to 192.168.1.100:5555");

            var result = _operator.Connect("192.168.1.100");

            result.Should().Contain("connected");
        }

        [Fact]
        public void GetSdkVersion_ReturnsCorrectProperty()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("getprop ro.build.version.sdk")), It.IsAny<int>()))
                       .Returns("30");

            var result = _operator.GetSdkVersion();

            result.Should().Be("30");
        }

        [Fact]
        public void StartActivity_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("am start -n")), It.IsAny<int>()))
                       .Returns("Starting: Intent");

            _operator.StartActivity("com.test.app", "com.test.app.MainActivity");

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("am start -n com.test.app/com.test.app.MainActivity")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ForceStop_FormatsCommandCorrectly()
        {
            _mockRunner.Setup(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("force-stop")), It.IsAny<int>()))
                       .Returns("");

            _operator.ForceStop("com.test.app");

            _mockRunner.Verify(r => r.ProcessString(It.IsAny<string>(), It.Is<string>(s => s.Contains("am force-stop com.test.app")), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Constructor_ThrowsOnNullProcessRunner()
        {
            var act = () => new AdbOperator(null!);
            act.Should().Throw<System.ArgumentNullException>();
        }
    }
}
