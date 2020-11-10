namespace Biovation.Tools.UserAdapter
{
    partial class UserAdaptationForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserAdaptationForm));
            this.BiovationConnectionButton = new System.Windows.Forms.Button();
            this.BiovationServerAddressTextBox = new System.Windows.Forms.TextBox();
            this.BiovationServerPortTextBox = new System.Windows.Forms.TextBox();
            this.BiovationDeviceListComboBox = new System.Windows.Forms.ComboBox();
            this.DownloadSampleButton = new System.Windows.Forms.Button();
            this.PickUserAdaptationFileButton = new System.Windows.Forms.Button();
            this.StartProcessButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BiovationDeviceLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.MinimizeButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BiovationConnectionButton
            // 
            this.BiovationConnectionButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BiovationConnectionButton.BackgroundImage")));
            this.BiovationConnectionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BiovationConnectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BiovationConnectionButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BiovationConnectionButton.Location = new System.Drawing.Point(36, 37);
            this.BiovationConnectionButton.Name = "BiovationConnectionButton";
            this.BiovationConnectionButton.Size = new System.Drawing.Size(97, 31);
            this.BiovationConnectionButton.TabIndex = 0;
            this.BiovationConnectionButton.Text = "اتصال";
            this.BiovationConnectionButton.UseVisualStyleBackColor = true;
            this.BiovationConnectionButton.Click += new System.EventHandler(this.BiovationConnectionButton_Click);
            // 
            // BiovationServerAddressTextBox
            // 
            this.BiovationServerAddressTextBox.Location = new System.Drawing.Point(389, 39);
            this.BiovationServerAddressTextBox.Name = "BiovationServerAddressTextBox";
            this.BiovationServerAddressTextBox.Size = new System.Drawing.Size(258, 27);
            this.BiovationServerAddressTextBox.TabIndex = 1;
            this.BiovationServerAddressTextBox.Text = "127.0.0.1";
            // 
            // BiovationServerPortTextBox
            // 
            this.BiovationServerPortTextBox.Location = new System.Drawing.Point(152, 39);
            this.BiovationServerPortTextBox.Name = "BiovationServerPortTextBox";
            this.BiovationServerPortTextBox.Size = new System.Drawing.Size(108, 27);
            this.BiovationServerPortTextBox.TabIndex = 1;
            this.BiovationServerPortTextBox.Text = "9038";
            this.BiovationServerPortTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BiovationDeviceListComboBox
            // 
            this.BiovationDeviceListComboBox.Enabled = false;
            this.BiovationDeviceListComboBox.FormattingEnabled = true;
            this.BiovationDeviceListComboBox.Location = new System.Drawing.Point(152, 108);
            this.BiovationDeviceListComboBox.Name = "BiovationDeviceListComboBox";
            this.BiovationDeviceListComboBox.Size = new System.Drawing.Size(452, 28);
            this.BiovationDeviceListComboBox.TabIndex = 2;
            // 
            // DownloadSampleButton
            // 
            this.DownloadSampleButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("DownloadSampleButton.BackgroundImage")));
            this.DownloadSampleButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.DownloadSampleButton.Enabled = false;
            this.DownloadSampleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DownloadSampleButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.DownloadSampleButton.Location = new System.Drawing.Point(538, 181);
            this.DownloadSampleButton.Name = "DownloadSampleButton";
            this.DownloadSampleButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.DownloadSampleButton.Size = new System.Drawing.Size(196, 37);
            this.DownloadSampleButton.TabIndex = 3;
            this.DownloadSampleButton.Text = "دریافت فایل نمونه";
            this.DownloadSampleButton.UseVisualStyleBackColor = true;
            // 
            // PickUserAdaptationFileButton
            // 
            this.PickUserAdaptationFileButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PickUserAdaptationFileButton.BackgroundImage")));
            this.PickUserAdaptationFileButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PickUserAdaptationFileButton.Enabled = false;
            this.PickUserAdaptationFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PickUserAdaptationFileButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.PickUserAdaptationFileButton.Location = new System.Drawing.Point(254, 181);
            this.PickUserAdaptationFileButton.Name = "PickUserAdaptationFileButton";
            this.PickUserAdaptationFileButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.PickUserAdaptationFileButton.Size = new System.Drawing.Size(252, 37);
            this.PickUserAdaptationFileButton.TabIndex = 3;
            this.PickUserAdaptationFileButton.Text = "انتخاب فایل اطلاعات تغییر شماره";
            this.PickUserAdaptationFileButton.UseVisualStyleBackColor = true;
            this.PickUserAdaptationFileButton.Click += new System.EventHandler(this.PickUserAdaptationFileButton_Click);
            // 
            // StartProcessButton
            // 
            this.StartProcessButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("StartProcessButton.BackgroundImage")));
            this.StartProcessButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.StartProcessButton.Enabled = false;
            this.StartProcessButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartProcessButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.StartProcessButton.Location = new System.Drawing.Point(37, 268);
            this.StartProcessButton.Name = "StartProcessButton";
            this.StartProcessButton.Size = new System.Drawing.Size(113, 35);
            this.StartProcessButton.TabIndex = 4;
            this.StartProcessButton.Text = "شروع عملیات";
            this.StartProcessButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(63)))), ((int)(((byte)(136)))));
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 376);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 74);
            this.panel1.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(175, 62);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.panel2.Controls.Add(this.BiovationDeviceLabel);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.PickUserAdaptationFileButton);
            this.panel2.Controls.Add(this.DownloadSampleButton);
            this.panel2.Controls.Add(this.BiovationServerAddressTextBox);
            this.panel2.Controls.Add(this.BiovationServerPortTextBox);
            this.panel2.Controls.Add(this.BiovationDeviceListComboBox);
            this.panel2.Controls.Add(this.StartProcessButton);
            this.panel2.Controls.Add(this.BiovationConnectionButton);
            this.panel2.Location = new System.Drawing.Point(12, 44);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(776, 332);
            this.panel2.TabIndex = 6;
            // 
            // BiovationDeviceLabel
            // 
            this.BiovationDeviceLabel.AutoSize = true;
            this.BiovationDeviceLabel.Enabled = false;
            this.BiovationDeviceLabel.Location = new System.Drawing.Point(634, 111);
            this.BiovationDeviceLabel.Name = "BiovationDeviceLabel";
            this.BiovationDeviceLabel.Size = new System.Drawing.Size(100, 20);
            this.BiovationDeviceLabel.TabIndex = 6;
            this.BiovationDeviceLabel.Text = "انتخاب دستگاه";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(266, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "پورت باوویشن";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(672, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "آدرس سرور";
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.CloseButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CloseButton.BackgroundImage")));
            this.CloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseButton.ForeColor = System.Drawing.Color.Transparent;
            this.CloseButton.Location = new System.Drawing.Point(24, 12);
            this.CloseButton.Margin = new System.Windows.Forms.Padding(0);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(27, 29);
            this.CloseButton.TabIndex = 7;
            this.CloseButton.UseVisualStyleBackColor = false;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // MinimizeButton
            // 
            this.MinimizeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.MinimizeButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("MinimizeButton.BackgroundImage")));
            this.MinimizeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.MinimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MinimizeButton.ForeColor = System.Drawing.Color.Transparent;
            this.MinimizeButton.Location = new System.Drawing.Point(56, 12);
            this.MinimizeButton.Margin = new System.Windows.Forms.Padding(0);
            this.MinimizeButton.Name = "MinimizeButton";
            this.MinimizeButton.Size = new System.Drawing.Size(27, 29);
            this.MinimizeButton.TabIndex = 7;
            this.MinimizeButton.UseVisualStyleBackColor = false;
            this.MinimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            // 
            // UserAdaptationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MinimizeButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UserAdaptationForm";
            this.Text = "Biovation User Adaptor Tool";
            this.Load += new System.EventHandler(this.UserAdaptationForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BiovationConnectionButton;
        private System.Windows.Forms.TextBox BiovationServerAddressTextBox;
        private System.Windows.Forms.TextBox BiovationServerPortTextBox;
        private System.Windows.Forms.ComboBox BiovationDeviceListComboBox;
        private System.Windows.Forms.Button DownloadSampleButton;
        private System.Windows.Forms.Button PickUserAdaptationFileButton;
        private System.Windows.Forms.Button StartProcessButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label BiovationDeviceLabel;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button MinimizeButton;
    }
}

