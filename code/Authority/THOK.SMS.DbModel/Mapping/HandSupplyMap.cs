using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations.Schema;

namespace THOK.SMS.DbModel.Mapping
{
    public class HandSupplyMap : EntityMappingBase<HandSupply>
    {
        public HandSupplyMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.SortBatchId)
                .IsRequired();
            this.Property(t => t.SupplyId)
                .IsRequired();
            this.Property(t => t.SupplyBatch)
                .IsRequired();
            this.Property(t => t.PackNo)
               .IsRequired();
            this.Property(t => t.ChannelCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.ProductCode)
                .HasMaxLength(20);
            this.Property(t => t.ProductName)
                .HasMaxLength(50);
            this.Property(t => t.Quantity)
                .IsRequired();

            // Table & Column Mappings
            this.Property(t => t.Id).HasColumnName(ColumnMap.Value.To("Id"));
            this.Property(t => t.SortBatchId).HasColumnName(ColumnMap.Value.To("SortBatchId"));
            this.Property(t => t.SupplyId).HasColumnName(ColumnMap.Value.To("SupplyId"));
            this.Property(t => t.SupplyBatch).HasColumnName(ColumnMap.Value.To("SupplyBatch"));
            this.Property(t => t.PackNo).HasColumnName(ColumnMap.Value.To("PackNo"));
            this.Property(t => t.ChannelCode).HasColumnName(ColumnMap.Value.To("ChannelCode"));
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.ProductName).HasColumnName(ColumnMap.Value.To("ProductName"));
            this.Property(t => t.Quantity).HasColumnName(ColumnMap.Value.To("Quantity"));

            // Relationships
            this.HasRequired(t => t.sortBatch)
                .WithMany(t => t.HandSupplys)
                .HasForeignKey(d => d.SortBatchId)
                .WillCascadeOnDelete(false);
            this.HasRequired(t => t.channel)
                .WithMany(t => t.HandSupplys)
                .HasForeignKey(d => d.ChannelCode)
                .WillCascadeOnDelete(false);

        }
    }
}
