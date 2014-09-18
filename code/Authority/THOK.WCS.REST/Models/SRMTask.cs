using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.WCS.REST.Models
{
    public class SRMTask
    {
        public int ID { get; set; }
        public string TaskType { get; set; }

        public string OrderID { get; set; }
        public string OrderType { get; set; }
        public int AllotID { get; set; }

        public string OriginCellCode { get; set; }
        public string TargetCellCode { get; set; }
        public string OriginStorageCode { get; set; }
        public string TargetStorageCode { get; set; }

        public int Quantity { get; set; }
        public int TaskQuantity { get; set; }

        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int TravelPos1 { get; set; }
        public int LiftPos1 { get; set; }
        public int TravelPos2 { get; set; }
        public int LiftPos2 { get; set; }
        public int RealLiftPos2 { get; set; }

        public string CurrentPositionName { get; set; }
        public string CurrentPositionType { get; set; }
        public int CurrentPositionExtension { get; set; }

        public string NextPositionName { get; set; }
        public string NextPositionType { get; set; }
        public int NextPositionExtension { get; set; }

        public string NextTwoPositionName { get; set; }

        public string EndPositionName { get; set; }
        public string EndPositionType { get; set; }

        public bool HasGetRequest { get; set; }
        public bool HasPutRequest { get; set; }
        public bool GetFinish { get; set; }
        public bool PutFinish { get; set; }

        public string Barcode { get; set; }

        public bool IsSent { get; set; }

        public override string ToString()
        {
            return string.Format("任务号:{0} 位置1[{1}]:[{2},{3},{4}] 位置2[{5}]:[{6},{7},{8},{9}] 取货完成:[{10}] 放货完成:[{11}]", ID, CurrentPositionName, TravelPos1, LiftPos1, CurrentPositionExtension + 1, NextPositionName, TravelPos2, LiftPos2, RealLiftPos2, NextPositionExtension + 2, GetFinish, PutFinish);
        }    
    }
}
