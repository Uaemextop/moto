using System.Windows;

namespace lenovo.themes.generic;

/// <summary>
/// Interface for message view V6 dialogs.
/// </summary>
public interface IMessageViewV6
{
    void ShowMessage(string title, string message);
    void ShowMessage(string title, string message, string confirmText);
    bool ShowConfirm(string title, string message);
    bool ShowConfirm(string title, string message, string confirmText, string cancelText);
    void Close();
}

/// <summary>
/// Interface for message window V6.
/// </summary>
public interface IMessageWindowV6
{
    void ShowMessage(string title, string message);
    bool ShowConfirm(string title, string message);
    void Close();
}

/// <summary>
/// Interface for root page view.
/// </summary>
public interface IRootPageView
{
    void Navigate(object page);
    void GoBack();
}

/// <summary>
/// Interface for loading indicator.
/// </summary>
public interface ILoading
{
    void Show();
    void Hide();
    void Show(string message);
}
