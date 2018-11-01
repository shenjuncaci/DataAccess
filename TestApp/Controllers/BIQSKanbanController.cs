using DataAccess;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;

namespace TestApp.Controllers
{
    public class BIQSKanbanController : Controller
    {
        Repository<BIQS> KanBanRe = new Repository<BIQS>();
        // GET: BIQSKanban
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult KanBan()
        {
            return View();
        }

        public string ProblemCompleteCount()
        {

            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();

            string result = "";
            string temp1 = "";
            string temp2 = "";
            string temp3 = "";
            string temp4 = "";
            //string sql = "select count(*) as cishu,fullname from RapidList_New where YEAR(res_cdate)='" + year + 
            //    "' group by fullname ";
            string sql = @"select count(distinct a.res_id),count(distinct b.res_id),realname,(select count(res_id) from RapidList_New where RealName=a.realname and RapidState='已完成') from [RapidList_New] a 
left join [OverdueList] b on a.res_id=b.res_id where a.RapidState!='已完成' 
";
            //if(!CommonHelper.IsEmpty(startDate))
            //{
            //    sql += " and cast(res_cdate as date)>=cast('"+startDate+"' as date) ";
            //}
            //if(!CommonHelper.IsEmpty(endDate))
            //{
            //    sql += " and cast(res_cdate as date)<=cast('" + endDate + "' as date) ";
            //}
            sql += "group by realname ";

            //sql = string.Format(sql, year);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    temp1 = temp1 + dt.Rows[i][0] + ",";
                    temp2 = temp2 + dt.Rows[i][1] + ",";
                    temp3 = temp3 + dt.Rows[i][2] + ",";
                    temp4 = temp4 + dt.Rows[i][3] + ",";

                }
                temp1 = temp1.Substring(0, temp1.Length - 1);
                temp2 = temp2.Substring(0, temp2.Length - 1);
                temp3 = temp3.Substring(0, temp3.Length - 1);
                temp4 = temp4.Substring(0, temp4.Length - 1);
            }
            result = temp1 + "|" + temp2 + "|" + temp3 + "|" + temp4;
            return result;
        }

        public string GetPieData()
        {
            string result = "";
            string temp1 = "";
            string temp2 = "";
            string temp3 = "";
            //string sql = "select count(*) as cishu,fullname from RapidList_New where YEAR(res_cdate)='" + year + 
            //    "' group by fullname ";
            string sql = @"select count(distinct a.res_id),count(distinct b.res_id),FullName from [RapidList_New] a 
left join [OverdueList] b on a.res_id=b.res_id where  1=1
group by FullName ";
            
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    temp1 = temp1 + dt.Rows[i][0] + ",";
                    temp2 = temp2 + dt.Rows[i][1] + ",";
                    temp3 = temp3 + dt.Rows[i][2] + ",";

                }
                temp1 = temp1.Substring(0, temp1.Length - 1);
                temp2 = temp2.Substring(0, temp2.Length - 1);
                temp3 = temp3.Substring(0, temp3.Length - 1);
            }
            result = temp1 + "|" + temp2 + "|" + temp3;
            return result;
        }

        public ActionResult QPictureListJson()
        {

            string sql = @" select *,
(select count(*) from FY_Rapid where YEAR(res_cdate)='{0}' and MONTH(res_cdate)='{1}' and DAY(res_cdate)=a.basicday and res_ok='外部' ) as waibuNum,
(select count(*) from FY_Rapid where YEAR(res_cdate)='{0}' and MONTH(res_cdate)='{1}' and DAY(res_cdate)=a.basicday and res_ok='内部' ) as neiNum,
IsOverDue=case when '{1}'=MONTH(GETDATE()) and a.BasicDay>DAY(GETDATE()) then 1 else 0 end 
from base_day a where 1=1 order by a.basicday ";
            sql = string.Format(sql, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
            DataTable dt = KanBanRe.FindTableBySql(sql);
            return Content(dt.ToJson());
        }

        public string GetSperateReport()
        {
            string result = "";
            string tempX = "";
            string tempY1 = "";
            string tempY2 = "";
            string tempY3 = "";
            string tempY4 = "";
            DateTime now = DateTime.Now;
            DateTime d1 = new DateTime(now.Year, now.Month, 1);
            DateTime d2 = d1.AddMonths(1).AddDays(-1);
            try
            {
               
                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(DbFactory.CreateDbParameter("@StartDate", d1.ToString()));
                parameter.Add(DbFactory.CreateDbParameter("@EndDate", d2.ToString()));
                DataTable ListData = KanBanRe.FindDataSetByProc("BZCompleteRate", parameter.ToArray()).Tables[0];
                if (ListData.Rows.Count > 0)
                {
                    for (int i = 0; i < ListData.Rows.Count; i++)
                    {
                        tempX += ListData.Rows[i]["fullname"].ToString() + ",";
                        tempY1 += ListData.Rows[i]["BZrate"].ToString() + ",";
                        tempY2 += ListData.Rows[i]["CJZRrate"].ToString() + ",";
                        tempY3 += ListData.Rows[i]["CZRrate"].ToString() + ",";
                        tempY4 += ListData.Rows[i]["CJrate"].ToString() + ",";
                    }
                    tempX = tempX.Substring(0, tempX.Length - 1);
                    tempY1 = tempY1.Substring(0, tempY1.Length - 1);
                    tempY2 = tempY2.Substring(0, tempY2.Length - 1);
                    tempY3 = tempY3.Substring(0, tempY3.Length - 1);
                    tempY4 = tempY4.Substring(0, tempY4.Length - 1);
                }
                result = tempX + "|" + tempY1 + "|" + tempY2 + "|" + tempY3 + "|" + tempY4;
                return result;
            }
            catch (Exception ex)
            {
                
                return "";
            }
        }

        public string GetProblemActionData()
        {
            string result = "";
            string temp1 = "";
            string temp2 = "";
            string temp3 = "";
            string temp4 = "";
            //string sql = " select count(*) as rapidcount,MONTH(res_cdate) as month from FY_Rapid where YEAR(res_cdate)='"+year+"' group by MONTH(res_cdate),YEAR(res_cdate)  ";
            string sql = @"select isnull(problemcount,0),ISNULL(okcount,0),basicmonth,case when ISNULL(okcount,0)=0 then 0 else cast(100.0*ISNULL(okcount,0)/isnull(problemcount,0) as decimal(18,2)) end as rate
from
Base_Month a 
left join (select count(*) as problemcount,MONTH(createdt) as month 
from FY_ProblemAction where YEAR(createdt)=YEAR(Getdate()) group by MONTH(createdt),YEAR(createdt)) as b
on a.BasicMonth=b.month
left join  (select count(*) as okcount,MONTH(createdt) as month 
from FY_ProblemAction where YEAR(createdt)=YEAR(Getdate())
and ProblemState='已完成'
group by MONTH(createdt),YEAR(createdt)) as c
on a.BasicMonth=c.month ";
            sql = string.Format(sql);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    temp1 = temp1 + dt.Rows[i][0] + ",";
                    temp2 = temp2 + dt.Rows[i][1] + ",";
                    temp3 = temp3 + dt.Rows[i][2] + ",";
                    temp4 = temp4 + dt.Rows[i][3] + ",";
                }
                temp1 = temp1.Substring(0, temp1.Length - 1);
                temp2 = temp2.Substring(0, temp2.Length - 1);
                temp3 = temp3.Substring(0, temp3.Length - 1);
                temp4 = temp4.Substring(0, temp4.Length - 1);
            }
            result = temp1 + "|" + temp2 + "|" + temp3 + "|" + temp4;


            return result;
        }

        public string GetProblemActionDataByResponse()
        {
            DateTime now = DateTime.Now;
            DateTime d1 = new DateTime(now.Year, now.Month, 1);
            DateTime d2 = d1.AddMonths(1).AddDays(-1);
            string result = "";
            string temp1 = "";
            string temp2 = "";
            string temp3 = "";
            string temp4 = "";
            //string sql = " select count(*) as rapidcount,MONTH(res_cdate) as month from FY_Rapid where YEAR(res_cdate)='"+year+"' group by MONTH(res_cdate),YEAR(res_cdate)  ";
            string sql = @" select count(*),
(select count(*) from FY_ProblemAction aa 
left join Base_User bb on aa.ResponseBy=bb.UserId
left join Base_Department cc on bb.DepartmentId=cc.DepartmentId
where cast(aa.CreateDt as date)>= cast('{0}' as date) 
and cast(aa.CreateDt as date)<=cast('{1}' as date) 
and ProblemState='已完成'
and cc.DepartmentId=c.DepartmentId),c.FullName
 from FY_ProblemAction a 
left join Base_User b on a.ResponseBy=b.UserId
left join Base_Department c on b.DepartmentId=c.DepartmentId
where cast(a.CreateDt as date)>= cast('{0}' as date) 
and cast(a.CreateDt as date)<=cast('{1}' as date) 
group by c.FullName,c.DepartmentId ";

            sql = string.Format(sql, d1.ToString(), d2.ToString());
            DataTable dt = KanBanRe.FindTableBySql(sql);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    temp1 = temp1 + dt.Rows[i][0] + ",";
                    temp2 = temp2 + dt.Rows[i][1] + ",";
                    temp3 = temp3 + dt.Rows[i][2] + ",";

                }
                temp1 = temp1.Substring(0, temp1.Length - 1);
                temp2 = temp2.Substring(0, temp2.Length - 1);
                temp3 = temp3.Substring(0, temp3.Length - 1);

            }
            result = temp1 + "|" + temp2 + "|" + temp3;


            return result;
        }
    }
}