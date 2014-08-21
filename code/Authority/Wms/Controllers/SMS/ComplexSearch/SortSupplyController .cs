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

namespace Wms.Controllers.SMS.ComplexSearch
{
    [TokenAclAuthorize]
    public class SortSupplyController : Controller
    {
        [Dependency]
        public ISortSupplyService SortSupplyServer { get; set; }
        //
        // GET: /SortSupplySearch/

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
            string SortBatchId = collection["SortBatchId"] ?? "";
            if (SortBatchId != "" && SortBatchId != null)
            {
                sortSupply.SortBatchId = Convert.ToInt32(SortBatchId);
            }
            string PackNo = collection["PackNo"] ?? "";
            if (PackNo != "" && PackNo != null)
            {
                sortSupply.PackNo = Convert.ToInt32(PackNo);
            }
            sortSupply.ChannelCode = collection["ChannelCode"] ?? "";
            sortSupply.ProductCode = collection["ProductCode"] ?? "";
            sortSupply.ProductName = collection["ProductName"] ?? "";

            //string SortBatchId = collection["SortBatchId"] ?? "";
            //if (SortBatchId != "" && SortBatchId != null)
            //{
            //    sortSupply.SortBatchId = Convert.ToInt32(SortBatchId);
            //}
            //string PackNo = collection["PackNo"] ?? "";
            //if (PackNo != "" && PackNo != null)
            //{
            //    sortSupply.PackNo = Convert.ToInt32(PackNo);
            //}
            //sortSupply.ChannelCode = sortSupply.ChannelCode ?? "";
            //sortSupply.ProductCode = sortSupply.ProductCode ?? "";
            //sortSupply.ProductName = sortSupply.ProductName ?? "";

            var sortSupplyDetail = SortSupplyServer.GetDetails(page, rows, sortSupply);
            return Json(sortSupplyDetail, "text", JsonRequestBehavior.AllowGet);

        }

        //打印
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            int sortBatchId = Convert.ToInt32(Request.QueryString["SortBatchId"] );
            int packNo = Convert.ToInt32(Request.QueryString["PackNo"]);
            string channelCode = Request.QueryString["ChannelCode"] ;
            string productCode = Request.QueryString["ProductCode"] ;
            string productName = Request.QueryString["ProductName"] ;
            SortSupply sortSupply = new SortSupply();
            sortSupply.SortBatchId = sortBatchId;
            sortSupply.PackNo = packNo;
            sortSupply.ChannelCode = channelCode;
            sortSupply.ProductCode = productCode;
            sortSupply.ProductName = productName;

            ExportParam ep = new ExportParam();
            ep.DT1 = SortSupplyServer.GetSortSupply(page, rows, sortSupply);
            ep.HeadTitle1 = "分拣补货查询";
            return PrintService.Print(ep);
        }
    }
}

        