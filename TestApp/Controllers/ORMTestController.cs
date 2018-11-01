using DataAccess;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utilities;

namespace TestApp.Controllers
{
    public class ORMTestController : Controller
    {
        Repository<WanWuYunDevice> re = new Repository<WanWuYunDevice>();
        // GET: ORMTest
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridPageListJson(string keywords, string CompanyId, string DepartmentId,
            JqGridParam jqgridparam, string ParameterJson)
        {

            try
            {
               
                Stopwatch watch = CommonHelper.TimerStart();
                StringBuilder strSql = new StringBuilder();
                List<DbParameter> parameter = new List<DbParameter>();
                strSql.Append(@" select * from WanWuYunDevice where 1=1  ");
                DataTable ListData = re.FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonConvert.SerializeObject(JsonData));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult SetForm(string id)
        {
            WanWuYunDevice entity = re.FindEntity(id);
            if (entity == null)
            {
                return Content("");
            }
            string strJson = JsonConvert.SerializeObject(entity);
            return Content(strJson);
        }

        public ActionResult SubmitForm(string id, WanWuYunDevice entity)
        {
            if (!string.IsNullOrEmpty(id))
            {
                entity.ID = id;
                re.Update(entity);
            }
            else
            {
                entity.ID = CommonHelper.GetGuid;
                re.Insert(entity);
            }
            return Content(new JsonMessage { Success = true, Code = "1", Message = "成功喽~~" }.ToString());
        }
    }
}