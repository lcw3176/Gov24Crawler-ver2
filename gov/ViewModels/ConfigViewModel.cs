using gov.Commands;
using gov.Models;
using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace gov.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {

        public string FilePath
        {
            get { return Config.filePath; }
            set
            {
                Config.filePath = value;
                OnPropertyUpdate("FilePath");
            }
        }

        public string SavePath
        {
            get { return Config.savePath; }
            set
            {
                Config.savePath = value;
                OnPropertyUpdate("SavePath");
            }
        }

   
        public string FisrtAddress
        {
            get { return Config.firstAddress; }
            set
            {
                Config.firstAddress = value.Trim();
                OnPropertyUpdate("FisrtAddress");
            }
        }

        
        public string SecondAddress
        {
            get { return Config.secondAddress; }
            set
            {
                Config.secondAddress = value.Trim();
                OnPropertyUpdate("SecondAddress");
            }
        }

        
        public string ThirdAddress
        {
            get { return Config.thirdAddress; }
            set
            {
                Config.thirdAddress = value.Trim();
                OnPropertyUpdate("ThirdAddress ");
            }
        }

        public string StartCol
        {
            get { return Config.startCol; }
            set
            {
                Config.startCol = value.Trim();
                OnPropertyUpdate("StartCol");
            }
        }


        public string EndCol
        {
            get { return Config.endCol; }
            set
            {
                Config.endCol = value.Trim();
                OnPropertyUpdate("EndCol");
            }
        }

        public string AddressRow
        {
            get { return Config.addressRow; }
            set
            {
                Config.addressRow = value.Trim();
                OnPropertyUpdate("AddressRow");
            }
        }

        public string MokRow
        {
            get { return Config.mokRow; }
            set
            {
                Config.mokRow = value;
                OnPropertyUpdate("MokRow");
            }
        }

        public string AreaRow
        {
            get { return Config.areaRow; }
            set
            {
                Config.areaRow = value.Trim();
                OnPropertyUpdate("AreaRow");
            }
        }

        public string OwnerRow
        {
            get { return Config.ownerRow; }
            set
            {
                Config.ownerRow = value.Trim();
                OnPropertyUpdate("OwnerRow");
            }
        }

        public ICommand GetExcelPathCommand { get; set; }
        public ICommand SetSavePathCommand { get; set; }

        public ConfigViewModel()
        {
            GetExcelPathCommand = new RelayCommand(GetExcelPathExecuteMethod);
            SetSavePathCommand = new RelayCommand(SetSavePathExecuteMethod);
        }

        private void SetSavePathExecuteMethod(object obj)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            
            if (folder.ShowDialog() == DialogResult.OK)
            {
                SavePath = folder.SelectedPath;
            }
        }

        private void GetExcelPathExecuteMethod(object obj)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFile.ShowDialog();

            if (result == true)
            {
                FilePath = openFile.FileName;
            }
        }
    }
}
