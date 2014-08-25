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

        [Dependency]
        public ISupplyTaskRepository SupplyTaskRepository { get; set; }

        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

        #endregion
        public bool CreateSortSupply(int supplyCachePositionNo, int vacancyQuantity, DateTime orderdate, int batchNO, out string errorInfo)
        {
            var SortSupplyQuery = SortSupplyRepository.GetQueryable();
            var ProductQuery=ProductRepository.GetQueryable();
            var SortTask = SortSupplyQuery.Where
                (s => s.SortBatch.OrderDate == orderdate && s.SortBatch.BatchNo == batchNO && s.Channel.SupplyCachePosition == supplyCachePositionNo)
                .Join(ProductQuery,
                  a=>a.ProductCode,
                  b=>b.ProductCode,
                  (a,b)=>new
                  {
                     a.Id,
                     a.PackNo,
                     a.SortBatch.SortingLineCode,
                     a.Channel.GroupNo,
                     a.Channel.ChannelCode,
                     a.Channel.ChannelName,
                     a.ProductCode,
                     a.ProductName,
                     ProductBarcode=b.PieceBarcode==null?b.OneProjectBarcode.Substring(7,6).ToString():b.PieceBarcode,
                     a.Channel.SupplyAddress
                  })
                 .Select(s => new
                 {
                     s.Id,
                     s.PackNo,
                     s.SortingLineCode,
                     s.GroupNo,
                     s.ChannelCode,
                     s.ChannelName,
                     s.ProductCode,
                     s.ProductName,
                     s.ProductBarcode,
                     OriginPositionAddress = 0,
                     s.SupplyAddress,
                     status = "0"
                 }).OrderBy(s=>s.PackNo).ToArray();
            //产生任务量=空位数（vacancyQuantity）-未下单总量（status=0）任务SupplyId=sum(补货计划所有任务)+1
            var SupplyTaskQuery = SupplyTaskRepository.GetQueryable();
            int OrderQuantity = SupplyTaskQuery
                .Join(ChannelRepository.GetQueryable()
                , a => a.ChannelCode
                , b => b.ChannelCode
                , (a, b) => new { b.SupplyCachePosition }).Where(a => a.SupplyCachePosition == supplyCachePositionNo).Count();
            int notOrderQuantity = SupplyTaskQuery
                .Join(ChannelRepository.GetQueryable()
                , a => a.ChannelCode
                , b => b.ChannelCode
                , (a, b) => new { b.SupplyCachePosition ,a.Status}).Where(a => a.SupplyCachePosition == supplyCachePositionNo&&a.Status=="0").Count();
            if (SortTask.Count() > 0)
            {
                if (SortTask.Count() == OrderQuantity)
                {
                    errorInfo = "补货计划已完成！";
                    return false;
                }
                else
                {
                    SupplyTask supplyTask = new SupplyTask();
                    for (int i = 0; i < vacancyQuantity - notOrderQuantity; i++)
                    {
                        supplyTask.SupplyId = SortTask[OrderQuantity + i].Id;
                        supplyTask.PackNo = SortTask[OrderQuantity + i].PackNo;
                        supplyTask.SortingLineCode = SortTask[OrderQuantity + i].SortingLineCode;
                        supplyTask.GroupNo = SortTask[OrderQuantity + i].GroupNo;
                        supplyTask.ChannelCode = SortTask[OrderQuantity + i].ChannelCode;
                        supplyTask.ChannelName = SortTask[OrderQuantity + i].ChannelName;
                        supplyTask.ProductCode = SortTask[OrderQuantity + i].ProductCode;
                        supplyTask.ProductName = SortTask[OrderQuantity + i].ProductName;
                        supplyTask.ProductBarcode = SortTask[OrderQuantity + i].ProductBarcode;
                        supplyTask.OriginPositionAddress = SortTask[OrderQuantity + i].OriginPositionAddress;
                        supplyTask.TargetSupplyAddress = SortTask[OrderQuantity + i].SupplyAddress;
                        supplyTask.Status = SortTask[OrderQuantity + i].status;
                        SupplyTaskRepository.Add(supplyTask);
                        SupplyTaskRepository.SaveChanges();
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
