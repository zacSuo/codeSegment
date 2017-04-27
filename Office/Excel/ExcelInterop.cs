using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace SimuProteus
{
    public class ExcelInterop
    {
        private static int itemIdx;
        private static Excel.Application app;
        private static Excel.Workbook workBook;

        /// <summary>
        /// 在制定的文件夹创建Excel文件
        /// </summary>
        static ExcelInterop()
        {
            app = new Excel.Application();
            app.Visible = false;
            Excel.Worksheet worksheet = null;
            string fileName = System.Environment.CurrentDirectory + '\\' + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
            if (System.IO.File.Exists(fileName))
            {
                workBook = app.Workbooks.Open(fileName);
                worksheet = (Excel.Worksheet)workBook.Sheets[1];
                itemIdx = worksheet.UsedRange.Rows.Count + 1;
                return;
            }

            object Nothing = System.Reflection.Missing.Value;
            string childName = "Data Info";
            workBook = app.Workbooks.Add(Nothing);
            worksheet = (Excel.Worksheet)workBook.Sheets[1];
            worksheet.Name = childName;
            itemIdx = 2;

            string[] headItems = { "时间", "ID编号", "ID号", "X坐标", "Y坐标" };
            int idx = 1;
            foreach (string strItem in headItems)
            {
                worksheet.Cells[1, idx++] = strItem;
            }

            worksheet.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
        }

        /// <summary>
        /// 加入一条数据
        /// </summary>
        /// <param name="itemInfo">数据内容</param>
        public static void InsertItemInfo(CommunicateInfo itemInfo)
        {
            int idx = 1;

            Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Sheets[1];

            worksheet.Cells[itemIdx, idx++] = DateTime.Now.ToLongTimeString();
            worksheet.Cells[itemIdx, idx++] = itemInfo.ID;
            worksheet.Cells[itemIdx, idx++] = itemInfo.Number;
            worksheet.Cells[itemIdx, idx++] = itemInfo.X;
            worksheet.Cells[itemIdx, idx++] = itemInfo.Y;

            itemIdx++;
        }

        /// <summary>
        /// 保存当前信息
        /// </summary>
        public static void SaveInfo()
        {
            workBook.Save();
        }

        /// <summary>
        /// 关闭并退出
        /// </summary>
        public static void FinishExcel()
        {
            SaveInfo();
            workBook.Close(true);
            app.Quit();
        }

    }
}