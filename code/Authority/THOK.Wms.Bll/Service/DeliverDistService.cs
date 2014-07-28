using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Authority.DbModel;
using THOK.Authority.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class DeliverDistService : ServiceBase<DeliverDist>, IDeliverDistService
    {
        [Dependency]
        public IDeliverDistRepository DeliverDistRepository { get; set; }
        [Dependency]
        public ICompanyRepository CompanyRepository { get; set; }

        [Dependency]
        public ICompanyRepository CompanysRepository { get; set; }

        [Dependency]
        public ICityRepository CityRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region 增删该查

        public object GetDetails(int page, int rows, string DistCode, string CustomCode, string DistName, string IsActive)
        {
            IQueryable<DeliverDist> DeliverDistQuery = DeliverDistRepository.GetQueryable();
            IQueryable<Company> companyQuery = CompanyRepository.GetQueryable();
            IQueryable<City> cityQuery = CityRepository.GetQueryable();

            var DeliverDist = DeliverDistQuery.Where(c => c.DistCode.Contains(DistCode) &&
                                                          c.DistName.Contains(DistName) &&
                                                          c.IsActive.Contains(IsActive) &&
                                                          c.CustomCode.Contains(CustomCode));

            DeliverDist = DeliverDist.OrderBy(h => h.DeliverOrder);
            int total = DeliverDist.Count();
            DeliverDist = DeliverDist.Skip((page - 1) * rows).Take(rows);
            var cityDetail = cityQuery.FirstOrDefault().CityName;

            var temp = DeliverDist.ToArray().Select(c => new
            {
                DistCode = c.DistCode,
                CustomCode = c.CustomCode,
                c.DistName,
                c.DistCenterCode,
                DistCenterName = companyQuery.FirstOrDefault(a => a.CompanyCode == c.DistCenterCode) != null ? companyQuery.FirstOrDefault(a => a.CompanyCode == c.DistCenterCode).CompanyName : cityDetail + "烟草物流配送中心",
                c.CompanyCode,
                CompanyName = companyQuery.FirstOrDefault(a => a.CompanyCode == c.CompanyCode) != null ? companyQuery.FirstOrDefault(a => a.CompanyCode == c.CompanyCode).CompanyName : cityDetail + "烟草公司",
                UniformCode = c.UniformCode,
                Description = c.Description,
                c.DeliverOrder,
                IsActive = c.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return new { total, rows = temp.ToArray() };
        }

        public bool Add(DeliverDist deliverDist, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var deliver = new DeliverDist();
            if (deliverDist != null)
            {
                try
                {
                    deliver.DistCode = deliverDist.DistCode;
                    deliver.CustomCode = deliverDist.CustomCode;
                    deliver.DistName = deliverDist.DistName;
                    deliver.DistCenterCode = deliverDist.DistCenterCode;
                    deliver.CompanyCode = deliverDist.CompanyCode;
                    deliver.UniformCode = deliverDist.UniformCode;
                    deliver.Description = deliverDist.Description;
                    deliver.IsActive = deliverDist.IsActive;
                    deliver.UpdateTime = DateTime.Now;
                    DeliverDistRepository.Add(deliver);
                    DeliverDistRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.InnerException;
                }
            }
            return result;
        }

        public object S_Details(int page, int rows, string QueryString, string Value)
        {
            string DistName = "";
            string CompanyCode = "";
            if (QueryString == "DistName")
            {
                DistName = Value;
            }
            else
            {
                CompanyCode = Value;
            }
            IQueryable<DeliverDist> deliverQuery = DeliverDistRepository.GetQueryable();
            var deliver = deliverQuery.Where(c => c.DistName.Contains(DistName) && c.CompanyCode.Contains(CompanyCode))
                .OrderBy(c => c.CompanyCode)
                .Select(c => c);
            if (!DistName.Equals(string.Empty))
            {
                deliver = deliver.Where(p => p.DistName == DistName);
            }
            int total = deliver.Count();
            deliver = deliver.Skip((page - 1) * rows).Take(rows);

            var temp = deliver.ToArray().Select(c => new
            {

                DistCode = c.DistCode,
                DistName = c.DistName,
                CompanyCode = c.CompanyCode,
                DistCenterCode = c.DistCenterCode,
                IsActive = c.IsActive == "1" ? "可用" : "不可用"
            });
            return new { total, rows = temp.ToArray() };
        }

        public bool Save(string DistCode, string DeliverOrder, out string strResult)
        {
            strResult = string.Empty;
            try
            {
                var deliver = DeliverDistRepository.GetQueryable()
                    .FirstOrDefault(i => i.DistCode == DistCode);
                deliver.DeliverOrder = Convert.ToInt32(DeliverOrder);
                DeliverDistRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                strResult = "原因：" + ex.InnerException;
            }
            return true;
        }

        public bool Delete(string DistCode)
        {
            var deliver = DeliverDistRepository.GetQueryable()
              .FirstOrDefault(i => i.DistCode == DistCode);
            if (DistCode != null)
            {
                DeliverDistRepository.Delete(deliver);
                DeliverDistRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        #endregion

        public System.Data.DataTable GetDeliverDistInfo(int page, int rows, string DistCode, string CustomCode, string DistName, string IsActive)
        {          
            var DeliverDistQuery = DeliverDistRepository.GetQueryable();
            var companyQuery = CompanyRepository.GetQueryable();
            var cityQuery = CityRepository.GetQueryable();
            var cityDetail = cityQuery.FirstOrDefault().CityName;

            var dist = DeliverDistQuery.Where(a => a.DistCode.Contains(DistCode)).OrderBy(a => a.DeliverOrder).Select(a => a);

            if (!DistName.Equals(string.Empty))
            {
                dist=dist.Where(a => a.DistName.Contains(DistName));
            }
            if (!CustomCode.Equals(string.Empty))
            {
                dist=dist.Where(a => a.CustomCode.Contains(CustomCode));
            }
            if (!IsActive.Equals(string.Empty))
            {
                dist = dist.Where(a => a.IsActive.Contains(IsActive));
            }

            var DeliverDistinfo = dist.ToArray().Select(a => new
            {
                //a.DistCode,
                a.CustomCode,
                a.DistName,
                a.DistCenterCode,
                DistCenterName = companyQuery.FirstOrDefault(c => c.CompanyCode == a.DistCenterCode) != null ? companyQuery.FirstOrDefault(c => c.CompanyCode == a.DistCenterCode).CompanyName : cityDetail + "烟草物流配送中心",
                a.CompanyCode,
                CompanyName = companyQuery.FirstOrDefault(c => c.CompanyCode == a.CompanyCode) != null ? companyQuery.FirstOrDefault(c => c.CompanyCode == a.CompanyCode).CompanyName : cityDetail + "烟草公司",
                a.UpdateTime,
                IsActive = a.IsActive == "1" ? "可用" : "不可用"

            });
            System.Data.DataTable dt = new System.Data.DataTable();
            //dt.Columns.Add("配送区域编码", typeof(string));
            dt.Columns.Add("配送区域名称", typeof(string));
            dt.Columns.Add("自定义编码", typeof(string));
            dt.Columns.Add("配送中心", typeof(string));
            dt.Columns.Add("所属单位", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));

            foreach (var item in DeliverDistinfo)
            {
                dt.Rows.Add(
                    //item.DistCode,
                item.DistName,
                item.CustomCode,
                item.DistCenterName,
                item.CompanyName,
                item.UpdateTime,
                item.IsActive);
            }
            return dt;
        }
    }
}
