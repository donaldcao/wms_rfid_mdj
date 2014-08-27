using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Common.WebUtil;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;

namespace Wms.Controllers.WMS.StockBill
{
    public class StockBillTypeController : Controller
    {
        //
        // GET: /StockBillType/

        [Dependency]
        public IBillTypeService BillTypeService { get; set; }

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

        // GET: /StockBillType/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string billClass = collection["BillClass"] ?? "";
            string isActive = collection["IsActive"] ?? "";
            object data = BillTypeService.GetDetails(page, rows, billClass, isActive);
            return Json(data, "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /StockBillType/Create/
        public ActionResult Create(BillType billtype)
        {
            bool bResult = BillTypeService.Add(billtype);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /StockBillType/Edit/
        public ActionResult Edit(BillType billtype)
        {
            bool bResult = BillTypeService.Save(billtype);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /StockBillType/Delete/
        public ActionResult Delete(string billtypeCode)
        {
            bool bResult = BillTypeService.Delete(billtypeCode);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        // GET: /StockBillType/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string billClass = Request.QueryString["billClass"] ?? "";
            string isActive = Request.QueryString["isActive"] ?? "";
            ExportParam ep = new ExportParam();
            ep.FirstTable = BillTypeService.BillTypeTable(page, rows, billClass, isActive);
            ep.FirstHeadTitle = "订单类型";
            return PrintService.Print(ep);
        }
    }
}
