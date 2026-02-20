using System;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion.impl;

public class VersionCheckV1Impl : IVersionCheckV1, IVersionEvent
{
	private long locker;

	public IVersionDataV1 Data { get; }

	public bool IsRunning => Interlocked.Read(ref locker) != 0;

	public event EventHandler<VersionV1EventArgs>? OnVersionEvent;

	public VersionCheckV1Impl(IVersionDataV1 data)
	{
		Data = data;
	}

	public void Check()
	{
		Do(async: false);
	}

	public void CheckAsync()
	{
		Do(async: true);
	}

	private void Do(bool async)
	{
		if (IsRunning)
		{
			return;
		}
		Interlocked.Exchange(ref locker, 1L);
		Task task = Task.Factory.StartNew(delegate
		{
			FireVersionEvent(new VersionV1EventArgs(VersionV1Status.VERSION_CHECK_START));
			object? obj = Data.Get();
			if (obj != null)
			{
				VersionModel? versionModel = obj as VersionModel;
				if (versionModel != null && CompareVersionCode(GlobalFun.GetLmsaVersion(), versionModel.version))
				{
					FireVersionEvent(new VersionV1EventArgs(VersionV1Status.VERSION_CHECK_HAVE_NEW, versionModel));
				}
				else
				{
					FireVersionEvent(new VersionV1EventArgs(VersionV1Status.VERSION_CHECK_HAVE_LATEST));
				}
			}
		}).ContinueWith((Task s) => Interlocked.Exchange(ref locker, 0L));
		if (!async)
		{
			task.Wait();
		}
	}

	private void FireVersionEvent(VersionV1EventArgs args)
	{
		this.OnVersionEvent?.BeginInvoke(this, args, null, null);
	}

	protected bool CompareVersionCode(string current, string server)
	{
		if (!string.IsNullOrEmpty(server))
		{
			string[] array = server.Split(new char[1] { '.' });
			string[] array2 = current.Split(new char[1] { '.' });
			int num = array.Length;
			int num2 = array2.Length;
			int[] array3 = Array.ConvertAll(array, delegate(string s)
			{
				int result = 0;
				int.TryParse(s, out result);
				return result;
			});
			int[] array4 = Array.ConvertAll(array2, delegate(string s)
			{
				int result = 0;
				int.TryParse(s, out result);
				return result;
			});
			int num3 = ((num > num2) ? num2 : num);
			for (int num4 = 0; num4 < num3; num4++)
			{
				if (array3[num4] > array4[num4])
				{
					return true;
				}
				if (array3[num4] < array4[num4])
				{
					return false;
				}
			}
			if (num > num2)
			{
				int num5 = 0;
				for (int num6 = num2; num6 < num; num6++)
				{
					num5 += array3[num6];
				}
				if (num5 > 0)
				{
					return true;
				}
			}
		}
		return false;
	}
}
