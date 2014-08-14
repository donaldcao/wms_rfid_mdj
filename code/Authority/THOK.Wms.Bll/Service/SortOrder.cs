using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.Bll.Interfaces;
using System.Data;

namespace THOK.Wms.Bll.Service
{
    public class SortOrderService : ServiceBase<SortOrder>, ISortOrderService
    {
        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }
        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }
        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }
        [Dependency]
        public IProductRepository ProductRepository { get; set; }      
        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }
        [Dependency]
        public ISystemParameterService SystemParameterService { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISortOrderService 成员

        public object GetDetails(int page, int rows, string OrderID, string orderDate, string productCode)
        {
            var sortOrderQuery = SortOrderRepository.GetQueryable();
            var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
            if (productCode != string.Empty && productCode != null)
            {
                var sortOrderDetail = sortOrderDetailQuery.Where(s => s.ProductCode == productCode).ToArray().Select(a => a.OrderID );
                sortOrderQuery = sortOrderQuery.Where(s => sortOrderDetail.Contains(s.OrderID));             
            }

            if (orderDate != string.Empty && orderDate != null)
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
                sortOrderQuery = sortOrderQuery.Where(a => a.OrderDate == orderDate);
            }

            if (OrderID != string.Empty && OrderID != null)
            {
                sortOrderQuery = sortOrderQuery.Where(s => s.OrderID.Contains(OrderID));
            }

            var temp = sortOrderQuery.AsEnumerable().OrderBy(t => t.OrderID).Select(s => new
            {
                s.OrderID,
                s.CompanyCode,
                s.SaleRegionCode,
                s.OrderDate,
                OrderType = s.OrderType == "1" ? "普通客户" : "大客户",
                s.CustomerCode,
                s.CustomerName,
                s.QuantitySum,
                s.DeliverLineCode,
                s.AmountSum,
                s.DetailNum,
                s.DeliverOrder,
                s.DeliverDate,
                IsActive = s.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.Description
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
            //IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            //IQueryable<SortOrderDetail> sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
            //var sortOrderDetail = sortOrderDetailQuery.Where(s => s.OrderDetailID == s.OrderDetailID).Select(s => new { s.OrderID, s.Product, s.RealQuantity });
            //if (productCode != string.Empty && productCode != null)
            //{
            //    sortOrderDetail = sortOrderDetail.Where(s => s.Product.ProductCode == productCode && s.RealQuantity > 0);
            //}

            //var sortOrder = sortOrderQuery.Where(s => s.OrderDate == orderDate && sortOrderDetail.Any(d => d.OrderID == s.OrderID));

            //if (OrderID != string.Empty && OrderID != null)
            //{
            //    sortOrder = sortOrder.Where(s => s.OrderID == OrderID);
            //}

            //var temp = sortOrder.AsEnumerable().OrderBy(t => t.OrderID).Select(s => new
            //{
            //    s.OrderID,
            //    s.CompanyCode,
            //    s.SaleRegionCode,
            //    s.OrderDate,
            //    OrderType = s.OrderType == "1" ? "普通客户" : "大客户",
            //    s.CustomerCode,
            //    s.CustomerName,
            //    s.QuantitySum,
            //    s.DeliverLineCode,
            //    s.AmountSum,
            //    s.DetailNum,
            //    s.DeliverOrder,
            //    s.DeliverDate,
            //    IsActive = s.IsActive == "1" ? "可用" : "不可用",
            //    UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //    s.Description
            //});

            //int total = temp.Count();
            //temp = temp.Skip((page - 1) * rows).Take(rows);
            //return new { total, rows = temp.ToArray() };
        }


        public object GetDetails(string orderDate)
        {
            if (orderDate == string.Empty || orderDate == null)
            {
                orderDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
            }
            var sortOrderQuery = SortOrderRepository.GetQueryable();
            var SortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortorderDetail = SortOrderDetailRepository.GetQueryable();
            var sortorderDisp = SortOrderDispatchQuery.Where(s => s.OrderDate == orderDate);
            var sortOrder = sortOrderQuery.Where(s => s.OrderDate == orderDate && !sortorderDisp.Any(d => d.DeliverLineCode == s.DeliverLineCode))
                                          .Join(sortorderDetail,
                                          sm => new { sm.OrderID },
                                          sd => new { sd.OrderID },
                                          (sm, sd) => new { sm.OrderDate, sm.DeliverLine, sm.DetailNum, sm.AmountSum, sd.Amount, sd.RealQuantity })
                                          .GroupBy(s => new { s.OrderDate, s.DeliverLine })
                                          .Select(s => new
                                          {
                                              DeliverLineCode = s.Key.DeliverLine.DeliverLineCode,
                                              DeliverLineName = s.Key.DeliverLine.DeliverLineName,
                                              OrderDate = s.Key.OrderDate,
                                              QuantitySum = s.Sum(p => p.RealQuantity),
                                              AmountSum = s.Sum(p => p.Amount),
                                              DetailNum = s.Count(),
                                              IsActive = "可用"
                                          });

            return sortOrder.OrderBy(s => s.DeliverLineName).ToArray();
        }

        #endregion

        //修改主单
        public bool Save(SortOrder sortOrder, out string strResult)
        {
            strResult = string.Empty;
            return true;
            #region
            //var sort = SortOrderRepository.GetQueryable().FirstOrDefault(a => a.OrderID == sortOrder.OrderID);

            //var sortQuantity = SortOrderDetailRepository.GetQueryable().Where(b => b.OrderID == sortOrder.OrderID)
            //    .GroupBy(s => s.OrderID).Select(c => new
            //                              {
            //                                  QuantitySum = c.Sum(p => p.SortQuantity)
            //                              }).ToArray();


            //if (sort != null && sortQuantity.Count() > 0)
            //{
            //    try
            //    {
            //      //  sort.CompanyCode = sort.CompanyCode;
            //      //  sort.CustomerCode = sort.CustomerCode;
            //      //  sort.CustomerName = sort.CustomerName;
            //      //  sort.DeliverDate = sort.DeliverDate;
            //      ////  sort.DeliverLine = sort.DeliverLine;
            //      //  sort.DeliverLineCode = sort.DeliverLineCode;
            //      //  sort.DeliverOrder = sort.DeliverOrder;
            //      //  sort.Description = sort.Description;
            //      //  sort.DetailNum = sort.DetailNum;
            //      //  sort.IsActive = sort.IsActive;
            //      //  sort.OrderDate = sort.OrderDate;
            //      //  sort.OrderType = sort.OrderType;
            //      //  sort.QuantitySum = sort.QuantitySum;
            //      //  sort.SaleRegionCode = sort.SaleRegionCode;
            //      //  sort.SortOrderDetails = sort.SortOrderDetails;
            //      //  sort.Status = sort.Status;
            //      //  sort.UpdateTime = sort.UpdateTime;
            //        sort.SortQuantitySum = sortQuantity[0].QuantitySum;
            //        SortOrderRepository.SaveChanges();
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        strResult = "修改失败，原因：" + ex.Message;
            //    }
            //    return false;
            //}
            //else
            //{
            //    strResult = "保存失败，未找到该条数据！";
            //    return false;
            //} 
            #endregion
        }


        //分拣订单管理打印
        public System.Data.DataTable GetSortOrder(int page, int rows, string OrderID, string orderDate, string ProductCode)
        {
           
                var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
                var sortOrderQuery = SortOrderRepository.GetQueryable();

                if (ProductCode != string.Empty && ProductCode != null)
                {                 
                    sortOrderDetailQuery = sortOrderDetailQuery.Where(s => s.ProductCode.Contains(ProductCode));
                }

                if (orderDate != string.Empty && orderDate != null)
                {
                    orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");                
                    sortOrderDetailQuery = sortOrderDetailQuery.Where(s => s.SortOrder.OrderDate == orderDate);
                }

                var outBillDetail = sortOrderDetailQuery.Where(i => i.OrderID.Contains(OrderID)).OrderBy(i => i.OrderID).AsEnumerable().Select(i => new
                {
                   
                    i.OrderID,
                    i.Product.ProductCode,
                    i.Product.ProductName,
                    i.UnitCode,
                    i.UnitName,
                    i.RealQuantity,
                    i.SortQuantity,
                    i.Price,
                    i.Amount
                });
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("订单编码", typeof(string));
                dt.Columns.Add("商品编码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("单位编码", typeof(string));
                dt.Columns.Add("单位名称", typeof(string));
                dt.Columns.Add("数量", typeof(string));
                dt.Columns.Add("分拣数量", typeof(string));
                dt.Columns.Add("单价", typeof(string));
                dt.Columns.Add("金额", typeof(string));
                foreach (var item in outBillDetail)
                {
                    dt.Rows.Add
                        (                            
                            item.OrderID,
                            item.ProductCode,
                            item.ProductName,
                            item.UnitCode,
                            item.UnitName,
                            item.RealQuantity,
                            item.SortQuantity,
                            item.Price,
                            item.Amount
                        );
                }
                return dt;
            }                
    }
}
