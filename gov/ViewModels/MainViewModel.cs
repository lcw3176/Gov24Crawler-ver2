using gov.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace gov.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand changeViewCommand { get; set; 
        }
        private BaseViewModel bindingViewModel;
        public BaseViewModel BindingViewModel 
        {
            get { return bindingViewModel; }
            set
            {
                bindingViewModel = value;
                OnPropertyUpdate("BindingViewModel");
            }
        }
        
        public MainViewModel()
        {
            changeViewCommand = new RelayCommand(ChangeViewExecuteMethod);
            BindingViewModel = new ConfigViewModel();
        }

        private void ChangeViewExecuteMethod(object obj)
        {
            if(obj.ToString() == "config")
            {
                BindingViewModel = new ConfigViewModel();
            }

            if(obj.ToString() == "work")
            {
                BindingViewModel = new WorkViewModel();
            }
        }
    }
}
