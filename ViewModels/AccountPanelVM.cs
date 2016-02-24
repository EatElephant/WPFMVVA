using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendGUI.Models;
using System.Windows.Input;

namespace BackendGUI.ViewModels
{
    class AccountPanelVM : WithNotification
    {
        private int _selindex;
        private AccountManager _accManager;
        private DelegateCommand _newCommand;
        private DelegateCommand _delCommand;


        public AccountPanelVM()
        {
            _accManager = new AccountManager();

            _newCommand = new DelegateCommand(NewAccount);
            _delCommand = new DelegateCommand(DelAccount);
        }

        public void NewAccount()
        {
            _accManager.NewAccount();
        }

        public void DelAccount()
        {
            _accManager.DeleteAccount(SelIndex);
        }
    

        public AccountManager AccManager
        {
            get { return _accManager; }
            set {_accManager = value; }
        }

        public int SelIndex
        {
            get { return _selindex; }
            set { _selindex = value; }
        }

        public ICommand NewAccountCommand
        {
            get { return _newCommand; }
        }

        public ICommand DelAccountCommand
        {
            get { return _delCommand; }
        }
    
    }
}
