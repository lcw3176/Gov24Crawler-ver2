using gov.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace gov.Services
{
    public class ExcelService
    {
        private string savePath = Config.savePath + @"/" + Config.firstAddress + " " + Config.secondAddress + " " + Config.thirdAddress;

        public List<ExcelData> ReadExcel()
        {
            Excel.Application app = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            List<ExcelData> address = new List<ExcelData>();
            string temp;
            bool san = false;
            StringBuilder addr = new StringBuilder();

            try
            {
                app = new Excel.Application();

                wb = app.Workbooks.Open(Config.filePath);
                ws = wb.Worksheets.get_Item("Sheet1") as Excel.Worksheet;


                for (int i = int.Parse(Config.startCol); i <= int.Parse(Config.endCol); i++)
                {
                    temp = Convert.ToString(ws.Cells[i, int.Parse(Config.addressRow)].value);

                    if (temp.Contains("산"))
                    {
                        temp = temp.Replace("산", "");
                        addr.Append("산");
                        san = true;
                    }

                    if(temp.Contains("-"))
                    {
                        address.Add(new ExcelData()
                        {
                            index = i,
                            fullAddress = addr.Append(temp.Trim()).ToString(),
                            bunzi = temp.Split('-')[0].Trim(),
                            ho = temp.Split('-')[1].Trim(),
                            isSan = san,
                        });
                    }

                    else
                    {
                        address.Add(new ExcelData()
                        {
                            index = i,
                            fullAddress = addr.Append(temp.Trim()).ToString(),
                            bunzi = temp.Trim(),
                            ho = string.Empty,
                            isSan = san,
                        });

                    }

                    addr.Clear();
                    san = false;
                }

                wb.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault);
            }

            catch(Exception e)
            {
                MessageBox.Show(e.Message, "메롱");
                Debug.WriteLine(e);
                Debug.WriteLine("엑셀 읽기 오류");
            }

            finally
            {
                wb.Close();
                app.Quit();
            }

            return address;
        }

        public Task<bool> SaveExcel(string area, string owner, int index)
        {
            Excel.Application app = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;

            try
            {
                app = new Excel.Application();

                wb = app.Workbooks.Open(savePath + ".xlsx");
                ws = wb.Worksheets.get_Item("Sheet1") as Excel.Worksheet;
                ws.Cells[index, int.Parse(Config.ownerRow)] = owner;
                ws.Cells[index, int.Parse(Config.areaRow)] = area;
                return Task.FromResult(true);
            }

            catch(Exception e)
            {
                MessageBox.Show(e.Message, "메롱");
                Debug.WriteLine("엑셀 쓰기 오류");
                return Task.FromResult(false);
            }

            finally
            {
                wb.Save();
                wb.Close();
                app.Quit();
            }
        }

    }

}
