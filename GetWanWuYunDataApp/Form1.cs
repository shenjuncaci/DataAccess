using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace GetWanWuYunDataApp
{
    public partial class Form1 : Form
    {
        private static DataTable dt = new DataTable();
        Repository<WanWuYunData> WanWuYunData = new Repository<WanWuYunData>();
        Repository<WanWuYunDevice> WanWuYunDevice = new Repository<WanWuYunDevice>();
        private static DateTime[] WanWuYunDataList;

        public Form1()
        {
            InitializeComponent();
            //string temp = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;
            BindData();
            dataGridView1.Columns[0].Visible = false;
            
            
        }


        //每隔30秒采集一次数据
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime LastDate = new DateTime();
                //便利datatable中的所有行
                if (dt.Rows.Count > 0)
                {
                    //for(int j=0;j<dt.Rows.Count;j++)
                    //{

                    //}
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        
                        string temp = HttpRequest.SendGet(dt.Rows[i][3].ToString() + "?count=1").Replace("\"data\":", "").Replace(",\"success\":\"true\"", "");
                        if (temp.Length > 10)
                        {
                            temp = temp.Substring(1, temp.Length - 2);
                            List<WanWuYunData> entityList = JsonHelper.JonsToList<WanWuYunData>(temp);
                            foreach (WanWuYunData entity in entityList)
                            {
                                if (WanWuYunDataList[i] == entity.TIME)
                                {
                                    this.richTextBox1.Text += DateTime.Now.ToString() + " 设备：" + dt.Rows[i][1].ToString() + "数据与上次相同，请查看设备状态" + "\n";
                                    TXTLogHelper.LogBackup(DateTime.Now.ToString() + " 设备：" + dt.Rows[i][1].ToString() + "数据与上次相同，请查看设备状态" + "\r\n");
                                    break;
                                }
                                entity.ID = CommonHelper.GetGuid;
                                WanWuYunData.Insert(entity);
                                this.richTextBox1.Text += DateTime.Now.ToString() + " 设备：" + dt.Rows[i][1].ToString() + "数据已更新" + "\n";
                                TXTLogHelper.LogBackup(DateTime.Now.ToString() + " 设备：" + dt.Rows[i][1].ToString() + "数据已更新" + "\r\n");
                                WanWuYunDataList[i] = Convert.ToDateTime(entity.TIME);
                            }
                        }
                        else
                        {
                            this.richTextBox1.Text += DateTime.Now.ToString() + " 设备：" + dt.Rows[i][1].ToString() + "数据未获取到" + "\n";
                            TXTLogHelper.LogBackup(DateTime.Now.ToString() + " 设备：" + dt.Rows[i][1].ToString() + "数据未获取到" + "\r\n");
                        }
                    }
                }
                //避免richTextBox1数据过大，占用内存过大，定期清除
                if(this.richTextBox1.Text.Length>500)
                {
                    this.richTextBox1.Text = "";
                }
            }
            catch(Exception ex)
            {
                TXTLogHelper.LogBackup(DateTime.Now.ToString() + "发生异常：" + ex.Message + "\r\n");
            }
            //string testData = "[{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"56\",\"HCHO\":\"0.053\",\"LIGHT\":\"0\",\"PM25\":\"61\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:42:40\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"57\",\"HCHO\":\"0.054\",\"LIGHT\":\"0\",\"PM25\":\"53\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:42:10\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"56\",\"HCHO\":\"0.054\",\"LIGHT\":\"0\",\"PM25\":\"50\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:41:40\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"56\",\"HCHO\":\"0.054\",\"LIGHT\":\"0\",\"PM25\":\"54\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:41:10\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"55\",\"HCHO\":\"0.051\",\"LIGHT\":\"0\",\"PM25\":\"53\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:40:40\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"55\",\"HCHO\":\"0.053\",\"LIGHT\":\"0\",\"PM25\":\"53\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:40:10\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"55\",\"HCHO\":\"0.053\",\"LIGHT\":\"0\",\"PM25\":\"52\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:39:39\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"54\",\"HCHO\":\"0.046\",\"LIGHT\":\"0\",\"PM25\":\"57\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:39:09\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"53\",\"HCHO\":\"0.042\",\"LIGHT\":\"0\",\"PM25\":\"55\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:38:39\",\"VERSION\":\"V4.0\"},{\"DEV_ID\":\"C8:93:46:72:E7:59\",\"G\":\"0.0\",\"GX\":\"0.0\",\"GY\":\"0.0\",\"GZ\":\"0.0\",\"H\":\"53\",\"HCHO\":\"0.044\",\"LIGHT\":\"0\",\"PM25\":\"52\",\"T\":\"28\",\"TIME\":\"2018-09-07 17:38:09\",\"VERSION\":\"V4.0\"}]";
            //List<WanWuYunData> entityListtemp= JsonHelper.JonsToList<WanWuYunData>(testData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textDeviceName.Text is null ||textDeviceName.Text=="")
            {
                MessageBox.Show("设备名称不能为空");
                return;
            }

            if (txtDeviceID.Text is null || txtDeviceID.Text == "")
            {
                MessageBox.Show("设备ID不能为空");
                return;
            }
            if (txtDeviceURL.Text is null || txtDeviceURL.Text == "")
            {
                MessageBox.Show("设备URL不能为空");
                return;
            }

            WanWuYunDevice entity = new WanWuYunDevice();
            entity.ID = CommonHelper.GetGuid;
            entity.DEV_ID = txtDeviceID.Text;
            entity.DEV_URL = this.txtDeviceURL.Text;
            entity.DEV_NAME = this.textDeviceName.Text;
            WanWuYunDevice.Insert(entity);
            BindData();
        }

        private void BindData()
        {
            string sql = " select ID,DEV_NAME as 设备名称,DEV_ID as 设备ID,DEV_URL as 设备URL from WanWuYunDevice order by DEV_NAME  ";
            dt = WanWuYunDevice.FindTableBySql(sql);
            this.dataGridView1.DataSource = dt;
            WanWuYunDataList = new DateTime[dt.Rows.Count];
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            var id=this.dataGridView1.CurrentRow.Cells["ID"].Value;
            StringBuilder strsql = new StringBuilder();
            strsql.Append(" delete from WanWuYunDevice where ID='"+id+"' ");
            WanWuYunData.ExecuteBySql(strsql);
            BindData();
        }
    }
}
