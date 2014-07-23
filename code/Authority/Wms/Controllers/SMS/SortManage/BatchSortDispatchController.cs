using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using THOK.SMS.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Common.WebUtil;

namespace Wms.Controllers.SMS.SortManage
{
    public class SortBatchDispatchController : Controller
    {
        [Dependency]
        public ISortBatchService SortBatchService { get; set; }
        //
        // GET: /SortBatchDispatch/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasOptimize = true;
            ViewBag.hasUpload = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public ActionResult Details(int page, int rows, SortBatch sortBatch)
        {
            sortBatch.SortingLineCode = sortBatch.SortingLineCode ?? "";
            sortBatch.Status = sortBatch.Status ?? "";
            var sortBatchDetails = SortBatchService.GetDetails(page, rows, sortBatch);
            return Json(sortBatchDetails, "text", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(string dispatchId)
        {
            string strResult = string.Empty;
            bool bResult = SortBatchService.Add(dispatchId, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(SortBatch sortBatch, string IsRemoveOptimization)
        {
            string strResult = string.Empty;
            bool bResult = SortBatchService.Edit(sortBatch,IsRemoveOptimization, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {
            string strResult = string.Empty;
            bool bResult = SortBatchService.Delete(id, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Optimize(string id)
        {
            string strResult = string.Empty;
            bool bResult = SortBatchService.Optimize(id, out strResult);
            string msg = bResult ? "优化成功" : "优化失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //上传一号工程
        public ActionResult UpLoad(SortBatch sortbatch)
        {
            string strResult = string.Empty;
            bool bResult = SortBatchService.UpLoad(sortbatch, out strResult);
            string msg = bResult ? "上传成功" : "上传失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }
    }
}
