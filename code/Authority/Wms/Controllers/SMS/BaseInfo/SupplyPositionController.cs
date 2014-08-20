using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using THOK.SMS.Bll.Interfaces;
using THOK.Common.WebUtil;
using Microsoft.Practices.Unity;

namespace Wms.Controllers.SMS.BaseInfo
{
    public class SupplyPositionController : Controller
    {
        //
        // GET: /SupplyPosition/

        [Dependency]
        public ISupplyPositionService SupplyPositionService { get; set; }

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
            SupplyPosition supplyPosition = new SupplyPosition();
            supplyPosition.PositionName = collection["PositionName"] ?? "";
            supplyPosition.PositionType = collection["PositionType"] ?? "";
            supplyPosition.ProductCode = collection["ProductCode"] ?? "";
            supplyPosition.ProductName = collection["ProductName"] ?? "";
            object data = SupplyPositionService.GetDetails(page, rows, supplyPosition);
            return Json(data, "text", JsonRequestBehavior.AllowGet);
        }

        // GET: /SupplyPosition/Add/
        public ActionResult Add(SupplyPosition supplyPosition)
        {
            string strResult = null;
            bool bResult = SupplyPositionService.Add(supplyPosition, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        // GET: /SupplyPosition/Save/
        public ActionResult Save(SupplyPosition supplyPosition)
        {
            string strResult = null;
            bool bResult = SupplyPositionService.Save(supplyPosition, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        // GET: /SupplyPosition/Delete/
        public ActionResult Delete(int id)
        {
            string strResult = null;
            bool bResult = SupplyPositionService.Delete(id, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }
    }
}
