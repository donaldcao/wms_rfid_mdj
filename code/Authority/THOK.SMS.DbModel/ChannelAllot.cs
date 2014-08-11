﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class ChannelAllot
    {
        public int Id { get; set; }
        public int SortBatchId { get; set; }
        public string ChannelCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public SortBatch SortBatch { get; set; }
        public Channel Channel { get; set; }

    }
}




