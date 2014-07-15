﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class SortOrderDetailService : ServiceBase<SortOrderDetail>, ISortOrderDetailService
    {
        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }
        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISortOrderDetailService 成员

        public object GetDetails(int page, int rows, string OrderID)
        {
            if (OrderID != string.Empty && OrderID != null)
            {
                var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
                var outBillDetail = sortOrderDetailQuery.Where(i => i.OrderID.Contains(OrderID)).OrderBy(i => i.OrderID).AsEnumerable().Select(i => new
                {
                    i.OrderDetailID,
                    i.OrderID,
                    i.Product.ProductCode,
                    i.Product.ProductName,
                    i.UnitCode,
                    i.UnitName,
                    i.DemandQuantity,
                    i.RealQuantity,
                    i.Price,
                    i.Amount,
                    i.UnitQuantity,
                    i.SortQuantity
                });
                int total = outBillDetail.Count();
                outBillDetail = outBillDetail.Skip((page - 1) * rows).Take(rows);
                return new { total, rows = outBillDetail.ToArray() };
            }
            return "";
        }

        #endregion

        public System.Data.DataTable GetSortOrderDetail(int page, int rows, string OrderID)
        {
            if (OrderID != string.Empty && OrderID != null)
            {
                var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
                var outBillDetail = sortOrderDetailQuery.Where(i => i.OrderID.Contains(OrderID)).OrderBy(i => i.OrderID).AsEnumerable().Select(i => new
                {
                    i.OrderDetailID,
                    i.OrderID,
                    i.Product.ProductCode,
                    i.Product.ProductName,
                    i.UnitCode,
                    i.UnitName,
                    i.DemandQuantity,
                    i.RealQuantity,
                    i.Price,
                    i.Amount,
                    i.UnitQuantity
                });
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("订单明细编码", typeof(string));
                dt.Columns.Add("订单编码", typeof(string));
                dt.Columns.Add("商品编码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("单位编码", typeof(string));
                dt.Columns.Add("单位名称", typeof(string));
                dt.Columns.Add("数量", typeof(string));
                dt.Columns.Add("单价", typeof(string));
                dt.Columns.Add("金额", typeof(string));
                foreach (var item in outBillDetail)
                {
                    dt.Rows.Add
                        (
                            item.OrderDetailID,
                            item.OrderID,
                            item.ProductCode,
                            item.ProductName,
                            item.UnitCode,
                            item.UnitName,
                            item.RealQuantity,
                            item.Price,
                            item.Amount
                        );
                }
                return dt;
            }
            return null;
        }

        public bool Save(SortOrderDetail sortOrderDetail, out string strResult)
        {
            strResult = string.Empty;        
            var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable().FirstOrDefault(a => a.OrderDetailID == sortOrderDetail.OrderDetailID);
           
            int sum=Convert.ToInt32(sortOrderDetail.RealQuantity)-Convert.ToInt32(sortOrderDetail.SortQuantity);

            if (sortOrderDetailQuery != null&&sum>=0)
            {
                try
                {                    
                        sortOrderDetailQuery.SortQuantity = sortOrderDetail.SortQuantity;
                        SortOrderDetailRepository.SaveChanges();                
                        return true;
                }
                catch (Exception ex)
                {
                   
                    strResult = "修改失败，原因：" + ex.Message;
                    return false;
                }
            }
            else
            {
                strResult = "保存失败，分拣数量不能大于实际数量";
                return false;
            }
          
        }
    }
}
