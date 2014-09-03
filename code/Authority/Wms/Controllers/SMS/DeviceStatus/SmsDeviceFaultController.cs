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
    public class SmsDeviceFaultController : Controller
    {
        [Dependency]
        public ISmsDeviceFaultService SmsDeviceFaultServer { get; set; }
        //
        // GET: /SmsDeviceFault/

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
            SmsDeviceFault smsDeviceFault = new SmsDeviceFault();

            string DeviceCode = collection["DeviceCode"] ?? "";
            string DeviceName = collection["DeviceName"] ?? "";
            string DeviceType = collection["DeviceType"] ?? "";
            string FaultCode = collection["FaultCode"] ?? "";
            string BeginTime = collection["BeginTime"] ?? "";
            string EndTime = collection["EndTime"] ?? "";
            string UseTime = collection["UseTime"] ?? "";

            var smsDeviceFaultDetail = SmsDeviceFaultServer.GetDetails(page, rows, DeviceCode, DeviceType, FaultCode, BeginTime, EndTime, UseTime);
            return Json(smsDeviceFaultDetail, "text", JsonRequestBehavior.AllowGet);

        }

        #region /SmsDeviceFault/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;

            string DeviceCode = Request.QueryString["DeviceCode"];
            string DeviceName = Request.QueryString["DeviceName"];
            string DeviceType = Request.QueryString["DeviceType"];
            string FaultCode = Request.QueryString["FaultCode"];
            string BeginTime = Request.QueryString["BeginTime"];
            string EndTime = Request.QueryString["EndTime"];
            string UseTime = Request.QueryString["UseTime"];
            SmsDeviceFault sdf = new SmsDeviceFault();
            sdf.DeviceCode = DeviceCode;
            sdf.DeviceType = DeviceType;

            ExportParam ep = new ExportParam();
            ep.FirstTable = SmsDeviceFaultServer.GetSmsDeviceFault(page, rows,sdf);
            ep.FirstHeadTitle = "故障状态查询";
            return PrintService.Print(ep);
        }
        #endregion
    }
}

