using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations.Schema;

namespace THOK.SMS.DbModel.Mapping
{
    public class SupplyPositionStorageMap : EntityMappingBase<SupplyPositionStorage>
    {
        public SupplyPositionStorageMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.PositionID)
                .IsRequired();
            this.Property(t => t.ProductCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.ProductName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.Quantity)
                .IsRequired();
            this.Property(t => t.WaitQuantity)
                .IsRequired();

            this.Property(t => t.Id).HasColumnName(ColumnMap.Value.To("Id"));
            this.Property(t => t.PositionID).HasColumnName(ColumnMap.Value.To("PositionID"));
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.ProductName).HasColumnName(ColumnMap.Value.To("ProductName"));
            this.Property(t => t.Quantity).HasColumnName(ColumnMap.Value.To("Quantity"));
            this.Property(t => t.WaitQuantity).HasColumnName(ColumnMap.Value.To("WaitQuantity"));

            // Relationships
            this.HasRequired(t => t.SupplyPosition)
                .WithMany(t => t.SupplyPositionStorage)
                .HasForeignKey(d => d.PositionID)
                .WillCascadeOnDelete(false);
        }
    }
}
