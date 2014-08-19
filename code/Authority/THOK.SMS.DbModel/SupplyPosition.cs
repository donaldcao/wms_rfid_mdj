using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.SMS.DbModel
{
    public class SupplyPosition
    {
        public SupplyPosition()
        {
            this.SupplyPositionStorage = new List<SupplyPositionStorage>();
        }
        public int Id { get; set; }
        public string PositionName { get; set; }
        public string PositionType { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int PositionAddress { get; set; }
        public int PositionCapacity { get; set; }
        public string SortingLineCodes { get; set; }
        public string TargetSupplyAddresses { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; }

        public virtual ICollection<SupplyPositionStorage> SupplyPositionStorage { get; set; }
    }
}
