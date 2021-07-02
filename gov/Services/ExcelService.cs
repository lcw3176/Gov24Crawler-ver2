using gov.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace gov.Services
{
    public class ExcelService
    {
        private string savePath = Config.savePath + @"/" + Config.firstAddress + Config.secondAddress + Config.thirdAddress;

        public ExcelService()
        {

        }

        public List<ExcelData> ReadExcel()
        {
            Excel.Application app = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            List<ExcelData> address = new List<ExcelData>();
            string temp;

            try
            {
                app = new Excel.Application();

                wb = app.Workbooks.Open(Config.filePath);
                ws = wb.Worksheets.get_Item("Sheet1") as Excel.Worksheet;


                for (int i = int.Parse(Config.startCol); i <= int.Parse(Config.endCol); i++)
                {
                    temp = ws.Cells[i, int.Parse(Config.addressRow)].value;

                    if(temp.Contains("-"))
                    {
                        address.Add(new ExcelData()
                        {
                            index = i,
                            bunzi = temp.Split('-')[0],
                            ho = temp.Split('-')[1]
                        });
                    }

                    else
                    {
                        address.Add(new ExcelData()
                        {
                            index = i,
                            bunzi = temp,
                            ho = string.Empty,
                        });
                    }

                }

                wb.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault);
            }

            catch (Exception ex)
            {
                Debug.WriteLine("엑셀 쓰기 오류");
            }

            finally
            {
                wb.Close();
                app.Quit();
            }

            return address;
        }

        public Task<bool> SetExcel(string area, string owner, int index)
        {
            Excel.Application app = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;

            try
            {
                app = new Excel.Application();

                wb = app.Workbooks.Open(savePath + ".xlsx");
                Debug.WriteLine("파일 열음");
                ws = wb.Worksheets.get_Item("Sheet1") as Excel.Worksheet;
                Debug.WriteLine("시트 열음");
                ws.Cells[index, int.Parse(Config.ownerRow)] = owner;
                ws.Cells[index, int.Parse(Config.areaRow)] = area;
                Debug.WriteLine("데이터 입력함 열음");
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
