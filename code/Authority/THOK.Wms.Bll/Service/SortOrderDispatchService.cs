﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class SortOrderDispatchService : ServiceBase<SortOrderDispatch>, ISortOrderDispatchService
    {
        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISortOrderDispatchService 成员

        public object GetDetails(int page, int rows, string OrderDate,string WorkStatus, string SortStatus,string SortingLineCode)
        {
            var sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortDispatch = sortDispatchQuery.Where(s => s.ID == s.ID);
            if (WorkStatus != string.Empty || WorkStatus != "")
            {
                sortDispatch = sortDispatchQuery.Where(s => s.WorkStatus ==WorkStatus);
            }

            if (SortStatus != string.Empty || SortStatus != "")
            {
                sortDispatch = sortDispatchQuery.Where(s => s.SortStatus == SortStatus);
            }
            if (OrderDate != string.Empty && OrderDate != null)
            {
                OrderDate = Convert.ToDateTime(OrderDate).ToString("yyyyMMdd");
                sortDispatch = sortDispatch.Where(s => s.OrderDate == OrderDate);
            }
            if (SortingLineCode != string.Empty && SortingLineCode != null)
            {
                sortDispatch = sortDispatch.Where(s => s.SortingLineCode == SortingLineCode);
            }
            var temp = sortDispatch.OrderBy(b => b.OrderDate).AsEnumerable().Select(b => new
           {
               b.ID,
               b.SortingLineCode,
               b.SortingLine.SortingLineName,
               b.OrderDate,
               b.DeliverLineCode,
               SortStatus=b.SortStatus=="1"?"未分拣":"已分拣",
               WorkStatus = b.WorkStatus == "1" ? "未作业" : "已作业",
               b.DeliverLine.DeliverLineName,
               IsActive = b.IsActive == "1" ? "可用" : "不可用",
               UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
           });

            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        public bool Add(string DeliverLineCodes, string orderDate)
        {
            IQueryable<SortingLine> sortingLineQuery = SortingLineRepository.GetQueryable();
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            bool bResult = false;
            if (orderDate == string.Empty || orderDate == null)
            {
                return bResult;
            }
            else
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
            }
            var sortOder = SortOrderRepository.GetQueryable().Where(s => s.OrderDate == orderDate)
                                              .GroupBy(s => new { s.DeliverLineCode, s.OrderDate })
                                              .Select(s => new { DeliverLineCode = s.Key.DeliverLineCode, OrderDate = s.Key.OrderDate ,Quantity=s.Sum(a=>a.QuantitySum)})
                                              .OrderByDescending(a=>a.Quantity);
            try
            {
                var sortingLineArray = sortingLineQuery.Where(a => a.ProductType.Equals("1") && a.IsActive.Equals("1"));
                foreach (var item in sortOder.Where(s=>DeliverLineCodes.Contains(s.DeliverLineCode)).OrderByDescending(s=>s.Quantity).ToArray())
                {
                    var sortDispatchArray=sortDispatchQuery.Where(b => b.OrderDate.Equals(orderDate) && b.SortStatus.Equals("1") && b.IsActive.Equals("1"))
                                .Join(sortOder,
                                so => so.DeliverLineCode,
                                sd => sd.DeliverLineCode,
                                (so, sd) => new {so.OrderDate,so.SortingLineCode,so.SortStatus,so.IsActive,sd.Quantity })
                                .GroupBy(b => new { b.OrderDate, b.SortingLineCode, b.SortStatus, b.IsActive })
                                .Select(b => new { b.Key.SortingLineCode, Quantity = b.Sum(s=>s.Quantity) });
                    var sortingLineCode = sortingLineArray.Select(a => new
                    {
                        a.SortingLineCode,
                        Quantity = sortDispatchArray.FirstOrDefault(b => b.SortingLineCode.Equals(a.SortingLineCode))==null ? 0 : sortDispatchArray.FirstOrDefault(b => b.SortingLineCode.Equals(a.SortingLineCode)).Quantity
                    }).OrderBy(a => a.Quantity).ToArray();
                    if (sortingLineCode.Count() <= 0)
                    {
                        return bResult;
                    }
                    var sortOrderDispatch = new SortOrderDispatch();
                    sortOrderDispatch.SortingLineCode = sortingLineCode[0].SortingLineCode;
                    sortOrderDispatch.DeliverLineCode = item.DeliverLineCode;
                    sortOrderDispatch.WorkStatus = "1";
                    sortOrderDispatch.OrderDate = orderDate;
                    sortOrderDispatch.IsActive = "1";
                    sortOrderDispatch.UpdateTime = DateTime.Now;
                    sortOrderDispatch.SortStatus = "1";

                    SortOrderDispatchRepository.Add(sortOrderDispatch);
                    SortOrderDispatchRepository.SaveChanges();
                }
                bResult = true;
            }
            catch
            { }
            return bResult;
        }

        public bool Add(string SortingLineCode, string DeliverLineCodes, string orderDate)
        {
            if (orderDate == string.Empty || orderDate == null)
            {
                orderDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
            }

            var sortOder = SortOrderRepository.GetQueryable().Where(s => DeliverLineCodes.Contains(s.DeliverLineCode) && s.OrderDate == orderDate)
                                              .GroupBy(s => new { s.DeliverLineCode, s.OrderDate })
                                              .Select(s => new { DeliverLineCode = s.Key.DeliverLineCode, OrderDate = s.Key.OrderDate });
            foreach (var item in sortOder.ToArray())
            {
                var sortOrderDispatch = new SortOrderDispatch();
                sortOrderDispatch.SortingLineCode = SortingLineCode;
                sortOrderDispatch.DeliverLineCode = item.DeliverLineCode;
                sortOrderDispatch.WorkStatus = "1";
                sortOrderDispatch.OrderDate = item.OrderDate;
                sortOrderDispatch.IsActive = "1";
                sortOrderDispatch.UpdateTime = DateTime.Now;
                sortOrderDispatch.SortStatus = "1";

                SortOrderDispatchRepository.Add(sortOrderDispatch);
            }
            SortOrderDispatchRepository.SaveChanges();
            return true;
        }

        public bool Delete(string id, out string errorInfo)
        {
            int ID = Convert.ToInt32(id);
            var sortOrderDispatch = SortOrderDispatchRepository.GetQueryable()
               .FirstOrDefault(s => s.ID == ID);
            if (sortOrderDispatch.WorkStatus == "1")
            {
                if (sortOrderDispatch.SortStatus == "1")
                {
                    if (sortOrderDispatch != null && sortOrderDispatch.SortBatchAbnormalId == 0 && sortOrderDispatch.SortBatchManualId == 0 && sortOrderDispatch.SortBatchPiecesId == 0)
                    {
                        SortOrderDispatchRepository.Delete(sortOrderDispatch);
                        SortOrderDispatchRepository.SaveChanges();
                        errorInfo = "删除成功！";
                        return true;
                    }
                    else
                    {
                        errorInfo = "删除失败！该线路已作业调度";
                        return false;
                    }
                }
                else
                {
                    errorInfo = "删除失败！该线路已分拣";
                    return false;
                }
            }
            else
            {
                errorInfo = "删除失败！该线路已作业"; 
                return false;
            }
        }

        public bool Save(SortOrderDispatch sortDispatch)
        {
            var sortOrderDispatch = SortOrderDispatchRepository.GetQueryable().FirstOrDefault(s => s.ID == sortDispatch.ID);
            sortOrderDispatch.SortingLineCode = sortDispatch.SortingLineCode;
            sortOrderDispatch.DeliverLineCode = sortDispatch.DeliverLineCode;
            sortOrderDispatch.WorkStatus = "1";
            sortOrderDispatch.OrderDate = sortDispatch.OrderDate;
            sortOrderDispatch.IsActive = sortDispatch.IsActive;
            sortOrderDispatch.UpdateTime = DateTime.Now;

            SortOrderDispatchRepository.SaveChanges();
            return true;
        }
        public bool Edit(string id, string SortingLineCode)
        {
            int Id = int.Parse(id);
            var sortOrderDispatch = SortOrderDispatchRepository.GetQueryable().FirstOrDefault(s => s.ID == Id);
            if (sortOrderDispatch.WorkStatus != "1" && sortOrderDispatch.SortStatus != "1")
            {
                sortOrderDispatch.SortingLineCode = SortingLineCode;
                sortOrderDispatch.UpdateTime = DateTime.Now;

                SortOrderDispatchRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public object GetWorkStatus()
        {
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();

            var temp = sortDispatchQuery.Where(s => s.WorkStatus == "1").ToArray().AsEnumerable()
                                           .Join(sortOrderQuery,
                                                dp => new { dp.OrderDate, dp.DeliverLineCode },
                                                om => new { om.OrderDate, om.DeliverLineCode },
                                                (dp, om) => new
                                                {
                                                    dp.ID,
                                                    dp.OrderDate,
                                                    dp.SortingLine,
                                                    dp.DeliverLine,
                                                    dp.IsActive,
                                                    dp.UpdateTime,
                                                    dp.WorkStatus,
                                                    om.QuantitySum
                                                }
                                           ).Select(r => new { r.ID, r.OrderDate, r.SortingLine, r.DeliverLine, r.WorkStatus, r.IsActive, r.UpdateTime, r.QuantitySum })
                                            .GroupBy(g => new { g.ID, g.OrderDate, g.DeliverLine, g.SortingLine, g.IsActive, g.UpdateTime, g.WorkStatus })
                                            .Select(r => new
                                            {
                                                r.Key.ID,
                                                r.Key.OrderDate,
                                                r.Key.SortingLine.SortingLineCode,
                                                r.Key.SortingLine.SortingLineName,
                                                r.Key.DeliverLine.DeliverLineCode,
                                                r.Key.DeliverLine.DeliverLineName,
                                                IsActive = r.Key.IsActive == "1" ? "可用" : "不可用",
                                                UpdateTime = r.Key.UpdateTime.ToString("yyyy-MM-dd"),
                                                WorkStatus = r.Key.WorkStatus == "1" ? "未作业" : "已作业",
                                                QuantitySum = r.Sum(q => q.QuantitySum)
                                            });
            return temp.ToArray();
        }

        public object GetBatchStatus()
        {
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();

            var temp = sortDispatchQuery.Where(s => s.SortBatchId.Equals(0) && s.SortBatchAbnormalId.Equals(0) && s.SortBatchPiecesId.Equals(0) && s.SortBatchManualId.Equals(0) && s.SortStatus.Equals("1")).ToArray().AsEnumerable()
                                           .Join(sortOrderQuery,
                                                dp => new { dp.OrderDate, dp.DeliverLineCode },
                                                om => new { om.OrderDate, om.DeliverLineCode },
                                                (dp, om) => new
                                                {
                                                    dp.ID,
                                                    dp.OrderDate,
                                                    dp.SortingLine,
                                                    dp.DeliverLine,
                                                    dp.IsActive,
                                                    dp.UpdateTime,
                                                    dp.SortStatus,
                                                    om.QuantitySum
                                                }
                                           ).GroupBy(g => new { g.ID, g.OrderDate, g.DeliverLine, g.SortingLine, g.IsActive, g.UpdateTime,g.SortStatus })
                                            .Select(r => new
                                            {
                                                r.Key.ID,
                                                r.Key.OrderDate,
                                                r.Key.SortingLine.SortingLineCode,
                                                r.Key.SortingLine.SortingLineName,
                                                r.Key.DeliverLine.DeliverLineCode,
                                                r.Key.DeliverLine.DeliverLineName,
                                                IsActive = r.Key.IsActive == "1" ? "可用" : "不可用",
                                                UpdateTime = r.Key.UpdateTime.ToString("yyyy-MM-dd"),
                                                SortStatus = r.Key.SortStatus == "1" ? "未分拣" : "已分拣",
                                                QuantitySum = r.Sum(q => q.QuantitySum)
                                            });
            return temp.ToArray();
        }

        public object GetNormalBatch()
        {
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();

            var temp = sortDispatchQuery.Where(s => s.SortBatchId.Equals(0) && s.SortStatus.Equals("1")).ToArray().AsEnumerable()
                                           .Join(sortOrderQuery,
                                                dp => new { dp.OrderDate, dp.DeliverLineCode },
                                                om => new { om.OrderDate, om.DeliverLineCode },
                                                (dp, om) => new
                                                {
                                                    dp.ID,
                                                    dp.OrderDate,
                                                    dp.SortingLine,
                                                    dp.DeliverLine,
                                                    dp.IsActive,
                                                    dp.UpdateTime,
                                                    dp.SortStatus,
                                                    om.QuantitySum
                                                }
                                           ).GroupBy(g => new { g.ID, g.OrderDate, g.DeliverLine, g.SortingLine, g.IsActive, g.UpdateTime, g.SortStatus })
                                            .Select(r => new
                                            {
                                                r.Key.ID,
                                                r.Key.OrderDate,
                                                r.Key.SortingLine.SortingLineCode,
                                                r.Key.SortingLine.SortingLineName,
                                                r.Key.DeliverLine.DeliverLineCode,
                                                r.Key.DeliverLine.DeliverLineName,
                                                IsActive = r.Key.IsActive == "1" ? "可用" : "不可用",
                                                UpdateTime = r.Key.UpdateTime.ToString("yyyy-MM-dd"),
                                                SortStatus = r.Key.SortStatus == "1" ? "未分拣" : "已分拣",
                                                QuantitySum = r.Sum(q => q.QuantitySum)
                                            });
            return temp.ToArray();
        }

        public object GetAbnormalBatch()
        {
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();

            var temp = sortDispatchQuery.Where(s => s.SortBatchAbnormalId.Equals(0) && s.SortStatus.Equals("1")).ToArray().AsEnumerable()
                                           .Join(sortOrderQuery,
                                                dp => new { dp.OrderDate, dp.DeliverLineCode },
                                                om => new { om.OrderDate, om.DeliverLineCode },
                                                (dp, om) => new
                                                {
                                                    dp.ID,
                                                    dp.OrderDate,
                                                    dp.SortingLine,
                                                    dp.DeliverLine,
                                                    dp.IsActive,
                                                    dp.UpdateTime,
                                                    dp.SortStatus,
                                                    om.QuantitySum
                                                }
                                           ).GroupBy(g => new { g.ID, g.OrderDate, g.DeliverLine, g.SortingLine, g.IsActive, g.UpdateTime, g.SortStatus })
                                            .Select(r => new
                                            {
                                                r.Key.ID,
                                                r.Key.OrderDate,
                                                r.Key.SortingLine.SortingLineCode,
                                                r.Key.SortingLine.SortingLineName,
                                                r.Key.DeliverLine.DeliverLineCode,
                                                r.Key.DeliverLine.DeliverLineName,
                                                IsActive = r.Key.IsActive == "1" ? "可用" : "不可用",
                                                UpdateTime = r.Key.UpdateTime.ToString("yyyy-MM-dd"),
                                                SortStatus = r.Key.SortStatus == "1" ? "未分拣" : "已分拣",
                                                QuantitySum = r.Sum(q => q.QuantitySum)
                                            });
            return temp.ToArray();
        }

        public object GetPieceBatch()
        {
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();

            var temp = sortDispatchQuery.Where(s => s.SortBatchPiecesId.Equals(0) && s.SortStatus.Equals("1")).ToArray().AsEnumerable()
                                           .Join(sortOrderQuery,
                                                dp => new { dp.OrderDate, dp.DeliverLineCode },
                                                om => new { om.OrderDate, om.DeliverLineCode },
                                                (dp, om) => new
                                                {
                                                    dp.ID,
                                                    dp.OrderDate,
                                                    dp.SortingLine,
                                                    dp.DeliverLine,
                                                    dp.IsActive,
                                                    dp.UpdateTime,
                                                    dp.SortStatus,
                                                    om.QuantitySum
                                                }
                                           ).GroupBy(g => new { g.ID, g.OrderDate, g.DeliverLine, g.SortingLine, g.IsActive, g.UpdateTime, g.SortStatus })
                                            .Select(r => new
                                            {
                                                r.Key.ID,
                                                r.Key.OrderDate,
                                                r.Key.SortingLine.SortingLineCode,
                                                r.Key.SortingLine.SortingLineName,
                                                r.Key.DeliverLine.DeliverLineCode,
                                                r.Key.DeliverLine.DeliverLineName,
                                                IsActive = r.Key.IsActive == "1" ? "可用" : "不可用",
                                                UpdateTime = r.Key.UpdateTime.ToString("yyyy-MM-dd"),
                                                SortStatus = r.Key.SortStatus == "1" ? "未分拣" : "已分拣",
                                                QuantitySum = r.Sum(q => q.QuantitySum)
                                            });
            return temp.ToArray();
        }

        public object GetManualBatch()
        {
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            IQueryable<SortingLine> sortingline = SortingLineRepository.GetQueryable();
            if (sortingline.Where(s => s.IsActive == "1" && s.ProductType.Contains("4")).Count() > 0)
            {
                var temp = sortDispatchQuery.Where(s => s.SortBatchManualId.Equals(0) && s.SortStatus.Equals("1")).ToArray().AsEnumerable()
                                               .Join(sortOrderQuery,
                                                    dp => new { dp.OrderDate, dp.DeliverLineCode },
                                                    om => new { om.OrderDate, om.DeliverLineCode },
                                                    (dp, om) => new
                                                    {
                                                        dp.ID,
                                                        dp.OrderDate,
                                                        dp.SortingLine,
                                                        dp.DeliverLine,
                                                        dp.IsActive,
                                                        dp.UpdateTime,
                                                        dp.SortStatus,
                                                        om.QuantitySum
                                                    }
                                               ).GroupBy(g => new { g.ID, g.OrderDate, g.DeliverLine, g.SortingLine, g.IsActive, g.UpdateTime, g.SortStatus })
                                                .Select(r => new
                                                {
                                                    r.Key.ID,
                                                    r.Key.OrderDate,
                                                    r.Key.SortingLine.SortingLineCode,
                                                    r.Key.SortingLine.SortingLineName,
                                                    r.Key.DeliverLine.DeliverLineCode,
                                                    r.Key.DeliverLine.DeliverLineName,
                                                    IsActive = r.Key.IsActive == "1" ? "可用" : "不可用",
                                                    UpdateTime = r.Key.UpdateTime.ToString("yyyy-MM-dd"),
                                                    SortStatus = r.Key.SortStatus == "1" ? "未分拣" : "已分拣",
                                                    QuantitySum = r.Sum(q => q.QuantitySum)
                                                });
                return temp.ToArray();
            }
            else
            {
                return null;
            }
        }

        public int GetActiveSortingLine(string productType)
        {
            IQueryable<SortingLine> sortingline = SortingLineRepository.GetQueryable();
            return sortingline.Where(s => s.IsActive == "1" && s.ProductType == productType).Count();
        }

        #endregion

        public System.Data.DataTable GetSortOrderDispatch(int page, int rows, string OrderDate,string WorkStatus,string SortStatus, string SortingLineCode)
        {
           
            var sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
          
            if (OrderDate != string.Empty && OrderDate != null)
            {
                OrderDate = Convert.ToDateTime(OrderDate).ToString("yyyyMMdd");
                sortDispatchQuery = sortDispatchQuery.Where(s => s.OrderDate == OrderDate);
            }
            if (SortingLineCode != string.Empty && SortingLineCode != null)
            {
                sortDispatchQuery = sortDispatchQuery.Where(s => s.SortingLineCode == SortingLineCode);
            }
            if (SortStatus != string.Empty && SortStatus != null)
            {
                sortDispatchQuery = sortDispatchQuery.Where(s => s.SortStatus == SortStatus);
            }
            if (WorkStatus != string.Empty && WorkStatus != null)
            {
                sortDispatchQuery = sortDispatchQuery.Where(s => s.WorkStatus == WorkStatus);
            }
            var temp = sortDispatchQuery.OrderBy(b => b.SortingLineCode).AsEnumerable().Select(b => new
            {
                b.ID,
                b.SortingLineCode,
                b.SortingLine.SortingLineName,
                b.OrderDate,
                b.DeliverLineCode,
                WorkStatus = b.WorkStatus == "1" ? "未作业" : "已作业",
                SortStatus=b.SortStatus=="1"?"未分拣":"已分拣",
                b.DeliverLine.DeliverLineName,
                IsActive = b.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });

            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("订单日期", typeof(string));
            dt.Columns.Add("分拣线编码", typeof(string));
            dt.Columns.Add("分拣线名称", typeof(string));
            dt.Columns.Add("送货线路编码", typeof(string));
            dt.Columns.Add("送货线路名称", typeof(string));
            dt.Columns.Add("分拣状态", typeof(string));
            dt.Columns.Add("作业状态", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("修改时间", typeof(string));
            foreach (var t in temp)
            {
                dt.Rows.Add(t.OrderDate,
                            t.SortingLineCode,
                            t.SortingLineName,
                            t.DeliverLineCode,
                            t.DeliverLineName,
                            t.SortStatus,
                            t.WorkStatus,
                            t.IsActive,
                            t.UpdateTime);
            }
            return dt;
        }
    }
}
