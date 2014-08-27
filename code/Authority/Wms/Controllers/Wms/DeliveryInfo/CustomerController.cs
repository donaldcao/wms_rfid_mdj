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

namespace Wms.Controllers.Wms.DeliveryInfo
{
    [TokenAclAuthorize]
    public class CustomerController : Controller
    {
        [Dependency]
        public ICustomerService CustomerService { get; set; }
        //
        // GET: /Customer/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        // GET: /Customer/Details/

        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string customerCode = collection["CustomerCode"] ?? "";
            string customerName = collection["CustomerName"] ?? "";
            string deliverLineCode = collection["DeliverLineCode"] ?? "";
            var users = CustomerService.GetDetails(page, rows, customerCode, customerName, deliverLineCode);
            return Json(users, "text", JsonRequestBehavior.AllowGet);
        }
        // POST: /Customer/Create
        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            string strResult = string.Empty;
            bool bResult = CustomerService.Add(customer, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Customer/C_Details/

        public ActionResult C_Details(int page, int rows, string QueryString, string Value)
        {
            if (QueryString == null)
            {
                QueryString = "CustomerCode";
            }
            if (Value == null)
            {
                Value = "";
            }
            var product = CustomerService.C_Details(page, rows, QueryString, Value);
            return Json(product, "text", JsonRequestBehavior.AllowGet);
        }
        // POST: /Customer/Edit/
        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            string strResult = string.Empty;
            bool bResult = CustomerService.Save(customer, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Customer/Delete/

        [HttpPost]
        public ActionResult Delete(string CustomerCode)
        {
            bool bResult = CustomerService.Delete(CustomerCode);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }


        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string CustomerCode = Request.QueryString["CustomerCode"];
            string CustomerName = Request.QueryString["CustomerName"];
            string DeliverLineCode = Request.QueryString["DeliverLineCode"];

            ExportParam ep = new ExportParam();
            ep.FirstTable = CustomerService.GetCustomerInfo(page, rows, CustomerCode, CustomerName, DeliverLineCode);
            ep.HeadTitle1 = "客户信息";

            return PrintService.Print(ep);
        }
    }
}
