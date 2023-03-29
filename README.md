# Incendiary User Service

This is a Windows service written in C# that manages the "Incendiary" user on the local machine. The service checks if the user exists, adds the user with a random password if not, and adds the user to the local administrators group. The service can also add the user to additional groups specified in the configuration file.

I had an engaegment where I could replace a service binary and didn't want to flag AV using venom.

## Installation

Using Topshelf
To install the service using Topshelf, follow these steps:

- Open a command prompt or PowerShell window.
- Navigate to the directory where the IncendiaryUserConsoleApp.exe binary is located.
- Run the following command to install the service:

`IncendiaryUserConsoleApp.exe install`

- The service will be installed and started automatically.
- To uninstall the service, run the following command:

`IncendiaryUserConsoleApp.exe uninstall`

## Build using Visual Studio 2022
To create the service using Visual Studio 2022:

- Open Visual Studio 2022. (very important)
- Select "Create a new project" from the start page, or select "File" > "New" > "Project..." from the menu bar.
- In the "Create a new project" dialog, select "Windows Service (.NET)" under "Project types", and "Windows Service" under "Templates".
- Give the project a name and click "Create".
- In the "Solution Explorer" pane, open the "Program.cs" file.
- Replace the contents of the file with the code from IncendiaryUserConsoleApp/Program.cs.
- Build the project.
- The service binary will be located in the bin\Debug or bin\Release directory, depending on your build configuration. You can install the service using the instructions in the previous section.


## Configuration

The service reads configuration values from a file located at c:\config.cfg. If the file is not present, the service uses default values. Here is an example configuration file:

```
ServiceName = IncendiaryUserService
SamAccountName = Incendiary
Name = Incendiary User
RandomPassword = MyRandomPassword123!
AdditionalGroups = Administrators, Power Users
```

In this example, the service name is "IncendiaryUserService", the SamAccountName is "Incendiary", the name is "Incendiary User", the random password is "MyRandomPassword123!", and the additional groups are "Administrators" and "Power Users". You can modify these values in the configuration file to suit your needs.
