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

        public object GetDetails(int page, int rows, SupplyPosition entity)
        {
            IQueryable<SupplyPosition> query = SupplyPositionRepository.GetQueryable();
            if (!string.IsNullOrEmpty(entity.PositionName))
            {
                query = query.Where(a => a.PositionName.Contains(entity.PositionName));
            }
            IQueryable<SupplyPosition> query1 = query;
            if (!string.IsNullOrEmpty(entity.PositionType))
            {
                query1 = query.Where(a => a.PositionType == entity.PositionType);
            }
            IQueryable<SupplyPosition> supplyPositionQuery2 = query1;
            if (!string.IsNullOrEmpty(entity.ProductCode))
            {
                supplyPositionQuery2 = query1.Where(a => a.ProductCode == entity.ProductCode);
            }
            IQueryable<SupplyPosition> query3 = supplyPositionQuery2;
            if (!string.IsNullOrEmpty(entity.ProductName))
            {
                query3 = supplyPositionQuery2.Where(a => a.ProductName.Contains(entity.ProductName));
            }
            var v1 = query3.ToArray().Select(a => new
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
        public object GetDetails(int page, int rows, string QueryString, string Value)
        {
            string id = ""; string positionName = "";

            if (QueryString == "Id")
            {
                id = Value;
            }
            else
            {
                positionName = Value;
            }
            IQueryable<SupplyPosition> query = SupplyPositionRepository.GetQueryable();
            var v1 = query.Where(a => a.Id.ToString().Contains(id) && a.PositionName.Contains(positionName))
                .OrderBy(a => a.Id)
                .Select(a => new
                {
                    a.Id,
                    a.PositionName
                });
            int total = query.Count();
            v1 = v1.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = v1.ToArray() };
        }

        public bool Add(SupplyPosition entity, out string strResult)
        {
            bool result = false;
            strResult = null;
            SupplyPosition en = new SupplyPosition();
            en.PositionName = entity.PositionName;
            en.PositionType = entity.PositionType;
            en.ProductCode = entity.ProductCode;
            en.ProductName = entity.ProductName;
            en.PositionAddress = entity.PositionAddress;
            en.PositionCapacity = entity.PositionCapacity;
            en.SortingLineCodes = entity.SortingLineCodes;
            en.TargetSupplyAddresses = entity.TargetSupplyAddresses;
            en.Description = entity.Description;
            en.IsActive = entity.IsActive;
            SupplyPositionRepository.Add(en);
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

        public bool Save(SupplyPosition entity, out string strResult)
        {
            bool result = false;
            strResult = null;

            SupplyPosition en = SupplyPositionRepository.GetQueryable().FirstOrDefault(a => a.Id == entity.Id);
            if (entity != null)
            {
                en.Id = entity.Id;
                en.PositionName = entity.PositionName;
                en.PositionType = entity.PositionType;
                en.ProductCode = entity.ProductCode;
                en.ProductName = entity.ProductName;
                en.PositionAddress = entity.PositionAddress;
                en.PositionCapacity = entity.PositionCapacity;
                en.SortingLineCodes = entity.SortingLineCodes;
                en.TargetSupplyAddresses = entity.TargetSupplyAddresses;
                en.Description = entity.Description;
                en.IsActive = entity.IsActive;
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

            SupplyPosition en = SupplyPositionRepository.GetQueryable().FirstOrDefault(a => a.Id == id);
            if (en != null)
            {
                try
                {
                    SupplyPositionRepository.Delete(en);
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

        public DataTable GetTable(int page, int rows, SupplyPosition entity)
        {
            IQueryable<SupplyPosition> query = SupplyPositionRepository.GetQueryable();
            if (!string.IsNullOrEmpty(entity.PositionName))
            {
                query = query.Where(a => a.PositionName.Contains(entity.PositionName));
            }
            IQueryable<SupplyPosition> query1 = query;
            if (!string.IsNullOrEmpty(entity.PositionType))
            {
                query1 = query.Where(a => a.PositionType == entity.PositionType);
            }
            IQueryable<SupplyPosition> query2 = query1;
            if (!string.IsNullOrEmpty(entity.ProductCode))
            {
                query2 = query1.Where(a => a.ProductCode == entity.ProductCode);
            }
            IQueryable<SupplyPosition> query3 = query2;
            if (!string.IsNullOrEmpty(entity.ProductName))
            {
                query3 = query2.Where(a => a.ProductName.Contains(entity.ProductName));
            }
            var v1 = query3.ToArray().Select(a => new
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
            foreach (var a in v1)
            {
                dt.Rows.Add
                (
                    a.Id,
                    a.PositionName,
                    a.PositionType,
                    a.ProductCode,
                    a.ProductName,
                    a.PositionAddress,
                    a.PositionCapacity,
                    a.SortingLineCodes,
                    a.TargetSupplyAddresses,
                    a.Description,
                    a.IsActive
                );
            }
            return dt;
        }
    }
}
