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
    public class HandSupplyController : Controller
    {
        [Dependency]
        public IHandSupplyService SortSupplyServer { get; set; }
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
            HandSupply handSupply = new HandSupply();
            //sortSupply.SortSupplyCode = collection["SortSupplyCode"] ?? "";
            handSupply.ChannelCode = collection["ChannelCode"] ?? "";
            handSupply.ProductCode = collection["ProductCode"] ?? "";
            handSupply.ProductName = collection["ProductName"] ?? "";

            //string SortBatchId = collection["SortBatchId"] ?? "";
            //if (SortBatchId != "" && SortBatchId != null)
            //{
            //    sortSupply.SortBatchId = Convert.ToInt32(SortBatchId);
            //}

            //string SupplyId = collection["SupplyId"] ?? "";
            //if (SupplyId != "" && SupplyId != null)
            //{
            //    sortSupply.SupplyId = Convert.ToInt32(SupplyId);
            //}

            //string PackNo = collection["PackNo"] ?? "";
            //if (PackNo != "" && PackNo != null)
            //{
            //    sortSupply.PackNo = Convert.ToInt32(PackNo);
            //}

            var sortSupplyDetail = SortSupplyServer.GetDetails(page, rows, handSupply);
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
            //int SortBatchId = Convert.ToInt32(Request.QueryString["SortBatchId"] ?? "");
            string ChannelCode = Request.QueryString["ChannelCode"] ?? "";
            //int SupplyId = Convert.ToInt32(Request.QueryString["SupplyId"] ?? "");
            //int PackNo = Convert.ToInt32(Request.QueryString["PackNo"] ?? "");
            string ProductCode = Request.QueryString["ProductCode"] ?? "";
            string ProductName = Request.QueryString["ProductName"] ?? "";

            HandSupply sortSupply = new HandSupply();
            //sortSupply.SortSupplyCode = SortSupplyCode;
            //sortSupply.SortBatchId = SortBatchId;
            sortSupply.ChannelCode = ChannelCode;
            //sortSupply.SupplyId = SupplyId;
            //sortSupply.PackNo = PackNo;
            sortSupply.ProductCode = ProductCode;
            sortSupply.ProductName = ProductName;

            ExportParam ep = new ExportParam();
            ep.DT1 = SortSupplyServer.GetSortSupply(page, rows, sortSupply);
            ep.HeadTitle1 = "手工补货";
            return PrintService.Print(ep);
        }

    }
}
