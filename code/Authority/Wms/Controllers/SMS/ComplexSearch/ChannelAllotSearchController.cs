using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using Microsoft.Practices.Unity;
using THOK.SMS.Bll.Interfaces;
using THOK.Security;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;

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

        //打印
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string orderDate = Request.QueryString["OrderDate"] ?? "";
            string batchNo = Request.QueryString["BatchNo"] ?? "";
            string sortingLineCode = Request.QueryString["SortingLineCode"] ?? "";
            string productCode = Request.QueryString["ProductCode"] ?? "";
            string text = "分拣烟道";

            ExportParam ep = new ExportParam();
            ep.DT1 = ChannelAllotServer.GetChannelAllot(page, rows, orderDate, batchNo, sortingLineCode, productCode, text);
            ep.HeadTitle1 = "分拣烟道查询";
            return PrintService.Print(ep);
        }
    }
}

        