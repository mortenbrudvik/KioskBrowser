using System.Windows.Input;

namespace KioskBrowser;

public class DelegateCommand : ICommand
{
    private readonly Predicate<object?> _canExecute;
    private readonly Action<object?> _execute;

    public event EventHandler? CanExecuteChanged;

    public DelegateCommand(Action<object?> execute) : this(execute, o => true) { }

    public DelegateCommand(Action<object?> execute, Predicate<object?> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute(parameter);
    public void Execute(object? parameter) => _execute(parameter);
}