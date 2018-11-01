using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        public Form1()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//Skins//Page.ssk";
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
        private void SendMail(string email, string name, string sdate, string edate, string course, string point)
        {
            string mailFrom = MailHelper.MailServerFrom;
            string mailTo = string.Empty;
            string mailSubject = string.Empty;
            string mailBody = string.Empty;
            string mailAttch = string.Empty;
            string mailCode = "sh514229";
            string mailPriority = string.Empty;
            string mailCC = string.Empty;
            string resultMessage = "";
            if (!string.IsNullOrEmpty(email))
            {
                mailTo = email;
                mailSubject = "Change By Innovation 加分通知";
                try
                {
                    string templetpath = System.Environment.CurrentDirectory+ "/MailTemplate/test.html";
                    NameValueCollection myCol = new NameValueCollection();
                    myCol.Add("ename", name);
                    myCol.Add("sdate", sdate);
                    myCol.Add("edate", edate);
                    myCol.Add("course", course);
                    myCol.Add("point", point);
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
            //测试一条数据看看
            SendMail("991011509@qq.com","shenjun","2018-1-1","2018-10-1","系统架构师","100");
        }

        
    }


}
