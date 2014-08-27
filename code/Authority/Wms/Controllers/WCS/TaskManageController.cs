﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.WCS.DbModel;
using THOK.Common.WebUtil;
using THOK.WCS.Bll.Interfaces;

namespace Wms.Controllers.WCS
{
    public class TaskManageController : Controller
    {
        [Dependency]
        public ITaskService TaskService { get; set; }

        //
        // GET: /TaskManage/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.hasEmpty = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        // GET: /TaskManage/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            Task task = new Task();
            task.TaskType = collection["TaskType"] ?? "";
            task.ProductCode = collection["ProductCode"] ?? "";
            task.ProductName = collection["ProductName"] ?? "";
            task.OriginCellCode = collection["OriginCellCode"] ?? "";
            task.TargetCellCode = collection["TargetCellCode"] ?? "";
            task.CurrentPositionState = collection["CurrentPositionState"] ?? "";
            task.State = collection["State"] ?? "";
            task.TagState = collection["TagState"] ?? "";
            task.OrderID = collection["OrderID"] ?? "";
            task.OrderType = collection["OrderType"] ?? "";
            task.DownloadState = collection["DownloadState"] ?? "";
            string id = collection["ID"] ?? "";
            string pathID = collection["PathID"] ?? "";
            string originPositionID = collection["OriginPositionID"] ?? "";
            string targetPositionID = collection["TargetPositionID"] ?? "";
            string currentPositionID = collection["CurrentPositionID"] ?? "";
            string allotID = collection["AllotID"] ?? "";
            if (id != null && id != "")
                task.ID = Convert.ToInt32(id);
            if (pathID != null && pathID != "") 
                task.PathID = Convert.ToInt32(pathID);
            if (originPositionID != null && originPositionID != "") 
                task.OriginPositionID = Convert.ToInt32(originPositionID);
            if (targetPositionID != null && targetPositionID != "") 
                task.TargetPositionID = Convert.ToInt32(targetPositionID);
            if (currentPositionID != null && currentPositionID != "") 
                task.CurrentPositionID = Convert.ToInt32(currentPositionID);
            if (allotID != null && allotID != "")
                task.AllotID = Convert.ToInt32(allotID);

            var detail = TaskService.GetDetails(page, rows, task);
            return Json(detail, "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /TaskManage/Create/
        public ActionResult Create(Task task)
        {
            string strResult = string.Empty;
            var bResult = TaskService.Add(task, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /TaskManage/Edit/
        public ActionResult Edit(Task task)
        {
            string strResult = string.Empty;
            bool bResult = TaskService.Save(task, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /TaskManage/Delete/
        public ActionResult Delete(string taskId)
        {
            string strResult = string.Empty;
            bool bResult = TaskService.Delete(taskId, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /TaskManage/Clear/
        public ActionResult Clear()
        {
            string errorInfo = string.Empty;
            bool bResult = TaskService.ClearTask(out errorInfo);
            string msg = bResult ? "清空成功" : "清空失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, errorInfo), "text", JsonRequestBehavior.AllowGet);
        }

        // GET: /TaskManage/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            Task task = new Task();
            task.TaskType = Request.QueryString["taskType"] ?? "";
            task.ProductCode = Request.QueryString["productCode"] ?? "";
            task.ProductName = Request.QueryString["productName"] ?? "";
            task.OriginCellCode = Request.QueryString["originCellCode"] ?? "";
            task.TargetCellCode = Request.QueryString["targetCellCode"] ?? "";
            task.CurrentPositionState = Request.QueryString["currentPositionState"] ?? "";
            task.State = Request.QueryString["state"] ?? "";
            task.TagState = Request.QueryString["tagState"] ?? "";
            task.OrderID = Request.QueryString["orderID"] ?? "";
            task.OrderType = Request.QueryString["orderType"] ?? "";
            task.DownloadState = Request.QueryString["downloadState"] ?? "";
            string id = Request.QueryString["id"] ?? "";
            string pathID = Request.QueryString["pathId"] ?? "";
            string originPositionID = Request.QueryString["originPositionId"] ?? "";
            string targetPositionID = Request.QueryString["targetPositionId"] ?? "";
            string currentPositionID = Request.QueryString["currentPositionId"] ?? "";
            string allotID = Request.QueryString["allotId"] ?? "";
            if (id != null && id != "")
                task.ID = Convert.ToInt32(id);
            if (pathID != null && pathID != "") 
                task.PathID = Convert.ToInt32(pathID);
            if (originPositionID != null && originPositionID != "") 
                task.OriginPositionID = Convert.ToInt32(originPositionID);
            if (targetPositionID != null && targetPositionID != "") 
                task.TargetPositionID = Convert.ToInt32(targetPositionID);
            if (currentPositionID != null && currentPositionID != "") 
                task.CurrentPositionID = Convert.ToInt32(currentPositionID);
            if (allotID != null && allotID != "")
                task.AllotID = Convert.ToInt32(allotID);
            THOK.Common.NPOI.Models.ExportParam ep = new THOK.Common.NPOI.Models.ExportParam();
            ep.FirstTable = TaskService.DetailsTable(page, rows, task);
            ep.FirstHeadTitle = "任务作业";
            return THOK.Common.NPOI.Service.PrintService.Print(ep);
        }
    }
}
