using System.Collections.Generic;
using System.Threading;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ReadPropertiesInFastboot : BaseStep
{
	private readonly List<string> elements = new List<string>
	{
		"sku", "ro.boot.hardware.sku", "serialno", "imei", "ro.carrier", "version-baseband", "ro.build.fingerprint", "ro.build.version.full", "ro.build.version.qcom", "oem hw dualsim",
		"softwareVersion", "ram", "emmc", "androidVer", "fdr-allowed", "securestate", "iswarrantyvoid", "channelid", "cid"
	};

	public override void Run()
	{
		DeviceEx device = GetDevice(ConnectType.Fastboot, (object s) => (s as DeviceEx).SoftStatus == DeviceSoftStateEx.Online);
		if (device == null)
		{
			SpinWait.SpinUntil(delegate
			{
				device = GetDevice(ConnectType.Fastboot, (object s) => (s as DeviceEx).SoftStatus == DeviceSoftStateEx.Online);
				return device != null;
			}, 30000);
		}
		IAndroidDevice androidDevice = device?.Property;
		if (androidDevice != null)
		{
			foreach (string element in elements)
			{
				string propertyValue = androidDevice.GetPropertyValue(element);
				base.Log.AddInfo(element, propertyValue);
			}
		}
		RecipeMessage recipeMessage = new RecipeMessage
		{
			Message = base.Log.Info,
			RecipeName = base.Recipe.Info.Name,
			UseCase = base.Log.UseCase,
			Device = base.Recipe.Device
		};
		base.Log.NotifyAsync(RecipeMessageType.DATA, recipeMessage);
		base.Log.AddResult(this, Result.PASSED, JsonHelper.SerializeObject2Json(base.Log.Info));
	}
}
