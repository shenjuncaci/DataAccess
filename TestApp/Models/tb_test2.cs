using DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TestApp
{
    [Description("固定资产表")]
    [PrimaryKey("MainID")]
    public class tb_test2
    {
        [DisplayName("主键")]
        public string MainID { get; set; }
        public string MainName { get; set; }
        public string Remark { get; set; }
    }
}