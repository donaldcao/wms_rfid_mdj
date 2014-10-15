using THOK.WCS.REST.Interfaces;
using THOK.WCS.REST.Models;
using Microsoft.Practices.Unity;
using THOK.WCS.Dal.Interfaces;
using THOK.Wms.Dal.Interfaces;
using System;
using System.Transactions;
using System.Linq;
using THOK.WCS.DbModel;
using System.Collections;
using System.Collections.Generic;
using THOK.WCS.Bll.Interfaces;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.DbModel;

namespace THOK.WCS.REST.Service
{
    public class TransportService :ITransportService
    {
        [Dependency]
        public ITaskRepository TaskRepository { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }
        
        [Dependency]
        public IProductSizeRepository ProductSizeRepository { get; set; }

        [Dependency]
        public IPositionRepository PositionRepository { get; set; }

        [Dependency]
        public ICellPositionRepository CellPositionRepository { get; set; }

        [Dependency]
        public IPathRepository PathRepository { get; set; }

        [Dependency]
        public ICellRepository CellRepository { get; set; }

        [Dependency]
        public IStorageRepository StorageRepository { get; set; }


        [Dependency]
        public IInBillMasterRepository InBillMasterRepository { get; set; }
        [Dependency]
        public IInBillDetailRepository InBillDetailRepository { get; set; }
        [Dependency]
        public IInBillAllotRepository InBillAllotRepository { get; set; }

        [Dependency]
        public IOutBillMasterRepository OutBillMasterRepository { get; set; }
        [Dependency]
        public IOutBillDetailRepository OutBillDetailRepository { get; set; }
        [Dependency]
        public IOutBillAllotRepository OutBillAllotRepository { get; set; }

        [Dependency]
        public IMoveBillMasterRepository MoveBillMasterRepository { get; set; }
        [Dependency]
        public IMoveBillDetailRepository MoveBillDetailRepository { get; set; }

        [Dependency]
        public ICheckBillMasterRepository CheckBillMasterRepository { get; set; }
        [Dependency]
        public ICheckBillDetailRepository CheckBillDetailRepository { get; set; }

        [Dependency]
        public ISortWorkDispatchRepository SortWorkDispatchRepository { get; set; }

        [Dependency]
        public ITaskService TaskService { get; set; }

        [Dependency]
        public ISupplyPositionRepository SupplyPositionRepository { get; set; }
        [Dependency]
        public ISupplyPositionStorageRepository SupplyPositionStorageRepository { get; set; }

        public bool Arrive(string positionName,string barcode, out string error)
        {
            error = string.Empty;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var taskQuery = TaskRepository.GetQueryable();
                    var productQuery = ProductRepository.GetQueryable();
                    var positionQuery = PositionRepository.GetQueryable();

                    var task = taskQuery.Join(productQuery, t => t.ProductCode, p => p.ProductCode, (t, p) => new { Task = t, Product = p })
                        .Join(positionQuery, r => r.Task.CurrentPositionID, p => p.ID, (r, p) => new { Task = r.Task, Product = r.Product, CurrentPosition = p })
                        .Where(r => r.CurrentPosition.PositionName == positionName && r.Task.CurrentPositionState == "01"
                            && r.Task.State == "01" && r.Product.PieceBarcode.Contains(barcode))
                        .OrderByDescending(r=>r.Task.TaskQuantity)
                        .FirstOrDefault();

                    if (task != null)
                    {
                        task.Task.CurrentPositionState = "02";
                        TaskRepository.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    else
                    {
                        error = "not found task!";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }      
        }

        public bool Arrive(string positionName, int taskid, out string error)
        {
            error = string.Empty;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var taskQuery = TaskRepository.GetQueryable();
                    var pathQuery = PathRepository.GetQueryable();
                    var positionQuery = PositionRepository.GetQueryable();
                    var task = taskQuery.Join(pathQuery, t => t.PathID, p => p.ID, (t, p) => new { Task = t, Path = p })
                        .Join(positionQuery, r => r.Task.OriginPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = p })
                        .Join(positionQuery, r => r.Task.CurrentPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = r.OriginPosition, CurrentPosition = p })
                        .Join(positionQuery, r => r.Task.TargetPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = r.OriginPosition, CurrentPosition = r.CurrentPosition, TargetPosition = p })
                        .Where(r => r.Task.ID == taskid && r.Task.State == "01")
                        .FirstOrDefault();

