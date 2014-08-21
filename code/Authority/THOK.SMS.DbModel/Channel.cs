using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class Channel
    {
        public Channel()
        {
            this.ChannelAllots = new List<ChannelAllot>();
            this.HandSupplys = new List<HandSupply>();
            this.SortOrderAllotDetails = new List<SortOrderAllotDetail>();
        }
        public string ChannelCode { get; set; }
        public string ChannelType { get; set; }
        public string ChannelName { get; set; }
        public string SortingLineCode { get; set; }
        public int LedNo { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int RemainQuantity { get; set; }
        public int ChannelCapacity { get; set; }
        public int GroupNo { get; set; }
        public int OrderNo { get; set; }
        public int SortAddress { get; set; }
        public int SupplyAddress { get; set; }
        public int SupplyCachePosition { get; set; }
        public string IsActive { get; set; }
        public DateTime UpdateTime { get; set; }

        public virtual ICollection<ChannelAllot> ChannelAllots { get; set; }
        public virtual ICollection<HandSupply> HandSupplys { get; set; }
        public virtual ICollection<SortOrderAllotDetail> SortOrderAllotDetails { get; set; }
        public virtual ICollection<SortSupply> SortSupplys { get; set; }
    }
}
