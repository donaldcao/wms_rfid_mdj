using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.Bll.Model
{
    class ChannelAllotInfo
    {
        public int Id { get; set; }

        public int GroupNo { get; set; }

        public string ChannelCode { get; set; }

        public int OrderNo { get; set; }

        public string ProductCode { get; set; }

        public int Quantity { get; set; }

        public int RemainQuantity { get; set; }

        public int ChannelCapacity { get; set; }
    }
}
