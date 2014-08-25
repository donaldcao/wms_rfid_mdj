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
        public bool CreateNewSupplyTask(int supplyCachePositionNo, int vacancyQuantity, DateTime orderdate, int batchNO, out string errorInfo)
        {
            try
            {
                var SortSupplyQuery = SortSupplyRepository.GetQueryable();
                var SupplyTaskQuery = SupplyTaskRepository.GetQueryable();
                var ProductQuery = ProductRepository.GetQueryable();
                var SortTasks = SortSupplyQuery.Where
                    (s => s.SortBatch.OrderDate == orderdate && s.SortBatch.BatchNo == batchNO && s.Channel.SupplyCachePosition == supplyCachePositionNo)
                    .Join(ProductQuery,
                      a => a.ProductCode,
                      b => b.ProductCode,
                      (a, b) => new
                      {
                          a.Id,
                          a.PackNo,
                          a.SortBatch.SortingLineCode,
                          a.Channel.GroupNo,
                          a.Channel.ChannelCode,
                          a.Channel.ChannelName,
                          b.ProductCode,
                          b.ProductName,
                          ProductBarcode = (b.PieceBarcode == null || b.PieceBarcode == "") ? b.OneProjectBarcode.Substring(7, 6).ToString() : b.PieceBarcode.Substring(0, 6).ToString(),
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
                     }).OrderBy(s => s.Id);
                //产生任务量=空位数（vacancyQuantity）-未下单总量（status=0）任务SupplyId=sum(补货计划所有任务)+1
                var SortTask = SortTasks.Where(a => !(SupplyTaskQuery.Select(s => s.SupplyId)).Contains(a.Id)).ToArray();
                int notOrderQuantity = SupplyTaskQuery
                    .Join(ChannelRepository.GetQueryable()
                    , a => a.ChannelCode
                    , b => b.ChannelCode
                    , (a, b) => new { b.SupplyCachePosition, a.Status }).Where(a => a.SupplyCachePosition == supplyCachePositionNo && a.Status == "0").Count();
                if (SortTask.Count() > 0 && vacancyQuantity - notOrderQuantity > 0)
                {
                    SupplyTask supplyTask = new SupplyTask();
                    string id = "";
                    for (int i = 0; i < vacancyQuantity - notOrderQuantity; i++)
                    {
                        supplyTask.SupplyId = SortTask[i].Id;
                        supplyTask.PackNo = SortTask[i].PackNo;
                        supplyTask.SortingLineCode = SortTask[i].SortingLineCode;
                        supplyTask.GroupNo = SortTask[i].GroupNo;
                        supplyTask.ChannelCode = SortTask[i].ChannelCode;
                        supplyTask.ChannelName = SortTask[i].ChannelName;
                        supplyTask.ProductCode = SortTask[i].ProductCode;
                        supplyTask.ProductName = SortTask[i].ProductName;
                        supplyTask.ProductBarcode = SortTask[i].ProductBarcode;
                        supplyTask.OriginPositionAddress = SortTask[i].OriginPositionAddress;
                        supplyTask.TargetSupplyAddress = SortTask[i].SupplyAddress;
                        supplyTask.Status = SortTask[i].status;
                        SupplyTaskRepository.Add(supplyTask);
                        SupplyTaskRepository.SaveChanges();
                        id += "[" + SortTask[i].Id + "]";
                    }
                    errorInfo = "任务生成成功！任务ID：" + id;
                    return true;
                }
                else
                {
                    errorInfo = "";
                    return true;
                }
            }
            catch (Exception e)
            {
                errorInfo = "原因：" + e.Message;
                return false;
            }
        }
    }
}
