using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class SortOrderAllotDetail
    {
        public int Id { get; set; }
        public int MasterId { get; set; }
        public string ChannelCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public SortOrderAllotMaster SortOrderAllotMaster { get; set; }
        public Channel Channel { get; set; }

    }
}