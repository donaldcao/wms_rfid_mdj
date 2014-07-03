using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.Security;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Common.WebUtil;

namespace Wms.Controllers.SMS.BaseInfo
{
    [TokenAclAuthorize]
    public class ProductInfoController : Controller
    {
        //
        // GET: /ProductInfo/
        [Dependency]
        public IProductService ProductService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasEdit = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string productName = collection["ProductName"] ?? "";
            string productCode = collection["ProductCode"] ?? "";
            string barCode = collection["BarCode"] ?? "";
            string isAbnormity = collection["IsAbnormity"] ?? "";
            var users = ProductService.GetDetails(page, rows, productCode, productName, barCode, isAbnormity);
            return Json(users, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string ProductCode, string PieceBarcode, string IsAbnormity)
        {
            bool bResult = ProductService.Change(ProductCode, PieceBarcode, IsAbnormity);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

     
        public FileStreamResult CreateExcelToClient()
        {
            //int page = 0, rows = 0;
            //string productName = Request.QueryString["productName"];
            //string productCode = Request.QueryString["productCode"];
            //string customCode = Request.QueryString["customCode"];
            //string brandCode = Request.QueryString["brandCode"];
            //string uniformCode = Request.QueryString["uniformCode"];
            //string abcTypeCode = Request.QueryString["abcTypeCode"];
            //string shortCode = Request.QueryString["shortCode"];
            //string priceLevelCode = Request.QueryString["priceLevelCode"];
            //string supplierCode = Request.QueryString["supplierCode"];

            //ExportParam ep = new ExportParam();
            //ep.DT1 = ProductService.GetProduct(page, rows, productName, productCode, customCode, brandCode, uniformCode, abcTypeCode, shortCode, priceLevelCode, supplierCode);
            //ep.HeadTitle1 = "卷烟信息";
            //return PrintService.Print(ep);
            return null;
        }
    }
}
