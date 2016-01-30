using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackendGUI.ViewModels
{
    class MainWinVM : WithNotification
    {
        DelegateCommand configCommand;
        public MainWinVM()
        {
            configCommand = new DelegateCommand(() => { });
        }

        public ICommand ConfigCommand
        {
            get { return configCommand; }
        }

    }
}
