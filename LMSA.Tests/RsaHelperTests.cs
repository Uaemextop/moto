using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace LMSA.Tests;

public class RsaHelperTests
{
    [Fact]
    public void GenerateRsaKey_ReturnsValidKeys()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();

        Assert.NotNull(key);
        Assert.NotNull(key.PublicKey);
        Assert.NotNull(key.PrivateKey);
        Assert.Contains("<RSAKeyValue>", key.PublicKey);
        Assert.Contains("<RSAKeyValue>", key.PrivateKey);
    }

    [Fact]
    public void RSAEncrypt_And_RSADecrypt_Roundtrip()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();
        string plainText = "Hello, LMSA!";

        string encrypted = RsaHelper.RSAEncrypt(key.PublicKey, plainText);
        Assert.NotNull(encrypted);
        Assert.NotEqual(plainText, encrypted);

        string decrypted = RsaHelper.RSADecrypt(key.PrivateKey, encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void RSAEncrypt_EmptyString_ReturnsEmpty()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();
        string result = RsaHelper.RSAEncrypt(key.PublicKey, "");

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void RSAPublicKeyDotNet2Java_And_Back()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();

        bool success1 = RsaHelper.RSAPublicKeyDotNet2Java(key.PublicKey, out string javaKey);
        Assert.True(success1);
        Assert.NotEmpty(javaKey);

        bool success2 = RsaHelper.RSAPublicKeyJava2DotNet(javaKey, out string dotNetKey);
        Assert.True(success2);
        Assert.Contains("<RSAKeyValue>", dotNetKey);
    }

    [Fact]
    public void RSAPrivateKeyDotNet2Java_And_Back()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();

        bool success1 = RsaHelper.RSAPrivateKeyDotNet2Java(key.PrivateKey, out string javaKey);
        Assert.True(success1);
        Assert.NotEmpty(javaKey);

        bool success2 = RsaHelper.RSAPrivateKeyJava2DotNet(javaKey, out string dotNetKey);
        Assert.True(success2);
        Assert.Contains("<RSAKeyValue>", dotNetKey);
    }

    [Fact]
    public void RSAPublicKeyJava2DotNet_InvalidKey_ReturnsFalse()
    {
        bool success = RsaHelper.RSAPublicKeyJava2DotNet("not-a-valid-key", out string key);
        Assert.Empty(key);
    }

    [Fact]
    public void RSADecryptByPublicKey_EmptyString_ReturnsEmpty()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();
        string result = RsaHelper.RSADecryptByPublicKey(key.PublicKey, "");
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void SignatureFormatter_CreatesValidSignature()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();
        byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(
            System.Text.Encoding.UTF8.GetBytes("test data"));
        byte[] signedData = null;

        bool success = RsaHelper.SignatureFormatter(key.PrivateKey, hash, ref signedData);

        Assert.True(success);
        Assert.NotNull(signedData);
        Assert.NotEmpty(signedData);
    }

    [Fact]
    public void SignatureDeformatter_VerifiesValidSignature()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();
        byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(
            System.Text.Encoding.UTF8.GetBytes("test data"));
        byte[] signedData = null;

        RsaHelper.SignatureFormatter(key.PrivateKey, hash, ref signedData);
        bool verified = RsaHelper.SignatureDeformatter(key.PublicKey, hash, signedData);

        Assert.True(verified);
    }

    [Fact]
    public void SignatureDeformatter_RejectsInvalidSignature()
    {
        RSAKey key = RsaHelper.GenerateRsaKey();
        byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(
            System.Text.Encoding.UTF8.GetBytes("test data"));
        byte[] fakeSignature = new byte[128];

        bool verified = RsaHelper.SignatureDeformatter(key.PublicKey, hash, fakeSignature);

        Assert.False(verified);
    }
}
