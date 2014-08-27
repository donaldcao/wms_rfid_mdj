using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Security;
using THOK.Common.WebUtil;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.Allot.Interfaces;

namespace Authority.Controllers.Wms.StockBill
{
    [TokenAclAuthorize]
    public class StockInBillAllotController : Controller
    {
        [Dependency]
        public IInBillAllotService InBillAllotService { get; set; }
        [Dependency]
        public IInBillDetailService InBillDetailService { get; set; }

        public ActionResult Search(string billNo, int page, int rows)
        {
            var result = InBillAllotService.Search(billNo, page, rows);
            return Json(result, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllotDelete(string billNo, long id)
        {
            string strResult = string.Empty;
            bool bResult = InBillAllotService.AllotDelete(billNo, id, out strResult);
            string msg = bResult ? "删除分配明细成功" : "删除分配明细失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllotEdit(string billNo, long id, string cellCode, decimal allotQuantity)
        {
            string strResult = string.Empty;
            bool bResult = InBillAllotService.AllotEdit(billNo, id, cellCode, allotQuantity, out strResult);
            string msg = bResult ? "修改分配成功" : "修改分配失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllotConfirm(string billNo)
        {
            string strResult = string.Empty;
            bool bResult = InBillAllotService.AllotConfirm(billNo, out strResult);
            string msg = bResult ? "确认分配成功" : "确认分配失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllotCancelConfirm(string billNo)
        {
            string strResult = string.Empty;
            bool bResult = InBillAllotService.AllotCancelConfirm(billNo, out strResult);
            string msg = bResult ? "取消分配确认成功" : "取消分配确认失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllotCancel(string billNo)
        {
            string strResult = string.Empty;
            bool bResult = InBillAllotService.AllotCancel(billNo, out strResult);
            string msg = bResult ? "取消分配成功" : "取消分配失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllotAdd(string billNo, long id, string cellCode, decimal allotQuantity)
        {
            string strResult = string.Empty;
            bool bResult = InBillAllotService.AllotAdd(billNo, id, cellCode, allotQuantity, out strResult);
            string msg = bResult ? "添加分配成功" : "添加分配失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllotAdds(string billNo, long id, string storageCode, string productname)
        {
            string strResult = string.Empty;
            decimal allotQuantity = 0;
            bool bResult = InBillAllotService.AllotAdd(billNo, id, storageCode, productname, out strResult, out allotQuantity);
            string msg = bResult ? "" : "添加分配失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult, allotQuantity), "text", JsonRequestBehavior.AllowGet);
        }
        #region /StockInBillAllot/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string billNo = Request.QueryString["billNo"];
            
            ExportParam ep = new ExportParam();
            ep.FirstTable = InBillDetailService.GetInBillDetail(page, rows, billNo);
            ep.SecondTable = InBillAllotService.AllotSearch(page, rows, billNo);;
            ep.FirstHeadTitle = "入库单据分配";
            ep.SecondHeadTitle = "入库单据分配明细";
            return PrintService.Print(ep);
        }
        #endregion
    }
}
