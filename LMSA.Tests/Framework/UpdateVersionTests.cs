using Xunit;
using System;
using lenovo.mbg.service.framework.updateversion;
using lenovo.mbg.service.framework.updateversion.model;
using lenovo.mbg.service.framework.updateversion.impl;

namespace LMSA.Tests.Framework.UpdateVersion
{
    public class VersionModelTests
    {
        [Fact]
        public void VersionModel_Constructor_WithUrl_SetsPropertiesCorrectly()
        {
            // Arrange
            string url = "http://example.com/update.exe";

            // Act
            var model = new VersionModel(url);

            // Assert
            Assert.Equal(url, model.url);
            Assert.Equal("update.exe", model.fileName);
            Assert.Equal("0 KB/s", model.speed);
            Assert.NotNull(model.localPath);
        }

        [Fact]
        public void VersionModel_Constructor_WithFullParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            string version = "2.0.0";
            string url = "example.com/update.exe";
            bool isFull = true;
            bool isForce = false;
            string notes = "Test release";
            DateTime? date = DateTime.Now;

            // Act
            var model = new VersionModel(version, url, isFull, isForce, notes, date);

            // Assert
            Assert.Equal(version, model.version);
            Assert.StartsWith("http://", model.url);
            Assert.Equal(isFull, model.isFull);
            Assert.Equal(isForce, model.isForce);
            Assert.Equal(notes, model.releaseNotes);
            Assert.Equal(date, model.releaseDate);
        }

        [Fact]
        public void VersionModel_GetFileName_ExtractsFileNameFromUrl()
        {
            // Arrange
            string url = "http://example.com/path/to/update.exe";
            var model = new VersionModel(url);

            // Act
            string fileName = model.fileName;

            // Assert
            Assert.Equal("update.exe", fileName);
        }

