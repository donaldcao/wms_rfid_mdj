using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Optimize.Interfaces;
using System.Linq;
using THOK.Common.Entity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
using THOK.SMS.Bll.Interfaces;
using THOK.WMS.DownloadWms.Bll;
using THOK.Wms.DownloadWms.Bll;
using THOK.Authority.Bll.Interfaces;


namespace THOK.SMS.Optimize.Service
{
    public class SortOrderDownService : ServiceBase<SortOrder>, ISortOrderDownService
    {
        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        [Dependency]
        public ISystemParameterService SystemParameterService { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        //仓储分拣一体化
        public bool IsWarehousSortIntegration(out string strResult)
        {
            strResult = string.Empty;
            bool result = false;

            IQueryable<SystemParameter> systemParameterQuery = SystemParameterRepository.GetQueryable();
            var parameterValue = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("IsWarehousSortIntegration")).ParameterValue;
            if (parameterValue == "1") //仓储分拣一体化
            {
                result = true;
            }
            return result;
        }

        //已下载
        public bool DownLoad(string beginDate, string endDate, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            var sort = sortOrderQuery.Where(a => a.OrderDate.Equals(beginDate.Replace("-", "")));
            int count = sort.Count();
            //已下载
            if (count > 0)
            {
                result = true;
            }
            return result;
        }
        //下载数据
        public bool DownSortOrder(string beginDate, string endDate)
        {
            string errorInfo = string.Empty;

            bool bResult = false;
            bool lineResult = false;

            beginDate = Convert.ToDateTime(beginDate).ToString("yyyyMMdd");
            endDate = Convert.ToDateTime(endDate).ToString("yyyyMMdd");

            DownSortingInfoBll sortBll = new DownSortingInfoBll();
            DownRouteBll routeBll = new DownRouteBll();
            DownSortingOrderBll orderBll = new DownSortingOrderBll();
            DownCustomerBll custBll = new DownCustomerBll();
            DownDistStationBll stationBll = new DownDistStationBll();
            DownDistCarBillBll carBll = new DownDistCarBillBll();
            DownUnitBll ubll = new DownUnitBll();
            DownProductBll pbll = new DownProductBll();

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
                    //从营销下载分拣数据 
                    lineResult = routeBll.DownRouteInfo();
                    bResult = orderBll.GetSortingOrderDate2(beginDate, endDate, out errorInfo);//牡丹江浪潮                  
                }
                else
                {
                    bool custResult = custBll.DownCustomerInfos();//创联

                    //从营销下载分拣数据 创联
                    lineResult = routeBll.DownRouteInfos();
                    bResult = orderBll.GetSortingOrderDates(beginDate, endDate, out errorInfo);
                }
            }
            catch (Exception e)
            {
                errorInfo += e.Message;
            }
            return bResult;
        }
    }
}
