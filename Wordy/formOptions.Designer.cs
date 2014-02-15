namespace Wordy
{
    partial class formOptions
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabPreferences = new System.Windows.Forms.TabPage();
            this.checkShowChangelog = new System.Windows.Forms.CheckBox();
            this.lblUpdateNotifications = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.trackUpdate = new System.Windows.Forms.TrackBar();
            this.textNewWordsPath = new System.Windows.Forms.TextBox();
            this.buttBrowse = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.checkAutoVisuals = new System.Windows.Forms.CheckBox();
            this.tabWordlist = new System.Windows.Forms.TabPage();
            this.textSynonyms = new System.Windows.Forms.TextBox();
            this.lblSyns = new System.Windows.Forms.Label();
            this.textFilter = new System.Windows.Forms.TextBox();
            this.lblVisualTrigger = new System.Windows.Forms.Label();
            this.textDef = new System.Windows.Forms.RichTextBox();
            this.textDefMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuToggleKeyword = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSurroundWParentheses = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSurroundWQuotes = new System.Windows.Forms.ToolStripMenuItem();
            this.buttSortName = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblDef = new System.Windows.Forms.Label();
            this.buttUpdateDef = new System.Windows.Forms.Button();
            this.lblWords = new System.Windows.Forms.Label();
            this.chklistWords = new System.Windows.Forms.CheckedListBox();
            this.buttDelete = new System.Windows.Forms.Button();
            this.tabWotD = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.textNewSubTitle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textNewSubAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttDelSub = new System.Windows.Forms.Button();
            this.buttAddSub = new System.Windows.Forms.Button();
            this.chklistSubscriptions = new System.Windows.Forms.CheckedListBox();
            this.openDiag = new System.Windows.Forms.OpenFileDialog();
            this.picWordnik = new System.Windows.Forms.PictureBox();
            this.tabs.SuspendLayout();
            this.tabPreferences.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackUpdate)).BeginInit();
            this.tabWordlist.SuspendLayout();
            this.textDefMenu.SuspendLayout();
            this.tabWotD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWordnik)).BeginInit();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabPreferences);
            this.tabs.Controls.Add(this.tabWordlist);
            this.tabs.Controls.Add(this.tabWotD);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(578, 425);
            this.tabs.TabIndex = 14;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            // 
            // tabPreferences
            // 
            this.tabPreferences.Controls.Add(this.checkShowChangelog);
            this.tabPreferences.Controls.Add(this.lblUpdateNotifications);
            this.tabPreferences.Controls.Add(this.label9);
            this.tabPreferences.Controls.Add(this.trackUpdate);
            this.tabPreferences.Controls.Add(this.textNewWordsPath);
            this.tabPreferences.Controls.Add(this.buttBrowse);
            this.tabPreferences.Controls.Add(this.label8);
            this.tabPreferences.Controls.Add(this.checkAutoVisuals);
            this.tabPreferences.Location = new System.Drawing.Point(4, 22);
            this.tabPreferences.Name = "tabPreferences";
            this.tabPreferences.Padding = new System.Windows.Forms.Padding(3);
            this.tabPreferences.Size = new System.Drawing.Size(570, 383);
            this.tabPreferences.TabIndex = 2;
            this.tabPreferences.Text = "Preferences";
            this.tabPreferences.UseVisualStyleBackColor = true;
            // 
            // checkShowChangelog
            // 
            this.checkShowChangelog.AutoSize = true;
            this.checkShowChangelog.Location = new System.Drawing.Point(69, 239);
            this.checkShowChangelog.Name = "checkShowChangelog";
            this.checkShowChangelog.Size = new System.Drawing.Size(166, 17);
            this.checkShowChangelog.TabIndex = 12;
            this.checkShowChangelog.Text = "Show changelog after update";
            this.checkShowChangelog.UseVisualStyleBackColor = true;
            this.checkShowChangelog.CheckedChanged += new System.EventHandler(this.checkShowChangelog_CheckedChanged);
            // 
            // lblUpdateNotifications
            // 
            this.lblUpdateNotifications.AutoSize = true;
            this.lblUpdateNotifications.Location = new System.Drawing.Point(248, 188);
            this.lblUpdateNotifications.Name = "lblUpdateNotifications";
            this.lblUpdateNotifications.Size = new System.Drawing.Size(60, 13);
            this.lblUpdateNotifications.TabIndex = 11;
            this.lblUpdateNotifications.Text = "Always ask";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(34, 172);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Update notifications:";
            // 
            // trackUpdate
            // 
            this.trackUpdate.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.trackUpdate.Location = new System.Drawing.Point(54, 188);
            this.trackUpdate.Maximum = 3;
            this.trackUpdate.Name = "trackUpdate";
            this.trackUpdate.Size = new System.Drawing.Size(188, 45);
            this.trackUpdate.TabIndex = 9;
            this.trackUpdate.Scroll += new System.EventHandler(this.trackUpdate_Scroll);
            // 
            // textNewWordsPath
            // 
            this.textNewWordsPath.Location = new System.Drawing.Point(54, 120);
            this.textNewWordsPath.Name = "textNewWordsPath";
            this.textNewWordsPath.Size = new System.Drawing.Size(394, 20);
            this.textNewWordsPath.TabIndex = 8;
            // 
            // buttBrowse
            // 
            this.buttBrowse.Location = new System.Drawing.Point(454, 118);
            this.buttBrowse.Name = "buttBrowse";
            this.buttBrowse.Size = new System.Drawing.Size(42, 23);
            this.buttBrowse.TabIndex = 7;
            this.buttBrowse.Text = "...";
            this.buttBrowse.UseVisualStyleBackColor = true;
            this.buttBrowse.Click += new System.EventHandler(this.buttBrowse_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(201, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Check this text file for a list of new words:";
            // 
            // checkAutoVisuals
            // 
            this.checkAutoVisuals.AutoSize = true;
            this.checkAutoVisuals.Location = new System.Drawing.Point(37, 49);
            this.checkAutoVisuals.Name = "checkAutoVisuals";
            this.checkAutoVisuals.Size = new System.Drawing.Size(347, 17);
            this.checkAutoVisuals.TabIndex = 0;
            this.checkAutoVisuals.Text = "When searching for word definitions, automatically search for visuals";
            this.checkAutoVisuals.UseVisualStyleBackColor = true;
            this.checkAutoVisuals.CheckedChanged += new System.EventHandler(this.checkAutoVisuals_CheckedChanged);
            // 
            // tabWordlist
            // 
            this.tabWordlist.Controls.Add(this.picWordnik);
            this.tabWordlist.Controls.Add(this.textSynonyms);
            this.tabWordlist.Controls.Add(this.lblSyns);
            this.tabWordlist.Controls.Add(this.textFilter);
            this.tabWordlist.Controls.Add(this.lblVisualTrigger);
            this.tabWordlist.Controls.Add(this.textDef);
            this.tabWordlist.Controls.Add(this.buttSortName);
            this.tabWordlist.Controls.Add(this.lblInfo);
            this.tabWordlist.Controls.Add(this.lblDef);
            this.tabWordlist.Controls.Add(this.buttUpdateDef);
            this.tabWordlist.Controls.Add(this.lblWords);
            this.tabWordlist.Controls.Add(this.chklistWords);
            this.tabWordlist.Controls.Add(this.buttDelete);
            this.tabWordlist.Location = new System.Drawing.Point(4, 22);
            this.tabWordlist.Name = "tabWordlist";
            this.tabWordlist.Padding = new System.Windows.Forms.Padding(3);
            this.tabWordlist.Size = new System.Drawing.Size(570, 399);
            this.tabWordlist.TabIndex = 1;
            this.tabWordlist.Text = "Word List";
            this.tabWordlist.UseVisualStyleBackColor = true;
            // 
            // textSynonyms
            // 
            this.textSynonyms.Location = new System.Drawing.Point(414, 180);
            this.textSynonyms.Name = "textSynonyms";
            this.textSynonyms.Size = new System.Drawing.Size(130, 20);
            this.textSynonyms.TabIndex = 27;
            this.textSynonyms.TextChanged += new System.EventHandler(this.textSynonyms_TextChanged);
            // 
            // lblSyns
            // 
            this.lblSyns.AutoSize = true;
            this.lblSyns.Location = new System.Drawing.Point(350, 183);
            this.lblSyns.Name = "lblSyns";
            this.lblSyns.Size = new System.Drawing.Size(58, 13);
            this.lblSyns.TabIndex = 26;
            this.lblSyns.Text = "Synonyms:";
            // 
            // textFilter
            // 
            this.textFilter.Location = new System.Drawing.Point(44, 9);
            this.textFilter.Name = "textFilter";
            this.textFilter.Size = new System.Drawing.Size(297, 20);
            this.textFilter.TabIndex = 25;
            this.textFilter.TextChanged += new System.EventHandler(this.textFilter_TextChanged);
            // 
            // lblVisualTrigger
            // 
            this.lblVisualTrigger.BackColor = System.Drawing.Color.Black;
            this.lblVisualTrigger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVisualTrigger.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVisualTrigger.ForeColor = System.Drawing.Color.White;
            this.lblVisualTrigger.Location = new System.Drawing.Point(350, 362);
            this.lblVisualTrigger.Name = "lblVisualTrigger";
            this.lblVisualTrigger.Size = new System.Drawing.Size(194, 23);
            this.lblVisualTrigger.TabIndex = 24;
            this.lblVisualTrigger.Text = "Click to locate visual";
            this.lblVisualTrigger.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVisualTrigger.Visible = false;
            this.lblVisualTrigger.Click += new System.EventHandler(this.lblVisualTrigger_Click);
            // 
            // textDef
            // 
            this.textDef.ContextMenuStrip = this.textDefMenu;
            this.textDef.Location = new System.Drawing.Point(350, 35);
            this.textDef.Name = "textDef";
            this.textDef.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textDef.Size = new System.Drawing.Size(194, 139);
            this.textDef.TabIndex = 23;
            this.textDef.Text = "";
            this.textDef.SelectionChanged += new System.EventHandler(this.textDef_SelectionChanged);
            this.textDef.TextChanged += new System.EventHandler(this.textDef_TextChanged);
            // 
            // textDefMenu
            // 
            this.textDefMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToggleKeyword,
            this.toolStripSeparator1,
            this.menuSurroundWParentheses,
            this.menuSurroundWQuotes});
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
            // menuSurroundWParentheses
            // 
            this.menuSurroundWParentheses.Enabled = false;
            this.menuSurroundWParentheses.Name = "menuSurroundWParentheses";
            this.menuSurroundWParentheses.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.W)));
            this.menuSurroundWParentheses.Size = new System.Drawing.Size(255, 22);
            this.menuSurroundWParentheses.Text = "Surround with parentheses";
            this.menuSurroundWParentheses.Click += new System.EventHandler(this.menuSurroundWParentheses_Click);
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
            // buttSortName
            // 
            this.buttSortName.Location = new System.Drawing.Point(239, 362);
            this.buttSortName.Name = "buttSortName";
            this.buttSortName.Size = new System.Drawing.Size(102, 23);
            this.buttSortName.TabIndex = 22;
            this.buttSortName.Text = "Sort by Name";
            this.buttSortName.UseVisualStyleBackColor = true;
            this.buttSortName.Click += new System.EventHandler(this.buttSortName_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(350, 271);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(0, 13);
            this.lblInfo.TabIndex = 20;
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Enabled = false;
            this.lblDef.Location = new System.Drawing.Point(347, 12);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(54, 13);
            this.lblDef.TabIndex = 19;
            this.lblDef.Text = "Definition:";
            // 
            // buttUpdateDef
            // 
            this.buttUpdateDef.Enabled = false;
            this.buttUpdateDef.Location = new System.Drawing.Point(350, 235);
            this.buttUpdateDef.Name = "buttUpdateDef";
            this.buttUpdateDef.Size = new System.Drawing.Size(194, 23);
            this.buttUpdateDef.TabIndex = 18;
            this.buttUpdateDef.Text = "Update Definition";
            this.buttUpdateDef.UseVisualStyleBackColor = true;
            this.buttUpdateDef.Click += new System.EventHandler(this.buttUpdateDef_Click);
            // 
            // lblWords
            // 
            this.lblWords.AutoSize = true;
            this.lblWords.Location = new System.Drawing.Point(6, 12);
            this.lblWords.Name = "lblWords";
            this.lblWords.Size = new System.Drawing.Size(32, 13);
            this.lblWords.TabIndex = 16;
            this.lblWords.Text = "Filter:";
            // 
            // chklistWords
            // 
            this.chklistWords.FormattingEnabled = true;
            this.chklistWords.Location = new System.Drawing.Point(6, 35);
            this.chklistWords.Name = "chklistWords";
            this.chklistWords.Size = new System.Drawing.Size(338, 319);
            this.chklistWords.TabIndex = 15;
            this.chklistWords.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chklistWords_ItemCheck);
            this.chklistWords.SelectedIndexChanged += new System.EventHandler(this.chklistWords_SelectedIndexChanged);
            this.chklistWords.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chklistWords_KeyDown);
            // 
            // buttDelete
            // 
            this.buttDelete.Enabled = false;
            this.buttDelete.Location = new System.Drawing.Point(6, 362);
            this.buttDelete.Name = "buttDelete";
            this.buttDelete.Size = new System.Drawing.Size(102, 23);
            this.buttDelete.TabIndex = 14;
            this.buttDelete.Text = "Delete";
            this.buttDelete.UseVisualStyleBackColor = true;
            this.buttDelete.Click += new System.EventHandler(this.buttDelete_Click);
            // 
            // tabWotD
            // 
            this.tabWotD.Controls.Add(this.label7);
            this.tabWotD.Controls.Add(this.textNewSubTitle);
            this.tabWotD.Controls.Add(this.label6);
            this.tabWotD.Controls.Add(this.textNewSubAddress);
            this.tabWotD.Controls.Add(this.label5);
            this.tabWotD.Controls.Add(this.buttDelSub);
            this.tabWotD.Controls.Add(this.buttAddSub);
            this.tabWotD.Controls.Add(this.chklistSubscriptions);
            this.tabWotD.Location = new System.Drawing.Point(4, 22);
            this.tabWotD.Name = "tabWotD";
            this.tabWotD.Padding = new System.Windows.Forms.Padding(3);
            this.tabWotD.Size = new System.Drawing.Size(570, 383);
            this.tabWotD.TabIndex = 3;
            this.tabWotD.Text = "WotD";
            this.tabWotD.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(154, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Word of the Day Subscriptions:";
            // 
            // textNewSubTitle
            // 
            this.textNewSubTitle.Location = new System.Drawing.Point(297, 105);
            this.textNewSubTitle.Name = "textNewSubTitle";
            this.textNewSubTitle.Size = new System.Drawing.Size(250, 20);
            this.textNewSubTitle.TabIndex = 10;
            this.textNewSubTitle.TextChanged += new System.EventHandler(this.textNewSubTitle_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(294, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "(Optional) Enter subscription title:";
            // 
            // textNewSubAddress
            // 
            this.textNewSubAddress.Location = new System.Drawing.Point(297, 66);
            this.textNewSubAddress.Name = "textNewSubAddress";
            this.textNewSubAddress.Size = new System.Drawing.Size(250, 20);
            this.textNewSubAddress.TabIndex = 8;
            this.textNewSubAddress.TextChanged += new System.EventHandler(this.textNewSubAddress_TextChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(294, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(231, 32);
            this.label5.TabIndex = 7;
            this.label5.Text = "To add a new Word of the Day subscription, enter its RSS address here:";
            // 
            // buttDelSub
            // 
            this.buttDelSub.Enabled = false;
            this.buttDelSub.Location = new System.Drawing.Point(22, 296);
            this.buttDelSub.Name = "buttDelSub";
            this.buttDelSub.Size = new System.Drawing.Size(255, 23);
            this.buttDelSub.TabIndex = 5;
            this.buttDelSub.Text = "Delete Subscription";
            this.buttDelSub.UseVisualStyleBackColor = true;
            this.buttDelSub.Click += new System.EventHandler(this.buttDelSub_Click);
            // 
            // buttAddSub
            // 
            this.buttAddSub.Enabled = false;
            this.buttAddSub.Location = new System.Drawing.Point(297, 131);
            this.buttAddSub.Name = "buttAddSub";
            this.buttAddSub.Size = new System.Drawing.Size(250, 23);
            this.buttAddSub.TabIndex = 4;
            this.buttAddSub.Text = "Add Subscription";
            this.buttAddSub.UseVisualStyleBackColor = true;
            this.buttAddSub.Click += new System.EventHandler(this.buttAddSub_Click);
            // 
            // chklistSubscriptions
            // 
            this.chklistSubscriptions.FormattingEnabled = true;
            this.chklistSubscriptions.Location = new System.Drawing.Point(22, 46);
            this.chklistSubscriptions.Name = "chklistSubscriptions";
            this.chklistSubscriptions.Size = new System.Drawing.Size(255, 244);
            this.chklistSubscriptions.TabIndex = 0;
            this.chklistSubscriptions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chklistSubscriptions_ItemCheck);
            this.chklistSubscriptions.SelectedIndexChanged += new System.EventHandler(this.chklistSubscriptions_SelectedIndexChanged);
            this.chklistSubscriptions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chklistSubscriptions_KeyDown);
            // 
            // openDiag
            // 
            this.openDiag.Filter = "Text files|*.txt";
            // 
            // picWordnik
            // 
            this.picWordnik.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picWordnik.Enabled = false;
            this.picWordnik.Image = global::Wordy.Properties.Resources.wordnik_badge_a2;
            this.picWordnik.Location = new System.Drawing.Point(350, 206);
            this.picWordnik.Name = "picWordnik";
            this.picWordnik.Size = new System.Drawing.Size(194, 23);
            this.picWordnik.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picWordnik.TabIndex = 28;
            this.picWordnik.TabStop = false;
            this.picWordnik.Click += new System.EventHandler(this.picWordnik_Click);
            // 
            // formOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 448);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "formOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wordy Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formWordlist_FormClosing);
            this.Load += new System.EventHandler(this.formOptions_Load);
            this.tabs.ResumeLayout(false);
            this.tabPreferences.ResumeLayout(false);
            this.tabPreferences.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackUpdate)).EndInit();
            this.tabWordlist.ResumeLayout(false);
            this.tabWordlist.PerformLayout();
            this.textDefMenu.ResumeLayout(false);
            this.tabWotD.ResumeLayout(false);
            this.tabWotD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWordnik)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabWordlist;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblDef;
        private System.Windows.Forms.Button buttUpdateDef;
        private System.Windows.Forms.Label lblWords;
        private System.Windows.Forms.CheckedListBox chklistWords;
        private System.Windows.Forms.Button buttDelete;
        private System.Windows.Forms.TabPage tabPreferences;
        private System.Windows.Forms.TabPage tabWotD;
        private System.Windows.Forms.CheckBox checkAutoVisuals;
        private System.Windows.Forms.CheckedListBox chklistSubscriptions;
        private System.Windows.Forms.Button buttDelSub;
        private System.Windows.Forms.Button buttAddSub;
        private System.Windows.Forms.TextBox textNewSubAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textNewSubTitle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttSortName;
        private System.Windows.Forms.TextBox textNewWordsPath;
        private System.Windows.Forms.Button buttBrowse;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.OpenFileDialog openDiag;
        private System.Windows.Forms.CheckBox checkShowChangelog;
        private System.Windows.Forms.Label lblUpdateNotifications;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TrackBar trackUpdate;
        private System.Windows.Forms.RichTextBox textDef;
        private System.Windows.Forms.ContextMenuStrip textDefMenu;
        private System.Windows.Forms.ToolStripMenuItem menuToggleKeyword;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuSurroundWQuotes;
        private System.Windows.Forms.ToolStripMenuItem menuSurroundWParentheses;
        private System.Windows.Forms.Label lblVisualTrigger;
        private System.Windows.Forms.TextBox textFilter;
        private System.Windows.Forms.TextBox textSynonyms;
        private System.Windows.Forms.Label lblSyns;
        private System.Windows.Forms.PictureBox picWordnik;

    }
}