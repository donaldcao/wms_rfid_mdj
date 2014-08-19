using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class SortingLineService : ServiceBase<SortingLine>, ISortingLineService
    {
        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }
        [Dependency]
        public IBillTypeRepository BillTypeRepository { get; set; }

        
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISortingLineService 成员

        public string WhatStatus(string status)
        {
            string statusStr = "";
            switch (status)
            {
                case "1":
                    statusStr = "正常";
                    break;
                case "2":
                    statusStr = "异型";
                    break;
                case "3":
                    statusStr = "整件";
                    break;
                case "4":
                    statusStr = "手工";
                    break;
            }
            return statusStr;
        }

        public object GetDetails(int page, int rows, string sortingLineCode, string sortingLineName, string productType, string sortingLineType, string IsActive)
        {
            var sortLineQuery = SortingLineRepository.GetQueryable();
            var sortLine = sortLineQuery.Where(s => s.SortingLineCode == s.SortingLineCode);
            var billtypeQuery = BillTypeRepository.GetQueryable();
            
            if (sortingLineCode != string.Empty && sortingLineCode != null)
            {
                sortLine = sortLine.Where(s => s.SortingLineCode.Contains(sortingLineCode));
            }
            if (sortingLineName != string.Empty && sortingLineName != null)
            {
                sortLine = sortLine.Where(s => s.SortingLineName.Contains(sortingLineName));
            }
            if (productType != string.Empty && productType != null)
            {
                sortLine = sortLine.Where(s => s.ProductType.Contains(productType));
            }
            if (sortingLineType != string.Empty && sortingLineType != null)
            {
                sortLine = sortLine.Where(s => s.SortingLineType.Contains(sortingLineType));
            }
            if (IsActive != string.Empty && IsActive != null)
            {
                sortLine = sortLine.Where(s => s.IsActive == IsActive);
            }

            var sortingLine = sortLine.OrderBy(b => b.SortingLineCode).AsEnumerable().Select(b => new
            {
                b.SortingLineCode,
                b.SortingLineName,
                ProductType = WhatStatus(b.ProductType),
                SortingLineType = b.SortingLineType == "1" ? "半自动分拣线" : "全自动分拣线",
                OutBillTypeCode=billtypeQuery.Where(s=>s.BillTypeCode==b.OutBillTypeCode).FirstOrDefault().BillTypeName,
                MoveBillTypeCode = billtypeQuery.Where(s => s.BillTypeCode == b.MoveBillTypeCode).FirstOrDefault().BillTypeName,
                CellName = b.Cell != null ? b.Cell.CellName : "",
                b.CellCode,
                IsActive = b.IsActive == "1" ? "启用" : "不启用",
                UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });

            int total = sortingLine.Count();
            sortingLine = sortingLine.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = sortingLine.ToArray() };
        }

        public object GetDetailsForSort(int page,int rows)
        {
            IQueryable<SortingLine> sortLineQuery = SortingLineRepository.GetQueryable();
            int total = sortLineQuery.Count();
            var temp = sortLineQuery.Where(a => a.IsActive.Equals("1") && (new string[] { "1", "3" }).Contains(a.SortingLineType))
                .OrderBy(b => b.SortingLineCode)
                .Skip((page - 1) * rows).Take(rows)
                .Select(b => new
                {
                    b.SortingLineCode,
                    b.SortingLineName,
                    SortingLineType = b.SortingLineType == "1" ? "分拣线" : "整件线",
                    IsActive = b.IsActive == "1" ? "启用" : "不启用"
                }).ToArray();
            
            return new { total, rows = temp };
        }

        public new bool Add(SortingLine sortingLine)
        {
            var sortLine = new SortingLine();
            sortLine.SortingLineCode = sortingLine.SortingLineCode;
            sortLine.SortingLineName = sortingLine.SortingLineName;
            sortLine.SortingLineType = sortingLine.SortingLineType;
            sortLine.ProductType = sortingLine.ProductType;
            sortLine.OutBillTypeCode = sortingLine.OutBillTypeCode;
            sortLine.MoveBillTypeCode = sortingLine.MoveBillTypeCode;
            sortLine.CellCode = sortingLine.CellCode;
            sortLine.IsActive = sortingLine.IsActive;
            sortLine.UpdateTime = DateTime.Now;

            SortingLineRepository.Add(sortLine);
            SortingLineRepository.SaveChanges();
            return true;
        }

        public bool Delete(string sortingLineCode)
        {
            var sortLine = SortingLineRepository.GetQueryable()
                .FirstOrDefault(s => s.SortingLineCode == sortingLineCode);
            if (sortLine != null)
            {
                SortingLineRepository.Delete(sortLine);
                SortingLineRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(SortingLine sortingLine)
        {
            var sortLineSave = SortingLineRepository.GetQueryable().FirstOrDefault(s => s.SortingLineCode == sortingLine.SortingLineCode);
            sortLineSave.SortingLineName = sortingLine.SortingLineName;
            sortLineSave.SortingLineType = sortingLine.SortingLineType;
            sortLineSave.ProductType = sortingLine.ProductType;
            sortLineSave.OutBillTypeCode = sortingLine.OutBillTypeCode;
            sortLineSave.MoveBillTypeCode = sortingLine.MoveBillTypeCode;
            sortLineSave.CellCode = sortingLine.CellCode;
            sortLineSave.IsActive = sortingLine.IsActive;
            sortLineSave.UpdateTime = DateTime.Now;

            SortingLineRepository.SaveChanges();
            return true;
        }

        public object GetSortLine()
        {
            var temp = SortingLineRepository.GetQueryable().OrderBy(b => b.SortingLineCode).AsEnumerable().Select(b => new
            {
                b.SortingLineCode,
                b.SortingLineName,
                SortingLineType = b.SortingLineType == "1" ? "半自动分拣线" : "全自动分拣线",
                IsActive = b.IsActive == "1" ? "启用" : "不启用",
                UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            temp = temp.Where(t => t.IsActive == "启用");
            return temp.ToArray();
        }

        #endregion

        public System.Data.DataTable GetSortingLine(int page, int rows, string sortingLineCode, string sortingLineName, string productType, string sortingLineType, string IsActive)
        {
            var sortLineQuery = SortingLineRepository.GetQueryable();
            var billtypeQuery = BillTypeRepository.GetQueryable();
            var sortingLine = sortLineQuery.Where(a => a.SortingLineCode.Contains(sortingLineCode)&&a.SortingLineName.Contains(sortingLineName)&&a.ProductType.Contains(productType)&&a.SortingLineType.Contains(sortingLineType)&&a.IsActive.Contains(IsActive)).OrderBy(a => a.SortingLineCode).Select(a => a);
         
            var temp = sortingLine.ToArray().Select(b => new
            {
                b.SortingLineCode,
                b.SortingLineName,
                SortingLineType = b.SortingLineType == "1" ? "半自动分拣线" : "全自动分拣线",
                ProductType = WhatStatus(b.ProductType),
                OutBillTypeCode = billtypeQuery.Where(s => s.BillTypeCode == b.OutBillTypeCode).FirstOrDefault().BillTypeName,
                MoveBillTypeCode = billtypeQuery.Where(s => s.BillTypeCode == b.MoveBillTypeCode).FirstOrDefault().BillTypeName,
                //CellName = b.Cell != null ? b.Cell.CellName : "",
                //b.CellCode,
                IsActive = b.IsActive == "1" ? "启用" : "不启用",
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("分拣线编码", typeof(string));
            dt.Columns.Add("分拣线名称", typeof(string));
            dt.Columns.Add("商品类型", typeof(string));
            dt.Columns.Add("分拣线类型", typeof(string));
            dt.Columns.Add("出库单类型", typeof(string));
            dt.Columns.Add("移库单类型", typeof(string));         
            dt.Columns.Add("是否启用", typeof(string));
            foreach (var item in temp)
            {
                dt.Rows.Add(
                        item.SortingLineCode,
                        item.SortingLineName,
                        item.ProductType,
                        item.SortingLineType,
                        item.OutBillTypeCode,
                        item.MoveBillTypeCode,
                        item.IsActive );
            }
            return dt;
        }
    }
}
