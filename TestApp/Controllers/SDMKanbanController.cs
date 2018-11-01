using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApp.Controllers
{
    public class SDMKanbanController : Controller
    {
        Repository<SDMKanban> KanBanRe = new Repository<SDMKanban>();

        public static string ScollerTxt = "";
        // GET: SDMKanban
        public ActionResult Index()
        {
            return View();
        }

        public string GetYearData()
        {
            string sql = @"WITH tmp AS 
                            (
	                             SELECT DISTINCT c.PRD_NO,a.Res_No AS SEB_NO,sb.NAME AS SEB_NAME,sb.CostCenter,CONVERT(varchar(6), dp.NAME) AS pdLine,
	                             (case CONVERT(varchar(6), dp.NAME) when '机加工' then 1 else ISNULL(p.PAK_MEAST,1) end) as PAK_MEAST,
	                             isnull(p.weight,0) as weight 
	                             FROM TF_ZC_RES a 
	                            INNER JOIN TF_ZC b ON b.ID = a.MID 
	                            INNER JOIN MF_ZC c ON c.ID = b.MID 
	                            INNER JOIN SEBEI sb ON a.Res_No=sb.SEB_NO 
	                            INNER JOIN PRDT p ON c.PRD_NO=p.PRD_NO
	                            INNER JOIN dbo.DEPT dp ON SB.CostCenter=DP.DEP
	                            WHERE A.Res_No IN('328-001','328-002','328-003','JJG001','JJG002','JJG003','JJG004','JJG005','JJG006') AND p.KND=2
                            )
                            
                            select sum(sj_qty) as sj_qty,sum(dun_qty) as dun_qty  from (
                            SELECT  r2.pdLine,
                            CAST(ISNULL(r1.jh_qty,0) AS INT) jh_qty,
                            CAST(ISNULL(r2.sj_qty,0) AS INT) sj_qty,
                            CAST(ISNULL(r2.dun_qty,0) AS INT) dun_qty,
                            
                            (CASE WHEN r1.jh_qty IS NULL OR r1.jh_qty=0 THEN 0 else ROUND((r2.sj_qty/r1.jh_qty),4) end) rate
							 from
                            (
	                            SELECT jh.pdLine,SUM(CEILING (jh.jh_qty)) jh_qty FROM(
	                            SELECT t.pdLine, so.QTY/t.PAK_MEAST AS jh_qty FROM dbo.TF_POS so INNER JOIN tmp t
	                            ON so.PRD_NO=t.PRD_NO 
	                            WHERE  DATEDIFF(yy,so.EST_DD,GETDATE())=0) jh GROUP BY jh.pdLine
                            ) r1
                            right join
                            (
	                            SELECT sj.pdLine,SUM(CEILING (sj.sj_qty)) sj_qty,SUM(CEILING (sj.dun_qty)) dun_qty FROM (
	                            SELECT t.pdLine,
	                            (mk.QTY+ISNULL(mk.QTY_LOST,0)+ISNULL(mk.QTY_Scrap,0))/t.PAK_MEAST AS sj_qty, 
	                            (mk.QTY+ISNULL(mk.QTY_LOST,0)+ISNULL(mk.QTY_Scrap,0))/t.PAK_MEAST*t.weight as dun_qty
	                            FROM MK_YG mk 
	                            INNER JOIN tmp t 
	                            ON mk.PRD_NO=t.PRD_NO AND mk.SEB_NO=t.SEB_NO
	                            WHERE  DATEDIFF(yy,mk.BIL_DD,GETDATE())=0) sj GROUP BY sj.pdLine
                            ) r2
                            ON  r1.pdLine=r2.pdLine) as abc
                            where abc.pdline!='机加工'";
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                return dt.Rows[0][0].ToString() + "|" + dt.Rows[0][1].ToString();
            }
            else
            {
                return "0|0";
            }
        }

        public ActionResult GetCurrentDayData()
        {
            //测试库中没有当天的数据，使用年数据测试，更新到正式服务器上时需要调整
            string sql = @"WITH tmp AS 
                            (
	                             SELECT DISTINCT c.PRD_NO,a.Res_No AS SEB_NO,sb.NAME AS SEB_NAME,sb.CostCenter,CONVERT(varchar(6), dp.NAME) AS pdLine,
	                             (case CONVERT(varchar(6), dp.NAME) when '机加工' then 1 else ISNULL(p.PAK_MEAST,1) end) as PAK_MEAST,
	                             isnull(p.weight,0) as weight 
	                             FROM TF_ZC_RES a 
	                            INNER JOIN TF_ZC b ON b.ID = a.MID 
	                            INNER JOIN MF_ZC c ON c.ID = b.MID 
	                            INNER JOIN SEBEI sb ON a.Res_No=sb.SEB_NO 
	                            INNER JOIN PRDT p ON c.PRD_NO=p.PRD_NO
	                            INNER JOIN dbo.DEPT dp ON SB.CostCenter=DP.DEP
	                            WHERE A.Res_No IN('328-001','328-002','328-003','JJG001','JJG002','JJG003','JJG004','JJG005','JJG006') 
	                            AND p.KND=2
                            ),
     tmp2 as                    
(
	                             SELECT DISTINCT c.PRD_NO,a.Res_No AS SEB_NO,sb.NAME AS SEB_NAME,sb.CostCenter,CONVERT(varchar(6), dp.NAME) AS pdLine,
	                             (case CONVERT(varchar(6), dp.NAME) when '机加工' then 1 else ISNULL(p.PAK_MEAST,1) end) as PAK_MEAST,
	                             isnull(p.weight,0) as weight 
	                             FROM TF_ZC_RES a 
	                            INNER JOIN TF_ZC b ON b.ID = a.MID 
	                            INNER JOIN MF_ZC c ON c.ID = b.MID 
	                            INNER JOIN SEBEI sb ON a.Res_No=sb.SEB_NO 
	                            INNER JOIN PRDT p ON c.PRD_NO=p.PRD_NO
	                            INNER JOIN dbo.DEPT dp ON SB.CostCenter=DP.DEP
	                            where CONVERT(varchar(6), dp.NAME) in ('垂直线','壳型线','水平线')
	                            --WHERE A.Res_No IN('328-001','328-002','328-003','JJG001','JJG002','JJG003','JJG004','JJG005','JJG006') 
	                            --AND p.KND=2
                            )       
                           
                            SELECT  r3.pdLine,
                            CAST(ISNULL(r1.jh_qty,0) AS INT) jh_qty,
                            CAST(ISNULL(r2.sj_qty,0) AS INT) sj_qty,
                            CAST(ISNULL(r2.dun_qty,0) AS int) dun_qty,
                            CAST(ISNULL(r3.lu_qty,0) AS int) lu_qty,
                            
                            
                            (CASE WHEN r1.jh_qty IS NULL OR r1.jh_qty=0 THEN 0 else ROUND((r2.sj_qty/r1.jh_qty),4)*100 end) rate
							 from
                            (
	                            SELECT jh.pdLine,SUM(CEILING (jh.jh_qty)) jh_qty FROM(
	                            SELECT t.pdLine, so.QTY/t.PAK_MEAST AS jh_qty FROM dbo.TF_JH so INNER JOIN tmp t
	                            ON so.PRD_NO=t.PRD_NO 
	                            WHERE  DATEDIFF(day,so.EST_DD,GETDATE())=0) jh GROUP BY jh.pdLine
                            ) r1
                            right join
                            (
	                            SELECT sj.pdLine,SUM(CEILING (sj.sj_qty)) sj_qty,SUM(CEILING (sj.dun_qty)) dun_qty FROM (
	                            SELECT t.pdLine,
	                            (mk.QTY+ISNULL(mk.QTY_LOST,0)+ISNULL(mk.QTY_Scrap,0))/t.PAK_MEAST AS sj_qty, 
	                            (mk.QTY+ISNULL(mk.QTY_LOST,0)+ISNULL(mk.QTY_Scrap,0))*t.weight/1000 as dun_qty
	                            --case when mk.ZC_NO IN('4-3','K4-3') then isnull(mk.QTY,0) else 0 end as lu_qty
	                            FROM MK_YG mk 
	                            INNER JOIN tmp t 
	                            ON mk.PRD_NO=t.PRD_NO AND mk.SEB_NO=t.SEB_NO
	                            WHERE  DATEDIFF(day,mk.BIL_DD,GETDATE())=0) sj GROUP BY sj.pdLine
                            ) r2
                            ON  r1.pdLine=r2.pdLine
                            right join
                            (
                               SELECT sj.pdLine,SUM(CEILING (sj.lu_qty)) lu_qty FROM (
	                            SELECT t.pdLine,
	                            --(mk.QTY+ISNULL(mk.QTY_LOST,0)+ISNULL(mk.QTY_Scrap,0))/t.PAK_MEAST AS sj_qty, 
	                            --(mk.QTY+ISNULL(mk.QTY_LOST,0)+ISNULL(mk.QTY_Scrap,0))/t.PAK_MEAST*t.weight as dun_qty,
	                            case when mk.ZC_NO IN('4-3','K4-3') then isnull(mk.QTY,0) else 0 end as lu_qty
	                            FROM MK_YG mk 
	                            INNER JOIN tmp2 t 
	                            ON mk.PRD_NO=t.PRD_NO AND mk.SEB_NO=t.SEB_NO
	                            WHERE  DATEDIFF(day,mk.BIL_DD,GETDATE())=0
	                            and isnull(mk.QTY,0)!=0
	                            ) sj  GROUP BY sj.pdLine
                            ) r3
                            ON  r1.pdLine=r3.pdLine
                            ";
            DataTable dt = KanBanRe.FindTableBySql(sql);
            ScollerTxt = "";
            if (dt.Rows.Count>0)
            {
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    ScollerTxt = ScollerTxt +"<li>"+ dt.Rows[i]["pdline"].ToString() + "今日生产吨数:" + dt.Rows[i]["dun_qty"].ToString() + "," + "今日生产炉数:" + dt.Rows[i]["lu_qty"].ToString()+"</li>";
                }
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                ScollerTxt = "";
                return null;
            }
            
        }

        public string getEquipmentOEEResult(string pdline,string sebname)
        {
            try
            {
                //DateTime now = DateTime.Now;
                //DateTime d1 = new DateTime(now.Year, now.Month, 1);
                //DateTime d2 = d1.AddMonths(1).AddDays(-1);
                string startime = DateTime.Now.AddMonths(-1).Year.ToString() + "-" + DateTime.Now.AddMonths(-1).Month.ToString() + "-01";
                string endtime = Convert.ToDateTime(DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-1").AddDays(-1).ToString();

                //测试库写死时间才能看到数据
                //startime = "2018-05-01";
                //endtime = "2018-05-30";

                string sql = "select SEBEI_BI.pdline as pdline,run.STD_Time as startime ,run.END_Time as endtime ,sebei.Name as name,run.Sta_No as status from SB_Run_Record as run left join SEBEI_BI on SEBEI_BI.SEB_NO  = run.SEB_NO   left join sebei on sebei.SEB_NO = SEBEI_BI.SEB_NO where Time_Diff is not null and (SEBEI_BI.PdLine ='垂直线' or SEBEI_BI.PdLine='壳型线' or SEBEI_BI.PdLine ='水平线'  or SEBEI_BI.PdLine ='机加工')    ";
                sql += "and((STD_Time <= '" + startime + "'  and  END_Time >='" + endtime + "') or (STD_Time >= '" + startime + "'  and  END_Time <='" + endtime + "') ";
                sql += " or (STD_Time >= '" + startime + "' and STD_Time < '" + endtime + "'  and  END_Time >= '" + endtime + "') or (STD_Time <= '" + startime + "'  and  END_Time >='" + startime + "' and END_Time < = '" + endtime + "')) ";
                if (pdline != null)
                {
                    sql += " and SEBEI_BI.pdline ='" + pdline + "' ";
                }
                if (sebname != null)
                {
                    sql += " and sebei.Name = '" + sebname + "'";
                }
                sql += " order by startime asc";
                List<EquipmentOEEModel> OEEModelList = new List<EquipmentOEEModel>();
                //BooleanValue<DataTable> bv = DAO.Default.GetDataTable(sql);
                DataTable bv = KanBanRe.FindTableBySql(sql);
                string MK_YGSql = @"select m.BIL_DD,SEBEI.NAME AS SEB_NAME,c.PRD_NO,c.PRD_NAME,c.CYCLE_TIME,m.QTY,isnull(M.QTY_LOST,0) as QTY_LOST,M.TZ_NO from MK_YG as m inner join CYCLE_TIME as c on m.PRD_NO = c.PRD_NO 
            left join SEBEI on sebei.SEB_NO = m.SEB_NO
            where  m.BIL_DD between '" + startime + "' and '" + endtime + "' and SEBEI.name = '" + sebname + "'";
                //BooleanValue<DataTable> MKbv = DAO.Default.GetDataTable(MK_YGSql);
                DataTable MKbv = KanBanRe.FindTableBySql(MK_YGSql);
                //计划工作天数
                string JHDatesql = @"SELECT COUNT(DISTINCT(CONVERT(NVARCHAR(10),JH_DD,120))) as JHWorkDay  FROM TF_JH  WHERE  JH_DD >=DATEADD(MM, DATEDIFF(MM,0,'" + startime + "'), 0) AND JH_DD <= DATEADD(MS,-3,DATEADD(MM, DATEDIFF(M,0,'" + startime + "')+1, 0))";
                DataTable JHWorkTimeBV = KanBanRe.FindTableBySql(JHDatesql);
                string OP_WorkStartSql = "select * from OP_WorkStart where OPN_DD between '" + startime + "' and '" + endtime + "'";
                DataTable OPbv = KanBanRe.FindTableBySql(OP_WorkStartSql);
                DateTime stardate = Convert.ToDateTime(startime);
                DateTime enddate;
                int subdays = Convert.ToDateTime(endtime).Subtract(stardate).Days;
                DateTime actualStartDate = new DateTime();
                DateTime actualEndDate = new DateTime();
                for (int i = 1; i < subdays + 1; i++)
                {
                    EquipmentOEEModel OEEModel = new EquipmentOEEModel();
                    decimal effectiveWH = 0;   //有效工作时间
                    decimal faultWH = 0;       //故障时间
                    decimal nofaultWH = 0;     //非故障时间
                    OEEModel.OEEID = i;
                    OEEModel.PlanWH = 1440;
                    DateTime oeetime = stardate;
                    enddate = stardate.AddDays(1);
                    foreach (DataRow dr in bv.Rows)
                    {
                        OEEModel.EquipmentName = dr[3].ToString(); //设备名称
                        DateTime stardt = (DateTime)dr[1]; //设备运行开始时间
                        DateTime enddt = (DateTime)dr[2];  //设备运行结束时间
                        string status = dr[4].ToString();  //运行状态
                        if (stardt > enddate) //开始时间大于结束时间  跳过
                        {
                            stardate = enddate;
                            break;
                        }
                        if (enddt < stardate)  //结束时间小于每天的开始时间 跳过
                        {
                            continue;
                        }

                        if (stardate > stardt)
                        {
                            actualStartDate = stardate;
                        }
                        else
                        {
                            actualStartDate = stardt;
                        }
                        if (enddate > enddt)
                        {
                            actualEndDate = enddt;
                        }
                        else
                        {
                            actualEndDate = enddate;
                        }
                        if (status == "01") //获得正常状态的运行总分钟
                        {
                            TimeSpan span = actualEndDate - actualStartDate;
                            effectiveWH += (decimal)span.TotalMinutes;
                        }
                        else if (status == "03") //获得故障的运行总分钟
                        {
                            TimeSpan span = actualEndDate - actualStartDate;
                            faultWH += (decimal)span.TotalMinutes;
                        }
                        else      //非故障的运行总分钟
                        {
                            TimeSpan span = actualEndDate - actualStartDate;
                            nofaultWH += (decimal)span.TotalMinutes;
                        }
                    }
                    OEEModel.OEEDate = oeetime.ToString("MM-dd");
                    OEEModel.EffectiveWH = Math.Round(effectiveWH, 2);
                    OEEModel.FaultWH = Math.Round(faultWH, 2);
                    OEEModel.NoFaultWH = Math.Round(nofaultWH, 2);
                    DateTime MKendtime = oeetime.AddDays(1);
                    DataTable MKdt = MKbv;
                    var a = MKdt.Select("BIL_DD >='" + oeetime + "' and BIL_DD<='" + MKendtime + "' and  SEB_NAME = '" + OEEModel.EquipmentName + "' ", "BIL_DD ASC");
                    DataTable OPdt = OPbv;
                    decimal qty = a.Sum(x => x.Field<decimal>("qty"));
                    decimal qty_lost = a.Sum(x => x.Field<decimal>("QTY_LOST"));
                    //生产数量
                    OEEModel.ProductionNum = Math.Round(qty + qty_lost, 2);
                    OEEModel.ActualP = Math.Round(qty + qty_lost, 2);
                    //合格数量
                    OEEModel.QualifiedNum = Math.Round(qty, 2);
                    var b = a.Select(x => x.Field<string>("TZ_NO")).Distinct();
                    int num = b.Count();
                    OEEModel.TheoreticalP = 0;
                    if (num > 0)
                    {
                        //获得理论生产量
                        foreach (var item in b)
                        {
                            //string prd_no = item;
                            var cycletimelist = a.Where(x => x.Field<string>("TZ_NO") == item).ToList();

                            string varcycletime = cycletimelist[0][4].ToString();//生产节拍
                            if (varcycletime == "" || varcycletime == null)//判断生产节拍是否为空
                            {
                                continue;
                            }
                            decimal cycle_time = cycletimelist.Last<DataRow>().Field<decimal>("CYCLE_TIME");
                            var pList = OPbv.Select("OPN_DD >='" + oeetime + "' and OPN_DD<='" + MKendtime + "' and  TZ_NO = '" + item + "' ", "OPN_DD ASC").ToList();
                            DateTime pStar = oeetime;
                            if (pList.Count > 0)
                            {
                                pStar = pList.First<DataRow>().Field<DateTime>("OPN_DD");
                            }
                            DateTime pEnd = a.Where(x => x.Field<string>("TZ_NO") == item).ToList().Last<DataRow>().Field<DateTime>("BIL_DD");
                            TimeSpan span = pEnd - pStar;
                            decimal Minutes = (decimal)span.TotalMinutes;
                            OEEModel.TheoreticalP += Math.Round(cycle_time * Minutes, 2);
                        }
                    }
                    //时间开动率
                    decimal tiava = Math.Round(OEEModel.EffectiveWH / OEEModel.PlanWH, 2);
                    OEEModel.TimeAVA = tiava * 100;

                    decimal perava = 0;
                    decimal qrate = 0;
                    //性能开动率
                    if (OEEModel.TheoreticalP != 0)
                    {
                        perava = Math.Round(OEEModel.ProductionNum / OEEModel.TheoreticalP, 2);
                        OEEModel.PerformanceAVA = perava * 100;

                    }
                    else
                    {
                        OEEModel.PerformanceAVA = 0;
                    }
                    //合格率
                    if (OEEModel.ProductionNum != 0)
                    {
                        qrate = Math.Round(OEEModel.QualifiedNum / OEEModel.ProductionNum, 2);
                        OEEModel.QualifiedRate = qrate * 100;
                    }
                    else
                    {
                        OEEModel.QualifiedRate = 0;
                    }
                    //OEE
                    OEEModel.OEE = Math.Round(tiava * perava * qrate, 2) * 100;
                    //当月计划工作天数
                    OEEModel.JHDays = Convert.ToInt32(JHWorkTimeBV.Rows[0][0]);
                    //当月天数
                    OEEModel.MonthDays = DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month);
                    //TEEP
                    OEEModel.TEEP = Math.Round(OEEModel.OEE * OEEModel.JHDays / OEEModel.MonthDays, 2);
                    OEEModelList.Add(OEEModel);
                    stardate = enddate;
                }


                decimal PlanWH = OEEModelList.Sum(x => x.PlanWH);
                decimal EffectiveWH = OEEModelList.Sum(x => x.EffectiveWH);



                decimal TheoreticalP = OEEModelList.Sum(x => x.TheoreticalP);
                decimal ActualP = OEEModelList.Sum(x => x.ActualP);




                decimal QualifiedNum = OEEModelList.Sum(x => x.QualifiedNum);
                decimal ProductionNum = OEEModelList.Sum(x => x.ProductionNum);

                decimal tiavaAll = 0;
                decimal peravaAll = 0;
                decimal qrateAll = 0;
                if (PlanWH!=0)
                {
                    tiavaAll = Math.Round(EffectiveWH / PlanWH, 2);
                }
                if(TheoreticalP!=0)
                {
                    peravaAll = Math.Round(ProductionNum / TheoreticalP, 2);
                }
                if(ProductionNum!=0)
                {
                    qrateAll = Math.Round(QualifiedNum / ProductionNum, 2);
                }
                 


                decimal rOEE = tiavaAll * peravaAll * qrateAll * 100;
                //decimal ROEE2= OEEModelList.Sum(x => x.OEE); 
                //string str = JsonConvert.SerializeObject(OEEModelList, new DataTableConverter());
                //result.Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json");
                return rOEE.ToString();
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }

        public ActionResult GetPouringOKRate()
        {
            string sql = @"WITH TMP AS  --浇注品号
                            (
	                            SELECT B.NAME AS pdLine,PRD_NO,BOM_NO,A.ID FROM MF_BOM A left join DEPT B ON A.DEP=B.DEP  WHERE PRD_KND='2'
                            )
                            ,
                             boxPRDS AS  --装箱品号
                            (
		                            SELECT DISTINCT A.pdLine,A.PRD_NO FROM TMP A LEFT JOIN 
		                            (SELECT TB.MID,TB.PRD_NO FROM dbo.TF_BOM TB INNER JOIN PRDT P ON TB.PRD_NO=P.PRD_NO WHERE P.KND='2')
		                            B ON A.ID=B.MID WHERE B.PRD_NO IS NULL
		                            UNION
		                            (
			                            SELECT A.pdLine,B.PRD_NO FROM TMP A LEFT JOIN 
			                            (SELECT TB.MID,TB.PRD_NO FROM dbo.TF_BOM TB INNER JOIN PRDT P ON TB.PRD_NO=P.PRD_NO WHERE P.KND='2')
			                            B ON A.ID=B.MID WHERE B.PRD_NO IS NOT  NULL
		                            )
                            )
                            --以当月装箱数反推浇注数
                            SELECT a.pdLine,b.pouringQTY,a.boxQTY,cast(ROUND(a.boxQTY/pouringQTY,2) as decimal(18,4)) rate FROM
                            (
	                            SELECT SUM(w.QTY) AS boxQTY,PD.pdLine FROM MK_YG W 
	                            INNER JOIN boxPRDS PD ON W.PRD_NO=PD.PRD_NO
	                            WHERE w.ZC_NO IN('K6-10','6-10') 	and DATEDIFF(m,w.BIL_DD,GETDATE())=0
	                            GROUP BY PD.pdLine

                            ) a 
                            INNER JOIN
                            (
	                            SELECT SUM(w.QTY) AS pouringQTY,t.pdLine FROM MK_YG W 
	                            INNER JOIN TMP t ON w.PRD_NO=t.PRD_NO 
	                            INNER JOIN dbo.MF_MO mo ON w.MO_NO=mo.MO_NO
	                            INNER JOIN dbo.MF_JH jh ON mo.SO_NO=jh.JH_NO
	                            WHERE w.ZC_NO IN('K5-3','5-3')
	                            AND  DATEDIFF(m,jh.EST_DD,GETDATE())=0
	                            GROUP BY t.pdLine

                            ) b 
                            ON a.pdLine=b.pdLine";

            DataTable dt = KanBanRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }
        }

        public string GetScollerTxt()
        {
            return ScollerTxt;
        }

        public ActionResult getDefectRate(string pdline)
        {
            DateTime now = DateTime.Now;
            string d1 = new DateTime(now.Year, now.Month, 1).ToString();
            string d2 = Convert.ToDateTime(d1).AddMonths(1).AddDays(-1).ToString();
            //测试数据
            //d1 = "2018-09-01";
            //d2 = "2018-10-24";
            
            string sql = @"SELECT r.SPC_NAME,ROUND(r.qty/(SUM(qty) OVER()),4) rate,r.qty FROM(
                           SELECT B.SPC_NAME,SUM(b.RATE) qty FROM MF_YEILD a INNER  JOIN dbo.TF_YEILD b ON a.id=b.MID 
                           WHERE a.YD_DD>='{0}' AND a.YD_DD<'{1}' AND b.LINE='{2}' GROUP BY b.SPC_NAME
                           ) r";
            sql = string.Format(sql, d1, d2, pdline);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }
        }

        public ActionResult getQualifiedRate()
        {
            DateTime now = DateTime.Now;
            string d1 = new DateTime(now.Year, now.Month, 1).ToString();
            string d2 = Convert.ToDateTime(d1).AddMonths(1).AddDays(-1).ToString();
            //测试数据
            d1 = "2018-09-01";
            d2 = "2018-10-24";

            string sql = @"SELECT 
convert(varchar(10),YD_DD,120)as YD_DD,RATE_DISA as DISArate,RATE_KW as KWrate,RATE_KX as KXrate 
FROM MF_YEILD a 
WHERE a.YD_DD>='{0}' AND a.YD_DD<'{1}'  
order by YD_DD
";
            sql = string.Format(sql, d1, d2);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }
        }

        public ActionResult getEquipmentYWBResult(string YWBType)
        {
            DateTime now = DateTime.Now;
            string d1 = new DateTime(now.Year, now.Month, 1).ToString();
            string d2 = Convert.ToDateTime(d1).AddMonths(1).AddDays(-1).ToString();
            //测试数据
            //d1 = "2018-09-01";
            //d2 = "2018-10-24";

            string sql = @"select SEBEI_BI.pdline,count(status)as allcount,sum(case status  when 3  then 1 else 0 end) as tcount,
            sum(case status   when 1  then 1 else 0 end) as fcount,
            Convert(decimal(18,2),(Convert(float,sum(case status  when 3  then 1 else 0 end)*100)/Convert(float,count(status)))) as rate 
            from SB_MT_Order 
            inner join SEBEI_BI on SEBEI_BI.SEB_NO = SB_MT_Order.SEB_NO  
            left join SEBEI on sebei.SEB_NO = SEBEI_BI.SEB_NO    
            where jh_dd between '{0}' and '{1}'
            and  busino='{2}' 
            and (SEBEI_BI.pdline ='垂直线' 
            or SEBEI_BI.pdline='壳型线' 
            or SEBEI_BI.pdline ='水平线'  
            or SEBEI_BI.pdline ='机加工')
            group by SEBEI_BI.pdline
";
            sql = string.Format(sql, d1, d2,YWBType);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if (dt.Rows.Count > 0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }

        }


        public ActionResult getSPDISAPASSrate()
        {
            DateTime now = DateTime.Now;
            string d1 = now.AddDays(-1).ToString();
            //d1 = "2018-10-05";
            string sql = @"SELECT zc.PdLine,zc.ZC_NO,zc.ZC_NAME,ISNULL(passRate,0) passRate  FROM 
                            (
	                            SELECT '水平线' AS PdLine, ZC_NO,NAME AS ZC_NAME FROM dbo.ZC_NO WHERE ZC_NO IN('2-2','3-3','5-3','6-5','6-8')
	                            UNION
	                            SELECT '垂直线' AS PdLine, ZC_NO,NAME AS ZC_NAME FROM dbo.ZC_NO WHERE ZC_NO IN('2-2','3-3','5-3','6-5','6-8')
                            ) zc
                            LEFT JOIN
                            (
	                            SELECT PdLine,c.ZC_NO,cast(ROUND(SUM(QTY)/SUM(QTY+QTY_LOST+QTY_Scrap),4) as decimal(19,4)) passRate FROM(
	                            SELECT R.*,(CASE 
	                            WHEN R.KND='2' AND r.ZC_NO<>'6-8' THEN 
	                            (
		                            SELECT PdLine FROM dbo.SEBEI WHERE SEB_NO=r.SEB_NO
	                            ) 
	                            ELSE (
		                            SELECT TOP 1 d.NAME FROM dbo.MF_BOM b 
		                            INNER JOIN dbo.DEPT d ON b.DEP=d.DEP 
		                            WHERE BOM_NO=
		                            (
			                            SELECT TOP 1 a.ID_NO FROM tf_jh a 
			                            INNER JOIN prdt p ON a.PRD_NO=p.PRD_NO 
			                            INNER JOIN dbo.MF_MO mo ON a.JH_NO=mo.SO_NO 
			                            WHERE mo.MO_NO=r.MO_NO AND p.KND=2
		                            )
	                            ) end) PdLine FROM
	                            (
		                            SELECT p.PRD_NO, ZC_NO,MO_NO,SEB_NO,qty,ISNULL(QTY_LOST,0) QTY_LOST,ISNULL(QTY_Scrap,0) QTY_Scrap,p.KND  FROM dbo.MK_YG mk
		                            INNER JOIN dbo.PRDT p ON mk.PRD_NO=p.PRD_NO
		                             WHERE ZC_NO IN('2-2','3-3','5-3','6-5','6-8') 
		                            AND cast(BIL_DD as date)=cast('{0}' as date)
	                            ) R
	                            ) c WHERE ISNULL(c.PdLine,'')<>'' GROUP BY ZC_NO,PdLine
                            ) rr
                             ON zc.PdLine=rr.PdLine and zc.ZC_NO=rr.ZC_NO ORDER BY zc.PdLine,zc.ZC_NO";
            sql = string.Format(sql, d1);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }
        }

        public ActionResult getKXPASSrate()
        {
            DateTime now = DateTime.Now;
            string d1 = now.AddDays(-1).ToString();
            //d1 = "2018-10-05";
            string sql = @"SELECT A.ZC_NO, A.ZC_NAME,ISNULL(B.passRate,0) passRate FROM
                        (
	                        SELECT ZC_NO,NAME AS ZC_NAME FROM dbo.ZC_NO WHERE ZC_NO IN('K2-2','K5-3','K6-5','K6-7')
                        )
                        A LEFT JOIN 
                        (
	                        SELECT ZC_NO,cast(ROUND(SUM(QTY)/SUM(QTY+ISNULL(QTY_LOST,0)+ISNULL(QTY_Scrap,0)),4) as decimal(18,4)) passRate FROM dbo.MK_YG WHERE ZC_NO IN('K2-2','K5-3','K6-5','K6-7') 
	                        AND cast(BIL_DD as date)>=cast('{0}' as date)
	                        GROUP BY ZC_NO
                        ) B ON A.ZC_NO=B.ZC_NO";
            sql = string.Format(sql, d1);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if (dt.Rows.Count > 0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }
        }

        public ActionResult getZCPASSrate()
        {
            DateTime now = DateTime.Now;
            string d1 = now.AddDays(-1).ToString();
            //d1 = "2018-10-05";
            string sql = @"
SELECT zc.PdLine,zc.ZC_NO,zc.ZC_NAME,ISNULL(passRate,0) passRate  FROM 
                            (
	                            SELECT '水平线' AS PdLine, ZC_NO,NAME AS ZC_NAME FROM dbo.ZC_NO WHERE ZC_NO IN('2-2','3-3','5-3','6-5','6-8')
	                            UNION
	                            SELECT '垂直线' AS PdLine, ZC_NO,NAME AS ZC_NAME FROM dbo.ZC_NO WHERE ZC_NO IN('2-2','3-3','5-3','6-5','6-8')
                            ) zc
                            LEFT JOIN
                            (
	                            SELECT PdLine,c.ZC_NO,cast(ROUND(SUM(QTY)/SUM(QTY+QTY_LOST+QTY_Scrap),4) as decimal(18,4)) passRate FROM(
	                            SELECT R.*,(CASE 
	                            WHEN R.KND='2' AND r.ZC_NO<>'6-8' THEN 
	                            (
		                            SELECT PdLine FROM dbo.SEBEI WHERE SEB_NO=r.SEB_NO
	                            ) 
	                            ELSE (
		                            SELECT TOP 1 d.NAME FROM dbo.MF_BOM b 
		                            INNER JOIN dbo.DEPT d ON b.DEP=d.DEP 
		                            WHERE BOM_NO=
		                            (
			                            SELECT TOP 1 a.ID_NO FROM tf_jh a 
			                            INNER JOIN prdt p ON a.PRD_NO=p.PRD_NO 
			                            INNER JOIN dbo.MF_MO mo ON a.JH_NO=mo.SO_NO 
			                            WHERE mo.MO_NO=r.MO_NO AND p.KND=2
		                            )
	                            ) end) PdLine FROM
	                            (
		                            SELECT p.PRD_NO, ZC_NO,MO_NO,SEB_NO,qty,ISNULL(QTY_LOST,0) QTY_LOST,ISNULL(QTY_Scrap,0) QTY_Scrap,p.KND  FROM dbo.MK_YG mk
		                            INNER JOIN dbo.PRDT p ON mk.PRD_NO=p.PRD_NO
		                             WHERE ZC_NO IN('2-2','3-3','5-3','6-5','6-8') 
		                            AND cast(BIL_DD as date)=cast('{0}' as date)
	                            ) R
	                            ) c WHERE ISNULL(c.PdLine,'')<>'' GROUP BY ZC_NO,PdLine
                            ) rr
                             ON zc.PdLine=rr.PdLine and zc.ZC_NO=rr.ZC_NO 
union
SELECT '壳型线',A.ZC_NO, A.ZC_NAME,ISNULL(B.passRate,0) passRate FROM
                        (
	                        SELECT ZC_NO,NAME AS ZC_NAME FROM dbo.ZC_NO WHERE ZC_NO IN('K2-2','K5-3','K6-5','K6-7')
                        )
                        A LEFT JOIN 
                        (
	                        SELECT ZC_NO,ROUND(SUM(QTY)/SUM(QTY+ISNULL(QTY_LOST,0)+ISNULL(QTY_Scrap,0)),4) passRate FROM dbo.MK_YG WHERE ZC_NO IN('K2-2','K5-3','K6-5','K6-7') 
	                        AND cast(BIL_DD as date)>=cast('{0}' as date)
	                        GROUP BY ZC_NO
                        ) B ON A.ZC_NO=B.ZC_NO        ";
            sql = string.Format(sql, d1);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if (dt.Rows.Count > 0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }
        }


        public ActionResult getHJMList()
        {
            string sql = @" select DEV_ID from wanwuyundevice order by dev_name ";
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }
        }

        public ActionResult getHJMData(string HJMID)
        {
            DateTime now = DateTime.Now;
            string d1 = now.AddDays(-1).ToString();
            d1 = "2018-09-28";
            string sql = @"select top 200 b.dev_name,convert(varchar,time,8) as newtime,H,T 
from wanwuyundata a left join wanwuyundevice b on a.dev_id=b.dev_id
where a.dev_id = '{0}' and cast(time as date)=cast('{1}' as date)
order by time,dev_name desc";
            sql = string.Format(sql, HJMID, d1);
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }

        }

        public ActionResult getPDLINESTOPTIME()
        {
            string sql = @"select pdline,sum(stoptime) as stoptime
from (
select pdline,datediff(mi,
case when cast(startdate as date)<cast(getdate() as date) 
then cast(cast( year(getdate()) as nvarchar(50))+'-'+cast(month(getdate()) as nvarchar(50))+'-'+cast(day(getdate()) as nvarchar(50))+' 00:00:00' as datetime)
else startdate end
,enddate) as stoptime
from
(
select a.id,a.sysdate as startdate,b.busino,b.sysdate as enddate,c.pdline 
from call_info a 
inner join resp_info b on a.id=b.mid and b.busino='RES'
left join SEBEI c on a.seb_no=c.seb_no
where 1=1
--and
--a.seb_no in ('328-001','328-002','328-003',
--'811-001-1','811-001-2','811-002-1','811-002-2','811-003-1','811-003-2'
--,'811-004-1','811-004-2','811-005-1','811-005-2','811-006-1','811-006-2')
) as aa
where cast(startdate as date)<=cast(getdate() as date)
and cast(enddate as date)>=cast(getdate() as date) 
--添加一直没有修好的记录
union
select c.pdline,datediff(mi,
case when cast(a.sysdate as date)<cast(getdate() as date) 
then cast(cast( year(getdate()) as nvarchar(50))+'-'+cast(month(getdate()) as nvarchar(50))+'-'+cast(day(getdate()) as nvarchar(50))+' 00:00:00' as datetime)
else a.sysdate end
,getdate()) as stoptime from call_info a
left join SEBEI c on a.seb_no=c.seb_no
where not exists (select * from resp_info where mid=a.id and busino='RES')

)
as aaa 
group by pdline
";
            DataTable dt = KanBanRe.FindTableBySql(sql);
            if(dt.Rows.Count>0)
            {
                return Content(JsonConvert.SerializeObject(dt));
            }
            else
            {
                return null;
            }
        }




        public ActionResult Test()
        {
            return View();
        }

        //用于1080p的页面
        public ActionResult Index1080p()
        {
            return View();
        }
    }
}