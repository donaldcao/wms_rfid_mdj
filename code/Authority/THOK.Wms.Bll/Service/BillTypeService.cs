using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{

    public class BillTypeService : ServiceBase<BillType>, IBillTypeService
    {
        [Dependency]
        public IBillTypeRepository BillTypeRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string billClass, string isActive)
        {
            IQueryable<BillType> query = BillTypeRepository.GetQueryable();
            var v1 = query.Where(a => a.BillClass.Contains(billClass)
                && a.IsActive.Contains(isActive))
                .OrderBy(a => a.BillTypeCode).AsEnumerable()
                .Select(a => new
                {
                    a.BillTypeCode,
                    a.BillTypeName,
                    BillClass = a.BillClass == "0001" ? "入库单" : a.BillClass == "0002" ? "出库单" : a.BillClass == "0003" ? "移库单" : a.BillClass == "0004" ? "盘点单" : a.BillClass == "0005" ? "损益单" : a.BillClass == "0006" ? "分拣单" : "异常",
                    a.Description,
                    IsActive = a.IsActive == "1" ? "可用" : "禁用",
                    UpdateTime = a.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss")
                });
            int total = v1.Count();
            v1 = v1.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = v1.ToArray() };
        }
        
        public new bool Add(BillType entity)
        {
            BillType en = new BillType();
            en.BillTypeCode = entity.BillTypeCode;
            en.BillTypeName = entity.BillTypeName;
            en.BillClass = entity.BillClass;
            en.Description = entity.Description;
            en.IsActive = entity.IsActive;
            en.UpdateTime = DateTime.Now;

            BillTypeRepository.Add(en);
            BillTypeRepository.SaveChanges();
            return true;
        }

        public bool Save(BillType entity)
        {
            BillType en = BillTypeRepository.GetQueryable().FirstOrDefault(b => b.BillTypeCode == entity.BillTypeCode);
            en.BillTypeCode = entity.BillTypeCode;
            en.BillTypeName = entity.BillTypeName;
            en.BillClass = entity.BillClass;
            en.Description = entity.Description;
            en.IsActive = entity.IsActive;
            en.UpdateTime = DateTime.Now;

            BillTypeRepository.SaveChanges();
            return true;
        }
        
        public bool Delete(string billTypeCode)
        {
            BillType billtype = BillTypeRepository.GetQueryable()
                .FirstOrDefault(b => b.BillTypeCode == billTypeCode);
            if (billTypeCode != null)
            {
                BillTypeRepository.Delete(billtype);
                BillTypeRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }
        
        public System.Data.DataTable BillTypeTable(int page, int rows, string billClass, string isActive)
        {
            IQueryable<BillType> query = BillTypeRepository.GetQueryable();
            var v1 = query.Where(a => a.BillClass.Contains(billClass)
                && a.IsActive.Contains(isActive))
                .OrderBy(a => a.BillTypeCode).AsEnumerable()
                .Select(a => new
                {
                    a.BillTypeCode,
                    a.BillTypeName,
                    BillClass = a.BillClass == "0001" ? "入库单" : a.BillClass == "0002" ? "出库单" : a.BillClass == "0003" ? "移库单" : a.BillClass == "0004" ? "盘点单" : a.BillClass == "0005" ? "损益单" : a.BillClass == "0006" ? "分拣单" : "异常",
                    a.Description,
                    IsActive = a.IsActive == "1" ? "可用" : "禁用",
                    UpdateTime = a.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss")
                });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("订单类型编码", typeof(string));
            dt.Columns.Add("订单类型名称", typeof(string));
            dt.Columns.Add("订单类别", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var a in v1)
            {
                dt.Rows.Add
                (
                    a.BillTypeCode,
                    a.BillTypeName,
                    a.BillClass,
                    a.Description,
                    a.IsActive,
                    a.UpdateTime
                );
            }
            return dt;
        }
    }
}
