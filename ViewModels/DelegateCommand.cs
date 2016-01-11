using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackendGUI.ViewModels
{
    class DelegateCommand : ICommand
    {
        private Action _action;
        private Func<bool> _validation;

        public DelegateCommand(Action action, Func<bool> validation = null)
        {
            _action = action;
            _validation = validation;
        }

        public bool CanExecute(object parameter)
        {
            if (_validation == null)
                return true;
            else
                return _validation();
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
