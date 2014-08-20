using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using THOK.SMS.DbModel;
using THOK.SMS.Bll.Interfaces;
using THOK.SMS.Dal.Interfaces;
using Microsoft.Practices.Unity;

namespace THOK.SMS.Bll.Service
{
    public class SupplyPositionService : ServiceBase<SupplyPosition>, ISupplyPositionService
    {
        [Dependency]
        public ISupplyPositionRepository SupplyPositionRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, SupplyPosition supplyPosition)
        {
            IQueryable<SupplyPosition> supplyPositionQuery = SupplyPositionRepository.GetQueryable();
            if (!string.IsNullOrEmpty(supplyPosition.PositionName))
            {
                supplyPositionQuery = supplyPositionQuery.Where(a => a.PositionName.Contains(supplyPosition.PositionName));
            }
            IQueryable<SupplyPosition> supplyPositionQuery1 = supplyPositionQuery;
            if (!string.IsNullOrEmpty(supplyPosition.PositionType))
            {
                supplyPositionQuery1 = supplyPositionQuery.Where(a => a.PositionType == supplyPosition.PositionType);
            }
            IQueryable<SupplyPosition> supplyPositionQuery2 = supplyPositionQuery1;
            if (!string.IsNullOrEmpty(supplyPosition.ProductCode))
            {
                supplyPositionQuery2 = supplyPositionQuery1.Where(a => a.ProductCode == supplyPosition.ProductCode);
            }
            IQueryable<SupplyPosition> supplyPositionQuery3 = supplyPositionQuery2;
            if (!string.IsNullOrEmpty(supplyPosition.ProductName))
            {
                supplyPositionQuery3 = supplyPositionQuery2.Where(a => a.ProductName.Contains(supplyPosition.ProductName));
            }
            var v1 = supplyPositionQuery3.ToArray().Select(a => new
            {
                a.Id,
                a.PositionName,
                PositionType = a.PositionType == "01" ? "正常拆盘位置" : a.PositionType == "02" ? "混合拆盘位置" : "异常",
                a.ProductCode,
                a.ProductName,
                a.PositionAddress,
                a.PositionCapacity,
                a.SortingLineCodes,
                a.TargetSupplyAddresses,
                a.Description,
                IsActive = a.IsActive == "0" ? "禁用" : a.IsActive == "1" ? "可用" : "异常"
            });
            int total = v1.Count();
            var v2 = v1.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = v2 };
        }

        public bool Add(SupplyPosition supplyPosition, out string strResult)
        {
            bool result = false;
            strResult = null;
            SupplyPosition sp = new SupplyPosition();
            sp.PositionName = supplyPosition.PositionName;
            sp.PositionType = supplyPosition.PositionType;
            sp.ProductCode = supplyPosition.ProductCode;
            sp.ProductName = supplyPosition.ProductName;
            sp.PositionAddress = supplyPosition.PositionAddress;
            sp.PositionCapacity = supplyPosition.PositionCapacity;
            sp.SortingLineCodes = supplyPosition.SortingLineCodes;
            sp.TargetSupplyAddresses = supplyPosition.TargetSupplyAddresses;
            sp.Description = supplyPosition.Description;
            sp.IsActive = supplyPosition.IsActive;
            SupplyPositionRepository.Add(sp);
            try
            {
                SupplyPositionRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                strResult = ex.Message;
            }
            return result;
        }

        public bool Save(SupplyPosition supplyPosition, out string strResult)
        {
            bool result = false;
            strResult = null;

            SupplyPosition supplyPositionQuery = SupplyPositionRepository.GetQueryable().FirstOrDefault(a => a.Id == supplyPosition.Id);
            SupplyPosition sp = new SupplyPosition();

            if (supplyPosition != null)
            {
                sp.Id = supplyPosition.Id;
                sp.PositionName = supplyPosition.PositionName;
                sp.PositionType = supplyPosition.PositionType;
                sp.ProductCode = supplyPosition.ProductCode;
                sp.ProductName = supplyPosition.ProductName;
                sp.PositionAddress = supplyPosition.PositionAddress;
                sp.PositionCapacity = supplyPosition.PositionCapacity;
                sp.SortingLineCodes = supplyPosition.SortingLineCodes;
                sp.TargetSupplyAddresses = supplyPosition.TargetSupplyAddresses;
                sp.Description = supplyPosition.Description;
                sp.IsActive = supplyPosition.IsActive;
                SupplyPositionRepository.Add(sp);
            }
            try
            {
                SupplyPositionRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                strResult = ex.Message;
            }
            return result;
        }

        public bool Delete(int id, out string strResult)
        {
            bool result = false;
            strResult = null;

            var SupplyPositionQuery = SupplyPositionRepository.GetQueryable().FirstOrDefault(a => a.Id == id);
            if (SupplyPositionQuery != null)
            {
                try
                {
                    SupplyPositionRepository.Delete(SupplyPositionQuery);
                    SupplyPositionRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = ex.Message;
                }
            }
            return result;
        }

        public DataTable GetTable(int page, int rows, SupplyPosition supplyPosition)
        {
            IQueryable<SupplyPosition> supplyPositionQuery = SupplyPositionRepository.GetQueryable();
            if (!string.IsNullOrEmpty(supplyPosition.PositionName))
            {
                supplyPositionQuery = supplyPositionQuery.Where(a => a.PositionName.Contains(supplyPosition.PositionName));
            }
            IQueryable<SupplyPosition> supplyPositionQuery1 = supplyPositionQuery;
            if (!string.IsNullOrEmpty(supplyPosition.PositionType))
            {
                supplyPositionQuery1 = supplyPositionQuery.Where(a => a.PositionType == supplyPosition.PositionType);
            }
            IQueryable<SupplyPosition> supplyPositionQuery2 = supplyPositionQuery1;
            if (!string.IsNullOrEmpty(supplyPosition.ProductCode))
            {
                supplyPositionQuery2 = supplyPositionQuery1.Where(a => a.ProductCode == supplyPosition.ProductCode);
            }
            IQueryable<SupplyPosition> supplyPositionQuery3 = supplyPositionQuery2;
            if (!string.IsNullOrEmpty(supplyPosition.ProductName))
            {
                supplyPositionQuery3 = supplyPositionQuery2.Where(a => a.ProductName.Contains(supplyPosition.ProductName));
            }
            var v1 = supplyPositionQuery3.ToArray().Select(a => new
            {
                a.Id,
                a.PositionName,
                PositionType = a.PositionType == "01" ? "正常拆盘位置" : a.PositionType == "02" ? "混合拆盘位置" : "异常",
                a.ProductCode,
                a.ProductName,
                a.PositionAddress,
                a.PositionCapacity,
                a.SortingLineCodes,
                a.TargetSupplyAddresses,
                a.Description,
                IsActive = a.IsActive == "0" ? "禁用" : a.IsActive == "1" ? "可用" : "异常"
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("编号", typeof(string));
            dt.Columns.Add("位置名称", typeof(string));
            dt.Columns.Add("位置类型", typeof(string));
            dt.Columns.Add("商品编码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("位置地址", typeof(string));
            dt.Columns.Add("位置容量", typeof(string));
            dt.Columns.Add("可补货分拣线", typeof(string));
            dt.Columns.Add("可补货目标地址", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("状态", typeof(string));
            foreach (var v in v1)
            {
                dt.Rows.Add
                    (
                        v.Id,
                        v.PositionName,
                        v.PositionType,
                        v.ProductCode,
                        v.ProductName,
                        v.PositionAddress,
                        v.PositionCapacity,
                        v.SortingLineCodes,
                        v.TargetSupplyAddresses,
                        v.Description,
                        v.IsActive
                    );
            }
            return dt;
        }
    }
}
