using System;
using System.Windows;
using System.Windows.Documents;

namespace lenovo.mbg.service.framework.lang;

public class LangRun : Run
{
	[Obsolete("LangText 不再使用，替换为 LangKey")]
	public static readonly DependencyProperty LangTextProperty = DependencyProperty.Register("LangText", typeof(string), typeof(LangRun), new PropertyMetadata(string.Empty, null));

	public static readonly DependencyProperty LangKeyProperty = DependencyProperty.RegisterAttached("LangKey", typeof(string), typeof(LangRun), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnLangKeyChanged)));

	public static readonly DependencyProperty RunWidthProperty = DependencyProperty.RegisterAttached("RunWidth", typeof(double), typeof(LangRun), new PropertyMetadata(0.0));

	public static readonly DependencyProperty IndentationCharacterProperty = DependencyProperty.RegisterAttached("IndentationCharacter", typeof(int), typeof(LangRun), new PropertyMetadata(0));

	[Obsolete("LangText 不再使用，替换为 LangKey")]
	public string LangText
	{
		get
		{
			return (string)GetValue(LangTextProperty);
		}
		set
		{
			SetValue(LangTextProperty, value);
		}
	}

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

	public double RunWidth
	{
		get
		{
			return (double)GetValue(RunWidthProperty);
		}
		set
		{
			SetValue(RunWidthProperty, value);
		}
	}

	public int IndentationCharacter
	{
		get
		{
			return (int)GetValue(IndentationCharacterProperty);
		}
		set
		{
			SetValue(IndentationCharacterProperty, value);
		}
	}

	public LangRun()
	{
		Loaded += delegate
		{
			if (IndentationCharacter > 0)
			{
				Text = RunHelper.ApplyIndent(this, RunWidth, IndentationCharacter);
			}
		};
	}

	private static void OnLangKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		if (args.NewValue != null && args.NewValue != args.OldValue)
		{
			((LangRun)obj).Text = LangTranslation.Translate(args.NewValue.ToString());
		}
	}
}
