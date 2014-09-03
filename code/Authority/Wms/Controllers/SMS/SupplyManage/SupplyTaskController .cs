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
        // GET: /SupplyTask/

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
        #region /SupplyTask/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;

            string channelCode = Request.QueryString["ChannelCode"] ;
            string productCode = Request.QueryString["ProductCode"] ;
            string productName = Request.QueryString["ProductName"] ;
            string sortingLineCode = Request.QueryString["SortingLineCode"];
            string status = Request.QueryString["Status"];
            SupplyTask supplyTask = new SupplyTask();
            supplyTask.ChannelCode = channelCode;
            supplyTask.ProductCode = productCode;
            supplyTask.ProductName = productName;
            supplyTask.SortingLineCode = sortingLineCode;
            supplyTask.Status = status;

            ExportParam ep = new ExportParam();
            ep.FirstTable = SupplyTaskServer.GetSupplyTask(page, rows, supplyTask);
            ep.FirstHeadTitle = "分拣补货查询";
            return PrintService.Print(ep);
        }
        #endregion
    }
}

        