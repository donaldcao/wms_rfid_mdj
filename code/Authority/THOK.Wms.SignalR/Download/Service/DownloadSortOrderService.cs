using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using THOK.Util;
using THOK.WMS.DownloadWms.Dao;
using THOK.Wms.SignalR.Download.Interfaces;
using THOK.Wms.SignalR.Model;
using THOK.WMS.DownloadWms.Bll;
using THOK.Wms.DownloadWms.Bll;
using THOK.Authority.Bll.Service;
using THOK.Authority.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.SignalR.Connection;

namespace THOK.Wms.SignalR.Download.Service
{
    public class DownloadSortOrderService : Notifier<DownloadSortOrderConnection>, IDownloadSortOrderService
    {
        [Dependency]
        public ISystemParameterService SystemParameterService { get; set; }

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
            bool result = false;
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
            int random2 = random.Next(1, 30);
            int random3 = random.Next(1, 100);
            int maxNum = 100;

            string errorInfo = string.Empty;
            string lineErrorInfo = string.Empty;
            string custErrorInfo = string.Empty;
            bool bResult = false;
            bool lineResult = false;

            beginDate = Convert.ToDateTime(beginDate).ToString("yyyyMMdd");
            endDate = Convert.ToDateTime(endDate).ToString("yyyyMMdd");

            //单位信息
            StateTypeForProcessing(ps, totalName1, random2, now1 + "单位信息", random3);
            bool bUnit = ubll.DownUnitCodeInfo();
            if (bUnit == false)
            {
                StateTypeForInfo(ps, "同步单位信息失败！");
                return;
            }
            StateTypeForProcessing(ps, totalName1, random1 + 30, now1 + "单位信息", maxNum);
            StateTypeForInfo(ps, "同步单位信息成功！");

            //卷烟信息
            StateTypeForProcessing(ps, totalName1, random1 + 35, now1 + "卷烟信息", random3);
            bool bPro = pbll.DownProductInfo();
            if (bPro == false)
            {
                StateTypeForInfo(ps, "同步卷烟信息失败！");
                return;
            }
            StateTypeForProcessing(ps, totalName1, random1 + 40, now1 + "卷烟信息", maxNum);
            StateTypeForInfo(ps, "同步卷烟信息成功！");

            //删除7日前的数据
            StateTypeForProcessing(ps, totalName1, random1 + 45, "正在优化数据", random3);
            routeBll.DeleteTable();
            StateTypeForProcessing(ps, totalName1, random1 + 50, "正在优化数据", maxNum);
            StateTypeForInfo(ps, "优化数据成功！");

            //配送区域信息
            StateTypeForProcessing(ps, totalName1, random1 + 55, now1 + "配送区域信息", random3);
            bool bDistStation = stationBll.DownDistStationInfo();
            if (bDistStation == false)
            {
                StateTypeForInfo(ps, "同步配送区域信息失败！");
                return;
            }
            StateTypeForProcessing(ps, totalName1, random1 + 60, now1 + "配送区域信息", maxNum);
            StateTypeForInfo(ps, "同步配送区域信息成功！");

