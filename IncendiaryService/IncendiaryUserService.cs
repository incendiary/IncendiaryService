using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace IncendiaryService
{
    public class IncendiaryUserService
    {
        private readonly string _logFilePath = "c:\\log.log";
        private readonly ILocalAccountManager _accountManager;
        private string _samAccountName = "Incendiary";
        private string _name = "Incendiary User";
        private string _randomPassword = GenerateRandomPassword();
        private string[] _additionalGroups = { "Administrators" };
        private bool _cleanupOnStop = false;
        private bool _useEventLog = false;

        public IncendiaryUserService(ILocalAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        internal IncendiaryUserService(ILocalAccountManager accountManager, bool cleanupOnStop)
            : this(accountManager)
        {
            _cleanupOnStop = cleanupOnStop;
        }

        public bool Start()
        {
            ReadConfiguration();
            try
            {
                if (!_accountManager.UserExists(_samAccountName))
                {
                    _accountManager.CreateUser(_samAccountName, _name, _randomPassword);
                    Log($"Incendiary user was created with password: {_randomPassword}");
                }

                foreach (var groupName in _additionalGroups)
                {
                    if (!_accountManager.IsUserInGroup(_samAccountName, groupName))
                    {
                        _accountManager.AddUserToGroup(_samAccountName, groupName);
                        Log($"Incendiary user was added to the {groupName} group");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"An error occurred: {ex.Message}");
            }

            return true;
        }

        public bool Stop()
        {
            if (_cleanupOnStop && _accountManager.UserExists(_samAccountName))
            {
                _accountManager.RemoveUser(_samAccountName);
                Log($"Incendiary user '{_samAccountName}' was removed");
            }

            Log("Service stopped");
            return true;
        }

        [SupportedOSPlatform("windows")]
        private void WriteEventLog(string message)
        {
            const string source = "IncendiaryUserService";
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, "Application");
            }

            EventLog.WriteEntry(source, message, EventLogEntryType.Information);
        }

        private void Log(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception)
            {
                // Ignore log write failures
            }

            if (_useEventLog && OperatingSystem.IsWindows())
            {
                try
                {
                    WriteEventLog(message);
                }
                catch (Exception)
                {
                    // Ignore event log failures
                }
            }
        }

        private void ReadConfiguration()
        {
            try
            {
                string configFilePath = "c:\\config.cfg";

                if (!File.Exists(configFilePath))
                {
                    return;
                }

                foreach (string line in File.ReadAllLines(configFilePath))
                {
                    string[] parts = line.Split(new[] { '=' }, 2).Select(x => x.Trim()).ToArray();
                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    switch (parts[0].ToLowerInvariant())
                    {
                        case "samaccountname":
                            _samAccountName = parts[1];
                            break;
                        case "name":
                            _name = parts[1];
                            break;
                        case "randompassword":
                            _randomPassword = parts[1];
                            break;
                        case "additionalgroups":
                            _additionalGroups = parts[1]
                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .ToArray();
                            break;
                        case "cleanuponstop":
                            _cleanupOnStop = parts[1].Equals("true", StringComparison.OrdinalIgnoreCase);
                            break;
                        case "useeventlog":
                            _useEventLog = parts[1].Equals("true", StringComparison.OrdinalIgnoreCase);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // Ignore config read failures; compiled defaults remain in effect
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
