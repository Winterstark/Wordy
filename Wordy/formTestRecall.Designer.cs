namespace Wordy
{
    partial class formTestRecall
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
            this.panelDef = new System.Windows.Forms.Panel();
            this.picVisual = new System.Windows.Forms.PictureBox();
            this.buttSkip = new System.Windows.Forms.Button();
            this.chklistDefs = new System.Windows.Forms.CheckedListBox();
            this.buttFinished = new System.Windows.Forms.Button();
            this.lblSynonyms = new System.Windows.Forms.Label();
            this.lblWord = new System.Windows.Forms.Label();
            this.picWrong = new System.Windows.Forms.PictureBox();
            this.picRight = new System.Windows.Forms.PictureBox();
            this.buttNext = new System.Windows.Forms.Button();
            this.rtbDef = new System.Windows.Forms.RichTextBox();
            this.lblDef = new System.Windows.Forms.Label();
            this.mtbTestWord = new System.Windows.Forms.MaskedTextBox();
            this.buttAnotherExample = new System.Windows.Forms.Button();
            this.panelTestWord = new System.Windows.Forms.Panel();
            this.textTestWord = new System.Windows.Forms.TextBox();
            this.flowpanelPickAnswers = new System.Windows.Forms.FlowLayoutPanel();
            this.buttPickWord1 = new System.Windows.Forms.Button();
            this.buttPickWord2 = new System.Windows.Forms.Button();
            this.buttPickWord3 = new System.Windows.Forms.Button();
            this.buttPickWord4 = new System.Windows.Forms.Button();
            this.buttPickWord5 = new System.Windows.Forms.Button();
            this.buttPickWord6 = new System.Windows.Forms.Button();
            this.lblTestWordDef = new System.Windows.Forms.Label();
            this.timerProgressChange = new System.Windows.Forms.Timer(this.components);
            this.timerWait = new System.Windows.Forms.Timer(this.components);
            this.picWordnik = new System.Windows.Forms.PictureBox();
            this.panelDef.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picVisual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWrong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRight)).BeginInit();
            this.panelTestWord.SuspendLayout();
            this.flowpanelPickAnswers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWordnik)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDef
            // 
            this.panelDef.Controls.Add(this.picVisual);
            this.panelDef.Controls.Add(this.buttSkip);
            this.panelDef.Controls.Add(this.chklistDefs);
            this.panelDef.Controls.Add(this.buttFinished);
            this.panelDef.Controls.Add(this.lblSynonyms);
            this.panelDef.Controls.Add(this.lblWord);
            this.panelDef.Controls.Add(this.picWrong);
            this.panelDef.Controls.Add(this.picRight);
            this.panelDef.Controls.Add(this.buttNext);
            this.panelDef.Controls.Add(this.rtbDef);
            this.panelDef.Controls.Add(this.lblDef);
            this.panelDef.Controls.Add(this.mtbTestWord);
            this.panelDef.Controls.Add(this.buttAnotherExample);
            this.panelDef.Location = new System.Drawing.Point(12, 12);
            this.panelDef.Name = "panelDef";
            this.panelDef.Size = new System.Drawing.Size(744, 552);
            this.panelDef.TabIndex = 6;
            this.panelDef.Visible = false;
            // 
            // picVisual
            // 
            this.picVisual.Location = new System.Drawing.Point(5, 63);
            this.picVisual.Name = "picVisual";
            this.picVisual.Size = new System.Drawing.Size(732, 480);
            this.picVisual.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picVisual.TabIndex = 7;
            this.picVisual.TabStop = false;
            this.picVisual.Visible = false;
            // 
            // buttSkip
            // 
            this.buttSkip.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttSkip.Location = new System.Drawing.Point(220, 295);
            this.buttSkip.Name = "buttSkip";
            this.buttSkip.Size = new System.Drawing.Size(196, 43);
            this.buttSkip.TabIndex = 16;
            this.buttSkip.Text = "Skip question";
            this.buttSkip.UseVisualStyleBackColor = true;
            this.buttSkip.Visible = false;
            this.buttSkip.Click += new System.EventHandler(this.buttSkip_Click);
            // 
            // chklistDefs
            // 
            this.chklistDefs.BackColor = System.Drawing.SystemColors.Control;
            this.chklistDefs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chklistDefs.CheckOnClick = true;
            this.chklistDefs.Font = new System.Drawing.Font("Calibri", 12F);
            this.chklistDefs.FormattingEnabled = true;
            this.chklistDefs.Location = new System.Drawing.Point(6, 105);
            this.chklistDefs.Name = "chklistDefs";
            this.chklistDefs.Size = new System.Drawing.Size(71, 22);
            this.chklistDefs.TabIndex = 22;
            this.chklistDefs.Visible = false;
            this.chklistDefs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chklistDefs_KeyDown);
            // 
            // buttFinished
            // 
            this.buttFinished.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttFinished.Location = new System.Drawing.Point(18, 295);
            this.buttFinished.Name = "buttFinished";
            this.buttFinished.Size = new System.Drawing.Size(196, 43);
            this.buttFinished.TabIndex = 15;
            this.buttFinished.Text = "Finished";
            this.buttFinished.UseVisualStyleBackColor = true;
            this.buttFinished.Visible = false;
            this.buttFinished.Click += new System.EventHandler(this.buttFinished_Click);
            // 
            // lblSynonyms
            // 
            this.lblSynonyms.AutoSize = true;
            this.lblSynonyms.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSynonyms.Location = new System.Drawing.Point(14, 83);
            this.lblSynonyms.Name = "lblSynonyms";
            this.lblSynonyms.Size = new System.Drawing.Size(49, 19);
            this.lblSynonyms.TabIndex = 9;
            this.lblSynonyms.Text = "label1";
            // 
            // lblWord
            // 
            this.lblWord.AutoSize = true;
            this.lblWord.Font = new System.Drawing.Font("Candara", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWord.Location = new System.Drawing.Point(6, 5);
            this.lblWord.Name = "lblWord";
            this.lblWord.Size = new System.Drawing.Size(124, 64);
            this.lblWord.TabIndex = 5;
            this.lblWord.Text = "asdf";
            // 
            // picWrong
            // 
            this.picWrong.Location = new System.Drawing.Point(437, 222);
            this.picWrong.Name = "picWrong";
            this.picWrong.Size = new System.Drawing.Size(150, 150);
            this.picWrong.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picWrong.TabIndex = 17;
            this.picWrong.TabStop = false;
            this.picWrong.Visible = false;
            // 
            // picRight
            // 
            this.picRight.Location = new System.Drawing.Point(422, 210);
            this.picRight.Name = "picRight";
            this.picRight.Size = new System.Drawing.Size(150, 150);
            this.picRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRight.TabIndex = 16;
            this.picRight.TabStop = false;
            this.picRight.Visible = false;
            // 
            // buttNext
            // 
            this.buttNext.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttNext.Location = new System.Drawing.Point(409, 378);
            this.buttNext.Name = "buttNext";
            this.buttNext.Size = new System.Drawing.Size(196, 43);
            this.buttNext.TabIndex = 18;
            this.buttNext.Text = "Next";
            this.buttNext.UseVisualStyleBackColor = true;
            this.buttNext.Visible = false;
            this.buttNext.Click += new System.EventHandler(this.buttNext_Click);
            // 
            // rtbDef
            // 
            this.rtbDef.BackColor = System.Drawing.SystemColors.Control;
            this.rtbDef.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbDef.Font = new System.Drawing.Font("Calibri", 12F);
            this.rtbDef.Location = new System.Drawing.Point(34, 108);
            this.rtbDef.Name = "rtbDef";
            this.rtbDef.ReadOnly = true;
            this.rtbDef.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtbDef.Size = new System.Drawing.Size(49, 19);
            this.rtbDef.TabIndex = 19;
            this.rtbDef.Text = "";
            this.rtbDef.Visible = false;
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDef.Location = new System.Drawing.Point(14, 108);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(49, 19);
            this.lblDef.TabIndex = 8;
            this.lblDef.Text = "label1";
            // 
            // mtbTestWord
            // 
            this.mtbTestWord.Font = new System.Drawing.Font("Candara", 20F, System.Drawing.FontStyle.Bold);
            this.mtbTestWord.Location = new System.Drawing.Point(6, 17);
            this.mtbTestWord.Name = "mtbTestWord";
            this.mtbTestWord.Size = new System.Drawing.Size(181, 40);
            this.mtbTestWord.TabIndex = 21;
            this.mtbTestWord.Visible = false;
            this.mtbTestWord.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mtbTestWord_KeyPress);
            // 
            // buttAnotherExample
            // 
            this.buttAnotherExample.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttAnotherExample.Location = new System.Drawing.Point(18, 300);
            this.buttAnotherExample.Name = "buttAnotherExample";
            this.buttAnotherExample.Size = new System.Drawing.Size(196, 43);
            this.buttAnotherExample.TabIndex = 13;
            this.buttAnotherExample.Text = "See another example";
            this.buttAnotherExample.UseVisualStyleBackColor = true;
            this.buttAnotherExample.Visible = false;
            this.buttAnotherExample.Click += new System.EventHandler(this.buttAnotherExample_Click);
            // 
            // panelTestWord
            // 
            this.panelTestWord.Controls.Add(this.textTestWord);
            this.panelTestWord.Controls.Add(this.flowpanelPickAnswers);
            this.panelTestWord.Controls.Add(this.lblTestWordDef);
            this.panelTestWord.Location = new System.Drawing.Point(12, 12);
            this.panelTestWord.Name = "panelTestWord";
            this.panelTestWord.Size = new System.Drawing.Size(744, 552);
            this.panelTestWord.TabIndex = 8;
            this.panelTestWord.Visible = false;
            // 
            // textTestWord
            // 
            this.textTestWord.Font = new System.Drawing.Font("Candara", 20F, System.Drawing.FontStyle.Bold);
            this.textTestWord.Location = new System.Drawing.Point(14, 141);
            this.textTestWord.Name = "textTestWord";
            this.textTestWord.Size = new System.Drawing.Size(611, 40);
            this.textTestWord.TabIndex = 13;
            this.textTestWord.Text = "asdf";
            this.textTestWord.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textTestWord.Visible = false;
            this.textTestWord.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textTestWord_KeyPress);
            // 
            // flowpanelPickAnswers
            // 
            this.flowpanelPickAnswers.Controls.Add(this.buttPickWord1);
            this.flowpanelPickAnswers.Controls.Add(this.buttPickWord2);
            this.flowpanelPickAnswers.Controls.Add(this.buttPickWord3);
            this.flowpanelPickAnswers.Controls.Add(this.buttPickWord4);
            this.flowpanelPickAnswers.Controls.Add(this.buttPickWord5);
            this.flowpanelPickAnswers.Controls.Add(this.buttPickWord6);
            this.flowpanelPickAnswers.Location = new System.Drawing.Point(59, 219);
            this.flowpanelPickAnswers.Name = "flowpanelPickAnswers";
            this.flowpanelPickAnswers.Size = new System.Drawing.Size(611, 300);
            this.flowpanelPickAnswers.TabIndex = 11;
            this.flowpanelPickAnswers.Visible = false;
            // 
            // buttPickWord1
            // 
            this.buttPickWord1.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttPickWord1.Location = new System.Drawing.Point(3, 3);
            this.buttPickWord1.Name = "buttPickWord1";
            this.buttPickWord1.Size = new System.Drawing.Size(196, 43);
            this.buttPickWord1.TabIndex = 12;
            this.buttPickWord1.Text = "1";
            this.buttPickWord1.UseVisualStyleBackColor = true;
            this.buttPickWord1.Click += new System.EventHandler(this.buttPickWord1_Click);
            // 
            // buttPickWord2
            // 
            this.buttPickWord2.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttPickWord2.Location = new System.Drawing.Point(205, 3);
            this.buttPickWord2.Name = "buttPickWord2";
            this.buttPickWord2.Size = new System.Drawing.Size(196, 43);
            this.buttPickWord2.TabIndex = 13;
            this.buttPickWord2.Text = "2";
            this.buttPickWord2.UseVisualStyleBackColor = true;
            this.buttPickWord2.Click += new System.EventHandler(this.buttPickWord2_Click);
            // 
            // buttPickWord3
            // 
            this.buttPickWord3.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttPickWord3.Location = new System.Drawing.Point(407, 3);
            this.buttPickWord3.Name = "buttPickWord3";
            this.buttPickWord3.Size = new System.Drawing.Size(196, 43);
            this.buttPickWord3.TabIndex = 14;
            this.buttPickWord3.Text = "3";
            this.buttPickWord3.UseVisualStyleBackColor = true;
            this.buttPickWord3.Click += new System.EventHandler(this.buttPickWord3_Click);
            // 
            // buttPickWord4
            // 
            this.buttPickWord4.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttPickWord4.Location = new System.Drawing.Point(3, 52);
            this.buttPickWord4.Name = "buttPickWord4";
            this.buttPickWord4.Size = new System.Drawing.Size(196, 43);
            this.buttPickWord4.TabIndex = 15;
            this.buttPickWord4.Text = "4";
            this.buttPickWord4.UseVisualStyleBackColor = true;
            this.buttPickWord4.Click += new System.EventHandler(this.buttPickWord4_Click);
            // 
            // buttPickWord5
            // 
            this.buttPickWord5.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttPickWord5.Location = new System.Drawing.Point(205, 52);
            this.buttPickWord5.Name = "buttPickWord5";
            this.buttPickWord5.Size = new System.Drawing.Size(196, 43);
            this.buttPickWord5.TabIndex = 16;
            this.buttPickWord5.Text = "5";
            this.buttPickWord5.UseVisualStyleBackColor = true;
            this.buttPickWord5.Click += new System.EventHandler(this.buttPickWord5_Click);
            // 
            // buttPickWord6
            // 
            this.buttPickWord6.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttPickWord6.Location = new System.Drawing.Point(407, 52);
            this.buttPickWord6.Name = "buttPickWord6";
            this.buttPickWord6.Size = new System.Drawing.Size(196, 43);
            this.buttPickWord6.TabIndex = 17;
            this.buttPickWord6.Text = "6";
            this.buttPickWord6.UseVisualStyleBackColor = true;
            this.buttPickWord6.Click += new System.EventHandler(this.buttPickWord6_Click);
            // 
            // lblTestWordDef
            // 
            this.lblTestWordDef.AutoSize = true;
            this.lblTestWordDef.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestWordDef.Location = new System.Drawing.Point(13, 15);
            this.lblTestWordDef.Name = "lblTestWordDef";
            this.lblTestWordDef.Size = new System.Drawing.Size(49, 19);
            this.lblTestWordDef.TabIndex = 9;
            this.lblTestWordDef.Text = "label1";
            // 
            // timerProgressChange
            // 
            this.timerProgressChange.Interval = 50;
            this.timerProgressChange.Tick += new System.EventHandler(this.timerProgressChange_Tick);
            // 
            // timerWait
            // 
            this.timerWait.Interval = 1500;
            this.timerWait.Tick += new System.EventHandler(this.timerWait_Tick);
            // 
            // picWordnik
            // 
            this.picWordnik.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picWordnik.Image = global::Wordy.Properties.Resources.wordnik_badge_a2;
            this.picWordnik.Location = new System.Drawing.Point(288, 277);
            this.picWordnik.Name = "picWordnik";
            this.picWordnik.Size = new System.Drawing.Size(194, 23);
            this.picWordnik.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picWordnik.TabIndex = 25;
            this.picWordnik.TabStop = false;
            this.picWordnik.Click += new System.EventHandler(this.picWordnik_Click);
            // 
            // formTestRecall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 577);
            this.Controls.Add(this.picWordnik);
            this.Controls.Add(this.panelDef);
            this.Controls.Add(this.panelTestWord);
            this.KeyPreview = true;
            this.Name = "formTestRecall";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test Recall";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formTestRecall_FormClosing);
            this.Load += new System.EventHandler(this.formTestRecall_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.formTestRecall_KeyDown);
            this.Resize += new System.EventHandler(this.formTestRecall_Resize);
            this.panelDef.ResumeLayout(false);
            this.panelDef.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picVisual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picWrong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRight)).EndInit();
            this.panelTestWord.ResumeLayout(false);
            this.panelTestWord.PerformLayout();
            this.flowpanelPickAnswers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picWordnik)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDef;
        private System.Windows.Forms.PictureBox picVisual;
        private System.Windows.Forms.Label lblWord;
        private System.Windows.Forms.Label lblSynonyms;
        private System.Windows.Forms.Label lblDef;
        private System.Windows.Forms.Panel panelTestWord;
        private System.Windows.Forms.FlowLayoutPanel flowpanelPickAnswers;
        private System.Windows.Forms.Button buttPickWord1;
        private System.Windows.Forms.Button buttPickWord2;
        private System.Windows.Forms.Button buttPickWord3;
        private System.Windows.Forms.Button buttPickWord4;
        private System.Windows.Forms.Button buttPickWord5;
        private System.Windows.Forms.Button buttPickWord6;
        private System.Windows.Forms.Label lblTestWordDef;
        private System.Windows.Forms.TextBox textTestWord;
        private System.Windows.Forms.Button buttFinished;
        private System.Windows.Forms.Button buttSkip;
        private System.Windows.Forms.Timer timerProgressChange;
        private System.Windows.Forms.PictureBox picRight;
        private System.Windows.Forms.PictureBox picWrong;
        private System.Windows.Forms.Button buttNext;
        private System.Windows.Forms.RichTextBox rtbDef;
        private System.Windows.Forms.Timer timerWait;
        private System.Windows.Forms.MaskedTextBox mtbTestWord;
        private System.Windows.Forms.CheckedListBox chklistDefs;
        private System.Windows.Forms.Button buttAnotherExample;
        private System.Windows.Forms.PictureBox picWordnik;

    }
}