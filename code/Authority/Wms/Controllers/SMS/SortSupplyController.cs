using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using Microsoft.Practices.Unity;
using THOK.SMS.Bll.Interfaces;
using THOK.Common.WebUtil;
using THOK.Security;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;

namespace Wms.Controllers.SMS
{
    public class SortSupplyController : Controller
    {
        [Dependency]
        public ISortSupplyService SortSupplyServer { get; set; }
        //
        // GET: /SortSupply/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }


    }
}
