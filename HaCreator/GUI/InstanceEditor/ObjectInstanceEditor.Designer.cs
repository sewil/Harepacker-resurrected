namespace HaCreator.GUI.InstanceEditor
{
    partial class ObjectInstanceEditor
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
            pathLabel = new System.Windows.Forms.Label();
            xInput = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            yInput = new System.Windows.Forms.NumericUpDown();
            label3 = new System.Windows.Forms.Label();
            zInput = new System.Windows.Forms.NumericUpDown();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            nameBox = new System.Windows.Forms.TextBox();
            rBox = new System.Windows.Forms.CheckBox();
            flowBox = new System.Windows.Forms.CheckBox();
            rxBox = new System.Windows.Forms.CheckBox();
            ryBox = new System.Windows.Forms.CheckBox();
            cxBox = new System.Windows.Forms.CheckBox();
            cyBox = new System.Windows.Forms.CheckBox();
            rxInt = new System.Windows.Forms.NumericUpDown();
            ryInt = new System.Windows.Forms.NumericUpDown();
            cxInt = new System.Windows.Forms.NumericUpDown();
            cyInt = new System.Windows.Forms.NumericUpDown();
            nameEnable = new System.Windows.Forms.CheckBox();
            hideBox = new System.Windows.Forms.CheckBox();
            reactorBox = new System.Windows.Forms.CheckBox();
            questList = new System.Windows.Forms.ListBox();
            questAdd = new System.Windows.Forms.Button();
            questRemove = new System.Windows.Forms.Button();
            questEnable = new System.Windows.Forms.CheckBox();
            tagsEnable = new System.Windows.Forms.CheckBox();
            tagsBox = new System.Windows.Forms.TextBox();
            panel1 = new System.Windows.Forms.Panel();
            flipBox = new System.Windows.Forms.CheckBox();
            reactorTimeInt = new System.Windows.Forms.NumericUpDown();
            reactorTimeCheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)xInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)zInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rxInt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ryInt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cxInt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cyInt).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)reactorTimeInt).BeginInit();
            SuspendLayout();
            // 
            // pathLabel
            // 
            pathLabel.Location = new System.Drawing.Point(0, 0);
            pathLabel.Name = "pathLabel";
            pathLabel.Size = new System.Drawing.Size(313, 101);
            pathLabel.TabIndex = 0;
            pathLabel.Text = "label1";
            pathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // xInput
            // 
            xInput.Location = new System.Drawing.Point(22, 103);
            xInput.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            xInput.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            xInput.Name = "xInput";
            xInput.Size = new System.Drawing.Size(59, 22);
            xInput.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 106);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(13, 13);
            label1.TabIndex = 2;
            label1.Text = "X";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 132);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(12, 13);
            label2.TabIndex = 4;
            label2.Text = "Y";
            // 
            // yInput
            // 
            yInput.Location = new System.Drawing.Point(22, 129);
            yInput.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            yInput.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            yInput.Name = "yInput";
            yInput.Size = new System.Drawing.Size(59, 22);
            yInput.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 158);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(13, 13);
            label3.TabIndex = 6;
            label3.Text = "Z";
            // 
            // zInput
            // 
            zInput.Location = new System.Drawing.Point(22, 155);
            zInput.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            zInput.Name = "zInput";
            zInput.Size = new System.Drawing.Size(59, 22);
            zInput.TabIndex = 2;
            // 
            // okButton
            // 
            okButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            okButton.Location = new System.Drawing.Point(2, 386);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(150, 41);
            okButton.TabIndex = 23;
            okButton.Text = "OK";
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            cancelButton.Location = new System.Drawing.Point(163, 386);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(150, 41);
            cancelButton.TabIndex = 24;
            cancelButton.Text = "Cancel";
            cancelButton.Click += cancelButton_Click;
            // 
            // nameBox
            // 
            nameBox.Enabled = false;
            nameBox.Location = new System.Drawing.Point(66, 184);
            nameBox.Name = "nameBox";
            nameBox.Size = new System.Drawing.Size(107, 22);
            nameBox.TabIndex = 4;
            // 
            // rBox
            // 
            rBox.AutoSize = true;
            rBox.Location = new System.Drawing.Point(240, 231);
            rBox.Name = "rBox";
            rBox.Size = new System.Drawing.Size(33, 17);
            rBox.TabIndex = 5;
            rBox.Text = "R";
            // 
            // flowBox
            // 
            flowBox.AutoSize = true;
            flowBox.Location = new System.Drawing.Point(189, 106);
            flowBox.Name = "flowBox";
            flowBox.Size = new System.Drawing.Size(51, 17);
            flowBox.TabIndex = 8;
            flowBox.Text = "Flow";
            // 
            // rxBox
            // 
            rxBox.AutoSize = true;
            rxBox.Location = new System.Drawing.Point(189, 127);
            rxBox.Name = "rxBox";
            rxBox.Size = new System.Drawing.Size(39, 17);
            rxBox.TabIndex = 9;
            rxBox.Text = "RX";
            rxBox.CheckedChanged += enablingCheckBox_CheckChanged;
            // 
            // ryBox
            // 
            ryBox.AutoSize = true;
            ryBox.Location = new System.Drawing.Point(189, 153);
            ryBox.Name = "ryBox";
            ryBox.Size = new System.Drawing.Size(38, 17);
            ryBox.TabIndex = 11;
            ryBox.Text = "RY";
            ryBox.CheckedChanged += enablingCheckBox_CheckChanged;
            // 
            // cxBox
            // 
            cxBox.AutoSize = true;
            cxBox.Location = new System.Drawing.Point(189, 179);
            cxBox.Name = "cxBox";
            cxBox.Size = new System.Drawing.Size(39, 17);
            cxBox.TabIndex = 13;
            cxBox.Text = "CX";
            cxBox.CheckedChanged += enablingCheckBox_CheckChanged;
            // 
            // cyBox
            // 
            cyBox.AutoSize = true;
            cyBox.Location = new System.Drawing.Point(189, 205);
            cyBox.Name = "cyBox";
            cyBox.Size = new System.Drawing.Size(38, 17);
            cyBox.TabIndex = 15;
            cyBox.Text = "CY";
            cyBox.CheckedChanged += enablingCheckBox_CheckChanged;
            // 
            // rxInt
            // 
            rxInt.Enabled = false;
            rxInt.Location = new System.Drawing.Point(233, 125);
            rxInt.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            rxInt.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            rxInt.Name = "rxInt";
            rxInt.Size = new System.Drawing.Size(62, 22);
            rxInt.TabIndex = 10;
            // 
            // ryInt
            // 
            ryInt.Enabled = false;
            ryInt.Location = new System.Drawing.Point(233, 151);
            ryInt.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            ryInt.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            ryInt.Name = "ryInt";
            ryInt.Size = new System.Drawing.Size(62, 22);
            ryInt.TabIndex = 12;
            // 
            // cxInt
            // 
            cxInt.Enabled = false;
            cxInt.Location = new System.Drawing.Point(233, 177);
            cxInt.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            cxInt.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            cxInt.Name = "cxInt";
            cxInt.Size = new System.Drawing.Size(62, 22);
            cxInt.TabIndex = 14;
            // 
            // cyInt
            // 
            cyInt.Enabled = false;
            cyInt.Location = new System.Drawing.Point(233, 203);
            cyInt.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            cyInt.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            cyInt.Name = "cyInt";
            cyInt.Size = new System.Drawing.Size(62, 22);
            cyInt.TabIndex = 16;
            // 
            // nameEnable
            // 
            nameEnable.AutoSize = true;
            nameEnable.Location = new System.Drawing.Point(9, 186);
            nameEnable.Name = "nameEnable";
            nameEnable.Size = new System.Drawing.Size(55, 17);
            nameEnable.TabIndex = 3;
            nameEnable.Text = "Name";
            nameEnable.CheckedChanged += enablingCheckBox_CheckChanged;
            // 
            // hideBox
            // 
            hideBox.AutoSize = true;
            hideBox.Location = new System.Drawing.Point(189, 254);
            hideBox.Name = "hideBox";
            hideBox.Size = new System.Drawing.Size(50, 17);
            hideBox.TabIndex = 6;
            hideBox.Text = "Hide";
            // 
            // reactorBox
            // 
            reactorBox.AutoSize = true;
            reactorBox.Location = new System.Drawing.Point(9, 212);
            reactorBox.Name = "reactorBox";
            reactorBox.Size = new System.Drawing.Size(65, 17);
            reactorBox.TabIndex = 7;
            reactorBox.Text = "Reactor";
            // 
            // questList
            // 
            questList.Enabled = false;
            questList.FormattingEnabled = true;
            questList.ItemHeight = 13;
            questList.Location = new System.Drawing.Point(66, 9);
            questList.Name = "questList";
            questList.Size = new System.Drawing.Size(241, 56);
            questList.TabIndex = 22;
            // 
            // questAdd
            // 
            questAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            questAdd.Enabled = false;
            questAdd.Location = new System.Drawing.Point(5, 32);
            questAdd.Name = "questAdd";
            questAdd.Size = new System.Drawing.Size(55, 23);
            questAdd.TabIndex = 20;
            questAdd.Text = "Add";
            questAdd.Click += questAdd_Click;
            // 
            // questRemove
            // 
            questRemove.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            questRemove.Enabled = false;
            questRemove.Location = new System.Drawing.Point(5, 55);
            questRemove.Name = "questRemove";
            questRemove.Size = new System.Drawing.Size(55, 23);
            questRemove.TabIndex = 21;
            questRemove.Text = "Remove";
            questRemove.Click += questRemove_Click;
            // 
            // questEnable
            // 
            questEnable.AutoSize = true;
            questEnable.Location = new System.Drawing.Point(6, 9);
            questEnable.Name = "questEnable";
            questEnable.Size = new System.Drawing.Size(56, 17);
            questEnable.TabIndex = 19;
            questEnable.Text = "Quest";
            questEnable.CheckedChanged += enablingCheckBox_CheckChanged;
            // 
            // tagsEnable
            // 
            tagsEnable.AutoSize = true;
            tagsEnable.Location = new System.Drawing.Point(9, 276);
            tagsEnable.Name = "tagsEnable";
            tagsEnable.Size = new System.Drawing.Size(48, 17);
            tagsEnable.TabIndex = 17;
            tagsEnable.Text = "Tags";
            tagsEnable.CheckedChanged += enablingCheckBox_CheckChanged;
            // 
            // tagsBox
            // 
            tagsBox.Enabled = false;
            tagsBox.Location = new System.Drawing.Point(65, 274);
            tagsBox.Name = "tagsBox";
            tagsBox.Size = new System.Drawing.Size(108, 22);
            tagsBox.TabIndex = 18;
            // 
            // panel1
            // 
            panel1.Controls.Add(questEnable);
            panel1.Controls.Add(questAdd);
            panel1.Controls.Add(questRemove);
            panel1.Controls.Add(questList);
            panel1.Location = new System.Drawing.Point(3, 298);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(310, 82);
            panel1.TabIndex = 25;
            // 
            // flipBox
            // 
            flipBox.AutoSize = true;
            flipBox.Location = new System.Drawing.Point(189, 231);
            flipBox.Name = "flipBox";
            flipBox.Size = new System.Drawing.Size(45, 17);
            flipBox.TabIndex = 26;
            flipBox.Text = "Flip";
            // 
            // reactorTimeInt
            // 
            reactorTimeInt.Location = new System.Drawing.Point(66, 240);
            reactorTimeInt.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            reactorTimeInt.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            reactorTimeInt.Name = "reactorTimeInt";
            reactorTimeInt.Enabled = false;
            reactorTimeInt.Size = new System.Drawing.Size(107, 22);
            reactorTimeInt.TabIndex = 28;
            // 
            // reactorTimeCheckbox
            // 
            reactorTimeCheckbox.AutoSize = true;
            reactorTimeCheckbox.Location = new System.Drawing.Point(9, 241);
            reactorTimeCheckbox.Name = "reactorTimeCheckbox";
            reactorTimeCheckbox.Size = new System.Drawing.Size(59, 17);
            reactorTimeCheckbox.TabIndex = 29;
            reactorTimeCheckbox.Text = "R Time";
            reactorTimeCheckbox.UseVisualStyleBackColor = true;
            reactorTimeCheckbox.CheckedChanged += enablingCheckBox_CheckChanged;
            // 
            // ObjectInstanceEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(315, 429);
            Controls.Add(reactorTimeCheckbox);
            Controls.Add(reactorTimeInt);
            Controls.Add(flipBox);
            Controls.Add(panel1);
            Controls.Add(tagsEnable);
            Controls.Add(tagsBox);
            Controls.Add(reactorBox);
            Controls.Add(hideBox);
            Controls.Add(nameEnable);
            Controls.Add(cyInt);
            Controls.Add(cxInt);
            Controls.Add(ryInt);
            Controls.Add(rxInt);
            Controls.Add(cyBox);
            Controls.Add(cxBox);
            Controls.Add(ryBox);
            Controls.Add(rxBox);
            Controls.Add(flowBox);
            Controls.Add(rBox);
            Controls.Add(nameBox);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(label3);
            Controls.Add(zInput);
            Controls.Add(label2);
            Controls.Add(yInput);
            Controls.Add(label1);
            Controls.Add(xInput);
            Controls.Add(pathLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ObjectInstanceEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Object";
            ((System.ComponentModel.ISupportInitialize)xInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)yInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)zInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)rxInt).EndInit();
            ((System.ComponentModel.ISupportInitialize)ryInt).EndInit();
            ((System.ComponentModel.ISupportInitialize)cxInt).EndInit();
            ((System.ComponentModel.ISupportInitialize)cyInt).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)reactorTimeInt).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown xInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown yInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown zInput;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.CheckBox rBox;
        private System.Windows.Forms.CheckBox rxBox;
        private System.Windows.Forms.CheckBox flowBox;
        private System.Windows.Forms.CheckBox ryBox;
        private System.Windows.Forms.CheckBox cxBox;
        private System.Windows.Forms.CheckBox cyBox;
        private System.Windows.Forms.NumericUpDown rxInt;
        private System.Windows.Forms.NumericUpDown ryInt;
        private System.Windows.Forms.NumericUpDown cxInt;
        private System.Windows.Forms.NumericUpDown cyInt;
        private System.Windows.Forms.CheckBox nameEnable;
        private System.Windows.Forms.CheckBox hideBox;
        private System.Windows.Forms.CheckBox reactorBox;
        private System.Windows.Forms.ListBox questList;
        private System.Windows.Forms.Button questAdd;
        private System.Windows.Forms.Button questRemove;
        private System.Windows.Forms.CheckBox questEnable;
        private System.Windows.Forms.CheckBox tagsEnable;
        private System.Windows.Forms.TextBox tagsBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox flipBox;
        private System.Windows.Forms.NumericUpDown reactorTimeInt;
        private System.Windows.Forms.CheckBox reactorTimeCheckbox;
    }
}