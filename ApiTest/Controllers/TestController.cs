using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ApiTest.Controllers
{
    public class TestController : ApiController
    {
        public HttpResponseMessage TestParams(JObject jsonObj)
        {
            //序列化为json字符串
            var jsonStr = JsonConvert.SerializeObject(jsonObj);
            //反序列化为动态json object
            var jsonParams = JsonConvert.DeserializeObject<User>(jsonStr);
            String userName = jsonParams.userName;
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(userName, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        //public HttpResponseMessage TestParams(string userName, string test)
        //{
        //    序列化为json字符串
        //    var jsonStr = JsonConvert.SerializeObject(jsonObj);
        //    //反序列化为动态json object
        //    var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
        //    String userName = jsonParams.userName;
        //    HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent("121", Encoding.GetEncoding("UTF-8"), "application/json") };
        //    return result;
        //}
    }
}
