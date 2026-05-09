namespace IncendiaryService
{
    public interface ILocalAccountManager
    {
        bool UserExists(string samAccountName);
        void CreateUser(string samAccountName, string displayName, string password);
        bool IsUserInGroup(string samAccountName, string groupName);
        void AddUserToGroup(string samAccountName, string groupName);
    }
}
