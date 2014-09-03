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
    public class WcsDeviceStateController : Controller
    {
        [Dependency]
        public IWcsDeviceStateService WcsDeviceStateServer { get; set; }
        //
        // GET: /WcsDeviceState/

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
            WcsDeviceState wcsDeviceState = new WcsDeviceState();

            string DeviceCode = collection["DeviceCode"] ?? "";
            string DeviceName = collection["DeviceName"] ?? "";
            string DeviceType = collection["DeviceType"] ?? "";
            string StateCode = collection["StateCode"] ?? "";
            string BeginTime = collection["BeginTime"] ?? "";
            string EndTime = collection["EndTime"] ?? "";
            string UseTime = collection["UseTime"] ?? "";

            var wcsDeviceStateDetail = WcsDeviceStateServer.GetDetails(page, rows, DeviceCode, DeviceType, StateCode, BeginTime, EndTime, UseTime);
            return Json(wcsDeviceStateDetail, "text", JsonRequestBehavior.AllowGet);

        }

        #region /WcsDeviceState/CreateExcelToClient/
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
            WcsDeviceState wds = new WcsDeviceState();
            wds.DeviceCode = DeviceCode;
            wds.DeviceType = DeviceType;

            ExportParam ep = new ExportParam();
            ep.FirstTable = WcsDeviceStateServer.GetWcsDeviceState(page, rows, wds);
            ep.FirstHeadTitle = "运行状态查询";
            return PrintService.Print(ep);
        }
        #endregion
    }
}

