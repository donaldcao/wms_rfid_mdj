using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using THOK.Wms.SignalR.Common;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;

namespace THOK.SMS.Bll.Service
{
    public class ChannelAllotService : ServiceBase<ChannelAllot>, IChannelAllotService
    {
        [Dependency]
        public IChannelAllotRepository ChannelAllotRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public object GetDetails(int page, int rows, ChannelAllot channelAllot)
        {
            IQueryable<ChannelAllot> channelAllotQuery = ChannelAllotRepository.GetQueryable();

            var channelAllotDetails = channelAllotQuery.Where(c =>
                c.ChannelAllotCode.Contains(channelAllot.ChannelAllotCode)).OrderBy(ul => ul.ChannelAllotCode);

            int total = channelAllotDetails.Count();
            var channelAllotDetail = channelAllotDetails.Skip((page - 1) * rows).Take(rows);
            var channelAllotArray = channelAllotDetail.ToArray().Select(c => new
            {
                c.ChannelAllotCode,
                c.BatchSortId,
                c.ChannelCode,
                c.ProductCode,
                c.ProductName,
                c.InQuantity,
                c.OutQuantity,
                c.RealQuantity,
                c.RemainQuantity
            });
            return new { total, rows = channelAllotArray.ToArray() };
        }

        public System.Data.DataTable GetChannelAllot(int page, int rows, ChannelAllot channelAllot)
        {
            IQueryable<ChannelAllot> channelAllotQuery = ChannelAllotRepository.GetQueryable();

            var channelAllotDetail = channelAllotQuery.Where(c =>
                c.ChannelAllotCode.Contains(channelAllot.ChannelAllotCode)
                && c.ChannelCode.Contains(channelAllot.ChannelCode)
                && c.ProductCode.Contains(channelAllot.ProductCode))
                    .OrderBy(ul => ul.ChannelAllotCode);
            var sortSupplyDetails = channelAllotDetail.ToArray().Select(c => new
            {
                c.ChannelAllotCode,
                c.BatchSortId,
                c.ChannelCode,
                c.ProductCode,
                c.ProductName,
                c.InQuantity,
                c.OutQuantity,
                c.RealQuantity,
                c.RemainQuantity
            });

            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("烟道分配代码", typeof(string));
            dt.Columns.Add("批次分拣编号", typeof(string));
            dt.Columns.Add("烟道代码", typeof(string));
            dt.Columns.Add("商品代码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("入库数量", typeof(string));
            dt.Columns.Add("出库数量", typeof(string));
            dt.Columns.Add("实际数量", typeof(string));
            dt.Columns.Add("提前量", typeof(string));

            foreach (var item in sortSupplyDetails)
            {
                dt.Rows.Add
                    (
                        item.ChannelAllotCode,
                        item.BatchSortId,
                        item.ChannelCode,
                        item.ProductCode,
                        item.ProductName,
                        item.InQuantity,
                        item.OutQuantity,
                        item.RealQuantity,
                        item.RemainQuantity
                    );
            }
            return dt;
        }

    }
}
