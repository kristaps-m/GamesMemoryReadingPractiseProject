using System.Diagnostics;

namespace WinFormsMemoReading
{
    public partial class Form1 : Form
    {
        private IntPtr _processHandle = IntPtr.Zero;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _readingTask;
        private ProcessInfo? _selectedProcessForScanning;
        private List<MemoryScannerUtil.MemoryScanResult> _lastScanResults = new();

        public Form1()
        {
            InitializeComponent();
            LoadProcesses();
        }

        private void LoadProcesses()
        {
            cmbProcess.Items.Clear();
            cmbProcessScanner.Items.Clear();
            var processes = MemoryReaderUtil.GetRunningProcesses();
            foreach (var proc in processes)
            {
                cmbProcess.Items.Add(proc);
                cmbProcessScanner.Items.Add(proc);
            }
            Log("Processes refreshed.");
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadProcesses();
        }

        private void BtnRefreshScanner_Click(object? sender, EventArgs e)
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

        // ============= SCANNER METHODS =============

        private void BtnSearchValue_Click(object? sender, EventArgs e)
        {
            if (cmbProcessScanner.SelectedItem is not ProcessInfo selectedProcess)
            {
                MessageBox.Show("Please select a process first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtSearchValue.Text, out int searchValue))
            {
                MessageBox.Show("Please enter a valid integer value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedProcessForScanning = selectedProcess;
            PerformSearch(searchValue);
        }

        private void BtnSearchRange_Click(object? sender, EventArgs e)
        {
            if (cmbProcessScanner.SelectedItem is not ProcessInfo selectedProcess)
            {
                MessageBox.Show("Please select a process first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtMinValue.Text, out int minValue))
            {
                MessageBox.Show("Please enter a valid integer for minimum value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtMaxValue.Text, out int maxValue))
            {
                MessageBox.Show("Please enter a valid integer for maximum value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (minValue > maxValue)
            {
                MessageBox.Show("Minimum value must be less than or equal to maximum value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedProcessForScanning = selectedProcess;
            PerformRangeSearch(minValue, maxValue);
        }

        private void PerformSearch(int searchValue)
        {
            btnSearchValue.Enabled = false;
            btnSearchRange.Enabled = false;
            progressBar.Value = 0;
            lblProgressStatus.Text = "Searching...";
            Log($"Starting search for value: {searchValue}");
            Log($"Process: {_selectedProcessForScanning?.Name} (PID: {_selectedProcessForScanning?.PID})");

            Task.Run(() =>
            {
                try
                {
                    IntPtr hProcess = MemoryReaderUtil.OpenProcessPublic(
                        MemoryReaderUtil.ProcessAccessFlags.VirtualMemoryRead,
                        false,
                        _selectedProcessForScanning!.PID);

                    if (hProcess == IntPtr.Zero)
                    {
                        Invoke(new Action(() =>
                        {
                            Log("ERROR: Failed to open process handle!");
                            MessageBox.Show("Failed to open process. Make sure you have sufficient permissions or try running as Administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblProgressStatus.Text = "Search failed - Could not open process";
                            btnSearchValue.Enabled = true;
                            btnSearchRange.Enabled = true;
                        }));
                        return;
                    }

                    Log("Process opened successfully");

                    var progress = new Progress<int>(p => Invoke(new Action(() =>
                    {
                        if (p > 0) progressBar.Value = Math.Min(p / 100, 100);
                    })));

                    _lastScanResults = MemoryScannerUtil.SearchForValue(hProcess, searchValue, progress);

                    Invoke(new Action(() =>
                    {
                        DisplayResults();
                        Log($"Search completed: Found {_lastScanResults.Count} result(s) matching value {searchValue}");
                        lblProgressStatus.Text = $"Found {_lastScanResults.Count} result(s)";
                        btnSearchValue.Enabled = true;
                        btnSearchRange.Enabled = true;
                        progressBar.Value = 100;
                    }));

                    MemoryReaderUtil.CloseHandlePublic(hProcess);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Log($"Search Exception: {ex.GetType().Name}: {ex.Message}");
                        MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblProgressStatus.Text = "Search failed";
                        btnSearchValue.Enabled = true;
                        btnSearchRange.Enabled = true;
                    }));
                }
            });
        }

        private void PerformRangeSearch(int minValue, int maxValue)
        {
            btnSearchValue.Enabled = false;
            btnSearchRange.Enabled = false;
            progressBar.Value = 0;
            lblProgressStatus.Text = "Searching...";
            Log($"Starting range search: {minValue} to {maxValue}");
            Log($"Process: {_selectedProcessForScanning?.Name} (PID: {_selectedProcessForScanning?.PID})");

            Task.Run(() =>
            {
                try
                {
                    IntPtr hProcess = MemoryReaderUtil.OpenProcessPublic(
                        MemoryReaderUtil.ProcessAccessFlags.VirtualMemoryRead,
                        false,
                        _selectedProcessForScanning!.PID);

                    if (hProcess == IntPtr.Zero)
                    {
                        Invoke(new Action(() =>
                        {
                            Log("ERROR: Failed to open process handle!");
                            MessageBox.Show("Failed to open process. Make sure you have sufficient permissions or try running as Administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblProgressStatus.Text = "Search failed - Could not open process";
                            btnSearchValue.Enabled = true;
                            btnSearchRange.Enabled = true;
                        }));
                        return;
                    }

                    Log("Process opened successfully");

                    var progress = new Progress<int>(p => Invoke(new Action(() =>
                    {
                        if (p > 0) progressBar.Value = Math.Min(p / 100, 100);
                    })));

                    _lastScanResults = MemoryScannerUtil.SearchForRange(hProcess, minValue, maxValue, progress);

                    Invoke(new Action(() =>
                    {
                        DisplayResults();
                        Log($"Search completed: Found {_lastScanResults.Count} result(s) in range [{minValue}, {maxValue}]");
                        lblProgressStatus.Text = $"Found {_lastScanResults.Count} result(s) in range [{minValue}, {maxValue}]";
                        btnSearchValue.Enabled = true;
                        btnSearchRange.Enabled = true;
                        progressBar.Value = 100;
                    }));

                    MemoryReaderUtil.CloseHandlePublic(hProcess);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Log($"Search Exception: {ex.GetType().Name}: {ex.Message}");
                        MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblProgressStatus.Text = "Search failed";
                        btnSearchValue.Enabled = true;
                        btnSearchRange.Enabled = true;
                    }));
                }
            });
        }

        private void DisplayResults()
        {
            lstResults.Items.Clear();
            foreach (var result in _lastScanResults.Take(1000)) // Limit to 1000 results for performance
            {
                lstResults.Items.Add(result);
            }
        }

        private void LstResults_DoubleClick(object? sender, EventArgs e)
        {
            BtnSelectAddress_Click(null, EventArgs.Empty);
        }

        private void BtnSelectAddress_Click(object? sender, EventArgs e)
        {
            if (lstResults.SelectedItem is not MemoryScannerUtil.MemoryScanResult selectedResult)
            {
                MessageBox.Show("Please select a result first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Set the address in the Reader tab
            txtAddress.Text = $"0x{selectedResult.Address.ToInt64():X}";
            Log($"Selected address: 0x{selectedResult.Address.ToInt64():X} with value {selectedResult.Value}");

            // Switch to Reader tab
            tabControl.SelectedIndex = 0;
        }

        private void Log(string message)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }
    }
}
