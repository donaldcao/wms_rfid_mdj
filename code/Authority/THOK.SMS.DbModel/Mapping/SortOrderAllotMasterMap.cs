using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.SMS.DbModel.Mapping
{
    public class SortOrderAllotMasterMap : EntityMappingBase<SortOrderAllotMaster>
    {
        public SortOrderAllotMasterMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.SortBatchId)
                .IsRequired();
            this.Property(t => t.PackNo)
                .IsRequired();
            this.Property(t => t.OrderId)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.DeliverLineCode)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.CustomerCode)
                .HasMaxLength(50);
            this.Property(t => t.CustomerName)
                .IsRequired()
                .HasMaxLength(100);
            this.Property(t => t.CustomerOrder)
                .IsRequired();
            this.Property(t => t.CustomerDeliverOrder)
                .IsRequired();
            this.Property(t => t.CustomerInfo)
                .IsRequired()
                .IsMaxLength();
            this.Property(t => t.Quantity)
                .IsRequired();
            this.Property(t => t.ExportNo)
                .IsRequired();
            this.Property(t => t.StartTime);
            this.Property(t => t.FinishTime);
            this.Property(t => t.Status)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            this.Property(t => t.Id).HasColumnName(ColumnMap.Value.To("Id"));
            this.Property(t => t.SortBatchId).HasColumnName(ColumnMap.Value.To("SortBatchId"));
            this.Property(t => t.PackNo).HasColumnName(ColumnMap.Value.To("PackNo"));
            this.Property(t => t.OrderId).HasColumnName(ColumnMap.Value.To("OrderId"));
            this.Property(t => t.DeliverLineCode).HasColumnName(ColumnMap.Value.To("DeliverLineCode"));
            this.Property(t => t.CustomerCode).HasColumnName(ColumnMap.Value.To("CustomerCode"));
            this.Property(t => t.CustomerName).HasColumnName(ColumnMap.Value.To("CustomerName"));
            this.Property(t => t.CustomerOrder).HasColumnName(ColumnMap.Value.To("CustomerOrder"));
            this.Property(t => t.CustomerDeliverOrder).HasColumnName(ColumnMap.Value.To("CustomerDeliverOrder"));
            this.Property(t => t.CustomerInfo).HasColumnName(ColumnMap.Value.To("CustomerInfo"));
            this.Property(t => t.Quantity).HasColumnName(ColumnMap.Value.To("Quantity"));
            this.Property(t => t.ExportNo).HasColumnName(ColumnMap.Value.To("ExportNo"));
            this.Property(t => t.StartTime).HasColumnName(ColumnMap.Value.To("StartTime"));
            this.Property(t => t.FinishTime).HasColumnName(ColumnMap.Value.To("FinishTime"));
            this.Property(t => t.Status).HasColumnName(ColumnMap.Value.To("Status"));

            // Relationships
            this.HasRequired(t => t.SortBatch)
                .WithMany(t => t.SortOrderAllotMasters)
                .HasForeignKey(d => d.SortBatchId)
                .WillCascadeOnDelete(false);

        }
    }
}
