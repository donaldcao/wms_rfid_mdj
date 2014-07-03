using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using Microsoft.Practices.Unity;
using THOK.SMS.Bll.Interfaces;
using THOK.Security;

namespace Wms.Controllers.SMS.ComplexSearch
{
    [TokenAclAuthorize]
    public class ChannelAllotSearchController : Controller
    {
        [Dependency]
        public IChannelAllotService ChannelAllotServer { get; set; }
        //
        // GET: /ChannelAllotSearch/

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

            string orderDate = collection["OrderDate"] ?? "";
            string batchNo = collection["BatchNo"] ?? "";
            string sortingLineCode = collection["SortingLineCode"] ?? "";
            string productCode = collection["ProductCode"] ?? "";

            var channelAllotDetail = ChannelAllotServer.GetDetails(page, rows, orderDate, batchNo, sortingLineCode, productCode);
            return Json(channelAllotDetail, "text", JsonRequestBehavior.AllowGet);

        }

        //////打印
        ////public FileStreamResult CreateExcelToClient()
        ////{
        ////    int page = 0, rows = 0;

        ////    string ChannelAllotCode = Request.QueryString["ChannelAllotCode"] ?? "";
        ////    int BatchSortId = Convert.ToInt32(Request.QueryString["BatchSortId"] ?? "");
        ////    string ChannelCode = Request.QueryString["ChannelCode"] ?? "";
        ////    string ProductCode = Request.QueryString["ProductCode"] ?? "";
        ////    string ProductName = Request.QueryString["ProductName"] ?? "";
        ////    int InQuantity = Convert.ToInt32(Request.QueryString["InQuantity"] ?? "");
        ////    int OutQuantity = Convert.ToInt32(Request.QueryString["OutQuantity"] ?? "");
        ////    int RealQuantity = Convert.ToInt32(Request.QueryString["RealQuantity"] ?? "");
        ////    int RemainQuantity = Convert.ToInt32(Request.QueryString["RemainQuantity"] ?? "");


        ////    ChannelAllot channelAllot = new ChannelAllot();
        ////    channelAllot.ChannelAllotCode = ChannelAllotCode;
        ////    channelAllot.BatchSortId = BatchSortId;
        ////    channelAllot.ChannelCode = ChannelCode;
        ////    channelAllot.ProductCode = ProductCode;
        ////    channelAllot.ProductName = ProductName;
        ////    channelAllot.InQuantity = InQuantity;
        ////    channelAllot.OutQuantity = OutQuantity;
        ////    channelAllot.RealQuantity = RealQuantity;
        ////    channelAllot.RemainQuantity = RemainQuantity;

        ////    ExportParam ep = new ExportParam();
        ////    ep.DT1 = ChannelAllotServer.GetChannelAllot(page, rows, channelAllot);
        ////    ep.HeadTitle1 = "烟道分配";
        ////    return PrintService.Print(ep);
        ////}
    }
}

        