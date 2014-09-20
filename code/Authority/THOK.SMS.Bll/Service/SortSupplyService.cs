using System;
using System.Data;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using THOK.SMS.Bll.Interfaces;
using THOK.SMS.Dal.EntityRepository;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Service
{
    public class SortSupplyService : ServiceBase<SortSupply>, ISortSupplyService
    {
        [Dependency]
        public ISortSupplyRepository SortSupplyRepository { get; set; }
        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, SortSupply sortSupply)
        {
            var sortSupplyQuery = SortSupplyRepository.GetQueryable();
            var channelQuery = ChannelRepository.GetQueryable();

            var sortSupplyDetail1 = sortSupplyQuery;
            if (sortSupply.SortBatchId != null && sortSupply.SortBatchId != 0)
            {
                sortSupplyDetail1 = sortSupplyQuery.Where(s => s.SortBatchId == sortSupply.SortBatchId).OrderBy(s => s.Id);
            }
            var sortSupplyDetail2 = sortSupplyDetail1;
            if (sortSupply.PackNo != null && sortSupply.PackNo != 0)
            {
                sortSupplyDetail2 = sortSupplyDetail1.Where(s => s.PackNo == sortSupply.PackNo).OrderBy(s => s.Id);
            }
            var sortSupplyDetail3 = sortSupplyDetail2;
            if (sortSupply.ProductCode != null)
            {
                sortSupplyDetail3 = sortSupplyDetail2.Where(s => s.ProductCode.Contains(sortSupply.ProductCode)).OrderBy(s => s.Id);
            }
            var sortSupplyDetail4 = sortSupplyDetail3;
            if (sortSupply.ChannelCode != null)
            {
                sortSupplyDetail4 = sortSupplyDetail3.Where(s => s.ChannelCode.Contains(sortSupply.ChannelCode)).OrderBy(s => s.Id);
            }
            int total = sortSupplyDetail4.Count();
            var sortSupplysArray = sortSupplyDetail4.OrderBy(s => s.Id).Skip((page - 1) * rows).Take(rows)
                .Select(s => new
                {
                    s.Id,
                    s.SortBatchId,
                    s.PackNo,
                    s.ProductCode,
                    s.ProductName,
                    s.ChannelCode,
                    s.Channel.ChannelName
                }).ToArray();
            return new { total, rows = sortSupplysArray };
        }

        public DataTable GetSortSupply(int page, int rows,SortSupply sortSupply)
        {
            IQueryable<SortSupply> sortSupplysQuery = SortSupplyRepository.GetQueryable();
            IQueryable<Channel> channel = ChannelRepository.GetQueryable();
            var sortSupplys = sortSupplysQuery.Where(a => a.ChannelCode.Contains(sortSupply.ChannelCode) && a.ProductCode.Contains(sortSupply.ProductCode));
            if (sortSupply.PackNo > 0)
            {
                sortSupplys = sortSupplys.Where(a => a.PackNo.Equals(sortSupply.PackNo));
            }
            if (sortSupply.SortBatchId > 0)
            {
                sortSupplys = sortSupplys.Where(a => a.SortBatchId.Equals(sortSupply.SortBatchId));
            }
            var sortSupplysArray = sortSupplys.OrderBy(s => s.Id)
                .Select(s => new
                {
                    s.Id,
                    s.SortBatchId,
                    s.PackNo,
                    s.ChannelCode,
                    s.Channel.ChannelName,
                    s.ProductCode,
                    s.ProductName
                }).ToArray();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("分拣批次ID", typeof(string));
            dt.Columns.Add("烟包包号", typeof(string));
            dt.Columns.Add("烟道代码", typeof(string));
            dt.Columns.Add("烟道名称", typeof(string));
            dt.Columns.Add("商品代码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));

            foreach (var item in sortSupplysArray)
            {
                dt.Rows.Add(
                    item.Id,
                    item.SortBatchId,
                    item.PackNo,
                    item.ChannelCode,
                    item.ChannelName,
                    item.ProductCode,
                    item.ProductName
                    );
            }
            return dt;
        }
    }
}
