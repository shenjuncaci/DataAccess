using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;

namespace TestApp.Controllers
{
    public class WeChatTestController : Controller
    {
        // GET: WeChatTest
        public ActionResult Index()
        {
            return View();
        }

        public string GetUserInfo(string code)
        {
            WeChatUser entity = JsonHelper.JsonToEntity<WeChatUser>(WeChatHelper.GetUserInfo(code));
            return entity.UserID;
        }
    }
}