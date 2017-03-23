using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace web2Excel
{
    class ExcelInterop
    {
        private static Excel.Application app;
        private static Excel.Workbook workBook;
        public static UpdateBoardValue UpdateProgess;
        public static UpdateProjCount updateItemCount;
        public static UpdateProgressInfo updateProgressInfo;

        /// <summary>
        /// 在制定的文件夹创建Excel文件
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <param name="childName">工作表名称（名称数量不超过默认工作表数量）</param>
        public static void CreateExcelFile( string fileName,string[] childName)
        {
            object Nothing = System.Reflection.Missing.Value;
            app = new Excel.Application();
            app.Visible = false;
            workBook = app.Workbooks.Add(Nothing);

            for (int i = 0; i < childName.Length; i++)
            {
                Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Sheets[i];
                worksheet.Name = childName[i];
            }

            worksheet.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
        }

        /// <summary>
        /// 加入一条数据
        /// </summary>
        /// <param name="excelIdx">表单序号</param>
        /// <param name="itemIdx">第几条数据</param>
        /// <param name="itemInfo">数据内容</param>
        public static void InsertItemInfo(int excelIdx, int itemIdx, Content itemInfo)
        {
            int idx = 1;

            Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Sheets[excelIdx];

            worksheet.Cells[itemIdx, idx++] = itemInfo.ProjectName;
            worksheet.Cells[itemIdx, idx++] = itemInfo.Address;
            worksheet.Cells[itemIdx, idx++] = itemInfo.BlockNumber;
            worksheet.Cells[itemIdx, idx++] = itemInfo.LayerCount;
            worksheet.Cells[itemIdx, idx++] = itemInfo.LayerNumber;
            worksheet.Cells[itemIdx, idx++] = itemInfo.RoomNumber;
            worksheet.Cells[itemIdx, idx++] = itemInfo.RoomType;
            worksheet.Cells[itemIdx, idx++] = itemInfo.FunctionName;
            worksheet.Cells[itemIdx, idx++] = itemInfo.AreaBuilding;
            worksheet.Cells[itemIdx, idx++] = itemInfo.AreaRoom;
            worksheet.Cells[itemIdx, idx++] = itemInfo.AreaPublic;
            worksheet.Cells[itemIdx, idx++] = itemInfo.HouseStatus;
            worksheet.Cells[itemIdx, idx++] = itemInfo.IsPrepare;
            worksheet.Cells[itemIdx, idx++] = itemInfo.DealDate;
        }

        /// <summary>
        /// 更新户型数据
        /// </summary>
        /// <param name="excelIdx">表单序号</param>
        /// <param name="houseType">数据内容</param>
        public static void updateTypeInfo(int excelIdx, IDictionary<int, string> houseType)
        {
            Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Sheets[excelIdx];
            foreach (KeyValuePair<int, string> item in houseType)
            {
                worksheet.Cells[item.Key, 7] = item.Value;
            }
        }

        /// <summary>
        /// 返回数据表数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dataList"></param>
        public static void GetNormalList(string fileName, out string[,] dataList)
        {
            app = new Excel.Application();
            workBook = app.Workbooks.Open(fileName);
            Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Sheets[2];
            GetExcelData(false, worksheet, out dataList);
        }

        /// <summary>
        /// 解析Excel表格数据
        /// </summary>
        /// <param name="upFlag"></param>
        /// <param name="worksheet"></param>
        /// <param name="dataList"></param>
        private static void GetExcelData(bool upFlag, Excel.Worksheet worksheet, out string[,] dataList)
        {
            int row = worksheet.UsedRange.Rows.Count;
            int column = worksheet.UsedRange.Columns.Count;
            dataList = new string[row - 1, column];
            for (int i = 2; i <= row; i++)
            {
                if (upFlag) UpdateProgess(false, i);
                for (int j = 1; j <= column; j++)
                {
                    dataList[i - 2, j - 1] = (worksheet.Cells[i, j] as Excel.Range).Text.ToString();
                }
            }
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