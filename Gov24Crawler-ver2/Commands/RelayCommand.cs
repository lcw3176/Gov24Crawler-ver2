using System;
using System.Windows.Input;

namespace Gov24Crawler_ver2.Commands
{
    class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> method;

        public RelayCommand(Action<object> method)
        {
            this.method = method;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            method(parameter);
        }
    }
}