        [Fact]
        public void VersionModel_GetFileName_WithBackslash_ExtractsFileNameCorrectly()
        {
            // Arrange
            string url = "http://example.com\\path\\to\\update.exe";
            var model = new VersionModel(url);

            // Act
            string fileName = model.fileName;

            // Assert
            Assert.Equal("update.exe", fileName);
        }
    }

    public class VersionV1EventArgsTests
    {
        [Fact]
        public void VersionV1EventArgs_Constructor_WithStatusOnly_SetsStatusAndNullData()
        {
            // Arrange
            var status = VersionV1Status.VERSION_CHECK_START;

            // Act
            var args = new VersionV1EventArgs(status);

            // Assert
            Assert.Equal(status, args.Status);
            Assert.Null(args.Data);
        }

        [Fact]
        public void VersionV1EventArgs_Constructor_WithStatusAndData_SetsBothProperties()
        {
            // Arrange
            var status = VersionV1Status.VERSION_CHECK_HAVE_NEW;
            var data = new { Version = "2.0.0" };

            // Act
            var args = new VersionV1EventArgs(status, data);

            // Assert
            Assert.Equal(status, args.Status);
            Assert.Equal(data, args.Data);
        }
    }

    public class CheckVersionEventArgsTests
    {
        [Fact]
        public void CheckVersionEventArgs_Constructor_WithStatusOnly_SetsProperties()
        {
            // Arrange
            bool isAutoMode = true;
            var status = CheckVersionStatus.CHECK_VERSION_START;

            // Act
            var args = new CheckVersionEventArgs(isAutoMode, status);

            // Assert
            Assert.Equal(status, args.Status);
            Assert.True(args.IsAutoMode);
            Assert.Null(args.Data);
        }

        [Fact]
        public void CheckVersionEventArgs_Constructor_WithData_SetsAllProperties()
        {
            // Arrange
            bool isAutoMode = false;
            var status = CheckVersionStatus.CHECK_VERSION_HAVE_NEW_VERSION;
            var data = new { Version = "2.0.0" };

            // Act
            var args = new CheckVersionEventArgs(isAutoMode, status, data);

            // Assert
            Assert.Equal(status, args.Status);
            Assert.False(args.IsAutoMode);
            Assert.Equal(data, args.Data);
        }
    }

    public class VersionInstallEventArgsTests
    {
        [Fact]
        public void VersionInstallEventArgs_Constructor_WithStatusOnly_SetsStatusAndNullData()
        {
            // Arrange
            var status = ServiceInstallStatus.INSTALL_START;

            // Act
            var args = new VersionInstallEventArgs(status);

            // Assert
            Assert.Equal(status, args.Status);
            Assert.Null(args.Data);
        }

        [Fact]
        public void VersionInstallEventArgs_Constructor_WithData_SetsBothProperties()
        {
            // Arrange
            var status = ServiceInstallStatus.INSTALL_SUCCESS;
            var data = "Installation completed";

            // Act
            var args = new VersionInstallEventArgs(status, data);

            // Assert
            Assert.Equal(status, args.Status);
            Assert.Equal(data, args.Data);
        }
    }

    public class DownloadStatusChangedArgsTests
    {
        [Fact]
        public void DownloadStatusChangedArgs_Constructor_WithStatusOnly_SetsStatus()
        {
            // Arrange
            var status = VersionDownloadStatus.DOWNLOAD_VERSION_START;

            // Act
            var args = new DownloadStatusChangedArgs(status);

            // Assert
            Assert.Equal(status, args.Status);
        }

        [Fact]
        public void DownloadStatusChangedArgs_Constructor_WithStatusAndData_SetsBothProperties()
        {
            // Arrange
            var status = VersionDownloadStatus.DOWNLOAD_VERSION_SUCCESS;
            var data = new { Progress = 100 };

            // Act
            var args = new DownloadStatusChangedArgs(status, data);

            // Assert
            Assert.Equal(status, args.Status);
            Assert.Equal(data, args.Data);
        }
    }

    public class UpdateVersionAutoPushTests
    {
        [Fact]
        public void UpdateVersionAutoPush_Constructor_InitializesCorrectly()
        {
            // Arrange
            var mockCheck = new MockVersionCheck();
            var mockDownload = new MockVersionDownload();
            var worker = new UpdateWoker(mockCheck, mockDownload);

            // Act
            var autoPush = new UpdateVersionAutoPush(worker);

            // Assert
            Assert.NotNull(autoPush);
            Assert.False(autoPush.IsStop);
        }

        [Fact]
        public void UpdateVersionAutoPush_Stop_SetsIsStopToTrue()
        {
            // Arrange
            var mockCheck = new MockVersionCheck();
            var mockDownload = new MockVersionDownload();
            var worker = new UpdateWoker(mockCheck, mockDownload);
            var autoPush = new UpdateVersionAutoPush(worker);

            // Act
            autoPush.Stop();

            // Assert
            Assert.True(autoPush.IsStop);
        }
    }

    public class VersionCheckV1ImplTests
    {
        [Fact]
        public void VersionCheckV1Impl_CompareVersionCode_CurrentLessThanServer_ReturnsTrue()
        {
            // Arrange
            var mockData = new MockVersionDataV1();
            var checker = new VersionCheckV1Impl(mockData);

            // Act - Use reflection to call protected method
            var method = typeof(VersionCheckV1Impl).GetMethod("CompareVersionCode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool result = (bool)method!.Invoke(checker, new object[] { "1.0.0", "2.0.0" })!;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VersionCheckV1Impl_CompareVersionCode_CurrentGreaterThanServer_ReturnsFalse()
        {
            // Arrange
            var mockData = new MockVersionDataV1();
            var checker = new VersionCheckV1Impl(mockData);

            // Act - Use reflection to call protected method
            var method = typeof(VersionCheckV1Impl).GetMethod("CompareVersionCode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool result = (bool)method!.Invoke(checker, new object[] { "2.0.0", "1.0.0" })!;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VersionCheckV1Impl_CompareVersionCode_SameVersion_ReturnsFalse()
        {
            // Arrange
            var mockData = new MockVersionDataV1();
            var checker = new VersionCheckV1Impl(mockData);

            // Act - Use reflection to call protected method
            var method = typeof(VersionCheckV1Impl).GetMethod("CompareVersionCode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool result = (bool)method!.Invoke(checker, new object[] { "1.0.0", "1.0.0" })!;

            // Assert
            Assert.False(result);
        }
    }

    // Mock classes for testing
    public class MockVersionCheck : IVersionCheck
    {
        public IVersionData VersionData => new MockVersionData();
        public event EventHandler<CheckVersionEventArgs>? OnCheckVersionStatusChanged;
        public void Check(bool isAutoMode) { }
    }

    public class MockVersionData : IVersionData
    {
        public object? GetData() => null;
        public void UpdateData(object data) { }
    }

    public class MockVersionDownload : IVersionDownload
    {
        public event EventHandler<DownloadStatusChangedArgs>? OnDownloadStatusChanged;
        public void Cancel() { }
        public void DownloadVersion(object data) { }
    }

    public class MockVersionDataV1 : IVersionDataV1
    {
        public object? Data => null;
        public event EventHandler<VersionV1EventArgs>? OnVersionEvent;
        public object? Get() => null;
    }
}
