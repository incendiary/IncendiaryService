namespace IncendiaryService
{
    public interface ILocalAccountManager
    {
        bool UserExists(string samAccountName);
        void CreateUser(string samAccountName, string displayName, string password);
        void RemoveUser(string samAccountName);
        bool IsUserInGroup(string samAccountName, string groupName);
        void AddUserToGroup(string samAccountName, string groupName);
    }
}
