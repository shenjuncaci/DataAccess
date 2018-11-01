using DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GetWanWuYunDataApp
{
    [Description("万物云设备信息表")]
    [PrimaryKey("ID")]
    public class WanWuYunDevice
    {
        [DisplayName("主键")]
        public string ID { get; set; }
        public string DEV_ID { get; set; }
        public string DEV_URL { get; set; }
        public string DEV_NAME { get; set; }
    }
}