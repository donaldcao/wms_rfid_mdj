using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;
using THOK.Common.WebUtil;
using THOK.Security;
using THOK.SMS.Bll.Interfaces;
using THOK.SMS.DbModel;

namespace Wms.Controllers.SMS.BaseInfo
{
    [TokenAclAuthorize]
    public class SmsAlarmInfoController : Controller
    {
        [Dependency]
        public ISmsAlarmInfoService SmsAlarmInfoService { get; set; }
        //
        // GET: /SmsAlarmInfo/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public JsonResult Details(int page, int rows,FormCollection collection)
        {
            SmsAlarmInfo smsAlarmInfo = new SmsAlarmInfo();
            string AlarmCode = collection["AlarmCode"] ?? "";
            string Description = collection["Description"] ?? "";
            var srmDetail = SmsAlarmInfoService.GetDetails(page, rows, AlarmCode, Description);
            return Json(srmDetail, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
