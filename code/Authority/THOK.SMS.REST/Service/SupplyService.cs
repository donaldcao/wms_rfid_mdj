using THOK.SMS.REST.Interfaces;
using THOK.SMS.Dal.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using THOK.WCS.Dal.Interfaces;
using THOK.SMS.DbModel;
using System.Transactions;
using THOK.Wms.Dal.Interfaces;

namespace THOK.SMS.REST.Service
{
    public class SupplyService :ISupplyService
    {
        [Dependency]
        public ISortBatchRepository SortBatchRepository { get; set; }
        [Dependency]
        public ISortSupplyRepository SortSupplyRepository { get; set; }
        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        [Dependency]
        public ISupplyPositionRepository SupplyPositionRepository { get; set; }
        [Dependency]
        public ISupplyPositionStorageRepository SupplyPositionStorageRepository { get; set; }
        [Dependency]
        public ISupplyTaskRepository SupplyTaskRepository { get; set; }

        [Dependency]
        public IPositionRepository PositionRepository { get; set; }

        public bool CreateSupplyTask(int position, int quantity,DateTime orderdate, int batchNo, out string error)
        {
            error = string.Empty;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var sortSupplyQuery = SortSupplyRepository.GetQueryable();
                    var supplyTaskQuery = SupplyTaskRepository.GetQueryable();
                    var channelQuery = ChannelRepository.GetQueryable();
                    var productQuery = ProductRepository.GetQueryable();

                    var sortSupplies = sortSupplyQuery.Where(s => s.SortBatch.OrderDate == orderdate
                            && s.SortBatch.BatchNo == batchNo
                            && s.Channel.SupplyCachePosition == position
                            && s.Status == "0")
                        .Join(productQuery,
                          a => a.ProductCode,
                          b => b.ProductCode,
                          (a, b) => new {
                              a.Id,
                              a.PackNo,
                              a.SortBatch.SortingLineCode,
                              a.Channel.GroupNo,
                              a.Channel.ChannelCode,
                              a.Channel.ChannelName,
                              b.ProductCode,
                              b.ProductName,
                              b.PieceBarcode,
                              a.Channel.SupplyAddress})
                         .OrderBy(s => s.Id);

                    int unDownToPlcQuantity = supplyTaskQuery
                        .Join(channelQuery
                        , a => a.ChannelCode
                        , b => b.ChannelCode
                        , (a, b) => new { b.SupplyCachePosition, a.Status })
                        .Where(a => a.SupplyCachePosition == position && a.Status == "0")
                        .Count();

                    int count = quantity - unDownToPlcQuantity > 2 ? 2 : quantity - unDownToPlcQuantity;

                    foreach (var sortSupply in sortSupplies)
                    {
                        if (count >= 1)
                        {
                            SupplyTask supplyTask = new SupplyTask();
                            string barcode = (sortSupply.PieceBarcode ?? string.Empty).PadLeft(13,'0');
                            supplyTask.SupplyId = sortSupply.Id;
                            supplyTask.PackNo = sortSupply.PackNo;
                            supplyTask.SortingLineCode = sortSupply.SortingLineCode;
                            supplyTask.GroupNo = sortSupply.GroupNo;
                            supplyTask.ChannelCode = sortSupply.ChannelCode;
                            supplyTask.ChannelName = sortSupply.ChannelName;
                            supplyTask.ProductCode = sortSupply.ProductCode;
                            supplyTask.ProductName = sortSupply.ProductName;
                            supplyTask.ProductBarcode = barcode.Substring(barcode.Length - 6);
                            supplyTask.OriginPositionAddress = 0;
                            supplyTask.TargetSupplyAddress = sortSupply.SupplyAddress;
                            supplyTask.Status = "0";
                            SupplyTaskRepository.Add(supplyTask);
                            count--;
                        }
                    }
                    SupplyTaskRepository.SaveChanges();
                    scope.Complete();
                    return true;
                }               
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        public bool AssignTaskOriginPositionAddress(out string error)
        {
            error = string.Empty;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var supplyPositionQuery = SupplyPositionRepository.GetQueryable();
                    var supplyPositionStorageQuery = SupplyPositionStorageRepository.GetQueryable();
                    var supplyTaskQuery = SupplyTaskRepository.GetQueryable();

                    var unAssignOriginPositionAddressTasks = supplyTaskQuery.Where(s => s.OriginPositionAddress == 0).OrderBy(s => s.Id);
                    var supplyPositionStorages = supplyPositionStorageQuery.Where(s => s.Quantity > 0);
                    foreach (var task in unAssignOriginPositionAddressTasks)
                    {
                        var supplyPositionStorage = supplyPositionStorages.Where(s => s.ProductCode == task.ProductCode
                            && s.SupplyPosition.IsActive == "1"
                            && (string.IsNullOrEmpty(s.SupplyPosition.SortingLineCodes)
                                || s.SupplyPosition.SortingLineCodes.Contains(task.SortingLineCode))
                            && (string.IsNullOrEmpty(s.SupplyPosition.TargetSupplyAddresses)
                                || s.SupplyPosition.TargetSupplyAddresses.Contains(task.TargetSupplyAddress.ToString())))
                            .OrderBy(s => s.Quantity).FirstOrDefault();

                        var supplyPositions = supplyPositionQuery.Where(s => s.ProductCode == task.ProductCode
                            && s.IsActive == "1"
                            && (string.IsNullOrEmpty(s.SortingLineCodes)
                                || s.SortingLineCodes.Contains(task.SortingLineCode))
                            && (string.IsNullOrEmpty(s.TargetSupplyAddresses)
                                || s.TargetSupplyAddresses.Contains(task.TargetSupplyAddress.ToString())));

                        var mixSupplyPositions = supplyPositionQuery.Where(s => s.IsActive == "1"
                            && s.PositionType == "04");

                        if (supplyPositionStorage != null && supplyPositionStorage.Quantity >= 1)
                        {
                            task.OriginPositionAddress = supplyPositionStorage.SupplyPosition.PositionAddress;
                            supplyPositionStorage.Quantity--;
                        }
                        else if (supplyPositions.Count() == 0 && mixSupplyPositions.Count() == 1)
                        {
                            task.OriginPositionAddress = mixSupplyPositions.Single().PositionAddress;
                        }
                        else
                        {
                            break;
                        }
                    }
                    SupplyTaskRepository.SaveChanges();
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool CheckSupplyPositionStorage(out string error)
        {
            error = string.Empty;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //todo:同步仓储拆盘位到补货拆盘位

                    var supplyPositionQuery = SupplyPositionRepository.GetQueryable();
                    var supplyPositionStorageQuery = SupplyPositionStorageRepository.GetQueryable();
                    var positionQuery = PositionRepository.GetQueryable();

                    var supplyPositions = supplyPositionQuery.Where(s => s.PositionType == "02" && s.PositionType == "03"
                        && s.IsActive == "1");

                    foreach (var supplyPosition in supplyPositions)
                    {
                        var supplyPositionStorages = supplyPositionStorageQuery.Where(s => s.PositionID == supplyPosition.Id
                            && s.ProductCode == supplyPosition.ProductCode);

                        if (supplyPositionStorages.Count() == 0)
                        {
                            var supplyPositionStorage = new SupplyPositionStorage();
                            supplyPositionStorage.PositionID = supplyPosition.Id;
                            supplyPositionStorage.ProductCode = supplyPosition.ProductCode;
                            supplyPositionStorage.ProductName = supplyPosition.ProductName;
                            supplyPositionStorage.Quantity = 0;
                            supplyPositionStorage.WaitQuantity = 0;
                            SupplyPositionStorageRepository.Add(supplyPositionStorage);
                        }

                        foreach (var supplyPositionStorage in supplyPositionStorages)
                        {
                            if (supplyPosition.PositionCapacity - supplyPositionStorage.Quantity - supplyPositionStorage.WaitQuantity >= 30)
                            {
                                var position = positionQuery.Where(p => p.ChannelCode == supplyPosition.PositionName).FirstOrDefault();
                                if (position != null)
                                {
                                    position.HasGoods = false;
                                }
                            }
                        }
                    }

                    SupplyPositionStorageRepository.SaveChanges();
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
