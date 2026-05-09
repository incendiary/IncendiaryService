using System;
using System.Runtime.Versioning;
using Topshelf;

internal static class Program
{
    [SupportedOSPlatform("windows")]
    private static void Main()
    {
        var exitCode = HostFactory.Run(x =>
        {
            x.Service<IncendiaryService.IncendiaryUserService>(s =>
            {
                s.ConstructUsing(_ => new IncendiaryService.IncendiaryUserService(new IncendiaryService.WindowsLocalAccountManager()));
                s.WhenStarted(service => service.Start());
                s.WhenStopped(service => service.Stop());
            });
            x.RunAsLocalSystem();
            x.SetServiceName("IncendiaryUserService");
            x.SetDisplayName("Incendiary User Service");
            x.SetDescription("Service to manage the Incendiary user on the local machine.");
        });
        Environment.ExitCode = (int)exitCode;
    }
}
