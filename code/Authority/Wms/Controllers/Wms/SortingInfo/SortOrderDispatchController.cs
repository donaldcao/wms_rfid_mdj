using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.Common.WebUtil;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.Security;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;

namespace Wms.Controllers.Wms.SortingInfo
{
    [TokenAclAuthorize]
    public class SortOrderDispatchController : Controller
    {
        [Dependency]
        public ISortOrderDispatchService SortOrderDispatchService { get; set; }
        //
        // GET: /SortOrderDispatch/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        //
        // GET: /SortOrderDispatch/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string SortingLineCode = collection["SortingLineCode"] ?? "";
            string OrderDate = collection["OrderDate"] ?? "";
            string WorkStatus = collection["WorkStatus"] ?? "";
            string SortStatus = collection["SortStatus"] ?? "";
            var sortOrder = SortOrderDispatchService.GetDetails(page, rows, OrderDate, WorkStatus,SortStatus,SortingLineCode);
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //查询未作业的线路调度数据
        // GET: /SortOrderDispatch/GetWorkStatus/
        public ActionResult GetWorkStatus()
        {
            var sortOrder = SortOrderDispatchService.GetWorkStatus();
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //查询未分配批次的线路调度数据
        // GET: /SortOrderDispatch/GetBatchStatus/
        public ActionResult GetBatchStatus()
        {
            var sortOrder = SortOrderDispatchService.GetBatchStatus();
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //新增线路调度
        // POST: /SortOrderDispatch/Create/
        public ActionResult Create(string SortingLineCode, string DeliverLineCodes,string orderDate,string IsAuto)
        {
            bool bResult;
            if (IsAuto == "1") 
            {
                bResult = SortOrderDispatchService.Add(DeliverLineCodes, orderDate);
            }
            else
            {
                bResult = SortOrderDispatchService.Add(SortingLineCode, DeliverLineCodes, orderDate);
            }
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SortOrderDispatch/Edit/
        public ActionResult Edit(string id, string SortingLineCode)
        {
            bool bResult = SortOrderDispatchService.Edit(id, SortingLineCode);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SortOrderDispatch/Delete/
        public ActionResult Delete(string id)
        {
            string errorInfo = string.Empty;
            bool bResult = SortOrderDispatchService.Delete(id, out errorInfo);
            string msg = errorInfo;
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        #region /SortOrderDispatch/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string orderDate = Request.QueryString["orderDate"];
            string sortingLineCode = Request.QueryString["sortingLineCode"];
            string WorkStatus = Request.QueryString["WorkStatus"];
            string SortStatus = Request.QueryString["SortStatus"];
          
            ExportParam ep = new ExportParam();
            ep.DT1 = SortOrderDispatchService.GetSortOrderDispatch(page, rows, orderDate,WorkStatus,SortStatus, sortingLineCode);
            ep.HeadTitle1 = "分拣线路调度";
            return PrintService.Print(ep);
        }
        #endregion
    }
}
