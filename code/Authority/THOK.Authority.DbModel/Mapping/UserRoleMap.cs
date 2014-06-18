using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.Authority.DbModel.Mapping
{
    public class UserRoleMap : EntityMappingBase<UserRole>
    {
        public UserRoleMap()
            : base("Auth")
        {
            // Primary Key
            this.HasKey(t => t.UserRoleID);

            // Properties
            // Table & Column Mappings
            this.Property(t => t.UserRoleID).HasColumnName(ColumnMap.Value.To("UserRoleID"));
            this.Property(t => t.Role_RoleID).HasColumnName(ColumnMap.Value.To("Role_RoleID"));
            this.Property(t => t.User_UserID).HasColumnName(ColumnMap.Value.To("User_UserID"));

            // Relationships
            this.HasRequired(t => t.Role)
                .WithMany(t => t.UserRoles)
                .HasForeignKey(d => d.Role_RoleID)
                .WillCascadeOnDelete(false);
            this.HasRequired(t => t.User)
                .WithMany(t => t.UserRoles)
                .HasForeignKey(d => d.User_UserID)
                .WillCascadeOnDelete(false);

        }
    }
}
