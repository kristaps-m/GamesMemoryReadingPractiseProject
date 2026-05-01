using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinFormsMemoReading
{
    internal static class MemoryScannerUtil
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        private static extern void SetLastError(uint dwErrCode);

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        private const uint PAGE_READWRITE = 0x04;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint MEM_COMMIT = 0x1000;

        public class MemoryScanResult
        {
            public IntPtr Address { get; set; }
            public int Value { get; set; }

            public override string ToString() => $"0x{Address.ToInt64():X} : {Value}";
        }

        /// <summary>
        /// Searches for a specific int value in process memory
        /// </summary>
        public static List<MemoryScanResult> SearchForValue(IntPtr hProcess, int searchValue, IProgress<int>? progress = null)
        {
            var results = new List<MemoryScanResult>();
            var scannedBytes = 0L;

            IntPtr currentAddress = IntPtr.Zero;

            while (true)
            {
                MEMORY_BASIC_INFORMATION mbi;
                uint size = (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));

                if (!VirtualQueryEx(hProcess, currentAddress, out mbi, size))
                {
                    break;
                }

                // Check if this region is readable and committed
                if ((mbi.State & MEM_COMMIT) != 0 && 
                    ((mbi.Protect & PAGE_READWRITE) != 0 || (mbi.Protect & PAGE_EXECUTE_READWRITE) != 0))
                {
                    try
                    {
                        long regionSize = mbi.RegionSize.ToInt64();
                        byte[] buffer = new byte[regionSize];

                        if (MemoryReaderUtil.ReadProcessMemoryPublic(hProcess, mbi.BaseAddress, buffer, (int)regionSize, out int bytesRead))
                        {
                            // Search for the value in this region
                            for (int i = 0; i <= bytesRead - sizeof(int); i++)
                            {
                                int value = BitConverter.ToInt32(buffer, i);
                                if (value == searchValue)
                                {
                                    results.Add(new MemoryScanResult
                                    {
                                        Address = new IntPtr(mbi.BaseAddress.ToInt64() + i),
                                        Value = value
                                    });
                                }
                            }

                            scannedBytes += bytesRead;
                            progress?.Report((int)(scannedBytes / 1024)); // Report in KB
                        }
                    }
                    catch
                    {
                        // Skip regions we can't read
                    }
                }

                // Move to next region
                currentAddress = new IntPtr(mbi.BaseAddress.ToInt64() + mbi.RegionSize.ToInt64());

                // Safety check to prevent infinite loops
                if (currentAddress.ToInt64() < 0)
                {
                    break;
                }
            }

            return results;
        }

        /// <summary>
        /// Searches for values within a range
        /// </summary>
        public static List<MemoryScanResult> SearchForRange(IntPtr hProcess, int minValue, int maxValue, IProgress<int>? progress = null)
        {
            var results = new List<MemoryScanResult>();
            var scannedBytes = 0L;

            IntPtr currentAddress = IntPtr.Zero;

            while (true)
            {
                MEMORY_BASIC_INFORMATION mbi;
                uint size = (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));

                if (!VirtualQueryEx(hProcess, currentAddress, out mbi, size))
                {
                    break;
                }

                // Check if this region is readable and committed
                if ((mbi.State & MEM_COMMIT) != 0 && 
                    ((mbi.Protect & PAGE_READWRITE) != 0 || (mbi.Protect & PAGE_EXECUTE_READWRITE) != 0))
                {
                    try
                    {
                        long regionSize = mbi.RegionSize.ToInt64();
                        byte[] buffer = new byte[regionSize];

                        if (MemoryReaderUtil.ReadProcessMemoryPublic(hProcess, mbi.BaseAddress, buffer, (int)regionSize, out int bytesRead))
                        {
                            // Search for values in this region
                            for (int i = 0; i <= bytesRead - sizeof(int); i++)
                            {
                                int value = BitConverter.ToInt32(buffer, i);
                                if (value >= minValue && value <= maxValue)
                                {
                                    results.Add(new MemoryScanResult
                                    {
                                        Address = new IntPtr(mbi.BaseAddress.ToInt64() + i),
                                        Value = value
                                    });
                                }
                            }

                            scannedBytes += bytesRead;
                            progress?.Report((int)(scannedBytes / 1024)); // Report in KB
                        }
                    }
                    catch
                    {
                        // Skip regions we can't read
                    }
                }

                // Move to next region
                currentAddress = new IntPtr(mbi.BaseAddress.ToInt64() + mbi.RegionSize.ToInt64());

                // Safety check to prevent infinite loops
                if (currentAddress.ToInt64() < 0)
                {
                    break;
                }
            }

            return results;
        }
    }
}
