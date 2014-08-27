using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;
using THOK.Security;
using THOK.SMS.Bll.Interfaces;
using THOK.SMS.DbModel;

namespace Wms.Controllers.SMS.ComplexSearch
{
    [TokenAclAuthorize]
    public class SortOrderAllotSearchController : Controller
    {
        [Dependency]
        public IChannelAllotService ChannelAllotServer { get; set; }
        [Dependency]
        public ISortOrderAllotMasterService SortOrderAllotMasterServer { get; set; }

        [Dependency]
        public ISortOrderAllotDetailService SortOrderAllotDetailService { get; set; }

        //
        // GET: /SortOrderAllotSearch/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult OrderMaster(int page, int rows, FormCollection collection)
        {
            string orderDate = collection["OrderDate"] ?? "";
            string batchNo = collection["BatchNo"] ?? "";
            string sortingLineCode = collection["SortingLineCode"] ?? "";
            string deliverLineCode = collection["DeliverLineCode"] ?? "";
            string customerCode = collection["CustomerCode"] ?? "";
            string status = collection["Status"] ?? "";
            var sortOrderMaster = SortOrderAllotMasterServer.GetDetails(page, rows, orderDate, batchNo, sortingLineCode, deliverLineCode, customerCode, status);
            return Json(sortOrderMaster, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderDetails(int page, int rows, int orderMasterCode)
        {
            var sortOrderDetail = SortOrderAllotDetailService.GetDetails(page, rows, orderMasterCode);
            return Json(sortOrderDetail, "text", JsonRequestBehavior.AllowGet);
        }

        //分拣订单  打印
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            ExportParam ep = new ExportParam();

            string batchNo = Request.QueryString["batchNo"];
            string sortingLineCode = Request.QueryString["SortingLineCode"];
            string deliverLineCode = Request.QueryString["DeliverLineCode"];
            string OrderDate = Request.QueryString["OrderDate"];
            ep.FirstTable = SortOrderAllotMasterServer.GetSortOrderAllotMaster(page, rows, OrderDate, batchNo, deliverLineCode,sortingLineCode);
            ep.FirstHeadTitle = "分拣订单";
            return PrintService.Print(ep);
        }
    }
}

        