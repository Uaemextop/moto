using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace lenovo.mbg.service.framework.lang;

public static class RunHelper
{
	public static string ApplyIndent(LangRun run, double maxWidth, int indentationCharacter = 2)
	{
		if (string.IsNullOrEmpty(run.Text))
		{
			return string.Empty;
		}
		string text = new string(' ', indentationCharacter);
		List<string> list = new List<string>();
		string text2 = "";
		foreach (string item in (from t in Regex.Split(run.Text, "(\\s+|\\p{IsCJKUnifiedIdeographs})")
			where !string.IsNullOrEmpty(t)
			select t).ToList())
		{
			string text3 = (string.IsNullOrEmpty(text2) ? item : (text2 + item));
			if (new FormattedText(text3, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(run.FontFamily, run.FontStyle, run.FontWeight, run.FontStretch), run.FontSize, Brushes.Black, new NumberSubstitution(), 1.0).Width <= maxWidth)
			{
				text2 = text3;
				continue;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				list.Add(text2);
			}
			text2 = item.TrimStart(Array.Empty<char>());
		}
		if (!string.IsNullOrEmpty(text2))
		{
			list.Add(text2);
		}
		for (int num = 1; num < list.Count; num++)
		{
			list[num] = text + list[num];
		}
		return string.Join("\n", list);
	}
}
