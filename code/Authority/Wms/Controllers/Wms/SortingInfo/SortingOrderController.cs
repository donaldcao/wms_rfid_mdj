using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Common.WebUtil;
using THOK.Authority.Bll.Interfaces;
using THOK.Security;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;
using THOK.Wms.DbModel;

namespace Authority.Controllers.Wms.SortingInfo
{
    [TokenAclAuthorize]
    public class SortingOrderController : Controller
    {
        [Dependency]
        public ISortOrderService SortOrderService { get; set; }
        [Dependency]
        public ISortOrderDetailService SortOrderDetailService { get; set; }
        [Dependency]
        public IDeliverLineService DeliverLineService { get; set; }
        [Dependency]
        public ICustomerService CustomerService { get; set; }
        [Dependency]
        public ISystemParameterService SystemParameterService { get; set; }

        //[Dependency]
        //public ISortOrderDownService SortOrderDownService { get; set; }
        //
        // GET: /SortingOrder/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasDownload = true;
            ViewBag.hasEdit = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        //查询主单
        // GET: /SortingOrder/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string OrderID = collection["OrderID"] ?? "";
            string orderDate = collection["orderDate"] ?? "";
            string productCode = collection["productCode"] ?? "";
            var sortOrder = SortOrderService.GetDetails(page, rows, OrderID, orderDate,productCode);
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //修改主单
        [HttpPost]
        public ActionResult SortOrderEdit(SortOrder sortOrder)
        {
            string strResult = string.Empty;
            bool bResult = SortOrderService.Save(sortOrder, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //查询细单
        // GET: /SortingOrder/sortOrderDetails/
        public ActionResult sortOrderDetails(int page, int rows, string OrderID)
        {
            var SortOrderDetail = SortOrderDetailService.GetDetails(page, rows, OrderID);
            return Json(SortOrderDetail, "text", JsonRequestBehavior.AllowGet);
        }
        //修改细单
        [HttpPost]
        public ActionResult SortOrderDetailEdit(SortOrderDetail sortOrderDetail)
        {
            string strResult = string.Empty;
            bool bResult = SortOrderDetailService.Save(sortOrderDetail, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }


        //根据时间分组查询主单
        // GET: /SortingOrder/GetOrderMaster/
        public ActionResult GetOrderMaster(string orderDate)
        {
            var sortOrder = SortOrderService.GetDetails(orderDate);
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        #region /SortingOrder/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string orderID = Request.QueryString["OrderID"];
            string orderDate = Request.QueryString["OrderDate"];
            string productCode = Request.QueryString["productCode"];
        
            ExportParam ep = new ExportParam();
            ep.DT1 = SortOrderService.GetSortOrder(page, rows, orderID, orderDate, productCode);
            ep.HeadTitle1 = "分拣订单管理";
            return PrintService.Print(ep);
        }
        #endregion
    }
}
