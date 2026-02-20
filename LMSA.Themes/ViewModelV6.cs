using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace lenovo.themes.generic.ViewModelV6;

/// <summary>
/// Base ViewModel class with INotifyPropertyChanged support.
/// </summary>
public class ViewModelV6 : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

/// <summary>
/// Alias for ViewModelV6 (used by many plugins as base class).
/// </summary>
public class ViewModelBase : ViewModelV6
{
    public virtual void LoadData() { }
    public virtual void LoadData(object data) { }
}

/// <summary>
/// Relay command implementation for MVVM.
/// </summary>
public class ReplayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;

    public ReplayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public ReplayCommand(Action execute) : this(_ => execute(), null)
    {
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object parameter) => _execute(parameter);
}
