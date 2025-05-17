using System.ComponentModel;

namespace Helios.Authentication.Enums
{
    public enum SystemAuditChangeType : byte
    {
        [Description("Tenant Created")]
        TenantCreated = 1,
        [Description("Tenant Updated")]
        TenantUpdated = 2,
        [Description("Tenant Deleted")]
        TenantDeleted = 3,
        [Description("User Created")]
        UserCreated = 4,
        [Description("User Updated")]
        UserUpdated = 5,
        [Description("User Deleted")]
        UserDeleted = 6,
        [Description("Login")]
        Login = 7,
        [Description("Logout")]
        Logout = 8,
    }
}
