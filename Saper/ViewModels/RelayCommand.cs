using System;
using System.Windows.Input;

namespace Saper.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        // Правильное объявление события
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public void Execute(object parameter) => _execute((T)parameter);

        // Правильное объявление события
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }

    public class RelayCommand<T1, T2> : ICommand
    {
        private readonly Action<T1, T2> _execute;

        public RelayCommand(Action<T1, T2> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is Tuple<T1, T2> tuple)
            {
                _execute(tuple.Item1, tuple.Item2);
            }
        }

        // Правильное объявление события
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        
        }
    }
}
