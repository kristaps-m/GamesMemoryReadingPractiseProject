namespace WinFormsMemoReading
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private ComboBox cmbProcess;
        private Button btnRefresh;
        private TextBox txtAddress;
        private Button btnStart;
        private Button btnStop;
        private Label lblStatus;
        private GroupBox grpResults;
        private Label lblHealthValue;
        private Label lblManaValue;
        private Label lblXValue;
        private Label lblYValue;
        private TextBox txtLog;

        // Memory Scanner controls
        private TabControl tabControl;
        private TabPage tabReader;
        private TabPage tabScanner;
        private TextBox txtSearchValue;
        private Button btnSearchValue;
        private TextBox txtMinValue;
        private TextBox txtMaxValue;
        private Button btnSearchRange;
        private ListBox lstResults;
        private Button btnSelectAddress;
        private ProgressBar progressBar;
        private Label lblProgressStatus;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // Tab Control
            tabControl = new TabControl();
            tabControl.Location = new Point(0, 0);
            tabControl.Size = new Size(800, 650);
            tabControl.Dock = DockStyle.Fill;
            Controls.Add(tabControl);

            // ============= READER TAB =============
            tabReader = new TabPage();
            tabReader.Text = "Memory Reader";
            tabControl.TabPages.Add(tabReader);

            // Labels
            var lblProcess = new Label();
            lblProcess.Text = "Target Process:";
            lblProcess.Location = new Point(20, 20);
            lblProcess.Size = new Size(150, 25);
            tabReader.Controls.Add(lblProcess);

            // Process ComboBox
            cmbProcess = new ComboBox();
            cmbProcess.Location = new Point(180, 20);
            cmbProcess.Size = new Size(300, 25);
            cmbProcess.DropDownStyle = ComboBoxStyle.DropDownList;
            tabReader.Controls.Add(cmbProcess);

            // Refresh Button
            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(490, 20);
            btnRefresh.Size = new Size(100, 25);
            btnRefresh.Click += BtnRefresh_Click;
            tabReader.Controls.Add(btnRefresh);

            // Address Label
            var lblAddress = new Label();
            lblAddress.Text = "Memory Address (hex):";
            lblAddress.Location = new Point(20, 60);
            lblAddress.Size = new Size(150, 25);
            tabReader.Controls.Add(lblAddress);

            // Address TextBox
            txtAddress = new TextBox();
            txtAddress.Location = new Point(180, 60);
            txtAddress.Size = new Size(300, 25);
            txtAddress.Text = "0x";
            tabReader.Controls.Add(txtAddress);

            // Start Button
            btnStart = new Button();
            btnStart.Text = "Start Reading";
            btnStart.Location = new Point(180, 100);
            btnStart.Size = new Size(100, 30);
            btnStart.Click += BtnStart_Click;
            tabReader.Controls.Add(btnStart);

            // Stop Button
            btnStop = new Button();
            btnStop.Text = "Stop";
            btnStop.Location = new Point(290, 100);
            btnStop.Size = new Size(100, 30);
            btnStop.Click += BtnStop_Click;
            btnStop.Enabled = false;
            tabReader.Controls.Add(btnStop);

            // Status Label
            lblStatus = new Label();
            lblStatus.Text = "Status: Ready";
            lblStatus.Location = new Point(20, 140);
            lblStatus.Size = new Size(400, 25);
            tabReader.Controls.Add(lblStatus);

            // Results GroupBox
            grpResults = new GroupBox();
            grpResults.Text = "Memory Values";
            grpResults.Location = new Point(20, 180);
            grpResults.Size = new Size(570, 150);
            tabReader.Controls.Add(grpResults);

            // Health Label
            lblHealthValue = new Label();
            lblHealthValue.Text = "Health: -";
            lblHealthValue.Location = new Point(20, 30);
            lblHealthValue.Size = new Size(200, 25);
            grpResults.Controls.Add(lblHealthValue);

            // Mana Label
            lblManaValue = new Label();
            lblManaValue.Text = "Mana: -";
            lblManaValue.Location = new Point(20, 60);
            lblManaValue.Size = new Size(200, 25);
            grpResults.Controls.Add(lblManaValue);

            // X Label
            lblXValue = new Label();
            lblXValue.Text = "X: -";
            lblXValue.Location = new Point(20, 90);
            lblXValue.Size = new Size(200, 25);
            grpResults.Controls.Add(lblXValue);

            // Y Label
            lblYValue = new Label();
            lblYValue.Text = "Y: -";
            lblYValue.Location = new Point(20, 120);
            lblYValue.Size = new Size(200, 25);
            grpResults.Controls.Add(lblYValue);

            // Log TextBox
            txtLog = new TextBox();
            txtLog.Location = new Point(20, 350);
            txtLog.Size = new Size(570, 100);
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.ReadOnly = true;
            tabReader.Controls.Add(txtLog);

            // ============= SCANNER TAB =============
            tabScanner = new TabPage();
            tabScanner.Text = "Memory Scanner";
            tabControl.TabPages.Add(tabScanner);

            // Search by specific value
            var lblSearchValue = new Label();
            lblSearchValue.Text = "Search for value:";
            lblSearchValue.Location = new Point(20, 20);
            lblSearchValue.Size = new Size(120, 25);
            tabScanner.Controls.Add(lblSearchValue);

            txtSearchValue = new TextBox();
            txtSearchValue.Location = new Point(150, 20);
            txtSearchValue.Size = new Size(150, 25);
            txtSearchValue.Text = "1000";
            tabScanner.Controls.Add(txtSearchValue);

            btnSearchValue = new Button();
            btnSearchValue.Text = "Search";
            btnSearchValue.Location = new Point(310, 20);
            btnSearchValue.Size = new Size(80, 25);
            btnSearchValue.Click += BtnSearchValue_Click;
            tabScanner.Controls.Add(btnSearchValue);

            // Separator line
            var lblSeparator = new Label();
            lblSeparator.Text = "────────────────── OR ──────────────────";
            lblSeparator.Location = new Point(20, 60);
            lblSeparator.Size = new Size(570, 25);
            tabScanner.Controls.Add(lblSeparator);

            // Range search
            var lblMinValue = new Label();
            lblMinValue.Text = "Min value:";
            lblMinValue.Location = new Point(20, 100);
            lblMinValue.Size = new Size(100, 25);
            tabScanner.Controls.Add(lblMinValue);

            txtMinValue = new TextBox();
            txtMinValue.Location = new Point(130, 100);
            txtMinValue.Size = new Size(100, 25);
            txtMinValue.Text = "900";
            tabScanner.Controls.Add(txtMinValue);

            var lblMaxValue = new Label();
            lblMaxValue.Text = "Max value:";
            lblMaxValue.Location = new Point(250, 100);
            lblMaxValue.Size = new Size(100, 25);
            tabScanner.Controls.Add(lblMaxValue);

            txtMaxValue = new TextBox();
            txtMaxValue.Location = new Point(360, 100);
            txtMaxValue.Size = new Size(100, 25);
            txtMaxValue.Text = "1100";
            tabScanner.Controls.Add(txtMaxValue);

            btnSearchRange = new Button();
            btnSearchRange.Text = "Search Range";
            btnSearchRange.Location = new Point(470, 100);
            btnSearchRange.Size = new Size(100, 25);
            btnSearchRange.Click += BtnSearchRange_Click;
            tabScanner.Controls.Add(btnSearchRange);

            // Progress bar
            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 140);
            progressBar.Size = new Size(570, 25);
            tabScanner.Controls.Add(progressBar);

            // Progress status
            lblProgressStatus = new Label();
            lblProgressStatus.Text = "Ready";
            lblProgressStatus.Location = new Point(20, 170);
            lblProgressStatus.Size = new Size(570, 25);
            tabScanner.Controls.Add(lblProgressStatus);

            // Results list
            var lblResults = new Label();
            lblResults.Text = "Search Results:";
            lblResults.Location = new Point(20, 205);
            lblResults.Size = new Size(150, 25);
            tabScanner.Controls.Add(lblResults);

            lstResults = new ListBox();
            lstResults.Location = new Point(20, 235);
            lstResults.Size = new Size(570, 150);
            lstResults.DoubleClick += LstResults_DoubleClick;
            tabScanner.Controls.Add(lstResults);

            // Select Address Button
            btnSelectAddress = new Button();
            btnSelectAddress.Text = "Select & Use Address";
            btnSelectAddress.Location = new Point(20, 395);
            btnSelectAddress.Size = new Size(150, 30);
            btnSelectAddress.Click += BtnSelectAddress_Click;
            tabScanner.Controls.Add(btnSelectAddress);

            // Form settings
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 650);
            Text = "Memory Reader & Scanner";
            StartPosition = FormStartPosition.CenterScreen;
        }

        #endregion
    }
}
