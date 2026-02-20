using FluentAssertions;
using lenovo.mbg.service.framework.devicemgt;
using Xunit;

namespace LMSA.Tests.DeviceManagement
{
    public class DeviceInfoTests
    {
        [Fact]
        public void DeviceInfo_DefaultState_IsUnknown()
        {
            var info = new DeviceInfo();
            info.State.Should().Be(DevicePhysicalStateEx.Unknown);
        }

        [Fact]
        public void DeviceInfo_DefaultSerialNumber_IsEmpty()
        {
            var info = new DeviceInfo();
            info.SerialNumber.Should().BeEmpty();
        }

        [Fact]
        public void DeviceInfo_DefaultDictionaries_AreInitialized()
        {
            var info = new DeviceInfo();
            info.FastbootVariables.Should().NotBeNull();
            info.AdbProperties.Should().NotBeNull();
        }

        [Fact]
        public void DeviceInfo_ToString_ContainsSerialNumber()
        {
            var info = new DeviceInfo
            {
                SerialNumber = "ABC123",
                Model = "Moto G",
                Manufacturer = "Motorola",
                State = DevicePhysicalStateEx.Online
            };

            string str = info.ToString();
            str.Should().Contain("ABC123");
            str.Should().Contain("Moto G");
            str.Should().Contain("Motorola");
        }

        [Theory]
        [InlineData(DevicePhysicalStateEx.Offline)]
        [InlineData(DevicePhysicalStateEx.Online)]
        [InlineData(DevicePhysicalStateEx.Fastboot)]
        [InlineData(DevicePhysicalStateEx.Recovery)]
        [InlineData(DevicePhysicalStateEx.EDL)]
        [InlineData(DevicePhysicalStateEx.Unknown)]
        public void DevicePhysicalStateEx_AllValuesExist(DevicePhysicalStateEx state)
        {
            state.Should().BeOneOf(
                DevicePhysicalStateEx.Offline,
                DevicePhysicalStateEx.Online,
                DevicePhysicalStateEx.Fastboot,
                DevicePhysicalStateEx.Recovery,
                DevicePhysicalStateEx.EDL,
                DevicePhysicalStateEx.Unknown);
        }
    }
}
