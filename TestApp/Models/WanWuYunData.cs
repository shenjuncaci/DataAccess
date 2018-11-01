using DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TestApp
{
    [Description("万物云数据获取表")]
    [PrimaryKey("ID")]
    public class WanWuYunData
    {
        [DisplayName("主键")]
        public string ID { get; set; }
        public string DEV_ID { get; set; }
        public string PM25 { get; set; }
        public string H { get; set; }
        public string T { get; set; }
        public string HCHO { get; set; }
        public DateTime? TIME { get; set; }
    }
}