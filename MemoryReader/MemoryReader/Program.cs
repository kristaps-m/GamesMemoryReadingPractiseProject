// Console.WriteLine("Hello, World!");
using System.Diagnostics;
using System.Runtime.InteropServices;



// Now the top-level statements come after the class
Console.WriteLine("Memory Reader App");
Console.WriteLine("================\n");

// Step 1: Find the target process
Console.Write("Enter target process name (e.g., 'ConsoleApp1'): ");
string? processName = Console.ReadLine();

if (string.IsNullOrEmpty(processName))
{
    Console.WriteLine("Invalid process name.");
    return;
}

Process[] processes = Process.GetProcessesByName(processName);

if (processes.Length == 0)
{
    Console.WriteLine($"Process '{processName}' not found. Make sure it's running.");
    return;
}

Process targetProcess = processes[0];
int pid = targetProcess.Id;

Console.WriteLine($"Found process: {targetProcess.ProcessName} (PID: {pid})");

// Step 2: Open handle to the process
IntPtr hProcess = MemoryReader.OpenProcessPublic(MemoryReader.ProcessAccessFlags.VirtualMemoryRead, false, pid);

if (hProcess == IntPtr.Zero)
{
    Console.WriteLine("Failed to open process. Make sure you have sufficient permissions.");
    return;
}

Console.WriteLine("Process handle opened successfully.");

// Step 3: Get the memory address
Console.Write("\nEnter the memory address (hex, e.g., 0x1A2B3C4D): ");
string? addressInput = Console.ReadLine();
// from !int to long
if (!long.TryParse(addressInput?.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out long addressValue))
{
    Console.WriteLine("Invalid address format.");
    MemoryReader.CloseHandlePublic(hProcess);
    return;
}

IntPtr memoryAddress = new IntPtr(addressValue);

Console.WriteLine("\nReading memory every 2 seconds (Press Ctrl+C to exit)...\n");

try
{
    while (true)
    {
        // Step 4: Read the entire Player struct
        // Player structure: int (4) + int (4) + float (4) + float (4) = 16 bytes
        byte[] buffer = new byte[16];
        bool success = MemoryReader.ReadProcessMemoryPublic(hProcess, memoryAddress, buffer, buffer.Length, out int bytesRead);

        if (success && bytesRead == 16)
        {
            // Parse all Player values
            int health = BitConverter.ToInt32(buffer, 0);      // Offset 0
            int mana = BitConverter.ToInt32(buffer, 4);        // Offset 4
            float x = BitConverter.ToSingle(buffer, 8);        // Offset 8
            float y = BitConverter.ToSingle(buffer, 12);       // Offset 12

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Health: {health}, Mana: {mana}, X: {x:F2}, Y: {y:F2}");
        }
        else
        {
            Console.WriteLine("Failed to read memory.");
        }

        Thread.Sleep(2000);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nStopped reading.");
}
finally
{
    // Step 5: Clean up
    MemoryReader.CloseHandlePublic(hProcess);
    Console.WriteLine("Process handle closed.");
}

// All the P/Invoke code goes in a static class
static class MemoryReader
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr OpenProcess(
        ProcessAccessFlags dwDesiredAccess,
        bool bInheritHandle,
        int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr hObject);

    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x00000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        SuspendResume = 0x00000800,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000
    }

    public static IntPtr OpenProcessPublic(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId)
    {
        return OpenProcess(dwDesiredAccess, bInheritHandle, dwProcessId);
    }

    public static bool ReadProcessMemoryPublic(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead)
    {
        return ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, dwSize, out lpNumberOfBytesRead);
    }

    public static bool CloseHandlePublic(IntPtr hObject)
    {
        return CloseHandle(hObject);
    }
}

/*
using System.Diagnostics;
using System.Runtime.InteropServices;

// P/Invoke declarations
[DllImport("kernel32.dll", SetLastError = true)]
static extern IntPtr OpenProcess(
    ProcessAccessFlags dwDesiredAccess,
    bool bInheritHandle,
    int dwProcessId);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool ReadProcessMemory(
    IntPtr hProcess,
    IntPtr lpBaseAddress,
    byte[] lpBuffer,
    int dwSize,
    out int lpNumberOfBytesRead);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool CloseHandle(IntPtr hObject);

[Flags]
public enum ProcessAccessFlags : uint
{
    All = 0x001F0FFF,
    Terminate = 0x00000001,
    CreateThread = 0x00000002,
    VirtualMemoryOperation = 0x00000008,
    VirtualMemoryRead = 0x00000010,
    VirtualMemoryWrite = 0x00000020,
    DuplicateHandle = 0x00000040,
    CreateProcess = 0x00000080,
    SetQuota = 0x00000100,
    SetInformation = 0x00000200,
    QueryInformation = 0x00000400,
    SuspendResume = 0x00000800,
    QueryLimitedInformation = 0x00001000,
    Synchronize = 0x00100000
}

// Main program
Console.WriteLine("Memory Reader App");
Console.WriteLine("================\n");

// Step 1: Find the target process
Console.Write("Enter target process name (e.g., 'ConsoleApp1'): ");
string? processName = Console.ReadLine();

if (string.IsNullOrEmpty(processName))
{
    Console.WriteLine("Invalid process name.");
    return;
}

Process[] processes = Process.GetProcessesByName(processName);

if (processes.Length == 0)
{
    Console.WriteLine($"Process '{processName}' not found. Make sure it's running.");
    return;
}

Process targetProcess = processes[0];
int pid = targetProcess.Id;

Console.WriteLine($"Found process: {targetProcess.ProcessName} (PID: {pid})");

// Step 2: Open handle to the process
IntPtr hProcess = OpenProcess(ProcessAccessFlags.VirtualMemoryRead, false, pid);

if (hProcess == IntPtr.Zero)
{
    Console.WriteLine("Failed to open process. Make sure you have sufficient permissions.");
    return;
}

Console.WriteLine("Process handle opened successfully.");

// Step 3: Get the memory address (you need to provide this manually or use address scanning)
Console.Write("\nEnter the memory address (hex, e.g., 0x1A2B3C4D): ");
string? addressInput = Console.ReadLine();

if (!int.TryParse(addressInput?.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out int addressValue))
{
    Console.WriteLine("Invalid address format.");
    CloseHandle(hProcess);
    return;
}

IntPtr memoryAddress = new IntPtr(addressValue);

Console.WriteLine("\nReading memory every 2 seconds (Press Ctrl+C to exit)...\n");

try
{
    while (true)
    {
        // Step 4: Read the memory
        byte[] buffer = new byte[4]; // int = 4 bytes
        bool success = ReadProcessMemory(hProcess, memoryAddress, buffer, buffer.Length, out int bytesRead);

        if (success && bytesRead == 4)
        {
            int healthValue = BitConverter.ToInt32(buffer, 0);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Health: {healthValue}");
        }
        else
        {
            Console.WriteLine("Failed to read memory.");
        }

        Thread.Sleep(2000);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nStopped reading.");
}
finally
{
    // Step 5: Clean up
    CloseHandle(hProcess);
    Console.WriteLine("Process handle closed.");
}
*/