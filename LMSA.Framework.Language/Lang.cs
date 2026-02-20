using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.lang;

public class Lang
{
	public static readonly DependencyProperty LangSourceProperty = DependencyProperty.RegisterAttached("LangSource", typeof(LangSource), typeof(Lang), new PropertyMetadata(null, delegate(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		string path = ((LangSource)args.NewValue).Path;
		if (!string.IsNullOrEmpty(path))
		{
			PropertyInfo property = obj.GetType().GetProperty(path);
			if (!(property == null))
			{
				string value = LangTranslation.Translate(property.GetValue(obj)?.ToString());
				property.SetValue(obj, value);
			}
		}
	}));

	public static LangSource GetLangSource(DependencyObject obj)
	{
		return (LangSource)obj.GetValue(LangSourceProperty);
	}

	public static void SetLangSource(DependencyObject obj, LangSource value)
	{
		obj.SetValue(LangSourceProperty, value);
	}

	public static void OnBaseTextBlockLangKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		TextBlock val = (TextBlock)(object)((obj is TextBlock) ? obj : null);
		val.Inlines.Clear();
		if (args.NewValue == null || args.NewValue == args.OldValue)
		{
			return;
		}
		string text = LangTranslation.Translate(args.NewValue.ToString());
		string pattern = "\\[(a|h1|h2|h3|h4|r|b4|b5|b6|b7|h|b|bc|c|bh|ch|bch)([^\\]]*?)\\](.*?)\\[/\\1\\]";
		object obj2 = Application.Current.TryFindResource("SystemFontKey");
		FontFamily fontFamily = (FontFamily)((obj2 is FontFamily) ? obj2 : null);
		int num = 0;
		foreach (Match item in Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline))
		{
			if (item.Index > num)
			{
				string text2 = text.Substring(num, item.Index - num).Replace("\\n", "\n");
				AddTextWithLineBreaks(val, text2, fontFamily);
			}
			string text3 = item.Groups[1].Value.ToLower();
			string value = item.Groups[2].Value;
			string text4 = item.Groups[3].Value.Replace("\\n", "\n");
			Run run = new Run
			{
				Text = text4,
				FontFamily = fontFamily
			};
			switch (text3)
			{
			case "a":
			{
				object obj3 = Application.Current.TryFindResource("V6_HyperLinkBrushKey");
				SolidColorBrush foreground = (SolidColorBrush)((obj3 is SolidColorBrush) ? obj3 : null);
				run.Foreground = foreground;
				run.FontWeight = FontWeight.FromOpenTypeWeight(400);
				run.Cursor = Cursors.Hand;
				run.TextDecorations.Add(new TextDecoration
				{
					Location = TextDecorationLocation.Underline
				});
				if (Enumerable.Contains(text4, '|'))
				{
					string[] array = text4.Split(new char[1] { '|' });
					run.Text = array[0];
					run.Tag = array[1];
				}
				else
				{
					run.Tag = text4;
				}
				run.MouseLeftButtonDown += delegate
				{
					string text6 = run.Tag?.ToString();
					if (text6 == "rasa.page.link/ma")
					{
						text6 = Configurations.QrCodeDownloadMaUrl;
					}
					if (!string.IsNullOrEmpty(text6))
					{
						GlobalFun.OpenUrlByBrowser(text6);
					}
				};
				val.Inlines.Add(run);
				break;
			}
			case "h1":
			case "h2":
			case "h3":
			case "h4":
			{
				int num2 = text3 switch
				{
					"h1" => 16, 
					"h2" => 15, 
					"h3" => 14, 
					"h4" => 13, 
					_ => 14, 
				};
				object obj5 = Application.Current.TryFindResource("V6_TitleBrushKey");
				SolidColorBrush foreground3 = (SolidColorBrush)((obj5 is SolidColorBrush) ? obj5 : null);
				run.FontSize = num2;
				run.FontWeight = FontWeight.FromOpenTypeWeight(500);
				run.Foreground = foreground3;
				val.Inlines.Add(run);
				break;
			}
			case "r":
			{
				object obj4 = Application.Current.TryFindResource("V6_WarnningBrushKey");
				SolidColorBrush foreground2 = (SolidColorBrush)((obj4 is SolidColorBrush) ? obj4 : null);
				run.FontWeight = FontWeight.FromOpenTypeWeight(500);
				run.Foreground = foreground2;
				val.Inlines.Add(run);
				break;
			}
			case "b4":
			case "b5":
			case "b6":
			case "b7":
				run.FontWeight = FontWeight.FromOpenTypeWeight(text3 switch
				{
					"b4" => 400, 
					"b5" => 500, 
					"b6" => 600, 
					"b7" => 700, 
					_ => 400, 
				});
				val.Inlines.Add(run);
				break;
			case "c":
			case "b":
			case "h":
			case "bc":
			case "bh":
			case "ch":
			case "bch":
				CustomStyle(ref run, value);
				val.Inlines.Add(run);
				break;
			}
			num = item.Index + item.Length;
		}
		if (num < text.Length)
		{
			string text5 = text.Substring(num).Replace("\\n", "\n");
			AddTextWithLineBreaks(val, text5, fontFamily);
		}
	}

	public static void CustomStyle(ref Run run, string attr)
	{
		string pattern = "color\\s*=\\s*['\"]?(#[0-9a-fA-F]{6,8})['\"]?";
		string pattern2 = "weight\\s*=\\s*['\"]?(\\d+)['\"]?";
		string pattern3 = "size\\s*=\\s*['\"]?(\\d+)['\"]?";
		Match match = Regex.Match(attr, pattern3);
		Match match2 = Regex.Match(attr, pattern);
		Match match3 = Regex.Match(attr, pattern2);
		int result = 14;
		if (match.Success && int.TryParse(match.Groups[1].Value, out result))
		{
			run.FontSize = result;
		}
		else
		{
			run.FontSize = 14.0;
		}
		if (match2.Success)
		{
			run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(match2.Groups[1].Value));
		}
		int result2 = 400;
		if (match3.Success && int.TryParse(match3.Groups[1].Value, out result2))
		{
			result2 = ((result2 < 100) ? 100 : result2);
			result2 = ((result2 > 900) ? 900 : result2);
			run.FontWeight = FontWeight.FromOpenTypeWeight(result2);
		}
		else
		{
			run.FontWeight = FontWeight.FromOpenTypeWeight(400);
		}
	}

	private static void AddTextWithLineBreaks(TextBlock block, string text, FontFamily fontFamily)
	{
		string[] array = text.Split(new char[1] { '\n' });
		for (int i = 0; i < array.Length; i++)
		{
			if (i > 0)
			{
				block.Inlines.Add(new LineBreak());
			}
			block.Inlines.Add(new Run
			{
				Text = array[i],
				FontFamily = fontFamily
			});
		}
	}
}
