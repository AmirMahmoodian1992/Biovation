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
            this.BiovationConnectionButton = new System.Windows.Forms.Button();
            this.BiovationServerAddressTextBox = new System.Windows.Forms.TextBox();
            this.BiovationServerPortTextBox = new System.Windows.Forms.TextBox();
            this.BiovationDeviceListComboBox = new System.Windows.Forms.ComboBox();
            this.DownloadSampleButton = new System.Windows.Forms.Button();
            this.AddSampleFilesInsterd = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BiovationConnectionButton
            // 
            this.BiovationConnectionButton.Location = new System.Drawing.Point(37, 69);
            this.BiovationConnectionButton.Name = "BiovationConnectionButton";
            this.BiovationConnectionButton.Size = new System.Drawing.Size(94, 29);
            this.BiovationConnectionButton.TabIndex = 0;
            this.BiovationConnectionButton.Text = "connect";
            this.BiovationConnectionButton.UseVisualStyleBackColor = true;
            this.BiovationConnectionButton.Click += new System.EventHandler(this.BiovationConnectionButton_Click);
            // 
            // BiovationServerAddressTextBox
            // 
            this.BiovationServerAddressTextBox.Location = new System.Drawing.Point(455, 70);
            this.BiovationServerAddressTextBox.Name = "BiovationServerAddressTextBox";
            this.BiovationServerAddressTextBox.Size = new System.Drawing.Size(258, 27);
            this.BiovationServerAddressTextBox.TabIndex = 1;
            this.BiovationServerAddressTextBox.Text = "127.0.0.1";
            // 
            // BiovationServerPortTextBox
            // 
            this.BiovationServerPortTextBox.Location = new System.Drawing.Point(196, 70);
            this.BiovationServerPortTextBox.Name = "BiovationServerPortTextBox";
            this.BiovationServerPortTextBox.Size = new System.Drawing.Size(108, 27);
            this.BiovationServerPortTextBox.TabIndex = 1;
            this.BiovationServerPortTextBox.Text = "9038";
            // 
            // BiovationDeviceListComboBox
            // 
            this.BiovationDeviceListComboBox.FormattingEnabled = true;
            this.BiovationDeviceListComboBox.Location = new System.Drawing.Point(210, 151);
            this.BiovationDeviceListComboBox.Name = "BiovationDeviceListComboBox";
            this.BiovationDeviceListComboBox.Size = new System.Drawing.Size(332, 28);
            this.BiovationDeviceListComboBox.TabIndex = 2;
            // 
            // DownloadSampleButton
            // 
            this.DownloadSampleButton.Location = new System.Drawing.Point(455, 214);
            this.DownloadSampleButton.Name = "DownloadSampleButton";
            this.DownloadSampleButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.DownloadSampleButton.Size = new System.Drawing.Size(196, 29);
            this.DownloadSampleButton.TabIndex = 3;
            this.DownloadSampleButton.Text = "Get Sample File";
            this.DownloadSampleButton.UseVisualStyleBackColor = true;
            // 
            // AddSampleFilesInsterd
            // 
            this.AddSampleFilesInsterd.Location = new System.Drawing.Point(108, 214);
            this.AddSampleFilesInsterd.Name = "AddSampleFilesInsterd";
            this.AddSampleFilesInsterd.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.AddSampleFilesInsterd.Size = new System.Drawing.Size(196, 29);
            this.AddSampleFilesInsterd.TabIndex = 3;
            this.AddSampleFilesInsterd.Text = "Get Sample File";
            this.AddSampleFilesInsterd.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(342, 264);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 4;
            this.button1.Text = "StartButton";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // UserAdaptationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.AddSampleFilesInsterd);
            this.Controls.Add(this.DownloadSampleButton);
            this.Controls.Add(this.BiovationDeviceListComboBox);
            this.Controls.Add(this.BiovationServerPortTextBox);
            this.Controls.Add(this.BiovationServerAddressTextBox);
            this.Controls.Add(this.BiovationConnectionButton);
            this.Name = "UserAdaptationForm";
            this.Text = "Biovation User Adaptor Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BiovationConnectionButton;
        private System.Windows.Forms.TextBox BiovationServerAddressTextBox;
        private System.Windows.Forms.TextBox BiovationServerPortTextBox;
        private System.Windows.Forms.ComboBox BiovationDeviceListComboBox;
        private System.Windows.Forms.Button DownloadSampleButton;
        private System.Windows.Forms.Button AddSampleFilesInsterd;
        private System.Windows.Forms.Button button1;
    }
}

