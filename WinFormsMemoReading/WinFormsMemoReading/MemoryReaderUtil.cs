using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinFormsMemoReading
{
    internal static class MemoryReaderUtil
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(
            ProcessAccessFlags dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int dwSize,
            out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

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

        public static List<ProcessInfo> GetRunningProcesses()
        {
            var processList = new List<ProcessInfo>();
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    processList.Add(new ProcessInfo
                    {
                        Name = process.ProcessName,
                        PID = process.Id,
                        DisplayName = $"{process.ProcessName} (PID: {process.Id})"
                    });
                }
                catch { /* Skip processes we can't access */ }
            }
            return processList.OrderBy(p => p.Name).ToList();
        }
    }

    public class ProcessInfo
    {
        public string Name { get; set; } = string.Empty;
        public int PID { get; set; }
        public string DisplayName { get; set; } = string.Empty;

        public override string ToString() => DisplayName;
    }

    public class MemoryReadResult
    {
        public bool Success { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