            if (!SystemParameterService.SetSystemParameter())
            {
                //客户信息
                StateTypeForProcessing(ps, totalName1, random1 + 65, now1 + "客户信息", random3);
                bool custResult = custBll.DownCustomerInfo();
                if (custResult == false)
                {
                    StateTypeForInfo(ps, "同步客户信息失败！");
                    return;
                }
                StateTypeForProcessing(ps, totalName1, random1 + 70, now1 + "客户信息", maxNum);
                StateTypeForInfo(ps, "同步客户信息成功！");

                //配车单信息
                StateTypeForProcessing(ps, totalName1, random1 + 75, now1 + "配车单信息", random3);
                bool bDistCar = carBll.DownDistCarBillInfo(beginDate);
                if (bDistCar == false)
                {
                    StateTypeForInfo(ps, "同步配车单信息失败！");
                    return;
                }
                StateTypeForProcessing(ps, totalName1, random1 + 80, now1 + "配车单信息", maxNum);
                StateTypeForInfo(ps, "同步配车单信息成功！");

                if (isSortDown)
                {
                    //从分拣下载分拣数据
                    StateTypeForProcessing(ps, totalName1, random1 + 85, now2 + "线路信息", random3);
                    lineResult = routeBll.DownSortRouteInfo();
                    if (lineResult == false)
                    {
                        StateTypeForInfo(ps, "同步线路信息失败！");
                        return;
                    }
                    StateTypeForProcessing(ps, totalName1, random1 + 90, now2 + "线路信息", maxNum);
                    StateTypeForInfo(ps, "同步线路信息成功！");

                    //分拣订单
                    StateTypeForProcessing(ps, totalName2, random1 + 95, now2 + "分拣订单", random3);
                    bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
                    if (bResult == false)
                    {
                        StateTypeForInfo(ps, "同步分拣订单失败！");
                        return;
                    }
                    StateTypeForProcessing(ps, totalName2, 100, now2 + "分拣订单", maxNum);
                    StateTypeForInfo(ps, "同步分拣订单成功！");
                    result = true;
                }
                else
                {
                    //从营销下载分拣数据 
                    StateTypeForProcessing(ps, totalName1, random1 + 85, now2 + "线路信息", random3);
                    lineResult = routeBll.DownRouteInfo();
                    if (lineResult == false)
                    {
                        StateTypeForInfo(ps, "同步线路信息失败！");
                        return;
                    }
                    StateTypeForProcessing(ps, totalName1, random1 + 90, now2 + "线路信息", maxNum);
                    StateTypeForInfo(ps, "同步线路信息成功！");

                    //分拣订单
                    StateTypeForProcessing(ps, totalName2, random1 + 95, now2 + "分拣订单", random3);
                    bResult = orderBll.GetSortingOrderDate2(beginDate, endDate, out errorInfo);//牡丹江浪潮
                    if (bResult == false)
                    {
                        StateTypeForInfo(ps, "下载分拣订单失败！");
                        return;
                    }
                    StateTypeForProcessing(ps, totalName2, 100, now2 + "分拣订单", maxNum);
                    StateTypeForInfo(ps, "下载分拣订单成功！");
                    result = true;
                }
            }
            else
            {
                StateTypeForProcessing(ps, totalName2, random1 + 75, now1 + "客户信息", random3);
                bool custResult = custBll.DownCustomerInfos();//创联
                if (custResult == false)
                {
                    StateTypeForInfo(ps, "同步客户信息失败！");
                    return;
                }
                StateTypeForProcessing(ps, totalName2, random1 + 80, now1 + "客户信息", maxNum);
                StateTypeForInfo(ps, "同步客户信息成功！");

                if (isSortDown)
                {
                    //从分拣下载分拣数据
                    StateTypeForProcessing(ps, totalName1, random1 + 85, now1 + "线路信息", random3);
                    lineResult = routeBll.DownSortRouteInfo();
                    if (lineResult == false)
                    {
                        StateTypeForInfo(ps, "同步线路信息失败！");
                        return;
                    }
                    StateTypeForProcessing(ps, totalName1, random1 + 90, now1 + "线路信息", maxNum);
                    StateTypeForInfo(ps, "同步线路信息成功！");

                    StateTypeForProcessing(ps, totalName2, random1 + 95, now2 + "分拣订单", random3);
                    bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
                    if (bResult == false)
                    {
                        StateTypeForInfo(ps, "下载分拣订单失败！");
                        return;
                    }
                    StateTypeForProcessing(ps, totalName2, 100, now2 + "分拣订单", maxNum);
                    StateTypeForInfo(ps, "下载分拣订单成功！");
                    result = true;
                }
                else
                {
                    //从营销下载分拣数据 创联
                    StateTypeForProcessing(ps, totalName1, random1 + 85, now1 + "线路信息", random3);
                    lineResult = routeBll.DownRouteInfos();
                    if (lineResult == false)
                    {
                        StateTypeForInfo(ps, "同步线路信息失败！");
                        return;
                    }
                    StateTypeForProcessing(ps, totalName1, random1 + 90, now1 + "线路信息", maxNum);
                    StateTypeForInfo(ps, "同步线路信息成功！");

                    StateTypeForProcessing(ps, totalName2, random1 + 95, now2 + "分拣订单", random3);
                    bResult = orderBll.GetSortingOrderDates(beginDate, endDate, out errorInfo);
                    if (bResult == false)
                    {
                        StateTypeForInfo(ps, "下载分拣订单失败！");
                        return;
                    }
                    StateTypeForProcessing(ps, totalName2, 100, now2 + "分拣订单", maxNum);
                    StateTypeForInfo(ps, "下载分拣订单成功！");
                    result = true;
                }
            }
            if (result == true)
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
