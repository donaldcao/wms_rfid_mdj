using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class HandSupply
    {
        public int Id { get; set; }
        public int SortBatchId { get; set; }
        public int SupplyId { get; set; }
        public int SupplyBatch { get; set; }
        public int PackNo { get; set; }
        public string ChannelCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public SortBatch sortBatch { get; set; }
        public Channel channel { get; set; }

    }
}





