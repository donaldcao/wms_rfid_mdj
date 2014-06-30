using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using THOK.SMS.DbModel;
using THOK.Common.WebUtil;
using THOK.Security;
using Microsoft.Practices.Unity;
using THOK.SMS.Bll.Interfaces;

using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;

namespace Wms.Controllers.SMS
{
     [TokenAclAuthorize]
    public class SortBatchController : Controller
    {
        //
        // GET: /SortBatch/

        [Dependency]

        public ISortBatchService SortBatchService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            //ViewBag.hasAudit = true;
            //ViewBag.hasAntiTrial = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public ActionResult AddPage()
        {
            return View();
        }

        public ActionResult SearchPage()
        {
            return View();
        }

        public ActionResult Details(int page, int rows, string Status, string BatchNo, string BatchName, string OrderDate)
        {
            var srmDetail = SortBatchService.GetDetails(page, rows, Status, BatchNo, BatchName, OrderDate);
            return Json(srmDetail, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBatch(int page, int rows, string queryString, string value)
        {

            if (queryString == null)
            {
                queryString = "BatchNo";
            }
            if (value == null)
            {
                value = "";
            }
            var batch =SortBatchService.GetBatch(page, rows, queryString, value);
            return Json(batch, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(SortBatch SortBatch)
        {
            string strResult = string.Empty;
            bool bResult = SortBatchService.Add(SortBatch, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(SortBatch SortBatch)
        {
            string strResult = string.Empty;
            bool bResult = SortBatchService.Save(SortBatch, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int SortBatchId)
        {
            string strResult = string.Empty;
            bool bResult = SortBatchService.Delete(SortBatchId, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string SortBatchId = Request.QueryString["SortBatchId"];
          //  int SortBatchId = Convert.ToInt16(Id);
           
            ExportParam ep = new ExportParam();
            ep.DT1 = SortBatchService.GetSortBatch(page, rows, SortBatchId);
            ep.HeadTitle1 = "分拣状态";
           
            return PrintService.Print(ep);
        }
    }
}
