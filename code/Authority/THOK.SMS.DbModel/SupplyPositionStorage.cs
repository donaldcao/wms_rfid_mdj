using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class SupplyPositionStorage
    {
        public int Id { get; set; }
        public int PositionID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int WaitQuantity { get; set; }

        public SupplyPosition SupplyPosition { get; set; }
    }
}
