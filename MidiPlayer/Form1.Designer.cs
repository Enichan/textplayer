namespace MidiPlayer {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnOpen = new System.Windows.Forms.Button();
            this.cmbInstruments = new System.Windows.Forms.ComboBox();
            this.chkNormalize = new System.Windows.Forms.CheckBox();
            this.chkLoop = new System.Windows.Forms.CheckBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.chkMute = new System.Windows.Forms.CheckBox();
            this.scrSeek = new System.Windows.Forms.ProgressBar();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblFile = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbMMLMode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(12, 62);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(96, 47);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // cmbInstruments
            // 
            this.cmbInstruments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstruments.FormattingEnabled = true;
            this.cmbInstruments.Location = new System.Drawing.Point(168, 132);
            this.cmbInstruments.Name = "cmbInstruments";
            this.cmbInstruments.Size = new System.Drawing.Size(246, 24);
            this.cmbInstruments.TabIndex = 1;
            this.cmbInstruments.SelectionChangeCommitted += new System.EventHandler(this.cmbInstruments_SelectionChangeCommitted);
            // 
            // chkNormalize
            // 
            this.chkNormalize.AutoSize = true;
            this.chkNormalize.Checked = true;
            this.chkNormalize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNormalize.Location = new System.Drawing.Point(12, 115);
            this.chkNormalize.Name = "chkNormalize";
            this.chkNormalize.Size = new System.Drawing.Size(93, 21);
            this.chkNormalize.TabIndex = 2;
            this.chkNormalize.Text = "Normalize";
            this.chkNormalize.UseVisualStyleBackColor = true;
            this.chkNormalize.CheckedChanged += new System.EventHandler(this.chkNormalize_CheckedChanged);
            // 
            // chkLoop
            // 
            this.chkLoop.AutoSize = true;
            this.chkLoop.Location = new System.Drawing.Point(12, 142);
            this.chkLoop.Name = "chkLoop";
            this.chkLoop.Size = new System.Drawing.Size(62, 21);
            this.chkLoop.TabIndex = 3;
            this.chkLoop.Text = "Loop";
            this.chkLoop.UseVisualStyleBackColor = true;
            this.chkLoop.CheckedChanged += new System.EventHandler(this.chkLoop_CheckedChanged);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(216, 62);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(96, 47);
            this.btnPause.TabIndex = 4;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // chkMute
            // 
            this.chkMute.AutoSize = true;
            this.chkMute.Location = new System.Drawing.Point(12, 169);
            this.chkMute.Name = "chkMute";
            this.chkMute.Size = new System.Drawing.Size(61, 21);
            this.chkMute.TabIndex = 5;
            this.chkMute.Text = "Mute";
            this.chkMute.UseVisualStyleBackColor = true;
            this.chkMute.CheckedChanged += new System.EventHandler(this.chkMute_CheckedChanged);
            // 
            // scrSeek
            // 
            this.scrSeek.Location = new System.Drawing.Point(12, 32);
            this.scrSeek.Name = "scrSeek";
            this.scrSeek.Size = new System.Drawing.Size(402, 24);
            this.scrSeek.TabIndex = 6;
            this.scrSeek.MouseDown += new System.Windows.Forms.MouseEventHandler(this.scrSeek_MouseDown);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(114, 62);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(96, 47);
            this.btnPlay.TabIndex = 7;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(318, 62);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(96, 47);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblFile
            // 
            this.lblFile.AutoEllipsis = true;
            this.lblFile.Location = new System.Drawing.Point(9, 9);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(315, 19);
            this.lblFile.TabIndex = 9;
            this.lblFile.Text = "File: ";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(346, 9);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(68, 17);
            this.lblTime.TabIndex = 10;
            this.lblTime.Text = "--:-- / --:--";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(165, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Instrument:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(165, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "MML Mode:";
            // 
            // cmbMMLMode
            // 
            this.cmbMMLMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMMLMode.FormattingEnabled = true;
            this.cmbMMLMode.Items.AddRange(new object[] {
            "Mabinogi",
            "ArcheAge"});
            this.cmbMMLMode.Location = new System.Drawing.Point(168, 184);
            this.cmbMMLMode.Name = "cmbMMLMode";
            this.cmbMMLMode.Size = new System.Drawing.Size(246, 24);
            this.cmbMMLMode.TabIndex = 12;
            this.cmbMMLMode.SelectionChangeCommitted += new System.EventHandler(this.cmbMMLMode_SelectionChangeCommitted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 220);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbMMLMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.scrSeek);
            this.Controls.Add(this.chkMute);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.chkLoop);
            this.Controls.Add(this.chkNormalize);
            this.Controls.Add(this.cmbInstruments);
            this.Controls.Add(this.btnOpen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "MidiPlayer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.ComboBox cmbInstruments;
        private System.Windows.Forms.CheckBox chkNormalize;
        private System.Windows.Forms.CheckBox chkLoop;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.CheckBox chkMute;
        private System.Windows.Forms.ProgressBar scrSeek;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMMLMode;
    }
}

