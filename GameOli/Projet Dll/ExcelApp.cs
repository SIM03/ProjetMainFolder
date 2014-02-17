using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace GAME
{
    public static class ExcelApp
    {
        static Excel.Application ExcelApplication = new Excel.Application();
        static Excel.Workbook Workbook = ExcelApplication.Workbooks.Open("D:\\Mes documents\\ProjetMainFolder\\GameOli\\GameOli\\GameOliContent\\Database\\Database.xls");

        public static string GetCell(int col, int row, int sheet)
        {
            string result;
            try
            {
                result = Workbook.Worksheets.get_Item(sheet).Cells.Columns[col].Rows[row].Value.ToString();
            }
            catch
            {
                result = Workbook.Worksheets.get_Item(sheet).Cells.Columns[col].Rows[row].Value;
            }
            return result;
        }

        public static void SetCell<T>(int col, int row, int sheet, T value)
        {
            Workbook.Worksheets.get_Item(sheet).Cells.Columns[col].Rows[row].Value = value.ToString();
        }

        public static void Quit()
        {
            Workbook.Close();
            ExcelApplication = null;
        }

        public static void SaveAndQuit()
        {
            Workbook.Save();
            Workbook.Close();
            ExcelApplication = null;
        }

        public static void Save()
        {
            Workbook.Save();
        }
    }
}
