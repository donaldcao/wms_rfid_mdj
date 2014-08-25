using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using System.Data;
using THOK.SMS.Dal.EntityRepository;
using THOK.Wms.Dal.Interfaces;

namespace THOK.SMS.Bll.Service
{
    public class SupplyTaskService : ServiceBase<SupplyTask>, ISupplyTaskService
    {
        [Dependency]
        public ISupplyTaskRepository SupplyTaskRepository { get; set; }
        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }
        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public object GetDetails(int page, int rows, SupplyTask supplyTask)
        {
            var supplyTaskQuery = SupplyTaskRepository.GetQueryable();
            var channelQuery = ChannelRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();

            var supplyTaskDetail1 = supplyTaskQuery;
            if (supplyTask.SupplyId != null && supplyTask.SupplyId != 0)
            {
                supplyTaskDetail1 = supplyTaskQuery.Where(s => s.SupplyId == supplyTask.SupplyId).OrderBy(s => s.Id);
            }
            var supplyTaskDetail2 = supplyTaskDetail1;
            if (supplyTask.PackNo != null && supplyTask.PackNo != 0)
            {
                supplyTaskDetail2 = supplyTaskDetail1.Where(s => s.PackNo == supplyTask.PackNo).OrderBy(s => s.Id);
            }
            var supplyTaskDetail3 = supplyTaskDetail2;
            if (supplyTask.ProductCode != null)
            {
                supplyTaskDetail3 = supplyTaskDetail2.Where(s => s.ProductCode.Contains(supplyTask.ProductCode)).OrderBy(s => s.Id);
            }
            var supplyTaskDetail4 = supplyTaskDetail3;
            if (supplyTask.Status != null)
            {
                supplyTaskDetail4 = supplyTaskDetail3.Where(s => s.Status.Contains(supplyTask.Status)).OrderBy(s => s.Id);
            }
            var supplyTaskDetail5 = supplyTaskDetail4;
            if (supplyTask.SortingLineCode != "")
            {
                supplyTaskDetail5 = supplyTaskDetail4.Where(s => s.SortingLineCode.Contains(supplyTask.SortingLineCode)).OrderBy(s => s.Id);
            }
            var supplyTaskDetail6 = supplyTaskDetail5;
            if (supplyTask.GroupNo != null&&supplyTask.GroupNo!=0)
            {
                supplyTaskDetail6 = supplyTaskDetail5.Where(s => s.GroupNo==supplyTask.GroupNo).OrderBy(s => s.Id);
            }
            int total = supplyTaskDetail6.Count();
            var supplyTasksArray = supplyTaskDetail6.OrderBy(s => s.Id).Skip((page - 1) * rows).Take(rows)
                .Select(s => new
                {
                    s.Id,
                    s.SupplyId,
                    s.PackNo,
                    s.SortingLineCode,
                    SortingLineName = sortingLineQuery.Where(sl => sl.SortingLineCode == sl.SortingLineCode).Select(sl => sl.SortingLineName),
                    GroupNo = s.GroupNo == 1 ? "A线" : "B线",
                    s.ProductCode,
                    s.ProductName,
                    s.ChannelCode,
                    s.ChannelName,
                    s.ProductBarcode,
                    s.OriginPositionAddress,
                    s.TargetSupplyAddress,
                    Status = s.Status == "1" ? "已下单" : "未下单"
                }).ToArray();
            return new { total, rows = supplyTasksArray };
        }

        public DataTable GetSupplyTask(int page, int rows,SupplyTask supplyTask)
        {
            IQueryable<SupplyTask> SupplyTasksQuery = SupplyTaskRepository.GetQueryable();
            IQueryable<Channel> channel = ChannelRepository.GetQueryable();
            //IQueryable<> sortingLineQuery = SortingLineRepository.GetQueryable();
            var supplyTasksDetail = SupplyTaskRepository.GetQueryable();
            var supplyTasks = supplyTasksDetail.Where(s => s.SupplyId == supplyTask.SupplyId);
            int total = supplyTasks.Count();
            var supplyTasksArray = supplyTasks.OrderBy(s => s.Id).AsEnumerable()
                .Select(s => new
                {
                    s.Id,
                    s.SupplyId,
                    s.PackNo,
                    s.SortingLineCode,
                    //SortingLineName = sortingLineQuery.Where(sl => sl.SortingLineCode == sl.SortingLineCode).Select(sl => sl.SortingLineName),
                    GroupNo = s.GroupNo == 1 ? "A线" : "B线",
                    s.ProductCode,
                    s.ProductName,
                    s.ChannelCode,
                    s.ChannelName,
                    s.ProductBarcode,
                    s.OriginPositionAddress,
                    s.TargetSupplyAddress,
                    Status = s.Status == "1" ? "已下单" : "未下单"
                });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("补货编码", typeof(int));
            dt.Columns.Add("烟包包号", typeof(int));
            dt.Columns.Add("分拣线代码", typeof(string));
            dt.Columns.Add("分拣线组号", typeof(int));
            dt.Columns.Add("烟道代码", typeof(string));
            dt.Columns.Add("烟道名称", typeof(string));
            dt.Columns.Add("商品代码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("商品条码", typeof(string));
            dt.Columns.Add("起始位置", typeof(int));
            dt.Columns.Add("目标地址", typeof(int));
            dt.Columns.Add("状态", typeof(string));

            foreach (var item in supplyTasksArray)
            {
                dt.Rows.Add(
                    item.Id,
                    item.SupplyId,
                    item.PackNo,
                    item.ProductCode,
                    item.ProductName,
                    item.ChannelCode,
                    item.ChannelName
                    );
            }
            return dt;
        }
    }
}
