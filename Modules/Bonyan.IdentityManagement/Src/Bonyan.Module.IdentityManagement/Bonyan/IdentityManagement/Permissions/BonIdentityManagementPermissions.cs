namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Defines all permissions for Identity Management module
/// </summary>
public static class BonIdentityManagementPermissions
{
    public const string GroupName = "IdentityManagement";

    /// <summary>
    /// User management permissions
    /// </summary>
    public static class Users
    {
        public const string Default = GroupName + ".Users";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string ManageRoles = Default + ".ManageRoles";
        public const string ManageClaims = Default + ".ManageClaims";
        public const string ManagePermissions = Default + ".ManagePermissions";
        public const string ChangePassword = Default + ".ChangePassword";
        public const string ResetPassword = Default + ".ResetPassword";
        public const string BanUser = Default + ".BanUser";
        public const string ActivateUser = Default + ".ActivateUser";
        public const string ViewUserDetails = Default + ".ViewUserDetails";
        public const string ExportUsers = Default + ".ExportUsers";
        public const string ImportUsers = Default + ".ImportUsers";
    }

    /// <summary>
    /// Role management permissions
    /// </summary>
    public static class Roles
    {
        public const string Default = GroupName + ".Roles";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string ManagePermissions = Default + ".ManagePermissions";
        public const string ManageClaims = Default + ".ManageClaims";
        public const string AssignToUsers = Default + ".AssignToUsers";
        public const string ViewRoleDetails = Default + ".ViewRoleDetails";
    }

    /// <summary>
    /// Claims management permissions
    /// </summary>
    public static class Claims
    {
        public const string Default = GroupName + ".Claims";
        public const string ManageUserClaims = Default + ".ManageUserClaims";
        public const string ManageRoleClaims = Default + ".ManageRoleClaims";
        public const string ViewClaims = Default + ".ViewClaims";
        public const string CreateClaims = Default + ".CreateClaims";
        public const string DeleteClaims = Default + ".DeleteClaims";
    }

    /// <summary>
    /// Authentication permissions
    /// </summary>
    public static class Authentication
    {
        public const string Default = GroupName + ".Authentication";
        public const string ManageTokens = Default + ".ManageTokens";
        public const string ViewTokens = Default + ".ViewTokens";
        public const string RevokeTokens = Default + ".RevokeTokens";
        public const string ManageSessions = Default + ".ManageSessions";
        public const string ViewLoginHistory = Default + ".ViewLoginHistory";
    }

    /// <summary>
    /// System administration permissions
    /// </summary>
    public static class Administration
    {
        public const string Default = GroupName + ".Administration";
        public const string ManageSystem = Default + ".ManageSystem";
        public const string ViewSystemLogs = Default + ".ViewSystemLogs";
        public const string ManageModules = Default + ".ManageModules";
        public const string SystemConfiguration = Default + ".SystemConfiguration";
        public const string BackupRestore = Default + ".BackupRestore";
    }

    /// <summary>
    /// Gets all permission names in this module
    /// </summary>
    public static IEnumerable<string> GetAllPermissions()
    {
        yield return Users.Default;
        yield return Users.Create;
        yield return Users.Edit;
        yield return Users.Delete;
        yield return Users.View;
        yield return Users.ManageRoles;
        yield return Users.ManageClaims;
        yield return Users.ManagePermissions;
        yield return Users.ChangePassword;
        yield return Users.ResetPassword;
        yield return Users.BanUser;
        yield return Users.ActivateUser;
        yield return Users.ViewUserDetails;
        yield return Users.ExportUsers;
        yield return Users.ImportUsers;

        yield return Roles.Default;
        yield return Roles.Create;
        yield return Roles.Edit;
        yield return Roles.Delete;
        yield return Roles.View;
        yield return Roles.ManagePermissions;
        yield return Roles.ManageClaims;
        yield return Roles.AssignToUsers;
        yield return Roles.ViewRoleDetails;

        yield return Claims.Default;
        yield return Claims.ManageUserClaims;
        yield return Claims.ManageRoleClaims;
        yield return Claims.ViewClaims;
        yield return Claims.CreateClaims;
        yield return Claims.DeleteClaims;

        yield return Authentication.Default;
        yield return Authentication.ManageTokens;
        yield return Authentication.ViewTokens;
        yield return Authentication.RevokeTokens;
        yield return Authentication.ManageSessions;
        yield return Authentication.ViewLoginHistory;

        yield return Administration.Default;
        yield return Administration.ManageSystem;
        yield return Administration.ViewSystemLogs;
        yield return Administration.ManageModules;
        yield return Administration.SystemConfiguration;
        yield return Administration.BackupRestore;
    }
} 