                    if (task != null)
                    {
                        Position nextPosition = null;

                        IDictionary<int, Position> pathNodePositions = new Dictionary<int, Position>();
                        pathNodePositions.Add(pathNodePositions.Count + 1, task.OriginPosition);
                        foreach (var pathNode in task.Path.PathNodes.OrderBy(p => p.PathNodeOrder).ToArray())
                        {
                            pathNodePositions.Add(pathNodePositions.Count + 1, pathNode.Position);
                        }
                        pathNodePositions.Add(pathNodePositions.Count + 1, task.TargetPosition);

                        var currentPositionIndex = pathNodePositions.Where(p => p.Value.ID == task.CurrentPosition.ID)
                            .Select(p => p.Key)
                            .OrderByDescending(k => k)
                            .FirstOrDefault();

                        if (currentPositionIndex <= 0)
                        {
                            error = "当前任务的当前位置不的任务路径上！";
                            return false;
                        }

                        if (currentPositionIndex + 1 <= pathNodePositions.Max(p => p.Key))
                        {
                            nextPosition = pathNodePositions[currentPositionIndex + 1];
                        }

                        if (nextPosition != null && nextPosition.ID != task.TargetPosition.ID && nextPosition.PositionName == positionName)
                        {
                            task.CurrentPosition.HasGoods = false;
                            task.Task.CurrentPositionID = nextPosition.ID;
                            nextPosition.HasGoods = true;
                            task.Task.State = "01";
                        }
                        else if (nextPosition != null && nextPosition.ID == task.TargetPosition.ID && nextPosition.PositionName == positionName)
                        {
                            task.CurrentPosition.HasGoods = false;
                            task.Task.CurrentPositionID = task.TargetPosition.ID;
                            nextPosition.HasGoods = true;
                            task.Task.State = "04";
                        }
                        else
                        {
                            error = "当前任务路径的下个位置与到达的位置不符！";
                            return false;
                        }

                        TaskRepository.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    else
                    {
                        error = "当前任务不存在！";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }      
        }

