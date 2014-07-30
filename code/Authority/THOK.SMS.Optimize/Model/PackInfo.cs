using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.Optimize.Model
{
    class PackInfo
    {
        public int Id { get; set; }

        public int PackNo { get; set; }

        public int Quantity { get; set; }

        public DbModel.SortOrderAllotMaster SortOrderAllot { get; set; }
    }
}
