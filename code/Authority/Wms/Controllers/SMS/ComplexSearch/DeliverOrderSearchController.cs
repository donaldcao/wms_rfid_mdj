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
    public class DeliverOrderSearchController : Controller
    {
        [Dependency]
        public ISortBatchService SortBatchService { get; set; }

        //
        // GET: /DeliverOrderSearch/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        // GET: /DeliverOrderSearch/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string orderDate = collection["OrderDate"] ?? "";
            string batchNo = collection["BatchNo"] ?? "";
            string sortingLineCode = collection["SortingLineCode"] ?? "";
            string status = collection["Status"] ?? "";
            var sortOrder = SortBatchService.GetDetails(page, rows, orderDate, batchNo, sortingLineCode);
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //打印 未完成
        public FileStreamResult CreateExcelToClient()
        {
            //int page = 0, rows = 0;
            ExportParam ep = new ExportParam();
            //ep.DT1 = ChannelAllotServer.GetChannelAllot(page, rows);
            ep.HeadTitle1 = "线路配送查询";
            return PrintService.Print(ep);
        }
    }
}

        