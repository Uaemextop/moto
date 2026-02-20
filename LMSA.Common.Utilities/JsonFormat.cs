using System.Text;
using Newtonsoft.Json;

namespace lenovo.mbg.service.common.utilities;

public class JsonFormat
{
	private static string SPACE = "    ";

	private static string NEWLINE = "\r\n";

	public static string Format(string json)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int length = json.Length;
		int num = 0;
		char c = ' ';
		for (int i = 0; i < length; i++)
		{
			c = json[i];
			switch (c)
			{
			case '[':
			case '{':
				if (i - 1 > 0 && json[i - 1] == ':')
				{
					stringBuilder.Append(NEWLINE);
					stringBuilder.Append(indent(num));
				}
				stringBuilder.Append(c);
				stringBuilder.Append(NEWLINE);
				num++;
				stringBuilder.Append(indent(num));
				break;
			case ']':
			case '}':
				stringBuilder.Append(NEWLINE);
				num--;
				stringBuilder.Append(indent(num));
				stringBuilder.Append(c);
				if (i + 1 < length && json[i + 1] != ',')
				{
					stringBuilder.Append(NEWLINE);
				}
				break;
			case ',':
				stringBuilder.Append(c);
				stringBuilder.Append(NEWLINE);
				stringBuilder.Append(indent(num));
				break;
			default:
				stringBuilder.Append(c);
				break;
			}
		}
		return stringBuilder.ToString();
	}

	public static string GetJsonString(object obj)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return JsonConvert.SerializeObject(obj);
	}

	public static T GetObject<T>(string json)
	{
		return JsonConvert.DeserializeObject<T>(json);
	}

	private static string indent(int number)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < number; i++)
		{
			stringBuilder.Append(SPACE);
		}
		return stringBuilder.ToString();
	}
}
