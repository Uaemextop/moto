using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace lenovo.mbg.service.common.utilities;

public class RandomAesKeyHelper
{
	private readonly byte[] _RandomCodeArr;

	public static RandomAesKeyHelper _Instance;

	public string EncryptCode { get; private set; }

	public static RandomAesKeyHelper Instance
	{
		get
		{
			if (_Instance != null)
			{
				return _Instance;
			}
			return _Instance = new RandomAesKeyHelper();
		}
	}

	private RandomAesKeyHelper()
	{
		EncryptCode = GenerateRandomCode(8);
		_RandomCodeArr = Encoding.UTF8.GetBytes(GetPBKDF2Str(EncryptCode));
	}

	private string GenerateRandomCode(int length)
	{
		char[] array = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ".ToCharArray();
		Random random = new Random();
		char[] array2 = new char[length];
		for (int i = 0; i < length; i++)
		{
			array2[i] = array[random.Next(array.Length)];
		}
		return new string(array2);
	}

	public static string GetPBKDF2Str(string param)
	{
		if (param == null || param.Length != 8)
		{
			return null;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["ConnectionField"] ?? "LenovoMBGServiceCommon");
		byte[] bytes2 = new Rfc2898DeriveBytes(param, bytes, 1000, HashAlgorithmName.SHA1).GetBytes(8);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = bytes2;
		foreach (byte b in array)
		{
			stringBuilder.Append($"{b:X2}");
		}
		return stringBuilder.ToString();
	}

	public byte[] GetKeyBytes(bool isRandomKey = false)
	{
		return Generice(isRandomKey);
	}

	private byte[] Generice(bool isRandomKey = false)
	{
		if (!isRandomKey)
		{
			return Encoding.UTF8.GetBytes("LsdW^19dG1s&Pc*M").ToArray();
		}
		return _RandomCodeArr.ToArray();
	}
}
