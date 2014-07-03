using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.Authority.DbModel.Mapping
{
    public class SystemInfoMap : EntityMappingBase<SystemInfo>
    {
        public SystemInfoMap()
            : base("Auth")
        {
            // Primary Key
            this.HasKey(t => t.SystemID);

            // Properties
            this.Property(t => t.SystemName)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.Property(t => t.SystemID).HasColumnName(ColumnMap.Value.To("SystemID"));
            this.Property(t => t.SystemName).HasColumnName(ColumnMap.Value.To("SystemName"));
            this.Property(t => t.Description).HasColumnName(ColumnMap.Value.To("Description"));
            this.Property(t => t.Status).HasColumnName(ColumnMap.Value.To("Status"));
        }
    }
}
