namespace HaCreator.GUI
{
    partial class Initialization
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Initialization));
            button_initialise = new System.Windows.Forms.Button();
            wzEncryptionBox = new System.Windows.Forms.ComboBox();
            gameVersionBox = new System.Windows.Forms.TextBox();
            gameVersionLabel = new System.Windows.Forms.Label();
            toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            wzEncryptionLabel = new System.Windows.Forms.Label();
            pathLabel = new System.Windows.Forms.Label();
            statusTextBox = new System.Windows.Forms.TextBox();
            pathBox = new System.Windows.Forms.ComboBox();
            browseButton = new System.Windows.Forms.Button();
            button_checkMapErrors = new System.Windows.Forms.Button();
            localisationLabel = new System.Windows.Forms.Label();
            comboBox_localisation = new System.Windows.Forms.ComboBox();
            localisationDisclaimer = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label3
            // 
            pathLabel.AutoSize = true;
            pathLabel.Location = new System.Drawing.Point(6, 15);
            pathLabel.Name = "label3";
            pathLabel.Size = new System.Drawing.Size(33, 13);
            pathLabel.TabIndex = 9;
            pathLabel.Text = "Path:";
            // 
            // pathBox
            // 
            pathBox.FormattingEnabled = true;
            pathBox.Location = new System.Drawing.Point(74, 12);
            pathBox.Name = "pathBox";
            pathBox.Size = new System.Drawing.Size(237, 21);
            pathBox.TabIndex = 13;
            // 
            // button2
            // 
            browseButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            browseButton.Location = new System.Drawing.Point(315, 11);
            browseButton.Name = "button2";
            browseButton.Size = new System.Drawing.Size(54, 21);
            browseButton.TabIndex = 14;
            browseButton.Text = "...";
            browseButton.Click += browseButton_Click;
            // 
            // label2
            // 
            wzEncryptionLabel.AutoSize = true;
            wzEncryptionLabel.Location = new System.Drawing.Point(6, 40);
            wzEncryptionLabel.Name = "label2";
            wzEncryptionLabel.Size = new System.Drawing.Size(85, 13);
            wzEncryptionLabel.TabIndex = 8;
            wzEncryptionLabel.Text = "WZ encryption:";
            // 
            // versionBox
            // 
            wzEncryptionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            wzEncryptionBox.FormattingEnabled = true;
            wzEncryptionBox.Items.AddRange(new object[] { "GMS", "EMS , MSEA , KMS", "BMS , JMS", "Auto-Detect" });
            wzEncryptionBox.Location = new System.Drawing.Point(142, 38);
            wzEncryptionBox.Name = "versionBox";
            wzEncryptionBox.Size = new System.Drawing.Size(227, 21);
            wzEncryptionBox.TabIndex = 3;
            // 
            // gameVersionLabel
            // 
            gameVersionLabel.AutoSize = true;
            gameVersionLabel.Enabled = true;
            gameVersionLabel.Location = new System.Drawing.Point(5, 69);
            gameVersionLabel.Name = "gameVersionLabel";
            gameVersionLabel.Size = new System.Drawing.Size(102, 13);
            gameVersionLabel.TabIndex = 17;
            gameVersionLabel.Text = "Version:";
            //
            // gameVersionBox
            //
            gameVersionBox.Name = "gameVersionBox";
            gameVersionBox.Size = new System.Drawing.Size(227, 21);
            gameVersionBox.Location = new System.Drawing.Point(142, 67);
            gameVersionBox.PlaceholderText = "Detect automatically";
            // 
            // label1
            // 
            localisationLabel.AutoSize = true;
            localisationLabel.Enabled = false;
            localisationLabel.Location = new System.Drawing.Point(5, 100);
            localisationLabel.Name = "label1";
            localisationLabel.Size = new System.Drawing.Size(102, 13);
            localisationLabel.TabIndex = 17;
            localisationLabel.Text = "Client localisation:";
            // 
            // comboBox_localisation
            // 
            comboBox_localisation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox_localisation.Enabled = false;
            comboBox_localisation.FormattingEnabled = true;
            comboBox_localisation.Items.AddRange(new object[] { "GMS", "EMS , MSEA , KMS", "BMS , JMS", "Auto-Detect" });
            comboBox_localisation.Location = new System.Drawing.Point(141, 100);
            comboBox_localisation.Name = "comboBox_localisation";
            comboBox_localisation.Size = new System.Drawing.Size(228, 21);
            comboBox_localisation.TabIndex = 16;
            // 
            // label4
            // 
            localisationDisclaimer.Enabled = false;
            localisationDisclaimer.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            localisationDisclaimer.Location = new System.Drawing.Point(4, 125);
            localisationDisclaimer.Name = "label4";
            localisationDisclaimer.Size = new System.Drawing.Size(365, 33);
            localisationDisclaimer.TabIndex = 18;
            localisationDisclaimer.Text = "Please select the right localisation, as the saved .wz data parameters might be different.";
            // 
            // button_initialise
            // 
            button_initialise.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            button_initialise.Location = new System.Drawing.Point(4, 161);
            button_initialise.Name = "button_initialise";
            button_initialise.Size = new System.Drawing.Size(247, 28);
            button_initialise.TabIndex = 1;
            button_initialise.Text = "Initialize";
            button_initialise.Click += button_initialise_Click;
            
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new System.Drawing.Size(150, 40);
            // 
            // statusTextBox
            // 
            statusTextBox.Location = new System.Drawing.Point(4, 195);
            statusTextBox.Name = "textBox2";
            statusTextBox.ReadOnly = true;
            statusTextBox.Size = new System.Drawing.Size(364, 22);
            statusTextBox.TabIndex = 2;
            // 
            // button_checkMapErrors
            // 
            button_checkMapErrors.Location = new System.Drawing.Point(257, 161);
            button_checkMapErrors.Name = "button_checkMapErrors";
            button_checkMapErrors.Size = new System.Drawing.Size(111, 28);
            button_checkMapErrors.TabIndex = 15;
            button_checkMapErrors.Text = "Check map errors";
            button_checkMapErrors.UseVisualStyleBackColor = true;
            button_checkMapErrors.Click += debugButton_Click;
            
            // 
            // Initialization
            // 
            AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(372, 220);
            Controls.Add(pathLabel);
            Controls.Add(pathBox);
            Controls.Add(wzEncryptionLabel);
            Controls.Add(wzEncryptionBox);
            Controls.Add(gameVersionLabel);
            Controls.Add(gameVersionBox);
            Controls.Add(localisationLabel);
            Controls.Add(comboBox_localisation);
            Controls.Add(localisationDisclaimer);
            Controls.Add(button_checkMapErrors);
            Controls.Add(browseButton);
            Controls.Add(statusTextBox);
            Controls.Add(button_initialise);
            Font = new System.Drawing.Font("Segoe UI", 8.25F);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            Name = "Initialization";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "HaCreator";
            Load += Initialization_Load;
            KeyDown += Initialization_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button button_initialise;
        private System.Windows.Forms.ComboBox wzEncryptionBox;
        private System.Windows.Forms.Label gameVersionLabel;
        private System.Windows.Forms.TextBox gameVersionBox;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Label wzEncryptionLabel;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.ComboBox pathBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button button_checkMapErrors;
        private System.Windows.Forms.Label localisationLabel;
        private System.Windows.Forms.ComboBox comboBox_localisation;
        private System.Windows.Forms.Label localisationDisclaimer;
    }
}

