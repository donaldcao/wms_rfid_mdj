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
                (s => s.SortBatch.OrderDate == orderdate && s.SortBatch.BatchNo == batchNO && s.Channel.SupplyCachePosition == supplyCachePositionNo)
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
            //产生任务量=空位数（vacancyQuantity）-未下单总量（status=0）任务SupplyId=max(补货计划所有任务)+1
            if (SupplyTask.Count() > 0)
            {
                if (SupplyTask.Count() == 0)//==max(补货计划所有任务)
                {
                    errorInfo = "补货计划已完成！";
                    return false;
                }
                else
                {
                    for (int i = 0; i < vacancyQuantity; i++)
                    {
                        i += 1;
                    }
                    errorInfo = "任务生成成功！";
                    return true;
                }
            }
            else
            {
                errorInfo = "没有新任务生成！";
                return false;
            }
        }
    }
}
