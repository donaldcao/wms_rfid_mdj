using THOK.WCS.REST.Interfaces;
using THOK.WCS.REST.Models;
using Microsoft.Practices.Unity;
using THOK.WCS.Dal.Interfaces;
using THOK.Wms.Dal.Interfaces;
using System;
using System.Transactions;

namespace THOK.WCS.REST.Service
{
    public class TransportService :ITransportService
    {
        [Dependency]
        public ITaskRepository TaskRepository { get; set; }

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

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var taskQuery = TaskRepository.GetQueryable();

                    scope.Complete();
                    return null;
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

                    scope.Complete();
                    return false;
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

                    scope.Complete();
                    return false;
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
