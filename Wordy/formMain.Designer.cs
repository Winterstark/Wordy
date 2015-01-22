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
            this.lblInfo = new System.Windows.Forms.Label();
            this.buttAbout = new System.Windows.Forms.Button();
            this.buttReview = new System.Windows.Forms.Button();
            this.buttReading = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttAdd
            // 
            this.buttAdd.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttAdd.Location = new System.Drawing.Point(21, 22);
            this.buttAdd.Name = "buttAdd";
            this.buttAdd.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.buttAdd.Size = new System.Drawing.Size(160, 80);
            this.buttAdd.TabIndex = 0;
            this.buttAdd.Text = "Add Words";
            this.buttAdd.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttAdd.UseVisualStyleBackColor = true;
            this.buttAdd.Click += new System.EventHandler(this.buttAdd_Click);
            this.buttAdd.MouseEnter += new System.EventHandler(this.buttAdd_MouseEnter);
            this.buttAdd.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // buttStudyWords
            // 
            this.buttStudyWords.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttStudyWords.Location = new System.Drawing.Point(187, 22);
            this.buttStudyWords.Name = "buttStudyWords";
            this.buttStudyWords.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.buttStudyWords.Size = new System.Drawing.Size(160, 80);
            this.buttStudyWords.TabIndex = 1;
            this.buttStudyWords.Text = "Study New Words";
            this.buttStudyWords.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttStudyWords.UseVisualStyleBackColor = true;
            this.buttStudyWords.Click += new System.EventHandler(this.buttStudyWords_Click);
            this.buttStudyWords.MouseEnter += new System.EventHandler(this.buttStudyWords_MouseEnter);
            this.buttStudyWords.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // buttRecall
            // 
            this.buttRecall.BackColor = System.Drawing.SystemColors.Control;
            this.buttRecall.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttRecall.Location = new System.Drawing.Point(353, 22);
            this.buttRecall.Name = "buttRecall";
            this.buttRecall.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.buttRecall.Size = new System.Drawing.Size(160, 80);
            this.buttRecall.TabIndex = 2;
            this.buttRecall.Text = "Test Recall of Learned Words";
            this.buttRecall.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttRecall.UseVisualStyleBackColor = true;
            this.buttRecall.Click += new System.EventHandler(this.buttRecall_Click);
            this.buttRecall.MouseEnter += new System.EventHandler(this.buttRecall_MouseEnter);
            this.buttRecall.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // buttOptions
            // 
            this.buttOptions.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttOptions.Location = new System.Drawing.Point(21, 240);
            this.buttOptions.Name = "buttOptions";
            this.buttOptions.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.buttOptions.Size = new System.Drawing.Size(243, 32);
            this.buttOptions.TabIndex = 5;
            this.buttOptions.Text = "Options";
            this.buttOptions.UseVisualStyleBackColor = true;
            this.buttOptions.Click += new System.EventHandler(this.buttOptions_Click);
            this.buttOptions.MouseEnter += new System.EventHandler(this.buttOptions_MouseEnter);
            this.buttOptions.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // buttNewWotD
            // 
            this.buttNewWotD.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttNewWotD.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttNewWotD.Location = new System.Drawing.Point(187, 112);
            this.buttNewWotD.Name = "buttNewWotD";
            this.buttNewWotD.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.buttNewWotD.Size = new System.Drawing.Size(160, 80);
            this.buttNewWotD.TabIndex = 3;
            this.buttNewWotD.Text = "New Word of the Day!";
            this.buttNewWotD.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttNewWotD.UseVisualStyleBackColor = true;
            this.buttNewWotD.Visible = false;
            this.buttNewWotD.Click += new System.EventHandler(this.buttNewWotD_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("Candara", 12F);
            this.lblInfo.Location = new System.Drawing.Point(21, 112);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(492, 80);
            this.lblInfo.TabIndex = 5;
            this.lblInfo.Text = "Welcome to Wordy!";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttAbout
            // 
            this.buttAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttAbout.Location = new System.Drawing.Point(270, 240);
            this.buttAbout.Name = "buttAbout";
            this.buttAbout.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.buttAbout.Size = new System.Drawing.Size(243, 32);
            this.buttAbout.TabIndex = 6;
            this.buttAbout.Text = "About";
            this.buttAbout.UseVisualStyleBackColor = true;
            this.buttAbout.Click += new System.EventHandler(this.buttAbout_Click);
            this.buttAbout.MouseEnter += new System.EventHandler(this.buttAbout_MouseEnter);
            this.buttAbout.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // buttReview
            // 
            this.buttReview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttReview.Location = new System.Drawing.Point(270, 202);
            this.buttReview.Name = "buttReview";
            this.buttReview.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.buttReview.Size = new System.Drawing.Size(243, 32);
            this.buttReview.TabIndex = 4;
            this.buttReview.Text = "Review Words";
            this.buttReview.UseVisualStyleBackColor = true;
            this.buttReview.Click += new System.EventHandler(this.buttReview_Click);
            this.buttReview.MouseEnter += new System.EventHandler(this.buttReview_MouseEnter);
            this.buttReview.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // buttReading
            // 
            this.buttReading.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttReading.Location = new System.Drawing.Point(21, 202);
            this.buttReading.Name = "buttReading";
            this.buttReading.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.buttReading.Size = new System.Drawing.Size(243, 32);
            this.buttReading.TabIndex = 7;
            this.buttReading.Text = "Assisted Reading";
            this.buttReading.UseVisualStyleBackColor = true;
            this.buttReading.Click += new System.EventHandler(this.buttReading_Click);
            this.buttReading.MouseEnter += new System.EventHandler(this.buttReading_MouseEnter);
            this.buttReading.MouseLeave += new System.EventHandler(this.button_MouseLeave);
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 297);
            this.Controls.Add(this.buttReading);
            this.Controls.Add(this.buttReview);
            this.Controls.Add(this.buttNewWotD);
            this.Controls.Add(this.buttAbout);
            this.Controls.Add(this.buttOptions);
            this.Controls.Add(this.buttRecall);
            this.Controls.Add(this.buttStudyWords);
            this.Controls.Add(this.buttAdd);
            this.Controls.Add(this.lblInfo);
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
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button buttAbout;
        private System.Windows.Forms.Button buttReview;
        private System.Windows.Forms.Button buttReading;
    }
}

