using System;
using System.Data;
using System.Drawing;
using System.Data.SQLite;
using System.Collections.Generic;


namespace SimuProteus
{
    public class DBUtility
    {
        private const string STR_CONNECTION = "Data Source=prot.s;Version=3;";
        private Coder code = new Coder();

        public DBUtility()
        {
        }

        public DBUtility(bool dbEncrypt)
        {
            SQLiteHelper.SetPassWordFlag = dbEncrypt;
        }

        /// <summary>
        /// 新建表结构
        /// </summary>
        /// <returns></returns>
        public void InitialTable()
        {
            List<string> tableList = this.CreateTableStruct();
            string strSql = string.Empty;
            foreach (string strItem in tableList)
            {
                strSql += strItem;
            }

            SQLiteHelper.CreateDatabase("prot.s");
            SQLiteHelper.ExecuteNonQuery(STR_CONNECTION, strSql, null);
            this.CreateModelData();
        }

        private List<string> CreateTableStruct()
        {
            List<string> tableList = new List<string>();
            //新建的项目
            tableList.Add(@"create table projects(
                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                               name nvarchar(20),
                               x0 int,
                               y0 int,
                               createtime datetime,
                               updatetime datetime,
                               chips nvarchar(100),
                               desc nvarchar(100));");

            //网格节点
            tableList.Add(@"create table netPoints(
                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                               projIdx int,
                               x int,
                               y int,
                               status int);");
            return tableList;
        }

        private void CreateModelData()
        {
            Color colorFoot = Color.FromArgb(Convert.ToInt32(Ini.GetItemValue("colorInfo", "colorFoot")));
            ElementInfo info = new ElementInfo();
            //箭头
            info.Name = enumComponent.NONE.ToString ();
            info.FootType = enumComponentType.NormalComponent;
            info.Size = new Size(0, 0);
            info.BackColor = Color.Gray;
            info.LineFoots = new List<LineFoot>();
            info.BackImage = "img\\arrow.png";
            AddNewBaseComponent(info);
        }

        public bool MoveComponentShow(ElementInfo info)
        {
            string strSql = string.Format("update componentView set locX={0},locY={1} where id={2}",info.Location.X,info.Location.Y,info.ID);
            return SQLiteHelper.ExecuteNonQuery(STR_CONNECTION,strSql,null) > 0;
        }

        public bool UpdateComponent(ElementInfo info)
        {
            string strSql = "update componentView set locX=@locX,locY=@locY where id=@id";
            List<SQLiteParameter> paraList = new List<SQLiteParameter> ();
            paraList.Add(SQLiteHelper.CreateParameter("@locX",DbType.Int32,info.Location.X));
            paraList.Add(SQLiteHelper.CreateParameter("@locY",DbType.Int32,info.Location.Y));
            paraList.Add(SQLiteHelper.CreateParameter("@id",DbType.Int32,info.ID));

            SQLiteCommand command = SQLiteHelper.CreateCommand(STR_CONNECTION,strSql,paraList.ToArray());
            return SQLiteHelper.ExecuteNonQuery(command) > 0;
        }
    }
}