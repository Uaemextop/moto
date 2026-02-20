using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
// using System.Windows;
using System.Windows.Forms;
// using System.Windows.Interop;

namespace lenovo.mbg.service.common.utilities;

public class HardwareHelper
{
	public class Com
	{
		public struct DEV_BROADCAST_HDR
		{
			public uint dbch_size;

			public uint dbch_devicetype;

			public uint dbch_reserved;
		}

		protected struct DEV_BROADCAST_PORT_Fixed
		{
			public uint dbcp_size;

			public uint dbcp_devicetype;

			public uint dbcp_reserved;
		}

		public const int WM_DEVICE_CHANGE = 537;

		public const int DBT_DEVICEARRIVAL = 32768;

		public const int DBT_DEVICE_REMOVE_COMPLETE = 32772;

		public const uint DBT_DEVTYP_PORT = 3u;

		public static string GetComPortName(IntPtr wParam, IntPtr lParam)
		{
			if (((DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR))).dbch_devicetype == 3)
			{
				return Marshal.PtrToStringUni(IntPtr.Add(lParam, Marshal.SizeOf(typeof(DEV_BROADCAST_PORT_Fixed))));
			}
			return string.Empty;
		}
	}

	public static string GetHardwareInfo(HardwareEnum hardType, string propKey, string valKeyWords)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		new List<string>();
		ManagementObjectSearcher val = null;
		try
		{
			val = new ManagementObjectSearcher("select * from " + hardType);
		}
		catch (Exception)
		{
			return string.Empty;
		}
		finally
		{
			if (val != null)
			{
				try
				{
					((Component)(object)val).Dispose();
				}
				catch (Exception)
				{
				}
			}
		}
		return string.Empty;
	}

	// WPF-dependent method - requires System.Windows
	// public static Rectangle GetPosition(Window win) { throw new NotImplementedException("WPF required"); }
}
