using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class SupplyTask
    {
        public int Id { get; set; }
        public int SupplyId { get; set; }
        public int PackNo { get; set; }
        public string SortingLineCode { get; set; }
        public int GroupNo { get; set; }
        public string ChannelCode { get; set; }
        public string ChannelName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductBarcode { get; set; }
        public int OriginPositionAddress { get; set; }
        public int TargetSupplyAddress { get; set; }
        public string Status { get; set; }
    }
}
