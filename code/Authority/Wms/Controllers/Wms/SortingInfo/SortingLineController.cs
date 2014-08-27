using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.Common.WebUtil;
using THOK.Security;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;

namespace Authority.Controllers.Wms.SortingInfo
{
    [TokenAclAuthorize]
    public class SortingLineController : Controller
    {
        [Dependency]
        public ISortingLineService SortingLineService { get; set; }
        //
        // GET: /SortingLine/
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
        // GET: /SortingLine/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string sortingLineCode = collection["sortingLineCode"] ?? "";
            string sortingLineName = collection["sortingLineName"] ?? "";
            string SortingLineType = collection["SortingLineType"] ?? "";
            string productType = collection["ProductType"] ?? "";
            string IsActive = collection["IsActive"] ?? "";
            var sortingLine = SortingLineService.GetDetails(page, rows, sortingLineCode, sortingLineName, productType, SortingLineType, IsActive);
            return Json(sortingLine, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailsForSort(int page, int rows)
        {
            var sortingLine = SortingLineService.GetDetailsForSort(page, rows);
            return Json(sortingLine, "text", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /SortingLine/GetSortLine/
        public ActionResult GetSortLine()
        {
            var sortOrder = SortingLineService.GetSortLine();
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SortingLine/Create/
        public ActionResult Create(SortingLine sortLine)
        {
            bool bResult = SortingLineService.Add(sortLine);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SortingLine/Edit/
        public ActionResult Edit(SortingLine sortLine)
        {
            bool bResult = SortingLineService.Save(sortLine);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SortingLine/Delete/
        public ActionResult Delete(string sortLineCode)
        {
            bool bResult = SortingLineService.Delete(sortLineCode);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        #region /SortingLine/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;          
            string sortingLineCode = Request.QueryString["sortingLineCode"] ?? "";
            string sortingLineName = Request.QueryString["sortingLineName"] ?? "";
            string SortingLineType = Request.QueryString["SortingLineType"] ?? "";
            string productType = Request.QueryString["ProductType"] ?? "";
            string IsActive = Request.QueryString["IsActive"] ?? "";

            ExportParam ep = new ExportParam();
            ep.FirstTable = SortingLineService.GetSortingLine(page, rows, sortingLineCode, sortingLineName, productType, SortingLineType, IsActive);
            ep.HeadTitle1 = "分拣设备信息";
            return PrintService.Print(ep);
        }
        #endregion
    }
}
