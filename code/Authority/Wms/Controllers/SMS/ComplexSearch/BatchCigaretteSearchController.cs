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
    public class BatchCigaretteSearchController : Controller
    {
        [Dependency]
        public IChannelAllotService ChannelAllotServer { get; set; }

        //
        // GET: /BatchCigaretteSearch/

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
            var channelAllotDetail = ChannelAllotServer.Details(page, rows, orderDate, batchNo, sortingLineCode, productCode);
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
            string text = "分拣备货";
            ExportParam ep = new ExportParam();
            ep.FirstTable = ChannelAllotServer.GetChannelAllot(page, rows, orderDate, batchNo, sortingLineCode, productCode,text);
            ep.FirstHeadTitle = "分拣备货";
            return PrintService.Print(ep);
        }
    }
}
