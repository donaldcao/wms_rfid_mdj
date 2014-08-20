using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.Bll.Interfaces;
using THOK.SMS.DbModel;
using THOK.SMS.Bll.Models;
using Microsoft.Practices.Unity;
using THOK.SMS.Dal.Interfaces;
using THOK.Wms.Dal.Interfaces;

namespace THOK.SMS.Bll.Service
{
    public class SortTaskService : ISortTaskService
    {
        #region Dependency
        [Dependency]
        public ISortBatchRepository SortBatchRepository { get; set; }

        [Dependency]
        public ISortSupplyRepository SortSupplyRepository { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }


        #endregion
        public bool CreateSortSupply(int supplyCachePositionNo, int vacancyQuantity, DateTime orderdate, int batchNO, out string errorInfo)
        {
            var SortSupplyQuery = SortSupplyRepository.GetQueryable();
            var ProductQuery=ProductRepository.GetQueryable();
            var SupplyTask = SortSupplyQuery.Where
                (s => s.SortBatch.OrderDate == orderdate && s.SortBatch.BatchNo == batchNO)
                .Join(ProductQuery,
                  a=>a.ProductCode,
                  b=>b.ProductCode,
                  (a,b)=>new
                  {
                     a.PackNo,
                     a.SortBatch.SortingLineCode,
                     a.Channel.GroupNo,
                     a.Channel.ChannelCode,
                     a.Channel.ChannelName,
                     a.ProductCode,
                     a.ProductName,
                     b.OneProjectBarcode,
                     a.Channel.SupplyAddress
                  })
                 .Select(s => new
                 {
                     s.PackNo,
                     s.SortingLineCode,
                     s.GroupNo,
                     s.ChannelCode,
                     s.ChannelName,
                     s.ProductCode,
                     s.ProductName,
                     originPositionAddress = "",
                     s.SupplyAddress,
                     status = "0"
                 }).OrderBy(s=>s.PackNo);

            for (int i = 0; i < vacancyQuantity; i++)
            {
                i += 1;
            }
            errorInfo = "任务生成成功！";
            return true;
        }
    }
}
