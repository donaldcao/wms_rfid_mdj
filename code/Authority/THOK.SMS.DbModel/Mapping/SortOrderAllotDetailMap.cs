using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.SMS.DbModel.Mapping
{
    public class SortOrderAllotDetailMap : EntityMappingBase<SortOrderAllotDetail>
    {
        public SortOrderAllotDetailMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                 .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.MasterId)
                .IsRequired();
            this.Property(t => t.ChannelCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.ProductCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.ProductName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.Quantity)
                .IsRequired();

            // Table & Column Mappings
            this.Property(t => t.Id).HasColumnName(ColumnMap.Value.To("Id"));
            this.Property(t => t.MasterId).HasColumnName(ColumnMap.Value.To("MasterId"));
            this.Property(t => t.ChannelCode).HasColumnName(ColumnMap.Value.To("ChannelCode"));
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.ProductName).HasColumnName(ColumnMap.Value.To("ProductName"));
            this.Property(t => t.Quantity).HasColumnName(ColumnMap.Value.To("Quantity"));

            // Relationships
            this.HasRequired(t => t.sortOrderAllotMaster)
                .WithMany(t => t.SortOrderAllotDetails)
                .HasForeignKey(d => d.MasterId)
                .WillCascadeOnDelete(false);
            this.HasRequired(t => t.channel)
                .WithMany(t => t.SortOrderAllotDetails)
                .HasForeignKey(d => d.ChannelCode)
                .WillCascadeOnDelete(false);

        }
    }
}
