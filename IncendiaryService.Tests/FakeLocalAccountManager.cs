using System.Collections.Generic;

namespace IncendiaryService
{
    internal class FakeLocalAccountManager : ILocalAccountManager
    {
        private readonly HashSet<string> _users = new();
        private readonly Dictionary<string, HashSet<string>> _groupMemberships = new();

        public List<string> CreatedUsers { get; } = new();
        public List<(string User, string Group)> AddedToGroups { get; } = new();

        public void SeedUser(string samAccountName) => _users.Add(samAccountName);

        public void SeedGroupMembership(string samAccountName, string groupName)
        {
            if (!_groupMemberships.TryGetValue(groupName, out var members))
                _groupMemberships[groupName] = members = new HashSet<string>();
            members.Add(samAccountName);
        }

        public bool UserExists(string samAccountName) => _users.Contains(samAccountName);

        public void CreateUser(string samAccountName, string displayName, string password)
        {
            _users.Add(samAccountName);
            CreatedUsers.Add(samAccountName);
        }

        public bool IsUserInGroup(string samAccountName, string groupName) =>
            _groupMemberships.TryGetValue(groupName, out var members) && members.Contains(samAccountName);

        public void AddUserToGroup(string samAccountName, string groupName)
        {
            if (!_groupMemberships.TryGetValue(groupName, out var members))
                _groupMemberships[groupName] = members = new HashSet<string>();
            members.Add(samAccountName);
            AddedToGroups.Add((samAccountName, groupName));
        }
    }
}
