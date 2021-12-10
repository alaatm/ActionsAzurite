
using System.Diagnostics;
#if WINDOWS
using System.Management;
#endif

if (Proc.IsAzuriteRunning())
{
    Console.WriteLine("Running.");
}
else
{
    Console.WriteLine("Not Running.");
}

public static class Proc
{
    public static bool IsAzuriteRunning()
    {
        foreach (var p in Process.GetProcessesByName("node"))
        {
            var cmdLine = p.GetCommandLine();
            Console.WriteLine($"{p.ProcessName} - {cmdLine}");

            if (cmdLine.Contains("azurite", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string GetCommandLine(this Process process)
    {
        try
        {
#if WINDOWS
#pragma warning disable CA1416 // Validate platform compatibility
            using var searcher = new ManagementObjectSearcher($"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}");
            using var objects = searcher.Get();
            return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString() ?? "";
#pragma warning restore CA1416 // Validate platform compatibility
#elif LINUX
            return File.ReadAllText($"/proc/{process.Id}/cmdline");
#else
            throw new PlatformNotSupportedException();
#endif
        }
        catch
        {
            return string.Empty;
        }
    }
}