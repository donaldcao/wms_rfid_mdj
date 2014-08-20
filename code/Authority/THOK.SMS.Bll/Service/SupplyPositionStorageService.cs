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
    public class SupplyPositionStorageService : ServiceBase<SupplyPositionStorage>, ISupplyPositionStorageService
    {
        [Dependency]
        public ISupplyPositionStorageRepository SupplyPositionStorageRepository { get; set; }
        [Dependency]
        public ISupplyPositionRepository SupplyPositionRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, SupplyPositionStorage entity)
        {
            IQueryable<SupplyPositionStorage> query = SupplyPositionStorageRepository.GetQueryable();
            IQueryable<SupplyPosition> queryN1 = SupplyPositionRepository.GetQueryable();

            if (!string.IsNullOrEmpty(entity.ProductCode))
            {
                query = query.Where(a => a.ProductCode == entity.ProductCode);
            }
            IQueryable<SupplyPositionStorage> query1 = query;
            if (!string.IsNullOrEmpty(entity.ProductName))
            {
                query1 = query.Where(a => a.ProductName.Contains(entity.ProductName));
            }
            var v1 = query1.ToArray().Join(queryN1, a => a.PositionID, b => b.Id, (a, b) => new
            {
                a.Id,
                a.PositionID,
                a.ProductCode,
                a.ProductName,
                a.Quantity,
                a.WaitQuantity,
                b.PositionName
            });
            int total = v1.Count();
            var v2 = v1.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = v2 };
        }

        public bool Add(SupplyPositionStorage entity, out string strResult)
        {
            bool result = false;
            strResult = null;
            SupplyPositionStorage en = new SupplyPositionStorage();
            en.PositionID = entity.PositionID;
            en.ProductCode = entity.ProductCode;
            en.ProductName = entity.ProductName;
            en.Quantity = entity.Quantity;
            en.WaitQuantity = entity.WaitQuantity;
            SupplyPositionStorageRepository.Add(en);
            try
            {
                SupplyPositionStorageRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                strResult = ex.Message;
            }
            return result;
        }

        public bool Save(SupplyPositionStorage entity, out string strResult)
        {
            bool result = false;
            strResult = null;

            SupplyPositionStorage en = SupplyPositionStorageRepository.GetQueryable().FirstOrDefault(a => a.Id == entity.Id);
            if (en != null)
            {
                en.Id = entity.Id;
                en.PositionID = entity.PositionID;
                en.ProductCode = entity.ProductCode;
                en.ProductName = entity.ProductName;
                en.Quantity = entity.Quantity;
                en.WaitQuantity = entity.WaitQuantity;
            }
            try
            {
                SupplyPositionStorageRepository.SaveChanges();
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

            SupplyPositionStorage en = SupplyPositionStorageRepository.GetQueryable().FirstOrDefault(a => a.Id == id);
            if (en != null)
            {
                try
                {
                    SupplyPositionStorageRepository.Delete(en);
                    SupplyPositionStorageRepository.SaveChanges();
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
            IQueryable<SupplyPositionStorage> query = SupplyPositionStorageRepository.GetQueryable();
            if (!string.IsNullOrEmpty(entity.ProductCode))
            {
                query = query.Where(a => a.ProductCode == entity.ProductCode);
            }
            IQueryable<SupplyPositionStorage> query1 = query;
            if (!string.IsNullOrEmpty(entity.ProductName))
            {
                query1 = query.Where(a => a.ProductName == entity.ProductName);
            }

            var v1 = query1.ToArray().Select(a => new
            {
                a.Id,
                a.PositionID,
                a.ProductCode,
                a.ProductName,
                a.Quantity,
                a.WaitQuantity
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("编号", typeof(string));
            dt.Columns.Add("位置编号", typeof(string));
            dt.Columns.Add("商品编码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("库存数量", typeof(string));
            dt.Columns.Add("待入数量", typeof(string));
            foreach (var a in v1)
            {
                dt.Rows.Add
                (
                    a.Id,
                    a.PositionID,
                    a.ProductCode,
                    a.ProductName,
                    a.Quantity,
                    a.WaitQuantity
                );
            }
            return dt;
        }
    }
}
