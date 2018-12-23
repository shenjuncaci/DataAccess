using Repository;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utilities;

namespace SendEmailApp
{
    public partial class Form1 : Form
    {
        Repository<MailHelperDTO> MailHelperDTORe = new Repository<MailHelperDTO>();

        private static List<DataTable> dtList = new List<DataTable>();
        public Form1()
        {
            InitializeComponent();
            //this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            //this.skinEngine1.SkinFile = Application.StartupPath + "//Skins//Page.ssk";

            //dateTimePicker1.Format = DateTimePickerFormat.Custom;
            //dateTimePicker1.CustomFormat = "H:mm";
            ////dateTimePicker1.Format = DateTimePickerFormat.Time;
            //dateTimePicker1.ShowUpDown = true;

            this.SenderTimer.Enabled = true;
            //设置定时器间隔
            this.SenderTimer.Interval = 1000 * 60*5;

            this.label2.Text = ConfigurationManager.AppSettings["labelText"];
            this.label2.BackColor = Color.Transparent;
            this.label2.Parent = pictureBox1;
        }
        #region 功能代码，将窗口最小化到右下角，右键关闭才退出
        private void Form1_Resize_1(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)    //最小化到系统托盘
            {
                notifyIcon1.Visible = true;    //显示托盘图标
                this.Hide();    //隐藏窗口
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //注意判断关闭事件Reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;    //取消"关闭窗口"事件
                this.WindowState = FormWindowState.Minimized;    //使关闭时窗口向右下角缩小的效果
                notifyIcon1.Visible = true;
                this.Hide();
                return;
            }

        }

        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            //notifyIcon1.Visible = false;
            //this.Show();
            //WindowState = FormWindowState.Normal;
            //this.Focus();
            if (e.Button == MouseButtons.Left)//判断鼠标的按键
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                }
                else if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (MessageBox.Show("是否需要关闭程序？", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)//出错提示
                {
                    //鼠标查询进程
                    //_MouseStop = false;
                    //if (Mouse_Thread != null)
                    //{
                    //    Mouse_Thread.Abort();
                    //    Mouse_Thread = null;
                    //}
                    //关闭窗口
                    DialogResult = DialogResult.No;
                    Dispose();
                    Close();
                }
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MessageBox.Show("是否需要关闭程序？", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)//出错提示
                {
                    //鼠标查询进程
                    //_MouseStop = false;
                    //if (Mouse_Thread != null)
                    //{
                    //    Mouse_Thread.Abort();
                    //    Mouse_Thread = null;
                    //}
                    //关闭窗口
                    DialogResult = DialogResult.No;
                    Dispose();
                    Close();
                }
            }
        }
        #endregion

        //调用html模板发送邮件通知
        private void SendMail(string email, string name, string date, string table1, string table2,string level,string qianzhui)
        {
            string mailFrom = MailHelper.MailServerFrom;
            string mailTo = string.Empty;
            string mailSubject = string.Empty;
            string mailBody = string.Empty;
            string mailAttch = string.Empty;
            string mailCode = MailHelper.MailPassword;
            string mailPriority = string.Empty;
            string mailCC = string.Empty;
            string resultMessage = "";
            if (!string.IsNullOrEmpty(email))
            {
                mailTo = email;
                mailSubject = "GP12遏制产品待处理信息通知";
                try
                {
                    string templetpath = System.Environment.CurrentDirectory+ "/MailTemplate/test.html";
                    NameValueCollection myCol = new NameValueCollection();
                    myCol.Add("ename", name);
                    myCol.Add("date", date);
                    myCol.Add("table1", table1);
                    myCol.Add("table2", table2);
                    myCol.Add("level", level);
                    myCol.Add("qianzhui", qianzhui);
                    //ConfigurationManager.ConnectionStrings["S1"].ToString()
                    //myCol.Add("link", System.Configuration.ConfigurationManager.AppSettings["rootUrl"]);
                    mailBody = TemplateHelper.BulidByFile(templetpath, myCol);
                    MailHelper.SendNetMail(mailFrom, mailTo, mailSubject, mailBody, mailAttch, mailCode, mailPriority, mailCC, out resultMessage);
                    TXTLogHelper.LogBackup(resultMessage);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BindData();
            //测试一条数据看看
            //SendMail("991011509@qq.com","shenjun","2018-1-1",huizongshuju(),mingxishuju());
            string tt1 = DateTime.Now.ToString("H:mm");
            //string tt2 = dateTimePicker1.Text;
            SendMailTick();
        }

        private void SenderTimer_Tick(object sender, EventArgs e)
        {
            BindData();
            SendMailTick();
            //if (DateTime.Now.ToString("H:mm")== dateTimePicker1.Text)
            //{
                
            //}
        }

        public bool IsSame(DataTable dt1, DataTable dt2)
        {
            DataTable dt3 = new DataTable();
            dt3.Merge(dt1);
            dt3.AcceptChanges();

            dt3.Merge(dt2);
            DataTable dt4 = dt3.GetChanges();
            return dt4 == null || dt4.Rows.Count == 0;
        }

        //根据策略处理邮件
        private void SendMailTick()
        {
            //第一步，找到预警消息的条件
            string sql = @"---预警人员和条件
with temp as
(
SELECT   ManufacturPlant,sCodeID,scodename,sParentCodeID,sParentCodeName,IsSort,sCodeNum
FROM      Sta_CodeCenter
WHERE   (sModule IN ('63', '62')) and winform='DefectWarnSet'
--ORDER BY sModule
)
SELECT   a.ManufacturPlant,a.sCodeID,a.scodename,a.sParentCodeID as gx,a.sParentCodeName,a.IsSort as bjdj,a.sCodeNum as ycsj,b.sfree1,c.sCodeID as qxdm,c.sCodeNum as qxsl
FROM      Sta_CodeCenter a 
left join Ba_Employee b on a.sCodeID=b.EmployeeID
left join temp c on c.ManufacturPlant=a.ManufacturPlant and c.sParentCodeID=a.sParentCodeID
WHERE   (sModule IN ('63', '62')) and winform='DefectWarnManSet' and b.sfree1 is not null
ORDER BY sModule
";
//            string sqlLevel = @" SELECT   a.ManufacturPlant,a.sCodeID,a.scodename,a.sParentCodeID,a.sParentCodeName,a.IsSort,a.sCodeNum,b.sfree1
//FROM Sta_CodeCenter a left join Ba_Employee b on a.sCodeID = b.EmployeeID
//WHERE(sModule IN('63', '62')) and winform = 'DefectWarnManSet'
//ORDER BY sModule  ";
            DataTable dt1 = MailHelperDTORe.FindTableBySql(sql);
            if (dt1.Rows.Count > 0)
            {
                //第二部，根据条件找到发送的内容
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    bool issend= false;
                    string ManufacturPlant = dt1.Rows[i]["ManufacturPlant"].ToString();
                    string Process= dt1.Rows[i]["gx"].ToString();
                    string qxdm = dt1.Rows[i]["qxdm"].ToString();
                    string qxsl = dt1.Rows[i]["qxsl"].ToString();
                    int yujingshijian = getshijian(dt1.Rows[i]["ycsj"].ToString());
                    string condition1 = string.Format(" and ManufacturePlant='{0}' and Process='{1}' and CreateRq<='{2}' and Cause='{3}' ", ManufacturPlant, Process,DateTime.Now.AddHours(yujingshijian), qxdm);
                    string condition2 = string.Format("having SUM(Sl)>={0}", qxsl);

                    string qianzhui = ManufacturPlant+"工厂,"+ Process+"工序"+ qxdm+"缺陷类型";

                    DataTable dttemp = new DataTable();
                    string huizong = huizongshuju(condition1, condition2,out dttemp);
                    
                    if(dtList.Count == dt1.Rows.Count)
                    {
                        if(!IsSame(dtList[i], dttemp))
                        {
                            dtList[i] = dttemp;
                            issend = true;
                        }

                    }
                    else
                    {
                        issend = true;
                        dtList.Add(dttemp);
                    }

                    //汇总不为空时，获取明细
                    if(huizong!="")
                    {
                        string mingxi = mingxishuju(condition1);
                        if(issend)
                        {
                            SendMail(dt1.Rows[i]["sfree1"].ToString(), dt1.Rows[i]["scodename"].ToString(), DateTime.Now.ToString("yyyy-MM-dd"), huizong, mingxi, dt1.Rows[i]["bjdj"].ToString(), qianzhui);

                        }
                        
                    }

                    issend = false;

                    
                }
            }
        }

        private void BindData()
        {
            string sql = @" --汇总
SELECT   OrganID as 组织, ProjectName as 产品编码, ProjectDesc as 产品描述, ManufacturePlant as 工厂, InputRq as 日期,Process as 工序,Cause as 缺陷代码, CauseDesc as 缺陷名称, sum(sl) as 待处理数 
                --SUM((CASE WHEN TaskSubType IN ('IM5001') 
                --THEN sl ELSE 0 END)) - SUM((CASE WHEN TaskSubType IN ('UT5001') THEN sl ELSE 0 END)) AS 投料数, 
                --SUM((CASE WHEN TaskSubType IN ('UM5001') THEN sl ELSE 0 END)) 
                --+ SUM((CASE WHEN TaskSubType IN ('DCLJC5001') AND DecisionType IN ('放行') THEN sl ELSE 0 END)) AS  完工数, 
                --SUM((CASE WHEN TaskSubType IN ('DCLJC5001') THEN sl ELSE 0 END)) 
                --- SUM((CASE WHEN TaskSubType IN ('DCLJC5001') and IsShow!=1 THEN sl ELSE 0 END)) AS  待处理数, 
                --SUM((CASE WHEN TaskSubType IN ('DCLJC5001') AND DecisionType IN ('退片', '退库') THEN sl ELSE 0 END)) AS  其他退回数, 
                --SUM((CASE WHEN TaskSubType IN ('DCLJC5001') AND DecisionType IN ('报废') THEN sl ELSE 0 END)) AS  报废数
FROM      dbo.Mes_TaskWork_D 
where TaskSubType='DCL5001' and DecisionType in ('新建','处理中') 
GROUP BY OrganID, ManufacturePlant, Process, ProjectName, ProjectDesc, InputRq,Cause,CauseDesc,CauseDesc,Cause
";
            DataTable dt = MailHelperDTORe.FindTableBySql(sql);
            dataGridView1.DataSource = dt;

        }

        public int getshijian(string shijian)
        {
            try
            {
                return int.Parse(shijian)*-1;
            }
            catch
            {
                return 0;
            }
        }

        private string huizongshuju(string condotion1,string condition2,out DataTable dt2)
        {
            string result = "";
            string sql = @" --汇总
SELECT    OrganID as 组织, ProjectName as 产品编码, ProjectDesc as 产品描述, ManufacturePlant as 工厂, InputRq as 日期,Process as 工序,Cause as 缺陷代码, CauseDesc as 缺陷名称, sum(sl) as 待处理数 
FROM      dbo.Mes_TaskWork_D 
where TaskSubType='DCL5001' and DecisionType in ('新建','处理中') {0}
GROUP BY OrganID, ManufacturePlant, Process, ProjectName, ProjectDesc, InputRq,CauseDesc,Cause
{1}";
            sql = string.Format(sql, condotion1, condition2);
            DataTable dt = MailHelperDTORe.FindTableBySql(sql);
            dt2 = dt;
            //dataGridView1.DataSource = dt;
            if (dt.Rows.Count>0)
            {
                
                result += "<table>";
                result += "<tr>";
                foreach (System.Data.DataColumn k in dt.Columns)
                {
                    //columnName = k.ColumnName;
                    //columnType = k.DataType.ToString();
                    result += "<th>"+k.ColumnName+"</th>";
                }
                result += "</tr>";
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    result += "<tr>";
                    for(int j=0;j<dt.Columns.Count;j++)
                    {
                        result += "<td>"+dt.Rows[i][j].ToString()+"</td>";
                    }
                    result += "</tr>";
                }
                result += "</table>";
            }
            return result;
        }

        private string mingxishuju(string condition1)
        {
            string result = "";
            string sql = @"SELECT   OrganID as 组织, ProjectName as 产品编码, ProjectDesc as 产品描述, ManufacturePlant as 工厂, Process as 工序,ClassGroup as 班组, ClassGrade as 班次, 
                Machine as 机台, Cause as 缺陷代码, CauseDesc as 缺陷名称, InputRq as 日期, 
SUM(Sl) AS  数量
FROM      dbo.Mes_TaskWork_D
WHERE   TaskSubType='DCL5001' and DecisionType in ('新建','退片') {0}
GROUP BY OrganID, ManufacturePlant, Process, ProjectName, ProjectDesc, Cause, CauseDesc, InputRq,ClassGroup,ClassGrade,Machine";
            sql = string.Format(sql,condition1);
            DataTable dt = MailHelperDTORe.FindTableBySql(sql);
            if (dt.Rows.Count > 0)
            {
                result += "<table>";
                result += "<tr>";
                foreach (System.Data.DataColumn k in dt.Columns)
                {
                    //columnName = k.ColumnName;
                    //columnType = k.DataType.ToString();
                    result += "<th>" + k.ColumnName + "</th>";
                }
                result += "</tr>";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result += "<tr>";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        result += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                    }
                    result += "</tr>";
                }
                result += "</table>";
            }
            return result;
        }
    }


}
