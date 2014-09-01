﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.WCS.DbModel
{
    public class WcsDeviceState
    {
        public int Id { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string StateCode { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public int UseTime { get; set; }
    }
}
