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
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public ActionResult Details(int page, int rows, SortBatch sortBatch,string sortingLineName,string sortingLineType)
        {
            sortBatch.SortingLineCode = sortBatch.SortingLineCode ?? "";
            sortBatch.Status = sortBatch.Status ?? "";
            sortingLineName = sortingLineName ?? "";
            sortingLineType = sortingLineType ?? "";
            var sortBatchDetails = SortBatchService.GetDetails(page, rows, sortBatch, sortingLineName, sortingLineType);
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
    }
}
