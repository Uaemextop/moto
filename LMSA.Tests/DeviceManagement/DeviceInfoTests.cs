using System.Collections.Generic;
using lenovo.mbg.service.framework.devicemgt;
using Xunit;
using FluentAssertions;

namespace LMSA.Tests.DeviceManagement
{
    public class DevicePropertyLoaderTests
    {
        [Fact]
        public void LoadFromGetProp_ParsesProperties()
        {
            var loader = new DevicePropertyLoader();
            string output = "[ro.product.model]: [moto g]\n[ro.build.version.sdk]: [30]\n[ro.product.manufacturer]: [motorola]";

            loader.LoadFromGetProp(output);

            loader.Count.Should().Be(3);
            loader.DeviceModel.Should().Be("moto g");
            loader.SdkVersion.Should().Be("30");
            loader.Manufacturer.Should().Be("motorola");
        }

        [Fact]
        public void LoadFromGetProp_HandlesEmptyInput()
        {
            var loader = new DevicePropertyLoader();
            loader.LoadFromGetProp("");
            loader.Count.Should().Be(0);
        }

        [Fact]
        public void LoadFromGetProp_HandlesNullInput()
        {
            var loader = new DevicePropertyLoader();
            loader.LoadFromGetProp(null);
            loader.Count.Should().Be(0);
        }

        [Fact]
        public void LoadFromFastbootVars_ParsesBootloaderPrefix()
        {
            var loader = new DevicePropertyLoader();
            var lines = new List<string>
            {
                "(bootloader) product: moto_g",
                "(bootloader) serialno: ABC123",
                "(bootloader) secure: yes"
            };

            loader.LoadFromFastbootVars(lines);

            loader.Count.Should().Be(3);
            loader.GetProperty("product").Should().Be("moto_g");
            loader.GetProperty("serialno").Should().Be("ABC123");
            loader.GetProperty("secure").Should().Be("yes");
        }

        [Fact]
        public void LoadFromFastbootVars_ParsesSimpleFormat()
        {
            var loader = new DevicePropertyLoader();
            var lines = new List<string>
            {
                "product: moto_g",
                "serialno: ABC123"
            };

            loader.LoadFromFastbootVars(lines);

            loader.Count.Should().Be(2);
            loader.GetProperty("product").Should().Be("moto_g");
        }

        [Fact]
        public void LoadFromFastbootVars_HandlesNullInput()
        {
            var loader = new DevicePropertyLoader();
            loader.LoadFromFastbootVars(null);
            loader.Count.Should().Be(0);
        }

        [Fact]
        public void GetProperty_ReturnsDefaultForMissingKey()
        {
            var loader = new DevicePropertyLoader();
            loader.GetProperty("nonexistent", "default").Should().Be("default");
        }

        [Fact]
        public void HasProperty_ReturnsTrueForExistingKey()
        {
            var loader = new DevicePropertyLoader();
            loader.LoadFromGetProp("[ro.test]: [value]");
            loader.HasProperty("ro.test").Should().BeTrue();
        }

        [Fact]
        public void HasProperty_ReturnsFalseForMissingKey()
        {
            var loader = new DevicePropertyLoader();
            loader.HasProperty("nonexistent").Should().BeFalse();
        }

        [Fact]
        public void GetAllProperties_ReturnsAllLoadedProperties()
        {
            var loader = new DevicePropertyLoader();
            loader.LoadFromGetProp("[key1]: [val1]\n[key2]: [val2]");

            var all = loader.GetAllProperties();

            all.Should().HaveCount(2);
            all.Should().ContainKey("key1");
            all.Should().ContainKey("key2");
        }

        [Fact]
        public void ConvenienceAccessors_ReturnDefaults_WhenEmpty()
        {
            var loader = new DevicePropertyLoader();
            loader.DeviceModel.Should().Be("");
            loader.AndroidVersion.Should().Be("");
            loader.SerialNumber.Should().Be("");
            loader.Imei.Should().Be("");
            loader.ProductName.Should().Be("");
            loader.BuildVersion.Should().Be("");
        }
    }

    public class AdbConnectionMonitorTests
    {
        [Fact]
        public void Start_SetsIsMonitoringTrue()
        {
            var mockRunner = new Moq.Mock<lenovo.mbg.service.framework.common.IProcessRunner>();
            var adbOp = new AdbOperator(mockRunner.Object, "/fake/adb");
            var monitor = new AdbConnectionMonitor(adbOp);

            monitor.Start();

            monitor.IsMonitoring.Should().BeTrue();
        }

        [Fact]
        public void Stop_SetsIsMonitoringFalse()
        {
            var mockRunner = new Moq.Mock<lenovo.mbg.service.framework.common.IProcessRunner>();
            var adbOp = new AdbOperator(mockRunner.Object, "/fake/adb");
            var monitor = new AdbConnectionMonitor(adbOp);

            monitor.Start();
            monitor.Stop();

            monitor.IsMonitoring.Should().BeFalse();
        }
    }

    public class FastbootConnectionMonitorTests
    {
        [Fact]
        public void Start_SetsIsMonitoringTrue()
        {
            var mockRunner = new Moq.Mock<lenovo.mbg.service.framework.common.IProcessRunner>();
            var fbOp = new FastbootOperator(mockRunner.Object, "/fake/fastboot");
            var monitor = new FastbootConnectionMonitor(fbOp);

            monitor.Start();

            monitor.IsMonitoring.Should().BeTrue();
        }

        [Fact]
        public void Stop_SetsIsMonitoringFalse()
        {
            var mockRunner = new Moq.Mock<lenovo.mbg.service.framework.common.IProcessRunner>();
            var fbOp = new FastbootOperator(mockRunner.Object, "/fake/fastboot");
            var monitor = new FastbootConnectionMonitor(fbOp);

            monitor.Start();
            monitor.Stop();

            monitor.IsMonitoring.Should().BeFalse();
        }
    }
}
