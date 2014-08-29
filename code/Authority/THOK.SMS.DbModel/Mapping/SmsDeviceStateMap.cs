using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations.Schema;

namespace THOK.SMS.DbModel.Mapping
{
    public class SmsDeviceStateMap : EntityMappingBase<SmsDeviceState>
    {
        public SmsDeviceStateMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.DeviceCode)
                .IsRequired()
                .HasMaxLength(8);
            this.Property(t => t.DeviceName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.DeviceType)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.StateCode)
                .IsRequired()
                .HasMaxLength(8);
            this.Property(t => t.BeginTime)
                .IsRequired();
            this.Property(t => t.EndTime)
                .IsRequired();
            this.Property(t => t.UseTime)
                .IsRequired();


            this.Property(t => t.Id).HasColumnName(ColumnMap.Value.To("Id"));
            this.Property(t => t.DeviceCode).HasColumnName(ColumnMap.Value.To("DeviceCode"));
            this.Property(t => t.DeviceName).HasColumnName(ColumnMap.Value.To("DeviceName"));
            this.Property(t => t.DeviceType).HasColumnName(ColumnMap.Value.To("DeviceType"));
            this.Property(t => t.StateCode).HasColumnName(ColumnMap.Value.To("StateCode"));
            this.Property(t => t.BeginTime).HasColumnName(ColumnMap.Value.To("BeginTime"));
            this.Property(t => t.EndTime).HasColumnName(ColumnMap.Value.To("EndTime"));
            this.Property(t => t.UseTime).HasColumnName(ColumnMap.Value.To("UseTime"));
        }
    }
}
