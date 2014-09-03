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

namespace Wms.Controllers.SMS.DeviceState
{
    [TokenAclAuthorize]
    public class SmsDeviceStateController : Controller
    {
        [Dependency]
        public ISmsDeviceStateService SmsDeviceStateServer { get; set; }
        //
        // GET: /SmsDeviceState/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            SmsDeviceState smsDeviceState = new SmsDeviceState();

            string DeviceCode = collection["DeviceCode"] ?? "";
            string DeviceName = collection["DeviceName"] ?? "";
            string DeviceType = collection["DeviceType"] ?? "";
            string StateCode = collection["StateCode"] ?? "";
            string BeginTime = collection["BeginTime"] ?? "";
            string EndTime = collection["EndTime"] ?? "";
            string UseTime = collection["UseTime"] ?? "";

            var smsDeviceStateDetail = SmsDeviceStateServer.GetDetails(page, rows, DeviceCode, DeviceType, StateCode, BeginTime, EndTime, UseTime);
            return Json(smsDeviceStateDetail, "text", JsonRequestBehavior.AllowGet);

        }

        #region /SmsDeviceState/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;

            string DeviceCode = Request.QueryString["DeviceCode"];
            string DeviceName = Request.QueryString["DeviceName"];
            string DeviceType = Request.QueryString["DeviceType"];
            string StateCode = Request.QueryString["StateCode"];
            string BeginTime = Request.QueryString["BeginTime"];
            string EndTime = Request.QueryString["EndTime"];
            string UseTime = Request.QueryString["UseTime"];
            SmsDeviceState sds = new SmsDeviceState();
            sds.DeviceCode = DeviceCode;
            sds.DeviceType = DeviceType;
 
            ExportParam ep = new ExportParam();
            ep.FirstTable = SmsDeviceStateServer.GetSmsDeviceState(page, rows,sds);
            ep.FirstHeadTitle = "运行状态查询";
            return PrintService.Print(ep);
        }
        #endregion
    }
}

