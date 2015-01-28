namespace Wordy
{
    partial class formReading
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formReading));
            this.rtbText = new System.Windows.Forms.RichTextBox();
            this.timerGetClipboard = new System.Windows.Forms.Timer(this.components);
            this.timerWaitUntilCopy = new System.Windows.Forms.Timer(this.components);
            this.timerPauseBeforeHiding = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // rtbText
            // 
            this.rtbText.BackColor = System.Drawing.SystemColors.Control;
            this.rtbText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbText.Font = new System.Drawing.Font("Calibri", 12F);
            this.rtbText.Location = new System.Drawing.Point(12, 12);
            this.rtbText.Name = "rtbText";
            this.rtbText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbText.Size = new System.Drawing.Size(735, 535);
            this.rtbText.TabIndex = 1;
            this.rtbText.Text = resources.GetString("rtbText.Text");
            this.rtbText.VScroll += new System.EventHandler(this.rtbText_VScroll);
            this.rtbText.Click += new System.EventHandler(this.rtbText_Click);
            this.rtbText.TextChanged += new System.EventHandler(this.rtbText_TextChanged);
            this.rtbText.Enter += new System.EventHandler(this.rtbText_Enter);
            // 
            // timerGetClipboard
            // 
            this.timerGetClipboard.Tick += new System.EventHandler(this.timerGetClipboard_Tick);
            // 
            // timerWaitUntilCopy
            // 
            this.timerWaitUntilCopy.Tick += new System.EventHandler(this.timerWaitUntilCopy_Tick);
            // 
            // timerPauseBeforeHiding
            // 
            this.timerPauseBeforeHiding.Tick += new System.EventHandler(this.timerPauseBeforeHiding_Tick);
            // 
            // formReading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 559);
            this.Controls.Add(this.rtbText);
            this.Name = "formReading";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Assisted Reading";
            this.Activated += new System.EventHandler(this.formReading_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formReading_FormClosing);
            this.Load += new System.EventHandler(this.formReading_Load);
            this.Resize += new System.EventHandler(this.formReading_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbText;
        private System.Windows.Forms.Timer timerGetClipboard;
        private System.Windows.Forms.Timer timerWaitUntilCopy;
        public System.Windows.Forms.Timer timerPauseBeforeHiding;
    }
}