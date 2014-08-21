using System;
using System.Linq;
using System.Threading;
using System.Data;
using THOK.Wms.SignalR.Download.Interfaces;
using THOK.Wms.Download.Bll;
using THOK.Wms.Download.Bll;
using THOK.Authority.Bll.Service;
using THOK.Authority.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.SignalR.Connection;
using THOK.Authority.Dal.Interfaces;
using THOK.Common.SignalR;
using THOK.Common.SignalR.Model;

namespace THOK.Wms.SignalR.Download.Service
{
    public class DownloadSortOrderService : Notifier<DownloadSortOrderConnection>, IDownloadSortOrderService
    {
        [Dependency]
        public ISystemParameterService SystemParameterService { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        DownUnitBll ubll = new DownUnitBll();
        DownProductBll pbll = new DownProductBll();
        DownSortingInfoBll sortBll = new DownSortingInfoBll();
        DownRouteBll routeBll = new DownRouteBll();
        DownSortingOrderBll orderBll = new DownSortingOrderBll();
        DownCustomerBll custBll = new DownCustomerBll();
        DownDistStationBll stationBll = new DownDistStationBll();
        DownDistCarBillBll carBll = new DownDistCarBillBll();

        public void Download(string connectionId, ProgressState ps, CancellationToken cancellationToken, string beginDate, string endDate, string sortLineCode, bool isSortDown, string batch)
        {
            if (string.IsNullOrEmpty(beginDate))
            {
                StateTypeForInfo(ps, "请填写日期！");
                return;
            }

            ConnectionId = connectionId;
            ps.State = StateType.Start;
            ps.Messages.Add("开始下载");
            NotifyConnection(ps.Clone());

            string totalName1 = "基础表信息";
            string now1 = "正在同步";
            string totalName2 = "订单信息";
            string now2 = "正在下载";
            Random random = new Random();
            int random1 = random.Next(1, 5);
            int maxNum = 100;

            string errorInfo = string.Empty;
            string lineErrorInfo = string.Empty;
            string custErrorInfo = string.Empty;
            bool bResult = false;
            bool lineResult = false;

            beginDate = Convert.ToDateTime(beginDate).ToString("yyyyMMdd");
            endDate = Convert.ToDateTime(endDate).ToString("yyyyMMdd");

            var systemParameterQuery = SystemParameterRepository.GetQueryable();
            var parameterValue = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("IsWarehousSortIntegration")).ParameterValue;
            string Type = parameterValue.ToString();

            switch (Type)
            {
                #region 仓储一体化
                case "1":

                    try
                    {
                        #region MyRegion
                        //单位信息
                        StateTypeForProcessing(ps, totalName1, random1, now1 + "单位信息", new Random().Next(1, 100));
                        bool bUnit = ubll.DownUnitCodeInfo();

                        StateTypeForProcessing(ps, totalName1, random1 + 5, now1 + "单位信息", maxNum);

                        //卷烟信息
                        StateTypeForProcessing(ps, totalName1, random1 + 10, now1 + "卷烟信息", new Random().Next(1, 100));
                        bool bPro = pbll.DownProductInfo();

                        StateTypeForProcessing(ps, totalName1, random1 + 15, now1 + "卷烟信息", maxNum);


                        //删除7日前的数据
                        StateTypeForProcessing(ps, totalName1, random1 + 20, "正在优化数据", new Random().Next(1, 100));
                        routeBll.DeleteTable();
                        StateTypeForProcessing(ps, totalName1, random1 + 25, "正在优化数据", maxNum);

                        //配送区域信息
                        StateTypeForProcessing(ps, totalName1, random1 + 30, now1 + "配送区域信息", new Random().Next(1, 100));
                        bool bDistStation = stationBll.DownDistStationInfo();

                        StateTypeForProcessing(ps, totalName1, random1 + 35, now1 + "配送区域信息", maxNum);

                        #endregion

                        if (!SystemParameterService.SetSystemParameter())
                        {
                            #region MyRegion
                            //客户信息
                            StateTypeForProcessing(ps, totalName1, random1 + 40, now1 + "客户信息", new Random().Next(1, 100));
                            bool custResult = custBll.DownCustomerInfo();

                            StateTypeForProcessing(ps, totalName1, random1 + 45, now1 + "客户信息", maxNum);

                            //配车单信息
                            StateTypeForProcessing(ps, totalName1, random1 + 50, now1 + "配车单信息", new Random().Next(1, 100));
                            bool bDistCar = carBll.DownDistCarBillInfo(beginDate);

                            StateTypeForProcessing(ps, totalName1, random1 + 55, now1 + "配车单信息", maxNum);


                            if (isSortDown)
                            {
                                //从分拣下载分拣数据
                                StateTypeForProcessing(ps, totalName1, random1 + 60, now2 + "线路信息", new Random().Next(1, 100));
                                lineResult = routeBll.DownSortRouteInfo();
 
                                StateTypeForProcessing(ps, totalName1, random1 + 65, now2 + "线路信息", maxNum);


                                //分拣订单
                                StateTypeForProcessing(ps, totalName2, random1 + 70, now2 + "分拣订单", new Random().Next(1, 100));
                                bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
                                if (bResult == false)
                                {
                                    StateTypeForInfo(ps, "未发现新分拣订单！");
                                    return;
                                }
                                else
                                {
                                    StateTypeForInfo(ps, "下载分拣订单成功！");
                                }
                                StateTypeForProcessing(ps, totalName2, 100, now2 + "分拣订单", maxNum);
                            }
                            else
                            {
                                //从营销下载分拣数据 
                                StateTypeForProcessing(ps, totalName1, random1 + 65, now2 + "线路信息", new Random().Next(1, 100));
                                lineResult = routeBll.DownRouteInfo();

                                StateTypeForProcessing(ps, totalName1, random1 + 70, now2 + "线路信息", maxNum);

                                //分拣订单
                                StateTypeForProcessing(ps, totalName2, random1 + 75, now2 + "分拣订单", new Random().Next(1, 100));
                                bResult = orderBll.GetSortingOrderDate2(beginDate, endDate, out errorInfo);//牡丹江浪潮
                                if (bResult == false)
                                {
                                    StateTypeForInfo(ps, "未发现新分拣订单！");
                                    return;
                                }
                                else
                                {
                                    StateTypeForInfo(ps, "下载分拣订单成功！");
                                }
                                StateTypeForProcessing(ps, totalName2, 100, now2 + "分拣订单", maxNum);
                            }
                            #endregion
                        }
                        else
                        {
                            #region MyRegion
                            StateTypeForProcessing(ps, totalName2, random1 + 40, now1 + "客户信息", new Random().Next(1, 100));
                            bool custResult = custBll.DownCustomerInfos();//创联
                            if (custResult == false)
                            {
                                StateTypeForInfo(ps, "未发现新客户信息！");
                            }
                            else
                            {
                                StateTypeForInfo(ps, "同步客户信息成功！");
                            }
                            StateTypeForProcessing(ps, totalName2, random1 + 45, now1 + "客户信息", maxNum);


                            if (isSortDown)
                            {
                                //从分拣下载分拣数据
                                StateTypeForProcessing(ps, totalName1, random1 + 50, now1 + "线路信息", new Random().Next(1, 100));
                                lineResult = routeBll.DownSortRouteInfo();
                                if (lineResult == false)
                                {
                                    StateTypeForInfo(ps, "未发现新线路信息！");
                                }
                                else
                                {
                                    StateTypeForInfo(ps, "同步线路信息成功！");
                                }
                                StateTypeForProcessing(ps, totalName1, random1 + 55, now1 + "线路信息", maxNum);

                                StateTypeForProcessing(ps, totalName2, random1 + 60, now2 + "分拣订单", new Random().Next(1, 100));
                                bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
                                if (bResult == false)
                                {
                                    StateTypeForInfo(ps, "未发现新分拣订单！");
                                }
                                else
                                {
                                    StateTypeForInfo(ps, "下载分拣订单成功！");
                                }
                                StateTypeForProcessing(ps, totalName2, 100, now2 + "分拣订单", maxNum);
                            }
                            else
                            {
                                //从营销下载分拣数据 创联
                                StateTypeForProcessing(ps, totalName1, random1 + 50, now1 + "线路信息", new Random().Next(1, 100));
                                lineResult = routeBll.DownRouteInfos();
                                if (lineResult == false)
                                {
                                    StateTypeForInfo(ps, "未发现新线路信息！");
                                }
                                else
                                {
                                    StateTypeForInfo(ps, "同步线路信息成功！");
                                }
                                StateTypeForProcessing(ps, totalName1, random1 + 55, now1 + "线路信息", maxNum);

                                StateTypeForProcessing(ps, totalName2, random1 + 60, now2 + "分拣订单", new Random().Next(1, 100));
                                bResult = orderBll.GetSortingOrderDates(beginDate, endDate, out errorInfo);
                                if (bResult == false)
                                {
                                    StateTypeForInfo(ps, "未发现新分拣订单！");
                                }
                                else
                                {
                                    StateTypeForInfo(ps, "下载分拣订单成功！");
                                }
                                StateTypeForProcessing(ps, totalName2, 100, now2 + "分拣订单", maxNum);
                            }
                            #endregion
                        }
                        if (bResult == true)
                        {
                            ps.State = StateType.Info;
                            ps.Messages.Clear();
                            ps.Messages.Add("下载成功！");
                            NotifyConnection(ps.Clone());
                        }
                        else
                        {
                            StateTypeForInfo(ps, "下载失败！");
                        }
                    }
                    catch (Exception ex)
                    {
                        StateTypeForInfo(ps, "下载异常！" + ex.Message);
                    }
                    break;
                #endregion

                case "0":

                    try
                    {

                        //DeleteHistoryBll ssDao = new DeleteHistoryBll();
                        DateTime dtOrder = DateTime.ParseExact(beginDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        string orderDate = dtOrder.AddDays(-7).ToShortDateString();
                        //清空数据
                        //ssDao.DeleteHistory(orderDate);


                        StateTypeForProcessing(ps, "配送区域下载", random1, now1 + "配送区域", new Random().Next(1, 100));
                        DataTable areTable = ubll.GetArea();
                        ubll.InsertArea(areTable);
                        StateTypeForProcessing(ps, "配送区域下载", random1 + 20, now1 + "配送区域", maxNum);

                        StateTypeForProcessing(ps, "配送线路下载", random1, now1 + "配送线路", new Random().Next(1, 100));
                        DataTable routeTable = routeBll.GetRoute();
                        routeBll.InsertRoute(routeTable);
                        StateTypeForProcessing(ps, "配送线路下载", random1 + 40, now1 + "配送线路", maxNum);


                        StateTypeForProcessing(ps, "客户信息下载", random1, now1 + "客户信息", new Random().Next(1, 100));
                        DataTable customerTable = custBll.GetCustomer(dtOrder);
                        custBll.InsertCustomer(customerTable);
                        StateTypeForProcessing(ps, "客户信息下载", random1 + 60, now1 + "客户信息", maxNum);


                        StateTypeForProcessing(ps, "卷烟信息下载", random1, now1 + "卷烟信息", new Random().Next(1, 100));
                        DataTable productTable = pbll.GetProduct();
                        pbll.InsertProduct(productTable);
                        StateTypeForProcessing(ps, "卷烟信息下载", random1 + 80, now1 + "卷烟信息", maxNum);

                        DownSortingOrderBll obll = new DownSortingOrderBll();
                        StateTypeForProcessing(ps, "分拣订单下载", random1, now1 + "分拣订单", new Random().Next(1, 100));
                        DataTable orderTable = obll.GetOrderMaster();
                        StateTypeForInfo(ps, "同步分拣订单成功！");
                        obll.InsertOrderMaster(orderTable);
                        StateTypeForProcessing(ps, "分拣订单下载", random1 + 90, now1 + "分拣订单", maxNum);

                        DataTable orderDetailTable = obll.GetOrderDetail();
                        StateTypeForProcessing(ps, "分拣细单下载", random1, now1 + "分拣细单", new Random().Next(1, 100));
                        StateTypeForInfo(ps, "同步分拣细单成功！");
                        obll.InsertOrderDetail(orderDetailTable);
                        StateTypeForProcessing(ps, "分拣细单下载", random1 + 100, now1 + "分拣细单", maxNum);

                        ps.State = StateType.Info;
                        ps.Messages.Clear();
                        ps.Messages.Add("下载成功！");
                        NotifyConnection(ps.Clone());
                    }
                    catch (Exception ex)
                    {
                        StateTypeForInfo(ps, "下载异常！" + ex.Message);
                    }                
                    break;
            }
        }

        void StateTypeForInfo(ProgressState ps, string message)
        {
            ps.State = StateType.Info;
            ps.Messages.Add(message);
            NotifyConnection(ps.Clone());
        }

        void StateTypeForError(ProgressState ps, string message, Exception ex)
        {
            ps.State = StateType.Error;
            ps.Messages.Add("失败！详情：" + ex.Message);
            NotifyConnection(ps.Clone());
        }

        void StateTypeForProcessing(ProgressState ps, string TotalName, int TotalValue, string CurrentName, int CurrentValue)
        {
            ps.State = StateType.Processing;
            ps.TotalProgressName = TotalName;
            ps.TotalProgressValue = TotalValue;
            ps.CurrentProgressName = CurrentName;
            ps.CurrentProgressValue = CurrentValue;
            NotifyConnection(ps.Clone());
        }
    }
}
