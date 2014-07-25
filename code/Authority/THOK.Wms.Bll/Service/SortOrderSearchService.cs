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
    public class SortOrderSearchService : ServiceBase<SortOrder>, ISortOrderSearchService
    {
        [Dependency]
        public ISortOrderSearchRepository SortOrderSearchRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISortOrderSearch 成员

        public object GetDetails(int page, int rows, string orderID, string orderDate, string customerCode, string customerName, string deliverLineCode)
        {

            var sortOrderQuery = SortOrderSearchRepository.GetQueryable();

            if (orderDate != string.Empty && orderDate != null) {

                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
                sortOrderQuery=sortOrderQuery.Where(a => a.OrderDate == orderDate);
            }
            if (orderID != string.Empty && orderID != null)
            {
                sortOrderQuery=sortOrderQuery.Where(a => a.OrderID.Contains(orderID));
            }
            if (customerCode != string.Empty && customerCode != null)
            {
                sortOrderQuery=sortOrderQuery.Where(a => a.CustomerCode.Contains(customerCode));
            }
            if (customerName != string.Empty && customerName != null)
            {
                sortOrderQuery=sortOrderQuery.Where(a => a.CustomerName.Contains(customerName));
            }
            if (deliverLineCode != string.Empty && deliverLineCode != null)
            {
                sortOrderQuery=sortOrderQuery.Where(a => a.DeliverLineCode.Contains(deliverLineCode));
            }

            var temp = sortOrderQuery.OrderByDescending(a => a.OrderID).AsEnumerable().Select(s => new
            {
                s.OrderID,
                OrderDate = Convert.ToInt32(s.OrderDate) / 10000 + "-" + (Convert.ToInt32(s.OrderDate) % 10000 / 100 > 10 ? "" : "0") + Convert.ToInt32(s.OrderDate) % 10000 / 100 + "-" + (Convert.ToInt32(s.OrderDate) % 100 > 10 ? "" : "0") + Convert.ToInt32(s.OrderDate) % 100,
                OrderType = s.OrderType == "1" ? "普通客户" : "大客户",
                s.CustomerCode,
                s.CustomerName,
                s.QuantitySum,
                s.DetailNum,
                s.DeliverLine.DeliverLineName
            });

            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);

            return new { total, rows = temp.ToArray() };
            //if (orderDate == string.Empty || orderDate == null)
            //{
            //    orderDate = DateTime.Now.ToString("yyyyMMdd"); 
            //}
            //else
            //{
            //    orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd"); 
            //}
            //IQueryable<SortOrder> SortOrderQuery = SortOrderSearchRepository.GetQueryable();
            //var SortOrderSearch = SortOrderQuery.Where(i => i.OrderDate.Contains(orderDate)
            //                                        && i.OrderID.Contains(orderID)
            //                                        && i.CustomerCode.Contains(customerCode)
            //                                        && i.CustomerName.Contains(customerName)
            //                                        && i.DeliverLineCode.Contains(deliverLineCode))
            //                                        .OrderBy(i => i.OrderID);

            //int total = SortOrderSearch.Count();
            //var SortOrder = SortOrderSearch.Skip((page - 1) * rows).Take(rows);
            //var sortOrder = SortOrder.ToArray().Select(s => new
            //{
            //    s.OrderID,
            //    OrderDate = Convert.ToInt32(s.OrderDate) / 10000 + "-" + (Convert.ToInt32(s.OrderDate) % 10000 / 100 > 10 ? "" : "0") + Convert.ToInt32(s.OrderDate) % 10000 / 100 + "-" + (Convert.ToInt32(s.OrderDate) % 100 > 10 ? "" : "0") + Convert.ToInt32(s.OrderDate) % 100,
            //    OrderType = s.OrderType =="1" ? "普通客户": "大客户", 
            //    s.CustomerCode,
            //    s.CustomerName,
            //    s.QuantitySum,
            //    s.DetailNum,
            //    s.DeliverLine.DeliverLineName
            //});
            //return new { total, rows = sortOrder.ToArray() };
        }

        #endregion

    }
}
