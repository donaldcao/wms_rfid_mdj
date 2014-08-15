using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations.Schema;

namespace THOK.SMS.DbModel.Mapping
{
    public class SupplyTaskMap : EntityMappingBase<SupplyTask>
    {
        public SupplyTaskMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.SupplyId)
                .IsRequired();
            this.Property(t => t.PackNo)
                .IsRequired();
            this.Property(t => t.SortingLineCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.GroupNo)
                .IsRequired();
            this.Property(t => t.ChannelCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.ChannelName)
                .IsRequired()
                .HasMaxLength(100);
            this.Property(t => t.ProductCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.ProductName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.ProductBarcode)
                .HasMaxLength(6);
            this.Property(t => t.OriginPositionAddress);
            this.Property(t => t.TargetSupplyAddress)
                .IsRequired();
            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(1);


            this.Property(t => t.Id).HasColumnName(ColumnMap.Value.To("Id"));
            this.Property(t => t.SupplyId).HasColumnName(ColumnMap.Value.To("SupplyId"));
            this.Property(t => t.PackNo).HasColumnName(ColumnMap.Value.To("PackNo"));
            this.Property(t => t.SortingLineCode).HasColumnName(ColumnMap.Value.To("SortingLineCode"));
            this.Property(t => t.GroupNo).HasColumnName(ColumnMap.Value.To("GroupNo"));
            this.Property(t => t.ChannelCode).HasColumnName(ColumnMap.Value.To("ChannelCode"));
            this.Property(t => t.ChannelName).HasColumnName(ColumnMap.Value.To("ChannelName"));
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.ProductName).HasColumnName(ColumnMap.Value.To("ProductName"));
            this.Property(t => t.ProductBarcode).HasColumnName(ColumnMap.Value.To("ProductBarcode"));
            this.Property(t => t.OriginPositionAddress).HasColumnName(ColumnMap.Value.To("OriginPositionAddress"));
            this.Property(t => t.TargetSupplyAddress).HasColumnName(ColumnMap.Value.To("TargetSupplyAddress"));
            this.Property(t => t.Status).HasColumnName(ColumnMap.Value.To("Status"));
        }
    }
}
