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
using THOK.WCS.Bll.Interfaces;
using THOK.WCS.DbModel;

namespace Wms.Controllers.WCS.DeviceState
{
    [TokenAclAuthorize]
    public class WcsDeviceFaultController : Controller
    {
        [Dependency]
        public IWcsDeviceFaultService WcsDeviceFaultServer { get; set; }
        //
        // GET: /WcsDeviceFault/

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
            WcsDeviceFault wcsDeviceFault = new WcsDeviceFault();

            string DeviceCode = collection["DeviceCode"] ?? "";
            string DeviceName = collection["DeviceName"] ?? "";
            string DeviceType = collection["DeviceType"] ?? "";
            string FaultCode = collection["FaultCode"] ?? "";
            string BeginTime = collection["BeginTime"] ?? "";
            string EndTime = collection["EndTime"] ?? "";
            string UseTime = collection["UseTime"] ?? "";

            var wcsDeviceFaultDetail = WcsDeviceFaultServer.GetDetails(page, rows, DeviceCode, DeviceType, FaultCode, BeginTime, EndTime, UseTime);
            return Json(wcsDeviceFaultDetail, "text", JsonRequestBehavior.AllowGet);

        }

        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            ExportParam ep = new ExportParam();

            string DeviceCode = Request.QueryString["DeviceCode"];
            string DeviceName = Request.QueryString["DeviceName"];
            string DeviceType = Request.QueryString["DeviceType"];
            string FaultCode = Request.QueryString["FaultCode"];
            string BeginTime = Request.QueryString["BeginTime"];
            string EndTime = Request.QueryString["EndTime"];
            string UseTime = Request.QueryString["UseTime"];
            ep.FirstTable = WcsDeviceFaultServer.GetWcsDeviceFault(page, rows, DeviceCode, DeviceType, FaultCode, BeginTime, EndTime, UseTime);
            ep.FirstHeadTitle = "故障状态查询";
            return PrintService.Print(ep);
        }

    }
}

