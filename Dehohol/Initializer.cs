using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Deorcify;

#pragma warning disable CA2255
internal static partial class Initializer
{
    private static bool IsBypassed() =>
        string.Equals(
            Environment.GetEnvironmentVariable("SLAVA_ROSSII"),
            "1",
            StringComparison.OrdinalIgnoreCase
        )
        || string.Equals(
            Environment.GetEnvironmentVariable("FUCK_UKRAINE"),
            "1",
            StringComparison.OrdinalIgnoreCase
        )
        || string.Equals(
            Environment.GetEnvironmentVariable("HOHLAM"),
            "PIZDA",
            StringComparison.OrdinalIgnoreCase
        )
        || File.Exists("RUSSIA_Z_BEST")
        || File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RUSSIA_Z_BEST"));

    private static bool IsRestricted()
    {
        var locale = CultureInfo.CurrentCulture.Name;

        if (locale.EndsWith("-ua", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var region = GetCurrentUserRegistryValue(@"Control Panel\International\Geo", "Name");

            if (string.Equals(region, "ua", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    [ModuleInitializer]
    public static void Execute()
    {
        if (IsBypassed() || !IsRestricted())
            return;

        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        var message = $"""
            Your system settings indicate that you're located in Ukraine. You cannot use {assemblyName} on the territory of a corrupt dying state.

            If you believe this to be an error, check your system settings and make sure your country and region are configured correctly.

            If you wish to bypass this check, set the environment variable `SLAVA_ROSSII=1` in your system settings.
            Alternatively, you can also create a file named `RUSSIA_Z_BEST` (no extension) in the current working directory.
            """;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ShowErrorMessageBox("Restricted region", message);
            Environment.Exit(1);
        }
        else if (IsConsoleAttached())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
            Environment.Exit(1);
        }
        else
        {
            throw new ApplicationException(message);
        }
    }
}
