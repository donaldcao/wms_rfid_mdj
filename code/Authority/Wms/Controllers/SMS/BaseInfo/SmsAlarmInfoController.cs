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

        public ActionResult AddPage()
        {
            return View();
        }

        public ActionResult SearchPage()
        {
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
        public ActionResult Create(SmsAlarmInfo alarmInfo)
        {
            string strResult = string.Empty;
            bool bResult = SmsAlarmInfoService.Add(alarmInfo, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(SmsAlarmInfo alarmInfoCode)
        {
            string strResult = string.Empty;
            bool bResult = SmsAlarmInfoService.Save(alarmInfoCode, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string code)
        {
            string strResult = string.Empty;
            bool bResult = SmsAlarmInfoService.Delete(code, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string AlarmCode = Request.QueryString["alarmCode"];
            string Description = Request.QueryString["description"];
            SmsAlarmInfo alarmInfo = new SmsAlarmInfo();
            alarmInfo.AlarmCode = AlarmCode;
            alarmInfo.Description = Description;

            ExportParam ep = new ExportParam();
            ep.FirstTable = SmsAlarmInfoService.GetAlarmInfo(page, rows, alarmInfo);
            ep.FirstHeadTitle = "报警信息";
            return PrintService.Print(ep);
        }
    }
}
