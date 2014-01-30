namespace Wordy
{
    partial class formMain
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
            this.buttAdd = new System.Windows.Forms.Button();
            this.buttStudyWords = new System.Windows.Forms.Button();
            this.buttRecall = new System.Windows.Forms.Button();
            this.buttOptions = new System.Windows.Forms.Button();
            this.buttNewWotD = new System.Windows.Forms.Button();
            this.lblCheckingWotDs = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttAdd
            // 
            this.buttAdd.Location = new System.Drawing.Point(21, 22);
            this.buttAdd.Name = "buttAdd";
            this.buttAdd.Size = new System.Drawing.Size(138, 43);
            this.buttAdd.TabIndex = 0;
            this.buttAdd.Text = "Add Words";
            this.buttAdd.UseVisualStyleBackColor = true;
            this.buttAdd.Click += new System.EventHandler(this.buttAdd_Click);
            // 
            // buttStudyWords
            // 
            this.buttStudyWords.Location = new System.Drawing.Point(21, 71);
            this.buttStudyWords.Name = "buttStudyWords";
            this.buttStudyWords.Size = new System.Drawing.Size(138, 43);
            this.buttStudyWords.TabIndex = 1;
            this.buttStudyWords.Text = "Study New Words";
            this.buttStudyWords.UseVisualStyleBackColor = true;
            this.buttStudyWords.Click += new System.EventHandler(this.buttStudyWords_Click);
            // 
            // buttRecall
            // 
            this.buttRecall.BackColor = System.Drawing.SystemColors.Control;
            this.buttRecall.Location = new System.Drawing.Point(21, 120);
            this.buttRecall.Name = "buttRecall";
            this.buttRecall.Size = new System.Drawing.Size(138, 43);
            this.buttRecall.TabIndex = 2;
            this.buttRecall.Text = "Test Recall of Learned Words";
            this.buttRecall.UseVisualStyleBackColor = false;
            this.buttRecall.Click += new System.EventHandler(this.buttRecall_Click);
            // 
            // buttOptions
            // 
            this.buttOptions.Location = new System.Drawing.Point(21, 218);
            this.buttOptions.Name = "buttOptions";
            this.buttOptions.Size = new System.Drawing.Size(138, 23);
            this.buttOptions.TabIndex = 3;
            this.buttOptions.Text = "Options";
            this.buttOptions.UseVisualStyleBackColor = true;
            this.buttOptions.Click += new System.EventHandler(this.buttOptions_Click);
            // 
            // buttNewWotD
            // 
            this.buttNewWotD.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttNewWotD.Location = new System.Drawing.Point(21, 169);
            this.buttNewWotD.Name = "buttNewWotD";
            this.buttNewWotD.Size = new System.Drawing.Size(138, 43);
            this.buttNewWotD.TabIndex = 4;
            this.buttNewWotD.Text = "New Word of the Day!";
            this.buttNewWotD.UseVisualStyleBackColor = true;
            this.buttNewWotD.Visible = false;
            this.buttNewWotD.Click += new System.EventHandler(this.buttNewWotD_Click);
            // 
            // lblCheckingWotDs
            // 
            this.lblCheckingWotDs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCheckingWotDs.Location = new System.Drawing.Point(0, 0);
            this.lblCheckingWotDs.Name = "lblCheckingWotDs";
            this.lblCheckingWotDs.Size = new System.Drawing.Size(181, 256);
            this.lblCheckingWotDs.TabIndex = 5;
            this.lblCheckingWotDs.Text = "Checking for new Words of the Day\r\nPlease wait a moment...";
            this.lblCheckingWotDs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCheckingWotDs.Visible = false;
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(181, 256);
            this.Controls.Add(this.lblCheckingWotDs);
            this.Controls.Add(this.buttNewWotD);
            this.Controls.Add(this.buttOptions);
            this.Controls.Add(this.buttRecall);
            this.Controls.Add(this.buttStudyWords);
            this.Controls.Add(this.buttAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wordy";
            this.Activated += new System.EventHandler(this.formMain_Activated);
            this.Load += new System.EventHandler(this.formMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttAdd;
        private System.Windows.Forms.Button buttStudyWords;
        private System.Windows.Forms.Button buttRecall;
        private System.Windows.Forms.Button buttOptions;
        private System.Windows.Forms.Button buttNewWotD;
        private System.Windows.Forms.Label lblCheckingWotDs;
    }
}

