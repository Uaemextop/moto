using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.webservices.WebApiModel;

namespace lenovo.mbg.service.common.webservices;

public class RsaHelper
{
    public static bool RSAPrivateKeyJava2DotNet(string privateKey, out string key)
    {
        try
        {
            RsaPrivateCrtKeyParameters rsaParams = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            key = $"<RSAKeyValue><Modulus>{Convert.ToBase64String(rsaParams.Modulus.ToByteArrayUnsigned())}</Modulus><Exponent>{Convert.ToBase64String(rsaParams.PublicExponent.ToByteArrayUnsigned())}</Exponent><P>{Convert.ToBase64String(rsaParams.P.ToByteArrayUnsigned())}</P><Q>{Convert.ToBase64String(rsaParams.Q.ToByteArrayUnsigned())}</Q><DP>{Convert.ToBase64String(rsaParams.DP.ToByteArrayUnsigned())}</DP><DQ>{Convert.ToBase64String(rsaParams.DQ.ToByteArrayUnsigned())}</DQ><InverseQ>{Convert.ToBase64String(rsaParams.QInv.ToByteArrayUnsigned())}</InverseQ><D>{Convert.ToBase64String(rsaParams.Exponent.ToByteArrayUnsigned())}</D></RSAKeyValue>";
            return true;
        }
        catch (Exception exception)
        {
            LogHelper.LogInstance.Error("RsaHelper.RSAPrivateKeyJava2DotNet error", exception);
            key = string.Empty;
            return false;
        }
    }

    public static bool RSAPrivateKeyDotNet2Java(string privateKey, out string key)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(privateKey);
            BigInteger modulus = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger publicExponent = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            BigInteger privateExponent = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
            BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
            BigInteger dP = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
            BigInteger dQ = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
            BigInteger qInv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));
            byte[] encoded = PrivateKeyInfoFactory.CreatePrivateKeyInfo(new RsaPrivateCrtKeyParameters(modulus, publicExponent, privateExponent, p, q, dP, dQ, qInv)).ToAsn1Object().GetEncoded();
            key = Convert.ToBase64String(encoded);
            return true;
        }
        catch (Exception exception)
        {
            LogHelper.LogInstance.Error("RsaHelper.RSAPrivateKeyDotNet2Java error", exception);
            key = string.Empty;
            return false;
        }
    }

    public static bool RSAPublicKeyJava2DotNet(string publicKey, out string key)
    {
        try
        {
            RsaKeyParameters rsaParams = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            key = $"<RSAKeyValue><Modulus>{Convert.ToBase64String(rsaParams.Modulus.ToByteArrayUnsigned())}</Modulus><Exponent>{Convert.ToBase64String(rsaParams.Exponent.ToByteArrayUnsigned())}</Exponent></RSAKeyValue>";
            return true;
        }
        catch (Exception exception)
        {
            LogHelper.LogInstance.Error("RsaHelper.RSAPublicKeyJava2DotNet error", exception);
            key = string.Empty;
            return false;
        }
    }

    public static bool RSAPublicKeyDotNet2Java(string publicKey, out string key)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(publicKey);
            BigInteger modulus = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger exponent = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            byte[] derEncoded = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(new RsaKeyParameters(false, modulus, exponent)).ToAsn1Object().GetDerEncoded();
            key = Convert.ToBase64String(derEncoded);
            return true;
        }
        catch (Exception exception)
        {
            LogHelper.LogInstance.Error("RsaHelper.RSAPublicKeyDotNet2Java error", exception);
            key = string.Empty;
            return false;
        }
    }

    public static RSAKey GenerateRsaKey()
    {
        RSAKey rsaKey = new RSAKey();
        using RSA rsa = RSA.Create(2048);
        rsaKey.PrivateKey = rsa.ToXmlString(true);
        rsaKey.PublicKey = rsa.ToXmlString(false);
        return rsaKey;
    }

    public static string RSAEncrypt(string xmlPublicKey, string encryptString)
    {
        if (string.IsNullOrEmpty(encryptString))
        {
            return string.Empty;
        }
        using RSA rsa = RSA.Create();
        rsa.FromXmlString(xmlPublicKey);
        byte[] bytes = Encoding.UTF8.GetBytes(encryptString);
        return Convert.ToBase64String(rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1));
    }

    public static string RSADecrypt(string xmlPrivateKey, string encryptString)
    {
        using RSA rsa = RSA.Create();
        rsa.FromXmlString(xmlPrivateKey);
        byte[] rgb = Convert.FromBase64String(encryptString);
        byte[] bytes = rsa.Decrypt(rgb, RSAEncryptionPadding.Pkcs1);
        return Encoding.UTF8.GetString(bytes);
    }

    public static string RSADecryptByPublicKey(string xmlPublicKey, string encryptString)
    {
        if (string.IsNullOrEmpty(encryptString))
        {
            return string.Empty;
        }
        using RSA rsa = RSA.Create();
        rsa.FromXmlString(xmlPublicKey);
        RsaKeyParameters rsaPublicKey = DotNetUtilities.GetRsaPublicKey(rsa);
        IBufferedCipher cipher = CipherUtilities.GetCipher("RSA");
        cipher.Init(false, rsaPublicKey);
        byte[] input = Convert.FromBase64String(encryptString);
        byte[] bytes = cipher.DoFinal(input);
        return Encoding.UTF8.GetString(bytes);
    }

    public static bool SignatureFormatter(string privateKeyXml, byte[] hashData, ref byte[] signedData)
    {
        using RSA rsa = RSA.Create();
        rsa.FromXmlString(privateKeyXml);
        RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(rsa);
        formatter.SetHashAlgorithm("MD5");
        signedData = formatter.CreateSignature(hashData);
        return true;
    }

    public static bool SignatureDeformatter(string publicKeyXml, byte[] hashData, byte[] signedData)
    {
        using RSA rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml);
        RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(rsa);
        deformatter.SetHashAlgorithm("MD5");
        return deformatter.VerifySignature(hashData, signedData);
    }
}
