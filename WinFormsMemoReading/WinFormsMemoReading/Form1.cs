using System.Diagnostics;

namespace WinFormsMemoReading
{
    public partial class Form1 : Form
    {
        private IntPtr _processHandle = IntPtr.Zero;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _readingTask;

        public Form1()
        {
            InitializeComponent();
            LoadProcesses();
        }

        private void LoadProcesses()
        {
            cmbProcess.Items.Clear();
            var processes = MemoryReaderUtil.GetRunningProcesses();
            foreach (var proc in processes)
            {
                cmbProcess.Items.Add(proc);
            }
            Log("Processes refreshed.");
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadProcesses();
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            if (cmbProcess.SelectedItem is not ProcessInfo selectedProcess)
            {
                MessageBox.Show("Please select a process first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please enter a memory address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!long.TryParse(txtAddress.Text.Replace("0x", "").Replace("0X", ""), System.Globalization.NumberStyles.HexNumber, null, out long addressValue))
            {
                MessageBox.Show("Invalid address format. Use hex (e.g., 0x1A2B3C4D).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Open process
            _processHandle = MemoryReaderUtil.OpenProcessPublic(MemoryReaderUtil.ProcessAccessFlags.VirtualMemoryRead, false, selectedProcess.PID);

            if (_processHandle == IntPtr.Zero)
            {
                MessageBox.Show("Failed to open process. Make sure you have sufficient permissions.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Start reading
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            txtAddress.ReadOnly = true;
            cmbProcess.Enabled = false;
            btnRefresh.Enabled = false;

            _cancellationTokenSource = new CancellationTokenSource();
            _readingTask = StartReadingMemoryAsync(new IntPtr(addressValue), _cancellationTokenSource.Token);

            Log($"Started reading from process {selectedProcess.Name} at address 0x{addressValue:X}");
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            txtAddress.ReadOnly = false;
            cmbProcess.Enabled = true;
            btnRefresh.Enabled = true;

            if (_processHandle != IntPtr.Zero)
            {
                MemoryReaderUtil.CloseHandlePublic(_processHandle);
                _processHandle = IntPtr.Zero;
            }

            Log("Stopped reading memory.");
        }

        private async Task StartReadingMemoryAsync(IntPtr memoryAddress, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Read the entire Player struct: int (4) + int (4) + float (4) + float (4) = 16 bytes
                    byte[] buffer = new byte[16];
                    bool success = MemoryReaderUtil.ReadProcessMemoryPublic(_processHandle, memoryAddress, buffer, buffer.Length, out int bytesRead);

                    if (success && bytesRead == 16)
                    {
                        // Parse all Player values
                        int health = BitConverter.ToInt32(buffer, 0);      // Offset 0
                        int mana = BitConverter.ToInt32(buffer, 4);        // Offset 4
                        float x = BitConverter.ToSingle(buffer, 8);        // Offset 8
                        float y = BitConverter.ToSingle(buffer, 12);       // Offset 12

                        // Update UI on main thread
                        Invoke(new Action(() =>
                        {
                            lblHealthValue.Text = $"Health: {health}";
                            lblManaValue.Text = $"Mana: {mana}";
                            lblXValue.Text = $"X: {x:F2}";
                            lblYValue.Text = $"Y: {y:F2}";
                            lblStatus.Text = $"Status: Reading... [{DateTime.Now:HH:mm:ss}]";
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            lblStatus.Text = "Status: Failed to read memory";
                        }));
                    }

                    await Task.Delay(2000, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    Log($"Error: {ex.Message}");
                    lblStatus.Text = "Status: Error occurred";
                }));
            }
        }

        private void Log(string message)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }
    }
}
