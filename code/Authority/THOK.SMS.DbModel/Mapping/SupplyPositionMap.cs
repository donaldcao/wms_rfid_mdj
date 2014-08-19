using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations.Schema;

namespace THOK.SMS.DbModel.Mapping
{
    public class SupplyPositionMap : EntityMappingBase<SupplyPosition>
    {
        public SupplyPositionMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.PositionName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.PositionType)
                .IsRequired()
                .HasMaxLength(2);
            this.Property(t => t.ProductCode)
                .HasMaxLength(20);
            this.Property(t => t.ProductName)
                .HasMaxLength(50);
            this.Property(t => t.PositionAddress)
                .IsRequired();
            this.Property(t => t.PositionCapacity)
                .IsRequired();
            this.Property(t => t.SortingLineCodes)
                .IsMaxLength();
            this.Property(t => t.TargetSupplyAddresses)
                .IsMaxLength(); ;
            this.Property(t => t.Description)
                .IsMaxLength(); ;
            this.Property(t => t.IsActive)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.Id).HasColumnName(ColumnMap.Value.To("Id"));
            this.Property(t => t.PositionName).HasColumnName(ColumnMap.Value.To("PositionName"));
            this.Property(t => t.PositionType).HasColumnName(ColumnMap.Value.To("PositionType"));
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.ProductName).HasColumnName(ColumnMap.Value.To("ProductName"));
            this.Property(t => t.PositionAddress).HasColumnName(ColumnMap.Value.To("PositionAddress"));
            this.Property(t => t.PositionCapacity).HasColumnName(ColumnMap.Value.To("PositionCapacity"));
            this.Property(t => t.SortingLineCodes).HasColumnName(ColumnMap.Value.To("SortingLineCodes"));
            this.Property(t => t.TargetSupplyAddresses).HasColumnName(ColumnMap.Value.To("TargetSupplyAddresses"));
            this.Property(t => t.Description).HasColumnName(ColumnMap.Value.To("Description"));
            this.Property(t => t.IsActive).HasColumnName(ColumnMap.Value.To("IsActive"));
        }
    }
}
