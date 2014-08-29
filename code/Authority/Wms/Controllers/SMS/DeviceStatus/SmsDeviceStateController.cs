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


        //public ActionResult Details(int page, int rows, FormCollection collection)
        //{
        //    SmsDeviceState smsDeviceState = new SmsDeviceState();
        //    string UseTime = collection["UseTime"] ?? "";
        //    if (UseTime != "" && UseTime != null)
        //    {
        //        smsDeviceState.UseTime = Convert.ToInt32(UseTime);
        //    }
        //    smsDeviceState.DeviceCode = collection["DeviceCode"] ?? "";
        //    smsDeviceState.DeviceName = collection["DeviceName"] ?? "";
        //    smsDeviceState.DeviceType = collection["DeviceType"] ?? "";
        //    smsDeviceState.StateCode = collection["StateCode"] ?? "";
        //    smsDeviceState.BeginTime = collection["BeginTime"] ?? "";
        //    smsDeviceState.EndTime = collection["EndTime"] ?? "";

        //    var smsDeviceStateDetail = SmsDeviceStateServer.GetDetails(page, rows, smsDeviceState);
        //    return Json(smsDeviceStateDetail, "text", JsonRequestBehavior.AllowGet);

        //}

        //打印
        //#region /SmsDeviceState/CreateExcelToClient/
        //public FileStreamResult CreateExcelToClient()
        //{
        //    int page = 0, rows = 0;
        //    //int SupplyId = Convert.ToInt32(Request.QueryString["SupplyId"]);
        //    //int PackNo = Convert.ToInt32(Request.QueryString["PackNo"]);
        //    string channelCode = Request.QueryString["ChannelCode"];
        //    string productCode = Request.QueryString["ProductCode"];
        //    string productName = Request.QueryString["ProductName"];
        //    string sortingLineCode = Request.QueryString["SortingLineCode"];
        //    //int groupNo = Convert.ToInt32(Request.QueryString["GroupNo"]);
        //    string status = Request.QueryString["Status"];
        //    SmsDeviceState smsDeviceState = new SmsDeviceState();
        //    //smsDeviceState.SupplyId = SupplyId;
        //    //smsDeviceState.PackNo = PackNo;
        //    smsDeviceState.ChannelCode = channelCode;
        //    smsDeviceState.ProductCode = productCode;
        //    smsDeviceState.ProductName = productName;
        //    smsDeviceState.SortingLineCode = sortingLineCode;
        //    //smsDeviceState.GroupNo = groupNo;
        //    smsDeviceState.Status = status;

        //    ExportParam ep = new ExportParam();
        //    ep.DT1 = SmsDeviceStateServer.GetSmsDeviceState(page, rows, smsDeviceState);
        //    ep.HeadTitle1 = "分拣补货查询";
        //    return PrintService.Print(ep);
        //}
        //#endregion

        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            ExportParam ep = new ExportParam();

            string DeviceCode = Request.QueryString["DeviceCode"];
            string DeviceName = Request.QueryString["DeviceName"];
            string DeviceType = Request.QueryString["DeviceType"];
            string StateCode = Request.QueryString["StateCode"];
            string BeginTime = Request.QueryString["BeginTime"];
            string EndTime = Request.QueryString["EndTime"];
            string UseTime = Request.QueryString["UseTime"];
            ep.FirstTable = SmsDeviceStateServer.GetSmsDeviceState(page, rows, DeviceCode, DeviceType, StateCode, BeginTime, EndTime, UseTime);
            ep.FirstHeadTitle = "运行状态查询";
            return PrintService.Print(ep);
        }

    }
}

