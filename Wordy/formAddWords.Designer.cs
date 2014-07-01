namespace Wordy
{
    partial class formAddWords
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttRemoveWord = new System.Windows.Forms.Button();
            this.textNewWords = new System.Windows.Forms.TextBox();
            this.listFoundWords = new System.Windows.Forms.ListBox();
            this.lblNext = new System.Windows.Forms.Label();
            this.lblRecognizedWords = new System.Windows.Forms.Label();
            this.lblDef = new System.Windows.Forms.Label();
            this.buttAcceptWords = new System.Windows.Forms.Button();
            this.buttFindDefs = new System.Windows.Forms.Button();
            this.labelProgress = new System.Windows.Forms.Label();
            this.picThumbnails = new System.Windows.Forms.PictureBox();
            this.picVisual = new System.Windows.Forms.PictureBox();
            this.lblVisuals = new System.Windows.Forms.Label();
            this.buttToggleVisuals = new System.Windows.Forms.Button();
            this.buttLoadMoreVisuals = new System.Windows.Forms.Button();
            this.buttReloadVisuals = new System.Windows.Forms.Button();
            this.buttUpdateDef = new System.Windows.Forms.Button();
            this.buttOpenWotD = new System.Windows.Forms.Button();
            this.textDef = new System.Windows.Forms.RichTextBox();
            this.textDefMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuToggleKeyword = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSurroundWQuotes = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSurroundWParentheses = new System.Windows.Forms.ToolStripMenuItem();
            this.lblSyns = new System.Windows.Forms.Label();
            this.textSynonyms = new System.Windows.Forms.TextBox();
            this.picWordnik = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picThumbnails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVisual)).BeginInit();
            this.textDefMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWordnik)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter words here:";
            // 
            // buttRemoveWord
            // 
            this.buttRemoveWord.Enabled = false;
            this.buttRemoveWord.Location = new System.Drawing.Point(235, 386);
            this.buttRemoveWord.Name = "buttRemoveWord";
            this.buttRemoveWord.Size = new System.Drawing.Size(194, 23);
            this.buttRemoveWord.TabIndex = 4;
            this.buttRemoveWord.Text = "Remove Word";
            this.buttRemoveWord.UseVisualStyleBackColor = true;
            this.buttRemoveWord.Click += new System.EventHandler(this.buttRemoveWord_Click);
            // 
            // textNewWords
            // 
            this.textNewWords.Location = new System.Drawing.Point(15, 25);
            this.textNewWords.Multiline = true;
            this.textNewWords.Name = "textNewWords";
            this.textNewWords.Size = new System.Drawing.Size(194, 355);
            this.textNewWords.TabIndex = 1;
            this.textNewWords.TextChanged += new System.EventHandler(this.textNewWords_TextChanged);
            // 
            // listFoundWords
            // 
            this.listFoundWords.Enabled = false;
            this.listFoundWords.FormattingEnabled = true;
            this.listFoundWords.Location = new System.Drawing.Point(235, 25);
            this.listFoundWords.Name = "listFoundWords";
            this.listFoundWords.Size = new System.Drawing.Size(194, 355);
            this.listFoundWords.TabIndex = 3;
            this.listFoundWords.SelectedIndexChanged += new System.EventHandler(this.listFoundWords_SelectedIndexChanged);
            this.listFoundWords.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listFoundWords_KeyDown);
            // 
            // lblNext
            // 
            this.lblNext.AutoSize = true;
            this.lblNext.Enabled = false;
            this.lblNext.Location = new System.Drawing.Point(213, 192);
            this.lblNext.Name = "lblNext";
            this.lblNext.Size = new System.Drawing.Size(16, 13);
            this.lblNext.TabIndex = 7;
            this.lblNext.Text = "->";
            // 
            // lblRecognizedWords
            // 
            this.lblRecognizedWords.AutoSize = true;
            this.lblRecognizedWords.Enabled = false;
            this.lblRecognizedWords.Location = new System.Drawing.Point(232, 9);
            this.lblRecognizedWords.Name = "lblRecognizedWords";
            this.lblRecognizedWords.Size = new System.Drawing.Size(98, 13);
            this.lblRecognizedWords.TabIndex = 8;
            this.lblRecognizedWords.Text = "Recognized words:";
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Enabled = false;
            this.lblDef.Location = new System.Drawing.Point(432, 9);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(59, 13);
            this.lblDef.TabIndex = 9;
            this.lblDef.Text = "Definitions:";
            // 
            // buttAcceptWords
            // 
            this.buttAcceptWords.Enabled = false;
            this.buttAcceptWords.Location = new System.Drawing.Point(435, 357);
            this.buttAcceptWords.Name = "buttAcceptWords";
            this.buttAcceptWords.Size = new System.Drawing.Size(194, 52);
            this.buttAcceptWords.TabIndex = 10;
            this.buttAcceptWords.Text = "Accept Words";
            this.buttAcceptWords.UseVisualStyleBackColor = true;
            this.buttAcceptWords.Click += new System.EventHandler(this.buttAcceptWords_Click);
            // 
            // buttFindDefs
            // 
            this.buttFindDefs.Enabled = false;
            this.buttFindDefs.Location = new System.Drawing.Point(12, 386);
            this.buttFindDefs.Name = "buttFindDefs";
            this.buttFindDefs.Size = new System.Drawing.Size(197, 23);
            this.buttFindDefs.TabIndex = 2;
            this.buttFindDefs.Text = "Find Definitions";
            this.buttFindDefs.UseVisualStyleBackColor = true;
            this.buttFindDefs.Click += new System.EventHandler(this.buttFindDefs_Click);
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Font = new System.Drawing.Font("Candara", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelProgress.Location = new System.Drawing.Point(11, 418);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(0, 19);
            this.labelProgress.TabIndex = 10;
            // 
            // picThumbnails
            // 
            this.picThumbnails.Location = new System.Drawing.Point(637, 40);
            this.picThumbnails.Name = "picThumbnails";
            this.picThumbnails.Size = new System.Drawing.Size(192, 32);
            this.picThumbnails.TabIndex = 11;
            this.picThumbnails.TabStop = false;
            this.picThumbnails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picThumbnails_MouseDown);
            this.picThumbnails.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picThumbnails_MouseMove);
            // 
            // picVisual
            // 
            this.picVisual.Location = new System.Drawing.Point(635, 78);
            this.picVisual.Name = "picVisual";
            this.picVisual.Size = new System.Drawing.Size(240, 240);
            this.picVisual.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picVisual.TabIndex = 12;
            this.picVisual.TabStop = false;
            this.picVisual.Click += new System.EventHandler(this.picVisual_Click);
            // 
            // lblVisuals
            // 
            this.lblVisuals.AutoSize = true;
            this.lblVisuals.Enabled = false;
            this.lblVisuals.Location = new System.Drawing.Point(634, 14);
            this.lblVisuals.Name = "lblVisuals";
            this.lblVisuals.Size = new System.Drawing.Size(43, 13);
            this.lblVisuals.TabIndex = 13;
            this.lblVisuals.Text = "Visuals:";
            // 
            // buttToggleVisuals
            // 
            this.buttToggleVisuals.Location = new System.Drawing.Point(580, 299);
            this.buttToggleVisuals.Name = "buttToggleVisuals";
            this.buttToggleVisuals.Size = new System.Drawing.Size(49, 52);
            this.buttToggleVisuals.TabIndex = 8;
            this.buttToggleVisuals.Text = "No Visuals <<";
            this.buttToggleVisuals.UseVisualStyleBackColor = true;
            this.buttToggleVisuals.Click += new System.EventHandler(this.buttToggleVisuals_Click);
            // 
            // buttLoadMoreVisuals
            // 
            this.buttLoadMoreVisuals.Location = new System.Drawing.Point(835, 9);
            this.buttLoadMoreVisuals.Name = "buttLoadMoreVisuals";
            this.buttLoadMoreVisuals.Size = new System.Drawing.Size(40, 63);
            this.buttLoadMoreVisuals.TabIndex = 12;
            this.buttLoadMoreVisuals.Text = "Load More";
            this.buttLoadMoreVisuals.UseVisualStyleBackColor = true;
            this.buttLoadMoreVisuals.Visible = false;
            this.buttLoadMoreVisuals.Click += new System.EventHandler(this.buttLoadMoreVisuals_Click);
            // 
            // buttReloadVisuals
            // 
            this.buttReloadVisuals.Location = new System.Drawing.Point(683, 9);
            this.buttReloadVisuals.Name = "buttReloadVisuals";
            this.buttReloadVisuals.Size = new System.Drawing.Size(146, 23);
            this.buttReloadVisuals.TabIndex = 11;
            this.buttReloadVisuals.Text = "Reload All";
            this.buttReloadVisuals.UseVisualStyleBackColor = true;
            this.buttReloadVisuals.Visible = false;
            this.buttReloadVisuals.Click += new System.EventHandler(this.buttReloadVisuals_Click);
            // 
            // buttUpdateDef
            // 
            this.buttUpdateDef.Enabled = false;
            this.buttUpdateDef.Location = new System.Drawing.Point(435, 244);
            this.buttUpdateDef.Name = "buttUpdateDef";
            this.buttUpdateDef.Size = new System.Drawing.Size(194, 23);
            this.buttUpdateDef.TabIndex = 6;
            this.buttUpdateDef.Text = "Update Definitions";
            this.buttUpdateDef.UseVisualStyleBackColor = true;
            this.buttUpdateDef.Click += new System.EventHandler(this.buttUpdateDef_Click);
            // 
            // buttOpenWotD
            // 
            this.buttOpenWotD.Enabled = false;
            this.buttOpenWotD.Location = new System.Drawing.Point(435, 328);
            this.buttOpenWotD.Name = "buttOpenWotD";
            this.buttOpenWotD.Size = new System.Drawing.Size(139, 23);
            this.buttOpenWotD.TabIndex = 9;
            this.buttOpenWotD.Text = "Open WotD Page";
            this.buttOpenWotD.UseVisualStyleBackColor = true;
            this.buttOpenWotD.Visible = false;
            this.buttOpenWotD.Click += new System.EventHandler(this.buttOpenWotD_Click);
            // 
            // textDef
            // 
            this.textDef.ContextMenuStrip = this.textDefMenu;
            this.textDef.Enabled = false;
            this.textDef.Location = new System.Drawing.Point(435, 25);
            this.textDef.Name = "textDef";
            this.textDef.Size = new System.Drawing.Size(194, 213);
            this.textDef.TabIndex = 5;
            this.textDef.Text = "";
            this.textDef.SelectionChanged += new System.EventHandler(this.textDef_SelectionChanged);
            this.textDef.TextChanged += new System.EventHandler(this.textDef_TextChanged);
            // 
            // textDefMenu
            // 
            this.textDefMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToggleKeyword,
            this.toolStripSeparator1,
            this.menuSurroundWQuotes,
            this.menuSurroundWParentheses});
            this.textDefMenu.Name = "textDefMenu";
            this.textDefMenu.Size = new System.Drawing.Size(256, 76);
            // 
            // menuToggleKeyword
            // 
            this.menuToggleKeyword.Enabled = false;
            this.menuToggleKeyword.Name = "menuToggleKeyword";
            this.menuToggleKeyword.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.menuToggleKeyword.Size = new System.Drawing.Size(255, 22);
            this.menuToggleKeyword.Text = "Toggle keyword";
            this.menuToggleKeyword.Click += new System.EventHandler(this.menuToggleKeyword_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(252, 6);
            // 
            // menuSurroundWQuotes
            // 
            this.menuSurroundWQuotes.Enabled = false;
            this.menuSurroundWQuotes.Name = "menuSurroundWQuotes";
            this.menuSurroundWQuotes.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Q)));
            this.menuSurroundWQuotes.Size = new System.Drawing.Size(255, 22);
            this.menuSurroundWQuotes.Text = "Surround with quotes";
            this.menuSurroundWQuotes.Click += new System.EventHandler(this.menuSurroundWQuotes_Click);
            // 
            // menuSurroundWParentheses
            // 
            this.menuSurroundWParentheses.Enabled = false;
            this.menuSurroundWParentheses.Name = "menuSurroundWParentheses";
            this.menuSurroundWParentheses.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.W)));
            this.menuSurroundWParentheses.Size = new System.Drawing.Size(255, 22);
            this.menuSurroundWParentheses.Text = "Surround with parentheses";
            this.menuSurroundWParentheses.Click += new System.EventHandler(this.menuSurroundWParentheses_Click);
            // 
            // lblSyns
            // 
            this.lblSyns.AutoSize = true;
            this.lblSyns.Enabled = false;
            this.lblSyns.Location = new System.Drawing.Point(435, 276);
            this.lblSyns.Name = "lblSyns";
            this.lblSyns.Size = new System.Drawing.Size(58, 13);
            this.lblSyns.TabIndex = 19;
            this.lblSyns.Text = "Synonyms:";
            // 
            // textSynonyms
            // 
            this.textSynonyms.Enabled = false;
            this.textSynonyms.Location = new System.Drawing.Point(499, 273);
            this.textSynonyms.Name = "textSynonyms";
            this.textSynonyms.Size = new System.Drawing.Size(130, 20);
            this.textSynonyms.TabIndex = 7;
            this.textSynonyms.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textSynonyms_KeyUp);
            // 
            // picWordnik
            // 
            this.picWordnik.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picWordnik.Enabled = false;
            this.picWordnik.Image = global::Wordy.Properties.Resources.wordnik_badge_a2;
            this.picWordnik.Location = new System.Drawing.Point(435, 299);
            this.picWordnik.Name = "picWordnik";
            this.picWordnik.Size = new System.Drawing.Size(139, 23);
            this.picWordnik.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picWordnik.TabIndex = 22;
            this.picWordnik.TabStop = false;
            this.picWordnik.Click += new System.EventHandler(this.picWordnik_Click);
            // 
            // formAddWords
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 462);
            this.Controls.Add(this.picWordnik);
            this.Controls.Add(this.textDef);
            this.Controls.Add(this.textSynonyms);
            this.Controls.Add(this.lblSyns);
            this.Controls.Add(this.buttOpenWotD);
            this.Controls.Add(this.buttReloadVisuals);
            this.Controls.Add(this.buttLoadMoreVisuals);
            this.Controls.Add(this.buttToggleVisuals);
            this.Controls.Add(this.lblVisuals);
            this.Controls.Add(this.picVisual);
            this.Controls.Add(this.picThumbnails);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.buttFindDefs);
            this.Controls.Add(this.buttAcceptWords);
            this.Controls.Add(this.lblDef);
            this.Controls.Add(this.lblRecognizedWords);
            this.Controls.Add(this.lblNext);
            this.Controls.Add(this.listFoundWords);
            this.Controls.Add(this.textNewWords);
            this.Controls.Add(this.buttUpdateDef);
            this.Controls.Add(this.buttRemoveWord);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formAddWords";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Words";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formAddWords_FormClosing);
            this.Load += new System.EventHandler(this.formAddWords_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picThumbnails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVisual)).EndInit();
            this.textDefMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picWordnik)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttRemoveWord;
        private System.Windows.Forms.TextBox textNewWords;
        private System.Windows.Forms.Label lblNext;
        private System.Windows.Forms.Label lblRecognizedWords;
        private System.Windows.Forms.Label lblDef;
        private System.Windows.Forms.Button buttAcceptWords;
        private System.Windows.Forms.Button buttFindDefs;
        private System.Windows.Forms.ListBox listFoundWords;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.PictureBox picThumbnails;
        private System.Windows.Forms.PictureBox picVisual;
        private System.Windows.Forms.Label lblVisuals;
        private System.Windows.Forms.Button buttToggleVisuals;
        private System.Windows.Forms.Button buttLoadMoreVisuals;
        private System.Windows.Forms.Button buttReloadVisuals;
        private System.Windows.Forms.Button buttUpdateDef;
        private System.Windows.Forms.Button buttOpenWotD;
        private System.Windows.Forms.RichTextBox textDef;
        private System.Windows.Forms.Label lblSyns;
        private System.Windows.Forms.TextBox textSynonyms;
        private System.Windows.Forms.ContextMenuStrip textDefMenu;
        private System.Windows.Forms.ToolStripMenuItem menuToggleKeyword;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuSurroundWQuotes;
        private System.Windows.Forms.ToolStripMenuItem menuSurroundWParentheses;
        private System.Windows.Forms.PictureBox picWordnik;
    }
}