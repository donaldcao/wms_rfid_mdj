﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Common.WebUtil;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.Security;
using THOK.Common.NPOI.Service;
using THOK.Common.NPOI.Models;

namespace Wms.Controllers.Wms.WarehouseInfo
{
    [TokenAclAuthorize]
    public class DefaultProductSetController : Controller
    {
        [Dependency]
        public IProductService ProductService { get; set; }
        [Dependency]
        public ICellService CellService { get; set; }
        
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

        //根据条件查找卷烟信息
        //POST: /DefaultProductSet/GetProductBy/
        public ActionResult GetProductBy(int page, int rows, string QueryString, string Value)
        {
            if (QueryString == null)
            {
                QueryString = "ProductCode";
            }
            if (Value == null)
            {
                Value = "";
            }
            var product = ProductService.GetProductBy(page, rows, QueryString, Value);
            return Json(product, "text", JsonRequestBehavior.AllowGet);
        }
        //根据条件查询卷烟货位
        //POST: /DefaultProductSet/GetCellBy/
        public ActionResult GetCellBy(int page ,int rows,string QueryString ,string Value)
        {
            if (QueryString == null)
            {
                QueryString = "ProductCode";
            }
            if (Value == null)
            {
                Value = "";
            }
            var cell = CellService.GetCellBy(page, rows, QueryString, Value);
            return Json(cell, "text", JsonRequestBehavior.AllowGet);
        }

        //加载烟卷信息表
        // POST: /DefaultProductSet/LoadProduct/
        public ActionResult LoadProduct(int page, int rows)
        {
            var product = ProductService.LoadProduct(page, rows);
            return Json(product, "text", JsonRequestBehavior.AllowGet);
        }

        //首页加载卷烟信息
        //POST: /DefaultProductSet/GetProductCell/
        public ActionResult GetProductCell(int page,int rows)
        {
            var product = CellService.GetCellInfo(page,rows);
            return Json(product, "text", JsonRequestBehavior.AllowGet);
        }

        //查找卷烟信息
        //POST: /DefaultProductSet/SearchProductCell/
        public ActionResult SearchProductCell(string productCode)
        {
            var product = CellService.GetCellInfo(productCode);
            return Json(product, "text", JsonRequestBehavior.AllowGet);
        }
        
        //添加货位预设编码
        // POST: /DefaultProductSet/CellInsertCode/
        public ActionResult CellInsertCode(string wareCodes, string areaCodes, string shelfCodes, string cellCodes, string defaultProductCode, string editType)
        {
            bool bResult = CellService.SaveCell(wareCodes, areaCodes, shelfCodes, cellCodes, defaultProductCode, editType);
            string msg = bResult ? "保存成功" : "保存失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        //获得货位勾选状态
        // GET: /DefaultProductSet/CellCodeSet/
        public ActionResult CellCodeSet(string productId)
        {
            var wareCell = CellService.GetCellCheck(productId);
            string msg = wareCell != null ? "" : "读取仓库信息失败";
            return Json(JsonMessageHelper.getJsonMessage(wareCell != null, msg, wareCell), "text", JsonRequestBehavior.AllowGet);
        }

        //删除货位信息
        //POST: /DefaultProductSet/CellDel/
        public ActionResult CellDel(string productCodes)
        {
            bool bResult = CellService.DeleteCell(productCodes);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        #region /DefaultProductSet/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string queryString = Request.QueryString["queryString"];
            string value = Request.QueryString["value"];
            
            ExportParam ep = new ExportParam();
            ep.FirstTable = CellService.GetCellByE(page, rows, queryString, value);
            ep.HeadTitle1 = "储位卷烟预设";
            return PrintService.Print(ep);
        } 
        #endregion
    }
}
