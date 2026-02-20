using System;
using Newtonsoft.Json.Linq;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion.impl;

public class VersionDataV1Impl : IVersionDataV1, IVersionEvent
{
	protected static ApiService service = new ApiService();

	public object? Data { get; private set; }

	public event EventHandler<VersionV1EventArgs>? OnVersionEvent;

	public virtual object? Get()
	{
		Data = null;
		object? obj = service.RequestContent(WebApiUrl.CLIENT_VERSION, null);
		if (obj == null)
		{
			this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_GETDATA_FAILED), null, null);
		}
		else
		{
			JObject? jObject = obj as JObject;
			if (jObject != null)
			{
				Data = new VersionModel(
					jObject.Value<string>("clientVersion") ?? string.Empty, 
					jObject.Value<string>("filePath") ?? string.Empty, 
					jObject.Value<bool>("increment"), 
					jObject.Value<bool>("forceUpdate"), 
					jObject.Value<string>("releaseNotes") ?? string.Empty, 
					GlobalFun.ConvertDateTime(jObject.Value<long>("releaseDate")));
				this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_GETDATA_SUCCESS, Data), null, null);
			}
		}
		return Data;
	}
}
