using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice;

public class RecipeInfo
{
	public string Name { get; private set; }

	public string UseCase { get; private set; }

	public bool CheckOpenSSL { get; private set; }

	public List<Tuple<bool, string, string>> DriverList { get; private set; }

	public List<StepInfo> Steps { get; private set; }

	public dynamic Args { get; private set; }

	public bool CheckClientVersion { get; private set; }

	public void Load(dynamic content)
	{
		Name = content.Name;
		UseCase = content.UseCase;
		CheckOpenSSL = content.CheckOpenSSL ?? ((object)false);
		DriverList = new List<Tuple<bool, string, string>>();
		if (content.CheckDrivers is JArray { HasValues: not false } jArray)
		{
			foreach (JToken item in jArray)
			{
				DriverList.Add(new Tuple<bool, string, string>(item.Value<bool>("IsInf"), item.Value<string>("DriverName"), item.Value<string>("FileName")));
			}
		}
		Steps = new List<StepInfo>();
		CheckClientVersion = content.CheckClientVersion ?? ((object)false);
		foreach (dynamic item2 in content.Steps)
		{
			StepInfo stepInfo = new StepInfo();
			stepInfo.Load(item2);
			Steps.Add(stepInfo);
		}
		Args = new ExpandoObject();
	}
}
