using System;
using System.IO;
using System.Text.RegularExpressions;

namespace lenovo.mbg.service.framework.updateversion.model;

public class VersionModel
{
	public string url { get; set; }

	public string name { get; set; }

	public string localPath { get; set; }

	public long downloadedSize { get; set; }

	public string fileName { get; set; }

	public string speed { get; set; }

	public string? md5 { get; set; }

	public long size { get; set; }

	public bool isFull { get; set; }

	public bool isForce { get; set; }

	public string version { get; set; }

	public string? releaseNotes { get; set; }

	public DateTime? releaseDate { get; set; }

	public VersionModel(string url)
	{
		this.url = url;
		name = string.Empty;
		version = string.Empty;
		speed = "0 KB/s";
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "LMSA");
		fileName = GetFileName(url);
		localPath = Path.Combine(path, fileName);
	}

	public VersionModel(string version, string url, bool isFull, bool isForce, string notes, DateTime? date)
	{
		this.version = version;
		this.isFull = isFull;
		this.isForce = isForce;
		releaseNotes = notes;
		releaseDate = date;
		name = string.Empty;
		fileName = string.Empty;
		localPath = string.Empty;
		speed = "0 KB/s";
		if (!url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
		{
			url = "http://" + url;
		}
		this.url = url;
	}

	protected string GetFileName(string fileUrl)
	{
		string[] array = Regex.Split(fileUrl, "\\\\|/");
		if (array != null && array.Length != 0)
		{
			return array[^1];
		}
		return fileUrl;
	}
}
