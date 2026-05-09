using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Versioning;

namespace IncendiaryService
{
    [SupportedOSPlatform("windows")]
    internal class WindowsLocalAccountManager : ILocalAccountManager
    {
        public bool UserExists(string samAccountName)
        {
            using (var pc = new PrincipalContext(ContextType.Machine))
            {
                return UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, samAccountName) != null;
            }
        }

        public void CreateUser(string samAccountName, string displayName, string password)
        {
            using (var pc = new PrincipalContext(ContextType.Machine))
            {
                var user = new UserPrincipal(pc)
                {
                    SamAccountName = samAccountName,
                    Name = displayName,
                    PasswordNeverExpires = true
                };
                user.SetPassword(password);
                user.Save();
            }
        }

        public void RemoveUser(string samAccountName)
        {
            using (var pc = new PrincipalContext(ContextType.Machine))
            {
                var user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, samAccountName);
                user?.Delete();
            }
        }

        public bool IsUserInGroup(string samAccountName, string groupName)
        {
            using (var pc = new PrincipalContext(ContextType.Machine))
            {
                var group = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName);
                return group != null && group.GetMembers().Any(m =>
                    m.SamAccountName.Equals(samAccountName, System.StringComparison.OrdinalIgnoreCase));
            }
        }

        public void AddUserToGroup(string samAccountName, string groupName)
        {
            using (var pc = new PrincipalContext(ContextType.Machine))
            {
                var user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, samAccountName);
                var group = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName);
                group?.Members.Add(user);
                group?.Save();
            }
        }
    }
}
