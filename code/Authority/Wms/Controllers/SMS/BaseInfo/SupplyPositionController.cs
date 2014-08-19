﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using THOK.SMS.Bll.Interfaces;
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
        public JsonResult Details(int page, int rows, SupplyPosition supplyPosition)
        {
            supplyPosition.PositionName = supplyPosition.ProductName ?? "";
            supplyPosition.PositionType = supplyPosition.PositionType ?? "";
            supplyPosition.ProductCode = supplyPosition.ProductCode ?? "";
            supplyPosition.ProductName = supplyPosition.ProductName ?? "";
            object data = SupplyPositionService.GetDetails(page, rows, supplyPosition);
            return Json(data, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
