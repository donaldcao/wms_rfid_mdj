using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using Microsoft.Practices.Unity;
using THOK.SMS.Bll.Interfaces;
using THOK.Common.WebUtil;
using THOK.Security;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;

namespace Wms.Controllers.SMS
{
    public class SortSupplyController : Controller
    {
        [Dependency]
        public ISortSupplyService SortSupplyServer { get; set; }
        //
        // GET: /SortSupply/

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
            SortSupply sortSupply = new SortSupply();
            sortSupply.SortSupplyCode = collection["SortSupplyCode"] ?? "";
            sortSupply.ChannelCode = collection["ChannelCode"] ?? "";
            sortSupply.ProductCode = collection["ProductCode"] ?? "";
            sortSupply.ProductName = collection["ProductName"] ?? "";

            string BatchSortId = collection["BatchSortId"] ?? "";
            if (BatchSortId != "" && BatchSortId != null)
            {
                sortSupply.BatchSortId = Convert.ToInt32(BatchSortId);
            }

            string SupplyId = collection["SupplyId"] ?? "";
            if (SupplyId != "" && SupplyId != null)
            {
                sortSupply.SupplyId = Convert.ToInt32(SupplyId);
            }

            string PackNo = collection["PackNo"] ?? "";
            if (PackNo != "" && PackNo != null)
            {
                sortSupply.PackNo = Convert.ToInt32(PackNo);
            }

            var sortSupplyDetail = SortSupplyServer.GetDetails(page, rows, sortSupply);
            return Json(sortSupplyDetail, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchPage()
        {
            return View();
        }

        //打印
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;

            string SortSupplyCode = Request.QueryString["SortSupplyCode"] ?? "";
            //int BatchSortId = Convert.ToInt32(Request.QueryString["BatchSortId"] ?? "");
            string ChannelCode = Request.QueryString["ChannelCode"] ?? "";
            //int SupplyId = Convert.ToInt32(Request.QueryString["SupplyId"] ?? "");
            //int PackNo = Convert.ToInt32(Request.QueryString["PackNo"] ?? "");
            string ProductCode = Request.QueryString["ProductCode"] ?? "";
            string ProductName = Request.QueryString["ProductName"] ?? "";

            SortSupply sortSupply = new SortSupply();
            sortSupply.SortSupplyCode = SortSupplyCode;
            //sortSupply.BatchSortId = BatchSortId;
            sortSupply.ChannelCode = ChannelCode;
            //sortSupply.SupplyId = SupplyId;
            //sortSupply.PackNo = PackNo;
            sortSupply.ProductCode = ProductCode;
            sortSupply.ProductName = ProductName;

            ExportParam ep = new ExportParam();
            ep.DT1 = SortSupplyServer.GetSortSupply(page, rows, sortSupply);
            ep.HeadTitle1 = "分拣补货";
            return PrintService.Print(ep);
        }

    }
}
