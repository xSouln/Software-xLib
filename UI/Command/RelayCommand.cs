﻿using System;
using System.Windows.Input;

namespace xLibV100.UI
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Action<RelayCommand, object> _extansionExecute;
        private readonly Func<object, bool> _canExecute;

        private object content;

        public object Content
        {
            get => content;
            set => content = value;
        }

        public object[] Parameters;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action<RelayCommand, object> execute, Func<object, bool> canExecute = null)
        {
            _extansionExecute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
            _extansionExecute?.Invoke(this, parameter);
        }
    }
}
