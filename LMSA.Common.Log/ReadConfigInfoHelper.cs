using System;
using System.Configuration;

namespace lenovo.mbg.service.common.log;

public class ReadConfigInfoHelper
{
	public static Configuration GetExecuteConfig()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		if (AppDomain.CurrentDomain.IsDefaultAppDomain())
		{
			return ConfigurationManager.OpenExeConfiguration((ConfigurationUserLevel)0);
		}
		return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
		{
			ExeConfigFilename = "./Software Fix.exe.config"
		}, (ConfigurationUserLevel)0);
	}
}
