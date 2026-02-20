using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.Controls;

/// <summary>
/// Icon button control stub.
/// </summary>
public class IconButton : Button
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(object), typeof(IconButton));

    public object Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}

/// <summary>
/// Custom ComboBox control stub.
/// </summary>
public class LComboBox : ComboBox
{
}
