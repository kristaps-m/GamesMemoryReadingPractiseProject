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

            // Labels
            var lblProcess = new Label();
            lblProcess.Text = "Target Process:";
            lblProcess.Location = new Point(20, 20);
            lblProcess.Size = new Size(150, 25);
            Controls.Add(lblProcess);

            // Process ComboBox
            cmbProcess = new ComboBox();
            cmbProcess.Location = new Point(180, 20);
            cmbProcess.Size = new Size(300, 25);
            cmbProcess.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(cmbProcess);

            // Refresh Button
            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(490, 20);
            btnRefresh.Size = new Size(100, 25);
            btnRefresh.Click += BtnRefresh_Click;
            Controls.Add(btnRefresh);

            // Address Label
            var lblAddress = new Label();
            lblAddress.Text = "Memory Address (hex):";
            lblAddress.Location = new Point(20, 60);
            lblAddress.Size = new Size(150, 25);
            Controls.Add(lblAddress);

            // Address TextBox
            txtAddress = new TextBox();
            txtAddress.Location = new Point(180, 60);
            txtAddress.Size = new Size(300, 25);
            txtAddress.Text = "0x";
            Controls.Add(txtAddress);

            // Start Button
            btnStart = new Button();
            btnStart.Text = "Start Reading";
            btnStart.Location = new Point(180, 100);
            btnStart.Size = new Size(100, 30);
            btnStart.Click += BtnStart_Click;
            Controls.Add(btnStart);

            // Stop Button
            btnStop = new Button();
            btnStop.Text = "Stop";
            btnStop.Location = new Point(290, 100);
            btnStop.Size = new Size(100, 30);
            btnStop.Click += BtnStop_Click;
            btnStop.Enabled = false;
            Controls.Add(btnStop);

            // Status Label
            lblStatus = new Label();
            lblStatus.Text = "Status: Ready";
            lblStatus.Location = new Point(20, 140);
            lblStatus.Size = new Size(400, 25);
            Controls.Add(lblStatus);

            // Results GroupBox
            grpResults = new GroupBox();
            grpResults.Text = "Memory Values";
            grpResults.Location = new Point(20, 180);
            grpResults.Size = new Size(570, 150);
            Controls.Add(grpResults);

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
            Controls.Add(txtLog);

            // Form settings
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(620, 470);
            Text = "Memory Reader";
            StartPosition = FormStartPosition.CenterScreen;
        }

        #endregion
    }
}
