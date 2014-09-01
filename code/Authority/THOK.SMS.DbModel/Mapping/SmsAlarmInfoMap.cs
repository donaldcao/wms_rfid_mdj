using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations.Schema;

namespace THOK.SMS.DbModel.Mapping
{
    public class SmsAlarmInfoMap : EntityMappingBase<SmsAlarmInfo>
    {
        public SmsAlarmInfoMap()
            : base("Sms")
        {
            // Primary Key
            this.HasKey(t => t.AlarmCode);

            // Properties
            this.Property(t => t.AlarmCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.Property(t => t.AlarmCode).HasColumnName(ColumnMap.Value.To("AlarmCode"));
            this.Property(t => t.Description).HasColumnName(ColumnMap.Value.To("Description"));
        }
    }
}
