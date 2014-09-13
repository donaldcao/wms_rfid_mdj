using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.Wms.DbModel.Mapping
{
    public class OrdDistBillMap : EntityMappingBase<OrdDistBill>
    {
        public OrdDistBillMap()
            : base("Wms")
        {
            // Primary Key
            this.HasKey(a => a.DistBillID);

            // Properties
            this.Property(a => a.DistBillID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(a => a.DeliverLineCode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(a => a.DeliverLineName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(a => a.DeliverManCode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(a => a.DeliverManName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.Property(t => t.DistBillID).HasColumnName(ColumnMap.Value.To("DistBillID"));
            this.Property(t => t.DeliverLineCode).HasColumnName(ColumnMap.Value.To("DeliverLineCode"));
            this.Property(t => t.DeliverLineName).HasColumnName(ColumnMap.Value.To("DeliverLineName"));
            this.Property(t => t.DeliverManCode).HasColumnName(ColumnMap.Value.To("DeliverManCode"));
            this.Property(t => t.DeliverManName).HasColumnName(ColumnMap.Value.To("DeliverManName"));
        }
    }
}
