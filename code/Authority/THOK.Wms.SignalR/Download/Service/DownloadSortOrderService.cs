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
            ConnectionId = connectionId;
            ps.State = StateType.Start;
            NotifyConnection(ps.Clone());

            string errorInfo = string.Empty;
            string lineErrorInfo = string.Empty;
            string custErrorInfo = string.Empty;
            bool bResult = false;
            bool lineResult = false;

            beginDate = Convert.ToDateTime(beginDate).ToString("yyyyMMdd");
            endDate = Convert.ToDateTime(endDate).ToString("yyyyMMdd");

            ubll.DownUnitCodeInfo(); //下载单位表

            ps.State = StateType.Processing;
            ps.TotalProgressName = "下载单位表成功！";
            ps.TotalProgressValue = (int)(10 / 100 * 100);
            ps.CurrentProgressName = "----------";
            ps.CurrentProgressValue = (int)(20 / 100 * 100);
            NotifyConnection(ps.Clone());

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
                    bResult = orderBll.GetSortingOrderDate2(beginDate, endDate, out errorInfo);//牡丹江浪潮
                }
            }
            else
            {
                bool custResult = custBll.DownCustomerInfos();//创联
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
            ps.State = StateType.Info;
            ps.Messages.Add("下载完成!");
            NotifyConnection(ps.Clone());
        }
    }
}
