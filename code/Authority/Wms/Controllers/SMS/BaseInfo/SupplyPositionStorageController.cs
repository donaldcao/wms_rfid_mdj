using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using THOK.SMS.Bll.Interfaces;
using THOK.Common.WebUtil;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;
using Microsoft.Practices.Unity;

namespace Wms.Controllers.SMS.BaseInfo
{
    public class SupplyPositionStorageController : Controller
    {
        //
        // GET: /SupplyPositionStorage/

        [Dependency]
        public ISupplyPositionStorageService SupplyPositionStorageService { get; set; }

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

        // GET: /SupplyPosition/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            SupplyPositionStorage entity = new SupplyPositionStorage();
            entity.ProductCode = collection["ProductCode"] ?? "";
            entity.ProductName = collection["ProductName"] ?? "";
            object data = SupplyPositionStorageService.GetDetails(page, rows, entity);
            return Json(data, "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /SupplyPosition/Add/
        public ActionResult Add(SupplyPositionStorage entity)
        {
            string strResult = null;
            bool bResult = SupplyPositionStorageService.Add(entity, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /SupplyPosition/Save/
        public ActionResult Save(SupplyPositionStorage entity)
        {
            string strResult = null;
            bool bResult = SupplyPositionStorageService.Save(entity, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /SupplyPosition/Delete/
        public ActionResult Delete(int id)
        {
            string strResult = null;
            bool bResult = SupplyPositionStorageService.Delete(id, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        // GET: /SupplyPosition/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            SupplyPosition entity = new SupplyPosition();
            entity.ProductCode = Request.QueryString["productCode"];
            entity.ProductName = Request.QueryString["productName"];
            ExportParam ep = new ExportParam();
            ep.FirstTable = SupplyPositionStorageService.GetTable(page, rows, entity);
            ep.HeadTitle1 = "拆盘位置库存";
            return PrintService.Print(ep);
        }
    }
}
