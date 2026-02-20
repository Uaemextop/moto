using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace lenovo.themes.generic.ViewModels;

/// <summary>
/// Left navigation item view model.
/// </summary>
public class LeftNavigationItemViewModel : INotifyPropertyChanged
{
    private string _title;
    private bool _isSelected;
    private ICommand _command;

    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    public ICommand Command
    {
        get => _command;
        set { _command = value; OnPropertyChanged(); }
    }

    public object Tag { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
