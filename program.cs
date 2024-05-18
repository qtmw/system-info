using System;
using System.Management;

class Program
{
    static void Main()
    {
        Console.WriteLine("System Information");
        Console.WriteLine("Unskidded - Version 1.0");
        Console.WriteLine("===================");


        string uuid = GetWmiProperty("Win32_ComputerSystemProduct", "UUID");
        Console.WriteLine($"System UUID: {uuid}");

        string systemName = Environment.MachineName;
        Console.WriteLine($"System Name: {systemName}");

        string hwid = GetWmiProperty("Win32_BIOS", "SerialNumber");
        Console.WriteLine($"Hardware ID (BIOS Serial Number): {hwid}");

        string processor = GetWmiProperty("Win32_Processor", "Name");
        Console.WriteLine($"Processor: {processor}");

        string ram = GetWmiProperty("Win32_ComputerSystem", "TotalPhysicalMemory");
        double ramInGB = Math.Round(Convert.ToDouble(ram) / (1024 * 1024 * 1024), 2);
        Console.WriteLine($"Installed RAM: {ramInGB} GB");


        var monitors = GetMonitors();
        foreach (var monitor in monitors)
        {
            Console.WriteLine($"Monitor: {monitor}");
        }


        var graphicsCards = GetGraphicsCards();
        foreach (var graphicsCard in graphicsCards)
        {
            Console.WriteLine($"Graphics Card: {graphicsCard}");
        }

        
        string os = GetWmiProperty("Win32_OperatingSystem", "Caption");
        string osArchitecture = GetWmiProperty("Win32_OperatingSystem", "OSArchitecture");
        Console.WriteLine($"Operating System: {os} ({osArchitecture})");

        Console.ReadLine();
    }

    static string GetWmiProperty(string wmiClass, string propertyName)
    {
        try
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT {propertyName} FROM {wmiClass}"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj[propertyName]?.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }

        return "N/A";
    }

    static string[] GetMonitors()
    {
        var monitors = new System.Collections.Generic.List<string>();
        try
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string monitorName = obj["Name"]?.ToString() ?? "N/A";
                    string screenHeight = obj["ScreenHeight"]?.ToString() ?? "N/A";
                    string screenWidth = obj["ScreenWidth"]?.ToString() ?? "N/A";
                    monitors.Add($"{monitorName} ({screenWidth}x{screenHeight})");
                }
            }
        }
        catch (Exception ex)
        {
            monitors.Add($"Error: {ex.Message}");
        }

        return monitors.ToArray();
    }

    static string[] GetGraphicsCards()
    {
        var graphicsCards = new System.Collections.Generic.List<string>();
        try
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = obj["Name"]?.ToString() ?? "N/A";
                    string ram = obj["AdapterRAM"]?.ToString() ?? "N/A";
                    double ramInMB = Math.Round(Convert.ToDouble(ram) / (1024 * 1024), 2);
                    graphicsCards.Add($"{name} with {ramInMB} MB RAM");
                }
            }
        }
        catch (Exception ex)
        {
            graphicsCards.Add($"Error: {ex.Message}");
        }

        return graphicsCards.ToArray();
    }
}
