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

namespace Wms.Controllers.SMS.SupplyManage
{
    [TokenAclAuthorize]
    public class SupplyTaskController : Controller
    {
        [Dependency]
        public ISupplyTaskService SupplyTaskServer { get; set; }
        //
        // GET: /SupplyTaskSearch/

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
            SupplyTask supplyTask = new SupplyTask();
            string SupplyId = collection["SupplyId"] ?? "";
            if (SupplyId != "" && SupplyId != null)
            {
                supplyTask.SupplyId = Convert.ToInt32(SupplyId);
            }
            string PackNo = collection["PackNo"] ?? "";
            if (PackNo != "" && PackNo != null)
            {
                supplyTask.PackNo = Convert.ToInt32(PackNo);
            }
            string GroupNo = collection["GroupNo"] ?? "";
            if (GroupNo != "" && GroupNo != null)
            {
                supplyTask.GroupNo = Convert.ToInt32(GroupNo);
            }
            supplyTask.ChannelCode = collection["ChannelCode"] ?? "";
            supplyTask.ProductCode = collection["ProductCode"] ?? "";
            supplyTask.ProductName = collection["ProductName"] ?? "";
            supplyTask.SortingLineCode = collection["SortingLineCode"] ?? "";
            supplyTask.Status = collection["Status"] ?? "";

            var supplyTaskDetail = SupplyTaskServer.GetDetails(page, rows, supplyTask);
            return Json(supplyTaskDetail, "text", JsonRequestBehavior.AllowGet);

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
            SupplyTask supplyTask = new SupplyTask();
            supplyTask.SupplyId = sortBatchId;
            supplyTask.PackNo = packNo;
            supplyTask.ChannelCode = channelCode;
            supplyTask.ProductCode = productCode;
            supplyTask.ProductName = productName;

            ExportParam ep = new ExportParam();
            ep.DT1 = SupplyTaskServer.GetSupplyTask(page, rows, supplyTask);
            ep.HeadTitle1 = "分拣补货查询";
            return PrintService.Print(ep);
        }
    }
}

        