        public SRMTask GetSrmTask(string srmName, int travelPos, int liftPos, out string error)
        {
            error = string.Empty; 
            SRMTask srmTask = null;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var taskQuery = TaskRepository.GetQueryable();
                    var pathQuery = PathRepository.GetQueryable();
                    var positionQuery = PositionRepository.GetQueryable();
                    var cellPositionQuery = CellPositionRepository.GetQueryable();
                    var productSizeQuery = ProductSizeRepository.GetQueryable();
                    var cellQuery = CellRepository.GetQueryable();
                    var storageQuery = StorageRepository.GetQueryable();
                    var productQuery = ProductRepository.GetQueryable();
                                        
                    var tasks = taskQuery.Join(pathQuery, t => t.PathID, p => p.ID, (t, p) => new { Task = t, Path = p })
                        .Join(positionQuery, r => r.Task.OriginPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = p })
                        .Join(positionQuery, r => r.Task.CurrentPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path,OriginPosition = r.OriginPosition, CurrentPosition = p})
                        .Join(positionQuery, r => r.Task.TargetPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path,OriginPosition = r.OriginPosition, CurrentPosition = r.CurrentPosition, TargetPosition = p })
                        .Where(r => r.Task.State == "01"
                            && r.Task.CurrentPositionState == "02"
                            && r.CurrentPosition.AbleStockOut
                            && r.CurrentPosition.ID != r.TargetPosition.ID
                            && r.CurrentPosition.SRMName.Contains(srmName)
                            && ((new string[] { "01", "05", "06" }).Contains(r.TargetPosition.PositionType) || !r.TargetPosition.HasGoods)
                        )
                        .OrderBy(r => r.Task.TaskLevel)
                        .ThenBy(r => Math.Abs(travelPos - r.CurrentPosition.TravelPos))
                        .ThenBy(r => Math.Abs(liftPos - r.CurrentPosition.LiftPos));

                    foreach (var task in tasks)
                    {
                        //todo:重新选择择路径

                        Position nextPosition = null;
                        Position nextTwoPosition = null;

                        IDictionary<int, Position> pathNodePositions = new Dictionary<int, Position>();
                        pathNodePositions.Add(pathNodePositions.Count + 1, task.OriginPosition);
                        foreach (var pathNode in task.Path.PathNodes.OrderBy(p => p.PathNodeOrder).ToArray())
                        {
                            pathNodePositions.Add(pathNodePositions.Count + 1, pathNode.Position);
                        }
                        pathNodePositions.Add(pathNodePositions.Count + 1, task.TargetPosition);

                        var currentPositionIndex = pathNodePositions.Where(p => p.Value.ID == task.CurrentPosition.ID)
                            .Select(p=>p.Key)
                            .OrderByDescending(k=>k)
                            .FirstOrDefault();

                        if (currentPositionIndex <= 0)
                        {
                            continue;
                        }

                        if (currentPositionIndex + 1 <= pathNodePositions.Max(p => p.Key))
                        {
                            nextPosition = pathNodePositions[currentPositionIndex + 1];
                        }
                        else
                        {
                            continue;
                        }

                        if (!nextPosition.SRMName.Contains(srmName))
                        {
                            continue;
                        }

                        if (currentPositionIndex + 2 <= pathNodePositions.Max(p => p.Key))
                        {
                            nextTwoPosition = pathNodePositions[currentPositionIndex + 2];
                        }

                        if ((new string []{"02","03","04"}).Contains(task.Task.OrderType)
                            && task.Task.StorageSequence != storageQuery.Where(s => s.CellCode == task.Task.OriginCellCode && s.Quantity > 0).Min(s => s.StorageSequence))
                        {
                            continue;
                        }

                        srmTask = new SRMTask();

                        srmTask.ID = task.Task.ID;
                        srmTask.TaskType = task.Task.TaskType;

                        srmTask.OrderID = task.Task.OrderID;
                        srmTask.OrderType = task.Task.OrderType;
                        srmTask.AllotID = task.Task.AllotID;

                        srmTask.OriginCellCode = task.Task.OriginCellCode;
                        srmTask.TargetCellCode = task.Task.TargetCellCode;
                        srmTask.OriginStorageCode = task.Task.OriginStorageCode;
                        srmTask.TargetStorageCode = task.Task.TargetStorageCode;

                        srmTask.Quantity = task.Task.Quantity;
                        srmTask.TaskQuantity = task.Task.TaskQuantity;

                        var productSize = productSizeQuery.Where(p => p.ProductCode == task.Task.ProductCode).FirstOrDefault();
                        if (productSize != null)
                        {
                            srmTask.Length = productSize.Length;
                            srmTask.Width = productSize.Width;
                            srmTask.Length = productSize.Length;
                        }

                        var targetStorage = storageQuery.Where(s => s.StorageCode == task.Task.TargetStorageCode).FirstOrDefault();
                        int targetQuantity = targetStorage != null ? Convert.ToInt32(targetStorage.Quantity) : 0;

                        srmTask.TravelPos1 = task.CurrentPosition.TravelPos;
                        srmTask.LiftPos1 = task.CurrentPosition.LiftPos;
                        srmTask.TravelPos2 = nextPosition.TravelPos;
                        srmTask.LiftPos2 = nextPosition.LiftPos;
                        srmTask.RealLiftPos2 = nextPosition.LiftPos + (task.Task.TaskType == "02" ? (targetQuantity * 150) : 0);

                        srmTask.CurrentPositionName = task.CurrentPosition.PositionName;
                        srmTask.CurrentPositionType = task.CurrentPosition.PositionType;
                        srmTask.CurrentPositionExtension = task.CurrentPosition.Extension;

                        srmTask.NextPositionName = nextPosition.PositionName;
                        srmTask.NextPositionType = nextPosition.PositionType;
                        srmTask.NextPositionExtension = nextPosition.Extension;

                        srmTask.NextTwoPositionName = nextTwoPosition != null ? nextTwoPosition.PositionName : "0";

                        srmTask.EndPositionName = task.TargetPosition.PositionName;
                        srmTask.EndPositionType = task.TargetPosition.PositionType;

                        srmTask.HasGetRequest = task.CurrentPosition.HasGetRequest;
                        srmTask.HasPutRequest = nextPosition.HasPutRequest;

                        var product = productQuery.Where(p => p.ProductCode == task.Task.ProductCode).FirstOrDefault();
                        string barcode = product != null ? (product.PieceBarcode ?? string.Empty).PadLeft(13, '0') : "888888";
                        srmTask.Barcode = barcode.Substring(barcode.Length - 6);

                        srmTask.ProductName = task.Task.ProductName;
                        var originCell = cellQuery.Join(cellPositionQuery, c => c.CellCode, p => p.CellCode, (c, p) => new { Cell = c,Position = p.StockOutPosition})
                            .Where(r => r.Position.ID == task.CurrentPosition.ID)
                            .Select(r=>r.Cell)
                            .FirstOrDefault();

                        var targetCell = cellQuery.Join(cellPositionQuery, c => c.CellCode, p => p.CellCode, (c, p) => new { Cell = c, Position = p.StockInPosition })
                            .Where(r => r.Position.ID == nextPosition.ID)
                            .Select(r => r.Cell)
                            .FirstOrDefault();

                        srmTask.OriginCellName = originCell != null ? originCell.CellName : string.Format("{0}(当前位置未配置货位)", task.CurrentPosition.PositionName);
                        srmTask.TargetCellName = targetCell != null ? targetCell.CellName : string.Format("{0}(当前位置未配置货位)", nextPosition.PositionName);
                        srmTask.PiecesQutity = task.Task.TaskQuantity;
                        srmTask.BarQutity = task.Task.BarQutity;

                        task.Task.State = "02";

                        break;
                    }
                    TaskRepository.SaveChanges();
                    scope.Complete();
                    return srmTask;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }      
        }

        public bool CancelTask(int taskid, out string error)
        {
            error = string.Empty;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var taskQuery = TaskRepository.GetQueryable();
                    var task = taskQuery.Where(t => t.ID == taskid).FirstOrDefault();
                    if (task != null)
                    {
                        task.State = "01";
                        TaskRepository.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    else
                    {
                        error = "当前任务不存在！";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }      
        }

        public bool FinishTask(int taskid, string operatorName, out string error)
        {
            error = string.Empty;

            try
            {
                var task = TaskRepository.GetQueryable().Where(i => i.ID == taskid).FirstOrDefault();
                if (task != null && task.State == "04" && TaskService.FinishTask(taskid, out error))
                {
                    return true;
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    var taskQuery = TaskRepository.GetQueryable();
                    var pathQuery = PathRepository.GetQueryable();
                    var positionQuery = PositionRepository.GetQueryable();
                    var supplyPositionQuery = SupplyPositionRepository.GetQueryable();
                    var supplyPositionStorageQuery = SupplyPositionStorageRepository.GetQueryable();

                    var taskinfo = taskQuery.Join(pathQuery, t => t.PathID, p => p.ID, (t, p) => new { Task = t, Path = p })
                        .Join(positionQuery, r => r.Task.OriginPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = p })
                        .Join(positionQuery, r => r.Task.CurrentPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = r.OriginPosition, CurrentPosition = p })
                        .Join(positionQuery, r => r.Task.TargetPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = r.OriginPosition, CurrentPosition = r.CurrentPosition, TargetPosition = p })
                        .Where(r => r.Task.ID == taskid && r.Task.State == "02")
                        .FirstOrDefault();

                    if (taskinfo != null)
                    {
                        Position nextPosition = null;

                        IDictionary<int, Position> pathNodePositions = new Dictionary<int, Position>();
                        pathNodePositions.Add(pathNodePositions.Count + 1, taskinfo.OriginPosition);
                        foreach (var pathNode in taskinfo.Path.PathNodes.OrderBy(p => p.PathNodeOrder).ToArray())
                        {
                            pathNodePositions.Add(pathNodePositions.Count + 1, pathNode.Position);
                        }
                        pathNodePositions.Add(pathNodePositions.Count + 1, taskinfo.TargetPosition);

                        var currentPositionIndex = pathNodePositions.Where(p => p.Value.ID == taskinfo.CurrentPosition.ID)
                            .Select(p => p.Key)
                            .OrderByDescending(k => k)
                            .FirstOrDefault();

                        if (currentPositionIndex <= 0)
                        {
                            error = "当前任务的当前位置不的任务路径上！";
                            return false;
                        }

                        if (currentPositionIndex + 1 <= pathNodePositions.Max(p => p.Key))
                        {
                            nextPosition = pathNodePositions[currentPositionIndex + 1];
                        }

                        if (nextPosition != null && nextPosition.ID != taskinfo.TargetPosition.ID)
                        {
                            taskinfo.CurrentPosition.HasGoods = false;
                            taskinfo.Task.CurrentPositionID = nextPosition.ID;
                            nextPosition.HasGoods = true;
                            taskinfo.Task.State = "01";
                        }
                        else if (nextPosition != null && nextPosition.ID == taskinfo.TargetPosition.ID)
                        {
                            taskinfo.CurrentPosition.HasGoods = false;
                            taskinfo.Task.CurrentPositionID = taskinfo.TargetPosition.ID;
                            nextPosition.HasGoods = true;
                            taskinfo.Task.State = "04";

                            var supplyPosition = supplyPositionQuery.Where(s => s.PositionName == nextPosition.ChannelCode
                                && s.ProductCode == taskinfo.Task.ProductCode).FirstOrDefault();
                            if (supplyPosition != null)
                            {
                                var supplyPositionStorage = supplyPositionStorageQuery.Where(s => s.PositionID == supplyPosition.Id
                                    && s.ProductCode == supplyPosition.ProductCode).FirstOrDefault();

                                if (supplyPositionStorage == null)
                                {
                                    supplyPositionStorage = new SupplyPositionStorage();
                                    supplyPositionStorage.PositionID = supplyPosition.Id;
                                    supplyPositionStorage.ProductCode = supplyPosition.ProductCode;
                                    supplyPositionStorage.ProductName = supplyPosition.ProductName;
                                    supplyPositionStorage.Quantity = taskinfo.Task.TaskQuantity;
                                    supplyPositionStorage.WaitQuantity = 0;
                                    SupplyPositionStorageRepository.Add(supplyPositionStorage);
                                }
                                else
                                {
                                    supplyPositionStorage.Quantity += taskinfo.Task.TaskQuantity;
                                }
                            }
                        }
                        else
                        {
                            taskinfo.Task.State = "04";                            
                        }

                        TaskRepository.SaveChanges();
                        scope.Complete();
                    }
                    else
                    {
                        error = "当前任务不存在！";
                        return false;
                    }
                }
                return TaskService.FinishTask(taskid, out error);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false ;
            }      
        }
    }
}
