using gov.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
            string addr;

            try
            {
                app = new Excel.Application();

                wb = app.Workbooks.Open(Config.filePath);
                ws = wb.Worksheets.get_Item("Sheet1") as Excel.Worksheet;


                for (int i = int.Parse(Config.startCol); i <= int.Parse(Config.endCol); i++)
                {
                    addr = ws.Cells[i, int.Parse(Config.addressRow)].value;
                    temp = addr;

                    if (temp.Contains("산"))
                    {
                        temp = temp.Replace("산", "");
                    }

                    if(temp.Contains("-"))
                    {
                        address.Add(new ExcelData()
                        {
                            index = i,
                            fullAddress = addr,
                            bunzi = temp.Split('-')[0],
                            ho = temp.Split('-')[1]
                        });
                    }

                    else
                    {
                        address.Add(new ExcelData()
                        {
                            index = i,
                            fullAddress = addr,
                            bunzi = temp,
                            ho = string.Empty,
                        });
                    }

                }

                wb.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault);
            }

            catch (Exception ex)
            {
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

            catch
            {
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
