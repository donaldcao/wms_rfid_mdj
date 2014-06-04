using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.SMS.Optimize.Interfaces;
using THOK.Wms.DbModel;
using THOK.Common.WebUtil;
using THOK.WMS.DownloadWms.Bll;
using THOK.Wms.DownloadWms.Bll;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
using THOK.Wms.Dal.Interfaces;

namespace Wms.Controllers.SMS
{
    public class SortAllotController : Controller
    {
        [Dependency]
        public IDeliverLineOptimizeService DeliverLineOptimizeService { get; set; }

        [Dependency]
        public ISortOrderDownService SortOrderDownService { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }
        //
        // GET: /SortAllot/


        [Dependency]
        public ISystemParameterService SystemParameterService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAllot = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.hasPrint = true;
            ViewBag.hasDownload = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public ActionResult OrderInformation()
        {
            var orderInfo = DeliverLineOptimizeService.GetOrderInfo();
            return Json(orderInfo, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDeliverLine(int page, int rows, string orderDate)
        {
            if (orderDate == null)
            {
                return null;
            }
            var deliverLineDetail = DeliverLineOptimizeService.GetDeliverLine(orderDate);
            return Json(deliverLineDetail, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnAllotDeliverLine(int page, int rows, string orderDate)
        {
            if (orderDate == null)
            {
                return null;
            }
            var unAllotDeliverLine = DeliverLineOptimizeService.GetUnAllotDeliverLine(orderDate);
            return Json(unAllotDeliverLine, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditDeliverLine(DeliverLine deliverLine)
        {
            string strResult = string.Empty;
            bool bResult = DeliverLineOptimizeService.EditDeliverLine(deliverLine, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateDeliverLineAllot(string orderDate, string deliverLineCodes)
        {
            string strResult = string.Empty;
            bool bResult = DeliverLineOptimizeService.UpdateDeliverLineAllot(orderDate, deliverLineCodes, this.User.Identity.Name.ToString(), out strResult);
            string msg = bResult ? "分配成功" : "分配失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }


        //判断是否仓储分拣一体化
        public ActionResult IsWareHouseSorting()
        {
            string strResult = string.Empty;
            bool bResult = SortOrderDownService.IsWarehousSortIntegration(out strResult);
            string msg = bResult ? "请从仓储自动化管理系统下载数据" : "";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
       }

        //判断是否已下载
        public ActionResult DownLoad(string beginDate, string endDate)
        {
            string strResult = string.Empty;
            bool bResult = SortOrderDownService.DownLoad(beginDate, endDate, out strResult);
            string msg = "已下载数据是否继续下载";           
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);          
        }

        //下载数据
        public ActionResult DownDataSource(string beginDate, string endDate)
        {
            string errorInfo = string.Empty;

            bool bResult = SortOrderDownService.DownSortOrder(beginDate, endDate);

            string msg = bResult ? "下载成功" : "下载失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, errorInfo), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateDeliverLineAllot2(string orderDate, string deliverLineCodes,string sortingLineCode)
        {
            string strResult = string.Empty;
            bool bResult = DeliverLineOptimizeService.UpdateDeliverLineAllot(orderDate, deliverLineCodes, sortingLineCode, this.User.Identity.Name.ToString(), out strResult);
            string msg = bResult ? "分配成功" : "分配失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }
    }
}
