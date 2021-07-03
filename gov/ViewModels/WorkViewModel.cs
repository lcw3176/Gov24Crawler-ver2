using gov.Commands;
using gov.Models;
using gov.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace gov.ViewModels
{
    public class WorkViewModel : BaseViewModel
    {

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

        private int completeCount = 0;
        public int CompleteCount
        {
            get { return completeCount; }
            set
            {
                completeCount = value;
                OnPropertyUpdate("CompleteCount");
            }
        }

        private int endCount = 0;
        public int EndCount
        {
            get { return endCount; }
            set
            {
                endCount = value;
                OnPropertyUpdate("EndCount");
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
            Task.Run(async () =>
            {
                EndCount = int.Parse(Config.endCol) - int.Parse(Config.startCol) + 1;
                CrawlerService crawlerService = new CrawlerService();

                string password = (obj as PasswordBox).Password;
                string owner;
                string area;
                bool captureResult = false;
                bool valueResult = false;

                if (await crawlerService.TryLogin(password))
                {
                    ExcelService excel = new ExcelService();
                    List<ExcelData> datas = excel.ReadExcel();
                    Stopwatch sw = new Stopwatch();

                    foreach (ExcelData i in datas)
                    {
                        sw.Start();

                        if (!await crawlerService.TryGetDocument(i.bunzi, i.ho) || !await crawlerService.CheckValidation(i.bunzi, i.ho))
                        {
                            continue;
                        }

                        captureResult = await crawlerService.CaptureImage(i.bunzi, i.ho);

                        area = await crawlerService.TryGetArea();
                        owner = await crawlerService.TryGetOwner();

                        if (!string.IsNullOrEmpty(area) && !string.IsNullOrEmpty(owner))
                        {
                            valueResult = await excel.SaveExcel(area, owner, i.index);
                        }

                        else
                        {
                            valueResult = false;
                        }

                        sw.Stop();

                        DispatcherService.Invoke(() =>
                        {
                            Results.Add(new Result()
                            {
                                index = i.index,
                                isPictureSuccess = captureResult,
                                isValueSuccess = valueResult,
                                duration = sw.Elapsed.Seconds.ToString() + "s"
                            });

                        });
                        sw.Reset();
                        await crawlerService.CloseTab();
                        CompleteCount++;
                    }
                }

            });
           
        }
    }

    public static class DispatcherService
    {
        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current != null ? Application.Current.Dispatcher : null;
            if (dispatchObject == null || dispatchObject.CheckAccess())
                action();
            else
                dispatchObject.Invoke(action);
        }
    }

}
