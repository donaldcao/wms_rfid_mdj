using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Download.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Wms.Bll.Service
{
    class DeliverLineService : ServiceBase<DeliverLine>, IDeliverLineService
    {

        [Dependency]
        public IDeliverLineRepository DeliverLineRepository { get; set; }

        [Dependency]
        public IDeliverLineDownService DeliverLineDownService { get; set; }

        [Dependency]
        public IDeliverDistRepository DeliverDistRepository { get; set; }

        [Dependency]
        public ICityRepository CityRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public bool DownDeliverLine(out string errorInfo)
        {
            errorInfo = string.Empty;
            bool result = false;
            try
            {
                var deliverLineCodes = DeliverLineRepository.GetQueryable().Where(d => d.DeliverLineCode == d.DeliverLineCode).Select(s => new { s.DeliverLineCode }).ToArray();
                string deliverStrs = "";
                for (int i = 0; i < deliverLineCodes.Length; i++)
                {
                    deliverStrs += deliverLineCodes[i].DeliverLineCode + ",";
                }
                DeliverLine[] deliverLines = DeliverLineDownService.GetDeliverLine(deliverStrs);
                foreach (var item in deliverLines)
                {
                    var deliverLine = new DeliverLine();
                    deliverLine.DeliverLineCode = item.DeliverLineCode;
                    deliverLine.DeliverLineName = item.DeliverLineName;
                    deliverLine.DeliverOrder = item.DeliverOrder;
                    deliverLine.Description = item.Description;
                    deliverLine.DistCode = item.DistCode;
                    deliverLine.CustomCode = item.CustomCode;
                    deliverLine.IsActive = item.IsActive;
                    deliverLine.UpdateTime = item.UpdateTime;
                    DeliverLineRepository.Add(deliverLine);
                }
                DeliverLineRepository.SaveChanges();
                result = true;
            }
            catch (Exception e)
            {
                errorInfo = "出错，原因:" + e.Message;
            }
            return result;
        }


        #region IDeliverLineService 成员


        public object GetDetails(int page, int rows, string DeliverLineCode, string CustomCode, string DeliverLineName, string DistCode, string DeliverOrder, string IsActive)
        {
            IQueryable<DeliverLine> deliverLineQuery = DeliverLineRepository.GetQueryable();
            IQueryable<DeliverDist> deliverDistQuery = DeliverDistRepository.GetQueryable();
            IQueryable<City> cityQuery = CityRepository.GetQueryable();
            var deliverLine = deliverLineQuery.Where(c => c.DeliverLineCode.Contains(DeliverLineCode) &&
                                                          c.DeliverLineName.Contains(DeliverLineName) &&
                                                          c.IsActive.Contains(IsActive));
            if (!CustomCode.Equals(string.Empty))
            {
                deliverLine = deliverLine.Where(d => d.CustomCode == CustomCode);
            }
            if (!DistCode.Equals(string.Empty))
            {
                deliverLine = deliverLine.Where(d => d.DistCode == DistCode);
            }
            int total = deliverLine.Count();

            var cityDetail = cityQuery.FirstOrDefault().CityName;
            var temp = deliverLine.OrderBy(d => d.DeliverOrder).ToArray().Skip((page - 1) * rows).Take(rows).Select(c => new
            {
                DeliverLineCode = c.DeliverLineCode,
                CustomCode = c.CustomCode,
                DeliverLineName = c.DeliverLineName,
                c.DistCode,
                DistName = deliverDistQuery.FirstOrDefault(a => a.DistCode == c.DistCode) != null ? deliverDistQuery.FirstOrDefault(a => a.DistCode == c.DistCode).DistName : cityDetail,
                DeliverOrder = c.DeliverOrder,
                Description = c.Description,
                IsActive = c.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return new { total, rows = temp.ToArray() };
        }

        #endregion

        #region IDeliverLineService 成员


        public bool Add(DeliverLine deliverLine, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var deliver_line = new DeliverLine();
            if (deliverLine != null)
            {
                try
                {
                    deliver_line.DeliverLineCode = deliverLine.DeliverLineCode;
                    deliver_line.CustomCode = deliverLine.CustomCode;
                    deliver_line.DeliverLineName = deliverLine.DeliverLineName;
                    deliver_line.DistCode = deliverLine.DistCode;
                    deliver_line.DeliverOrder = deliverLine.DeliverOrder;
                    deliver_line.Description = deliverLine.Description;
                    deliver_line.IsActive = deliverLine.IsActive;
                    deliver_line.UpdateTime = DateTime.Now;

                    DeliverLineRepository.Add(deliver_line);
                    DeliverLineRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.InnerException;
                }
            }
            return result;
        }

        #endregion

        #region IDeliverLineService 成员
        public object C_Details(int page, int rows, string QueryString, string Value)
        {
            string DeliverLineCode = "";
            string DeliverLineName = "";
            if (QueryString == "DeliverLineCode")
            {
                DeliverLineCode = Value;
            }
            else
            {
                DeliverLineName = Value;
            }
            var deliverLineQuery = DeliverLineRepository.GetQueryable();
            var deliverLine = deliverLineQuery.Where(c => c.DeliverLineCode.Contains(DeliverLineCode) && c.DeliverLineName.Contains(DeliverLineName))
                .OrderBy(c => c.DistCode)
                .Select(c => c);
            int total = deliverLine.Count();
            deliverLine = deliverLine.Skip((page - 1) * rows).Take(rows);

            var temp = deliverLine.ToArray().Select(c => new
            {
                c.DeliverLineCode,
                c.DeliverLineName,
                c.DeliverOrder,
                IsActive = c.IsActive == "1" ? "可用" : "不可用"
            });
            return new { total, rows = temp.ToArray() };
        }

        public object D_Details(int page, int rows, string QueryString, string Value)
        {
            string DistCode = "";
            string DistName = "";
            if (QueryString == "DistCode")
            {
                DistCode = Value;
            }
            else
            {
                DistName = Value;
            }
            IQueryable<DeliverDist> deliverQuery = DeliverDistRepository.GetQueryable();
            var deliver = deliverQuery.Where(c => c.DistName.Contains(DistCode) && c.CompanyCode.Contains(DistName))
                .OrderBy(c => c.DistCode)
                .Select(c => c);
            if (!DistName.Equals(string.Empty))
            {
                deliver = deliver.Where(p => p.DistCode == DistCode);
            }
            int total = deliver.Count();
            deliver = deliver.Skip((page - 1) * rows).Take(rows);

            var temp = deliver.ToArray().Select(c => new
            {

                DistCode = c.DistCode,
                CustomCode = c.CustomCode,
                DistName = c.DistName,
                DistCenterCode = c.DistCenterCode,
                CompanyCode = c.CompanyCode,
                IsActive = c.IsActive == "1" ? "可用" : "不可用"
            });
            return new { total, rows = temp.ToArray() };
        }

        #endregion

        #region IDeliverLineService 成员

        public bool Edit(string DeliverLineCode, string DeliverOrder, out string strResult)
        {
            strResult = string.Empty;
            try
            {
                var deliver_line = DeliverLineRepository.GetQueryable().FirstOrDefault(i => i.DeliverLineCode == DeliverLineCode);
                deliver_line.DeliverOrder = Convert.ToInt32(DeliverOrder);

                DeliverLineRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                strResult = "原因：" + ex.InnerException;
                return false;

            }
        }

        #endregion

        #region IDeliverLineService 成员


        public bool Delete(string DeliverLineCode)
        {
            var deliver_Line = DeliverLineRepository.GetQueryable()
               .FirstOrDefault(i => i.DeliverLineCode == DeliverLineCode);
            if (DeliverLineCode != null)
            {
                DeliverLineRepository.Delete(deliver_Line);
                DeliverLineRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        #endregion


        public System.Data.DataTable GetDeliverLineInfo(int page, int rows,string DeliverLineCode,string DistCode,string CustomCode,string IsActive,string DeliverLinename)
        {
          

            var deliverLineQuery = DeliverLineRepository.GetQueryable();
            var deliverDistQuery = DeliverDistRepository.GetQueryable();
            var cityQuery = CityRepository.GetQueryable();
            var cityDetail = cityQuery.FirstOrDefault().CityName;

            var deliverLine = deliverLineQuery.Where(a => a.DeliverLineCode.Contains(DeliverLineCode) && a.DistCode.Contains(DistCode)).OrderBy(a => a.DeliverLineCode).Select(a => a);
           
            //if (!DistCode.Equals(string.Empty))
            //{
            //    deliverLineInfo = deliverLineInfo.Where(a => a.DistCode.Contains(DistCode));
            //}
            if (!CustomCode.Equals(string.Empty))
            {
                deliverLine = deliverLine.Where(a => a.CustomCode.Contains(CustomCode));
            }
            if (!IsActive.Equals(string.Empty))
            {
                deliverLine = deliverLine.Where(a => a.IsActive.Contains(IsActive));
            }
            if (!DeliverLinename.Equals(string.Empty))
            {
                deliverLine = deliverLine.Where(a => a.DeliverLineName.Contains(DeliverLinename));
            }
            var DeliverLineInfoDetail = deliverLine.ToArray().Select(a => new
            {
                a.DeliverLineCode,
                a.CustomCode,
                a.DeliverLineName,
                a.DistCode,
                DistName = deliverDistQuery.FirstOrDefault(b => b.DistCode == a.DistCode) != null ? deliverDistQuery.FirstOrDefault(b => b.DistCode == a.DistCode).DistName : cityDetail + "烟草配送中心",
                a.DeliverOrder,
                a.UpdateTime,
                IsActive = a.IsActive == "1" ? "可用" : "不可用"

            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("送货线路编码", typeof(string));
            dt.Columns.Add("送货线路名称", typeof(string));
            dt.Columns.Add("配送区域名称", typeof(string));
            dt.Columns.Add("自定义编码", typeof(string));
            dt.Columns.Add("送货线路顺序", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));

            foreach (var item in DeliverLineInfoDetail)
            {
                dt.Rows.Add(
                item.DeliverLineCode,
                item.DeliverLineName,
                item.DistName,
                item.CustomCode,
                item.DeliverOrder,
                item.UpdateTime,
                item.IsActive );
            }
            return dt;
        }
    }
}
