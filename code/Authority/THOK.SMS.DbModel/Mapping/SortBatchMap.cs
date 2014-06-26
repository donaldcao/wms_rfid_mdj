using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.SMS.DbModel.Mapping
{
    public class SortBatchMap : EntityMappingBase<SortBatch>
    {
        public SortBatchMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.OrderDate)
                .IsRequired();
            this.Property(t => t.BatchNo)
                .IsRequired();
            this.Property(t => t.SortingLineCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.NoOneBatchNo);
            this.Property(t => t.SortDate)
                .IsRequired();
            this.Property(t => t.Status)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            this.Property(t => t.Id).HasColumnName(ColumnMap.Value.To("Id"));
            this.Property(t => t.OrderDate).HasColumnName(ColumnMap.Value.To("OrderDate"));
            this.Property(t => t.BatchNo).HasColumnName(ColumnMap.Value.To("BatchNo"));
            this.Property(t => t.SortingLineCode).HasColumnName(ColumnMap.Value.To("SortingLineCode"));
            this.Property(t => t.NoOneBatchNo).HasColumnName(ColumnMap.Value.To("NoOneBatchNo"));
            this.Property(t => t.SortDate).HasColumnName(ColumnMap.Value.To("SortDate"));
            this.Property(t => t.Status).HasColumnName(ColumnMap.Value.To("Status"));

        }
    }
}








