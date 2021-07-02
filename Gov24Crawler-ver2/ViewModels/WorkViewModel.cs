using Gov24Crawler_ver2.Commands;
using Gov24Crawler_ver2.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gov24Crawler_ver2.ViewModels
{
    public class WorkViewModel : BaseViewModel
    {
        private static bool isRun = false;

        public string UserId
        {
            get { return User.userId; }
            set
            {
                User.userId = value;
                OnPropertyUpdate("UserId");
            }
        }


        private int progressValue = 0;
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                progressValue = value;
                OnPropertyUpdate("ProgressValue");
            }
        }

        public ICommand StartCommand { get; set; }
        public static ObservableCollection<Result> Results { get; set; } = new ObservableCollection<Result>();

        public WorkViewModel()
        {
            StartCommand = new RelayCommand(StartExecuteMethod);
        }

        private void StartExecuteMethod(object obj)
        {
            if (!isRun)
            {
                string password = (obj as PasswordBox).Password;
            }

            else
            {
                MessageBox.Show("현재 실행중입니다.");
            }
            
        }
    }
}
