using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class SortBatch
    {
        public SortBatch()
        {
            this.ChannelAllots = new List<ChannelAllot>();
            this.HandSupplys = new List<HandSupply>();
            this.SortOrderAllotMasters = new List<SortOrderAllotMaster>();
        }
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int BatchNo { get; set; }
        public string SortingLineCode { get; set; }
        public int NoOneBatchNo { get; set; }
        public DateTime SortDate { get; set; }
        public string Status { get; set; }

        public virtual ICollection<ChannelAllot> ChannelAllots { get; set; }
        public virtual ICollection<HandSupply> HandSupplys { get; set; }
        public virtual ICollection<SortOrderAllotMaster> SortOrderAllotMasters { get; set; }

    }
}