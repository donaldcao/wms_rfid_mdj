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

namespace THOK.WCS.REST.Service
{
    public class TransportService :ITransportService
    {
        [Dependency]
        public ITaskRepository TaskRepository { get; set; }
        
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
                    var productSizeQuery = ProductSizeRepository.GetQueryable();
                    var storageQuery = StorageRepository.GetQueryable();
                                        
                    var tasks = taskQuery.Join(pathQuery, t => t.PathID, p => p.ID, (t, p) => new { Task = t, Path = p })
                        .Join(positionQuery, r => r.Task.OriginPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = p })
                        .Join(positionQuery, r => r.Task.CurrentPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path,OriginPosition = r.OriginPosition, CurrentPosition = p})
                        .Join(positionQuery, r => r.Task.TargetPositionID, p => p.ID, (r, p) => new { Task = r.Task, Path = r.Path,OriginPosition = r.OriginPosition, CurrentPosition = r.CurrentPosition, TargetPosition = p })
                        //.Join(productSizeQuery, r => r.Task.ProductCode, p => p.ProductCode, (r, p) => new { Task = r.Task, Path = r.Path, OriginPosition = r.OriginPosition, CurrentPosition = r.CurrentPosition, TargetPosition = r.TargetPosition, ProductSize = p })
                        .Where(r => r.Task.State == "01"
                            && r.Task.CurrentPositionState == "02"
                            && r.CurrentPosition.AbleStockOut
                            && r.CurrentPosition.ID != r.TargetPosition.ID
                            && r.CurrentPosition.SRMName.Contains(srmName)
                            && ((new string[] { "01", "05" }).Contains(r.TargetPosition.PositionType) || !r.TargetPosition.HasGoods)
                        )
                        .OrderBy(r=>r.Task.TaskLevel)
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

                        int currentPositionIndex = pathNodePositions.Where(p => p.Value.ID == task.CurrentPosition.ID).Max(p => p.Key);

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

                        //if (task.ProductSize != null)
                        //{
                        //    srmTask.Length = task.ProductSize.Length;
                        //    srmTask.Width = task.ProductSize.Width;
                        //    srmTask.Length = task.ProductSize.Length;
                        //}                          
                        
                        srmTask.TravelPos1 = task.CurrentPosition.TravelPos;
                        srmTask.LiftPos1 = task.CurrentPosition.LiftPos;
                        srmTask.TravelPos2 = task.TargetPosition.TravelPos;
                        srmTask.LiftPos2 = task.TargetPosition.LiftPos;
                        srmTask.RealLiftPos2 = task.TargetPosition.LiftPos;//?

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

                        srmTask.Barcode = "888888"; //?
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
                using (TransactionScope scope = new TransactionScope())
                {
                    var taskQuery = TaskRepository.GetQueryable();
                    var task = taskQuery.Where(t => t.ID == taskid).FirstOrDefault();
                    if (task != null)
                    {
                        task.State = "04";
                        TaskRepository.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false ;
            }      
        }
    }
}
