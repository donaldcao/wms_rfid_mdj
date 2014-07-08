using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;
using THOK.Common.WebUtil;
using THOK.Security;
using THOK.SMS.Bll.Interfaces;
using THOK.SMS.DbModel;

namespace Wms.Controllers.SMS.BaseInfo
{
    [TokenAclAuthorize]
    public class ChannelInfoController : Controller
    {
        [Dependency]
        public IChannelService ChannelService { get; set; }
        //
        // GET: /ChannelInfo/

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

        public JsonResult Details(int page, int rows,Channel channel)
        {
            channel.ProductCode = channel.ProductCode ?? "";
            channel.SortingLineCode = channel.SortingLineCode ?? "";
            channel.ChannelType = channel.ChannelType ?? "";
            channel.IsActive = channel.IsActive ?? "";
            var channelDetails = ChannelService.GetDetails(page, rows, channel);
            return Json(channelDetails, "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /ChannelInfo/Create
        [HttpPost]
        public ActionResult Create(Channel channel)
        {
            string strResult = string.Empty;
            bool bResult = ChannelService.Add(channel, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /ChannelInfo/Edit/
        public ActionResult Edit(Channel channel)
        {
            string strResult = string.Empty;
            bool bResult = ChannelService.Edit(channel, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /ChannelInfo/Delete/
        [HttpPost]
        public ActionResult Delete(string channelCode)
        {
            string strResult = string.Empty;
            bool bResult = ChannelService.Delete(channelCode, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //打印
        public FileStreamResult CreateExcelToClient()
        {

            int page=0,rows=0;
            string DefaultProductCode = Request.QueryString["DefaultProductCode"];
            string SortingLineCode = Request.QueryString["SortingLineCode"];
            string ChannelType = Request.QueryString["ChannelType"];
            string GroupNo = Request.QueryString["GroupNo"];
            string Status = Request.QueryString["Status"];



            ExportParam ep = new ExportParam();
            ep.DT1 = ChannelService.GetChannel(page, rows, DefaultProductCode, SortingLineCode, ChannelType, GroupNo, Status);
            ep.HeadTitle1 = "烟道信息";
            return PrintService.Print(ep);
        }
    }
}
