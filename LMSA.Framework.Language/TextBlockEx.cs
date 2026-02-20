using System.Windows;
using System.Windows.Controls;

namespace lenovo.mbg.service.framework.lang;

public class TextBlockEx : TextBlock
{
	public static readonly DependencyProperty LangKeyProperty = DependencyProperty.RegisterAttached("LangKey", typeof(string), typeof(TextBlockEx), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnLangKeyChanged)));

	public string LangKey
	{
		get
		{
			return (string)GetValue(LangKeyProperty);
		}
		set
		{
			SetValue(LangKeyProperty, value);
		}
	}

	private static void OnLangKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		Lang.OnBaseTextBlockLangKeyChanged(obj, args);
	}
}
