using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Download.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Wms.DownloadWms.Bll;
using THOK.WMS.DownloadWms.Bll;
using THOK.Authority.Bll.Interfaces;
using THOK.Wms.DownloadWms.Dao;
using THOK.WMS.DownloadWms.Dao;
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
        public ISortingDownService SortingDownService { get; set; }

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
            if (orderDate == string.Empty || orderDate == null)
            {
                orderDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
            }
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            IQueryable<SortOrderDetail> sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
            var sortOrderDetail = sortOrderDetailQuery.Where(s => s.OrderDetailID == s.OrderDetailID).Select(s => new { s.OrderID, s.Product, s.RealQuantity });
            if (productCode != string.Empty && productCode != null)
            {
                sortOrderDetail = sortOrderDetail.Where(s => s.Product.ProductCode == productCode && s.RealQuantity > 0);
            }

            var sortOrder = sortOrderQuery.Where(s => s.OrderDate == orderDate && sortOrderDetail.Any(d => d.OrderID == s.OrderID));

            if (OrderID != string.Empty && OrderID != null)
            {
                sortOrder = sortOrder.Where(s => s.OrderID == OrderID);
            }

            var temp = sortOrder.AsEnumerable().OrderBy(t => t.OrderID).Select(s => new
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
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            IQueryable<SortOrderDispatch> SortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
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

        public bool DownSortOrder(string beginDate, string endDate, out string errorInfo)
        {
            errorInfo = string.Empty;
            bool result = false;
            string sortOrderStrs = "";
            string sortOrderList = "";
            try
            {
                var sortOrderIds = SortOrderRepository.GetQueryable().Where(s => s.OrderID == s.OrderID).Select(s => new { s.OrderID }).ToArray();

                for (int i = 0; i < sortOrderIds.Length; i++)
                {
                    sortOrderStrs += sortOrderIds[i].OrderID + ",";
                }

                SortOrder[] SortOrders = SortingDownService.GetSortOrder(beginDate, endDate, sortOrderStrs);

                foreach (var item in SortOrders)
                {
                    var sortOrder = new SortOrder();
                    sortOrder.OrderID = item.OrderID;
                    sortOrder.CompanyCode = item.CompanyCode;
                    sortOrder.SaleRegionCode = item.SaleRegionCode;
                    sortOrder.OrderDate = item.OrderDate;
                    sortOrder.OrderType = item.OrderType;
                    sortOrder.CustomerCode = item.CustomerCode;
                    sortOrder.CustomerName = item.CustomerName;
                    sortOrder.DeliverLineCode = item.DeliverLineCode;
                    sortOrder.QuantitySum = item.QuantitySum;
                    sortOrder.AmountSum = item.AmountSum;
                    sortOrder.DetailNum = item.DetailNum;
                    sortOrder.DeliverOrder = item.DeliverOrder;
                    sortOrder.DeliverDate = item.DeliverDate;
                    sortOrder.Description = item.Description;
                    sortOrder.IsActive = item.IsActive;
                    sortOrder.UpdateTime = item.UpdateTime;
                    SortOrderRepository.Add(sortOrder);
                    sortOrderList += item.OrderID + ",";
                }
                if (sortOrderList != string.Empty)
                {
                    SortOrderDetail[] SortOrderDetails = null; //SortingDownService.GetSortOrderDetail(sortOrderList);
                    foreach (var detail in SortOrderDetails)
                    {
                        var sortOrderDetail = new SortOrderDetail();
                        var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == detail.ProductCode);
                        sortOrderDetail.OrderDetailID = detail.OrderDetailID;
                        sortOrderDetail.OrderID = detail.OrderID;
                        sortOrderDetail.ProductCode = detail.ProductCode;
                        sortOrderDetail.ProductName = detail.ProductName;
                        sortOrderDetail.UnitCode = detail.UnitCode;
                        sortOrderDetail.UnitName = detail.UnitName;
                        sortOrderDetail.DemandQuantity = detail.DemandQuantity * product.UnitList.Unit02.Count;
                        sortOrderDetail.RealQuantity = detail.RealQuantity * product.UnitList.Unit02.Count;
                        sortOrderDetail.Price = detail.Price;
                        sortOrderDetail.Amount = detail.Amount;
                        sortOrderDetail.UnitQuantity = product.UnitList.Quantity02;
                        SortOrderDetailRepository.Add(sortOrderDetail);
                    }
                }
                SortOrderRepository.SaveChanges();
                result = true;
            }
            catch (Exception e)
            {
                errorInfo = "出错，原因：" + e.Message;
            }
            return result;
        }

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

        #region  数据下载
        public bool Down(string beginDate, string endDate, string sortLineCode, bool isSortDown, string batch, out string strResult)
        {
            strResult = string.Empty;
            string errorInfo = string.Empty;
            string lineErrorInfo = string.Empty;
            string custErrorInfo = string.Empty;
            bool bResult = false;
            bool lineResult = false;


            DownSortingInfoBll sortBll = new DownSortingInfoBll();
            DownRouteBll routeBll = new DownRouteBll();
            DownSortingOrderBll orderBll = new DownSortingOrderBll();
            DownCustomerBll custBll = new DownCustomerBll();
            DownDistStationBll stationBll = new DownDistStationBll();
            DownDistCarBillBll carBll = new DownDistCarBillBll();
            DownUnitBll ubll = new DownUnitBll();
            DownProductBll pbll = new DownProductBll();

            beginDate = Convert.ToDateTime(beginDate).ToString("yyyyMMdd");
            endDate = Convert.ToDateTime(endDate).ToString("yyyyMMdd");

            // 判断是否仓储一体化 if type=1  是
            var systemParameterQuery = SystemParameterRepository.GetQueryable();
            var parameterValue = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("IsWarehousSortIntegration")).ParameterValue;
            string Type = parameterValue.ToString();

            switch (Type)
            {               
                case "1":

                    try
                    {
                        ubll.DownUnitCodeInfo();
                        pbll.DownProductInfo();
                        routeBll.DeleteTable();
                        stationBll.DownDistStationInfo();
                        if (!SystemParameterService.SetSystemParameter())
                        {
                            bool custResult = custBll.DownCustomerInfo();
                            carBll.DownDistCarBillInfo(beginDate);
                            if (isSortDown)
                            {
                                //从分拣下载分拣数据
                                lineResult = routeBll.DownSortRouteInfo();
                                bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
                            }
                            else
                            {
                                //从营销下载分拣数据 
                                lineResult = routeBll.DownRouteInfo();
                                //bResult = orderBll.GetSortingOrderDate(beginDate, endDate, out errorInfo);
                                bResult = orderBll.GetSortingOrderDate2(beginDate, endDate, out errorInfo);//牡丹江浪潮
                            }
                        }
                        else
                        {
                            bool custResult = custBll.DownCustomerInfos();//创联
                            //carBll.DownDistCarBillInfo(beginDate);
                            if (isSortDown)
                            {
                                //从分拣下载分拣数据
                                lineResult = routeBll.DownSortRouteInfo();
                                bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
                            }
                            else
                            {
                                //从营销下载分拣数据 创联
                                lineResult = routeBll.DownRouteInfos();
                                bResult = orderBll.GetSortingOrderDates(beginDate, endDate, out errorInfo);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        strResult += e.Message + "线路：" + errorInfo + "。客户：" + errorInfo + "。分拣" + errorInfo;
                    }                   
                    break;
                       
                default:
                    try
                    {

                        DownSalesSystemDao ssDao = new DownSalesSystemDao();
                        DateTime dtOrder = DateTime.ParseExact(beginDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        string orderDate = dtOrder.AddDays(-7).ToShortDateString();
                        //清空数据
                        ssDao.DeleteHistory(orderDate);

                        DownDistDao dd = new DownDistDao();
                        DataTable areTable = dd.FindArea();
                        dd.SynchronizeArea(areTable);

                        DownRouteDao dr = new DownRouteDao();
                        DataTable routeTable = dr.FindRoute();
                        dr.SynchronizeRoute(routeTable);
                        DownCustomerDao dc = new DownCustomerDao();
                        DataTable customerTable = dc.FindCustomer(dtOrder);
                        dc.SynchronizeCustomer(customerTable);
                        DownProductDao dp = new DownProductDao();
                        DataTable productTable = dp.FindProduct();
                        dp.SynchronizeCigarette(productTable);
                        DownSortingOrderDao ds = new DownSortingOrderDao();
                        DataTable orderTable = ds.FindOrder();
                        ds.SynchronizeMaster(orderTable);
                        DataTable orderDetailTable = ds.FindOrderDetail();
                        ds.SynchronizeDetail(orderDetailTable);

                    }
                    catch (Exception e)
                    {
                        strResult += e.Message + strResult;
                    }
                    break;
            }
            return bResult;
        }
                #endregion
    }
}
