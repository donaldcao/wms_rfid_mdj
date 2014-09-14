using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations.Schema;

namespace THOK.SMS.DbModel.Mapping
{
    public class ChannelMap : EntityMappingBase<Channel>
    {
        public ChannelMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.ChannelCode);

            // Properties
            this.Property(t => t.ChannelCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.ChannelType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);
            this.Property(t => t.ChannelName)
                .IsRequired()
                .HasMaxLength(100);
            this.Property(t => t.SortingLineCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.LedNo);
            this.Property(t => t.X);
            this.Property(t => t.Y);
            this.Property(t => t.Width);
            this.Property(t => t.Height);
            this.Property(t => t.ProductCode)
                .HasMaxLength(20);
            this.Property(t => t.ProductName)
                .HasMaxLength(50);
            this.Property(t => t.RemainQuantity)
                .IsRequired();
            this.Property(t => t.ChannelCapacity)
                .IsRequired();
            this.Property(t => t.GroupNo)
                .IsRequired();
            this.Property(t => t.OrderNo)
                .IsRequired();
            this.Property(t => t.SortAddress)
                .IsRequired();
            this.Property(t => t.SupplyAddress)
                .IsRequired();
            this.Property(t => t.SupplyCachePosition);

            this.Property(t => t.IsAcceptRemainQuantity)
                .IsRequired();

            this.Property(t => t.IsActive)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);
            this.Property(t => t.UpdateTime)
                .IsRequired();

            // Table & Column Mappings
            this.Property(t => t.ChannelCode).HasColumnName(ColumnMap.Value.To("ChannelCode"));
            this.Property(t => t.ChannelType).HasColumnName(ColumnMap.Value.To("ChannelType"));
            this.Property(t => t.ChannelName).HasColumnName(ColumnMap.Value.To("ChannelName"));
            this.Property(t => t.SortingLineCode).HasColumnName(ColumnMap.Value.To("SortingLineCode"));
            this.Property(t => t.LedNo).HasColumnName(ColumnMap.Value.To("LedNo"));
            this.Property(t => t.X).HasColumnName(ColumnMap.Value.To("X"));
            this.Property(t => t.Y).HasColumnName(ColumnMap.Value.To("Y"));
            this.Property(t => t.Width).HasColumnName(ColumnMap.Value.To("Width"));
            this.Property(t => t.Height).HasColumnName(ColumnMap.Value.To("Height"));
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.ProductName).HasColumnName(ColumnMap.Value.To("ProductName"));
            this.Property(t => t.RemainQuantity).HasColumnName(ColumnMap.Value.To("RemainQuantity"));
            this.Property(t => t.ChannelCapacity).HasColumnName(ColumnMap.Value.To("ChannelCapacity"));
            this.Property(t => t.GroupNo).HasColumnName(ColumnMap.Value.To("GroupNo"));
            this.Property(t => t.OrderNo).HasColumnName(ColumnMap.Value.To("OrderNo"));
            this.Property(t => t.SortAddress).HasColumnName(ColumnMap.Value.To("SortAddress"));
            this.Property(t => t.SupplyAddress).HasColumnName(ColumnMap.Value.To("SupplyAddress"));
            this.Property(t => t.SupplyCachePosition).HasColumnName(ColumnMap.Value.To("SupplyCachePosition"));
            this.Property(t => t.IsAcceptRemainQuantity).HasColumnName(ColumnMap.Value.To("IsAcceptRemainQuantity"));
            this.Property(t => t.IsActive).HasColumnName(ColumnMap.Value.To("IsActive"));
            this.Property(t => t.UpdateTime).HasColumnName(ColumnMap.Value.To("UpdateTime"));

        }
    }
}