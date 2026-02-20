using Xunit;
using FluentAssertions;
using lenovo.mbg.service.lmsa.common;
using lenovo.mbg.service.lmsa.common.Form.FormVerify;
using lenovo.mbg.service.lmsa.common.ImportExport;

namespace LMSA.Tests;

public class CommonTests
{
    [Fact]
    public void ExceptionResultCodes_ShouldHave_CorrectValues()
    {
        ExceptionResultCodes.HR_ERROR_HANDLE_DISK_FULL.Should().Be(-2147024857);
        ExceptionResultCodes.HR_ERROR_DISK_FULL.Should().Be(-2147024784);
    }

    [Fact]
    public void DiskSpaceNotEnoughExcpetion_ShouldBe_Exception()
    {
        var ex = new DiskSpaceNotEnoughExcpetion("Not enough space");
        ex.Message.Should().Be("Not enough space");
        ex.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void ResourceTypeDefine_ShouldHave_AllResourceTypes()
    {
        ResourceTypeDefine.ResourceTypeMap.Should().HaveCount(8);
        ResourceTypeDefine.ResourceTypeMap[ResourceTypeDefine.PIC].Should().Be("Photos");
        ResourceTypeDefine.ResourceTypeMap[ResourceTypeDefine.MUSIC].Should().Be("Songs");
        ResourceTypeDefine.ResourceTypeMap[ResourceTypeDefine.VIDEO].Should().Be("Video");
        ResourceTypeDefine.ResourceTypeMap[ResourceTypeDefine.CONTACT].Should().Be("Contacts");
        ResourceTypeDefine.ResourceTypeMap[ResourceTypeDefine.SMS].Should().Be("SMS");
        ResourceTypeDefine.ResourceTypeMap[ResourceTypeDefine.APP].Should().Be("App");
    }

    [Fact]
    public void CanEmptyVerify_WithData_ShouldReturnCorrect()
    {
        var verify = new CanEmptyVerify();
        var result = verify.Verify("some data");
        result.IsCorrect.Should().BeTrue();
        result.WraningCode.Should().Be(2);
    }

    [Fact]
    public void CanEmptyVerify_WithNull_ShouldReturnCorrect()
    {
        var verify = new CanEmptyVerify();
        var result = verify.Verify(null);
        result.IsCorrect.Should().BeTrue();
        result.WraningCode.Should().Be(0);
    }

    [Fact]
    public void CanNotEmptyVerify_WithData_ShouldReturnCorrect()
    {
        var verify = new CanNotEmptyVerify();
        var result = verify.Verify("some data");
        result.IsCorrect.Should().BeTrue();
        result.WraningCode.Should().Be(2);
    }

    [Fact]
    public void CanNotEmptyVerify_WithNull_ShouldReturnIncorrect()
    {
        var verify = new CanNotEmptyVerify();
        var result = verify.Verify(null);
        result.IsCorrect.Should().BeFalse();
        result.WraningCode.Should().Be(1);
        result.WraningContent.Should().Be("K0041");
    }

    [Fact]
    public void PasswordVerify_ShortPassword_ShouldFail()
    {
        var verify = new PasswordVerify();
        var result = verify.Verify("abc");
        result.IsCorrect.Should().BeFalse();
        result.WraningContent.Should().Be("K0026");
    }

    [Fact]
    public void PasswordVerify_ValidPassword_ShouldPass()
    {
        var verify = new PasswordVerify();
        var result = verify.Verify("password123!");
        result.IsCorrect.Should().BeTrue();
        result.WraningCode.Should().Be(3);
    }

    [Fact]
    public void PasswordVerify_EmptyPassword_ShouldFail()
    {
        var verify = new PasswordVerify();
        var result = verify.Verify(null);
        result.IsCorrect.Should().BeFalse();
        result.WraningContent.Should().Be("K0041");
    }

    [Fact]
    public void EmailAddressVerify_ValidEmail_ShouldPass()
    {
        var verify = new EmailAddressVerify();
        var result = verify.Verify("test@example.com");
        result.IsCorrect.Should().BeTrue();
    }

    [Fact]
    public void EmailAddressVerify_InvalidEmail_ShouldFail()
    {
        var verify = new EmailAddressVerify();
        var result = verify.Verify("not-an-email");
        result.IsCorrect.Should().BeFalse();
        result.WraningContent.Should().Be("K0042");
    }

    [Fact]
    public void PhoneNumberVerify_WithData_ShouldPass()
    {
        var verify = new PhoneNumberVerify();
        var result = verify.Verify("1234567890");
        result.IsCorrect.Should().BeTrue();
        result.WraningCode.Should().Be(2);
    }

    [Fact]
    public void ResourceExecuteResult_ShouldTrackStatus()
    {
        var result = new ResourceExecuteResult();
        result.Status.Should().Be(0);
        result.ResultMapEx["success"].Should().Be(0);
        result.ResultMapEx["failed"].Should().Be(0);

        result.Update(true);
        result.Status.Should().Be(1);
        result.ResultMapEx["success"].Should().Be(1);

        result.Update(false);
        result.ResultMapEx["failed"].Should().Be(1);
    }

    [Fact]
    public void ResourceExecuteResult_Update_WithBackupRestoreResult()
    {
        var result = new ResourceExecuteResult();
        result.Update(AppDataTransferHelper.BackupRestoreResult.Success);
        result.ResultMapEx["success"].Should().Be(1);

        result.Update(AppDataTransferHelper.BackupRestoreResult.Fail);
        result.ResultMapEx["failed"].Should().Be(1);
    }
}
