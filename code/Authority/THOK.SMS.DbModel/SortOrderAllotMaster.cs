using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class SortOrderAllotMaster
    {
        public SortOrderAllotMaster()
        {
            this.SortOrderAllotDetails = new List<SortOrderAllotDetail>();
        }
        public int Id { get; set; }
        public int SortBatchId { get; set; }
        public int PackNo { get; set; }
        public string OrderId { get; set; }
        public string DeliverLineCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int CustomerOrder { get; set; }
        public int CustomerDeliverOrder { get; set; }
        public string CustomerInfo { get; set; }
        public int Quantity { get; set; }
        public int ExportNo { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string Status { get; set; }

        public SortBatch SortBatch { get; set; }

        public virtual ICollection<SortOrderAllotDetail> SortOrderAllotDetails { get; set; }

    }
}