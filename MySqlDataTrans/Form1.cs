using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataAccess;
using Repository;
using Utilities;

namespace MySqlDataTrans
{
    public partial class Form1 : Form
    {
        private static string MaxRecord="";

        Repository<WeightCollect> WeightCollectRe = new Repository<WeightCollect>();


        public Form1()
        {
            InitializeComponent();
            ////皮肤美化
            //this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            //this.skinEngine1.SkinFile = Application.StartupPath + "//DeepCyan.ssk";


            string sql = " select max(UPDATE_TIME) from t_statistic ";
            DataTable dt = WeightCollectRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                MaxRecord = dt.Rows[0][0].ToString();
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = " select * from t_statistic where 1=1 and UPDATE_TIME is not null  ";
                if(MaxRecord!="")
                {
                    sql = sql + " and UPDATE_TIME>'{0}' ";
                }
                sql = string.Format(sql, MaxRecord);
                DataTable dt = MySqlHelper.Query(sql).Tables[0];
                SqlBulkCopyInsert(ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString, "t_statistic", dt, out string message);
                this.label1.Text = DateTime.Now.ToString() + message;
                TXTLogHelper.LogBackup(DateTime.Now.ToString() +"手动同步"+ message);

            }
            catch(Exception ex)
            {
                TXTLogHelper.LogBackup(DateTime.Now.ToString() + "手动同步出错" + ex.Message);
            }
        }

        #region 使用SqlBulkCopy将DataTable中的数据批量插入数据库中  
        /// <summary>  
        /// 注意：DataTable中的列需要与数据库表中的列完全一致。
        /// 已自测可用。
        /// </summary>  
        /// <param name="conStr">数据库连接串</param>
        /// <param name="strTableName">数据库中对应的表名</param>  
        /// <param name="dtData">数据集</param>  
        public void SqlBulkCopyInsert(string conStr, string strTableName, DataTable dtData,out string message)
        {
            try
            {
                using (SqlBulkCopy sqlRevdBulkCopy = new SqlBulkCopy(conStr))//引用SqlBulkCopy  
                {
                    sqlRevdBulkCopy.DestinationTableName = strTableName;//数据库中对应的表名  
                    sqlRevdBulkCopy.NotifyAfter = dtData.Rows.Count;//有几行数据  
                    sqlRevdBulkCopy.WriteToServer(dtData);//数据导入数据库  
                    sqlRevdBulkCopy.Close();//关闭连接  
                    message = "成功插入"+dtData.Rows.Count+"行数据";
                    
                }
                //删除重复ID并且日期在前面的记录
                StringBuilder strsql = new StringBuilder();
                strsql.AppendFormat("delete from t_statistic where UPDATE_TIME in (select min(UPDATE_TIME) from t_statistic group by ID having count(ID) > 1)");
                WeightCollectRe.ExecuteBySql(strsql);
                //重新获取最大更新时间
                string sql = " select max(UPDATE_TIME) from t_statistic ";
                DataTable dt = WeightCollectRe.FindTableBySql(sql);
                if (dt.Rows.Count > 0)
                {
                    MaxRecord = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string sql = " select * from t_statistic where 1=1 and UPDATE_TIME is not null   ";
                if (MaxRecord != "")
                {
                    sql = sql + " and UPDATE_TIME>'{0}' ";
                }
                sql = string.Format(sql, MaxRecord);
                DataTable dt = MySqlHelper.Query(sql).Tables[0];
                SqlBulkCopyInsert(ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString, "t_statistic", dt, out string message);
                this.label1.Text = DateTime.Now.ToString()+message;
                TXTLogHelper.LogBackup(DateTime.Now.ToString() + message);
            }
            catch(Exception ex)
            {
                TXTLogHelper.LogBackup(DateTime.Now.ToString() + ex.Message);
            }
        }
    }
}
