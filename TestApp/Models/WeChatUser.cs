using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp
{
    public class WeChatUser
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string UserID { get; set; }
        public string DeviceId { get; set; }
        public string user_ticket { get; set; }
        public string expires_in { get; set; }
    }
}