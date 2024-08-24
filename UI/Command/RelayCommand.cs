using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace xLibV100.UI
{
    public class RelayCommandExtension
    {
        public int Index { get; set; }
        public object Extension { get; set; }

        public RelayCommandExtension(int index = 0, object extension = null)
        {
            Index = index;
            Extension = extension;
        }
    }

    public class RelayCommand : ICommand
    {
        private string name;
        private readonly Action<object> _execute;
        private readonly Action<RelayCommand, object> _extansionExecute;
        private readonly Func<object, bool> _canExecute;
        private object content;

        public List<RelayCommandExtension> Extensions { get; set; } = new List<RelayCommandExtension>();

        public object Content
        {
            get => content;
            set => content = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public object[] Parameters;

        public virtual object this[int index]
        {
            get
            {
                foreach (var element in Extensions)
                {
                    if (element.Index == index)
                    {
                        return element.Extension;
                    }
                }

                return null;
            }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null, string name = null, params RelayCommandExtension[] extensions)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;

            Name = name;

            if (extensions != null)
            {
                Extensions.AddRange(extensions);
            }
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
