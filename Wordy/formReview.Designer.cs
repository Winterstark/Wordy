namespace Wordy
{
    partial class formReview
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
            this.rdbShowUnlearned = new System.Windows.Forms.RadioButton();
            this.rdbShowLearned = new System.Windows.Forms.RadioButton();
            this.rdbShowAll = new System.Windows.Forms.RadioButton();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.comboSortBy = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbDef = new System.Windows.Forms.RichTextBox();
            this.buttSortOrder = new System.Windows.Forms.Button();
            this.checkSearchDefs = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // rdbShowUnlearned
            // 
            this.rdbShowUnlearned.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdbShowUnlearned.Checked = true;
            this.rdbShowUnlearned.Location = new System.Drawing.Point(13, 455);
            this.rdbShowUnlearned.Name = "rdbShowUnlearned";
            this.rdbShowUnlearned.Size = new System.Drawing.Size(196, 23);
            this.rdbShowUnlearned.TabIndex = 1;
            this.rdbShowUnlearned.TabStop = true;
            this.rdbShowUnlearned.Text = "Show Unlearned";
            this.rdbShowUnlearned.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdbShowUnlearned.UseVisualStyleBackColor = true;
            this.rdbShowUnlearned.Click += new System.EventHandler(this.rdbShowUnlearned_Click);
            // 
            // rdbShowLearned
            // 
            this.rdbShowLearned.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdbShowLearned.Location = new System.Drawing.Point(215, 455);
            this.rdbShowLearned.Name = "rdbShowLearned";
            this.rdbShowLearned.Size = new System.Drawing.Size(196, 23);
            this.rdbShowLearned.TabIndex = 2;
            this.rdbShowLearned.Text = "Show Learned";
            this.rdbShowLearned.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdbShowLearned.UseVisualStyleBackColor = true;
            this.rdbShowLearned.Click += new System.EventHandler(this.rdbShowLearned_Click);
            // 
            // rdbShowAll
            // 
            this.rdbShowAll.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdbShowAll.Location = new System.Drawing.Point(417, 455);
            this.rdbShowAll.Name = "rdbShowAll";
            this.rdbShowAll.Size = new System.Drawing.Size(196, 23);
            this.rdbShowAll.TabIndex = 3;
            this.rdbShowAll.Text = "Show All";
            this.rdbShowAll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdbShowAll.UseVisualStyleBackColor = true;
            this.rdbShowAll.Click += new System.EventHandler(this.rdbShowAll_Click);
            // 
            // textSearch
            // 
            this.textSearch.Location = new System.Drawing.Point(65, 513);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(397, 20);
            this.textSearch.TabIndex = 4;
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            // 
            // comboSortBy
            // 
            this.comboSortBy.BackColor = System.Drawing.SystemColors.Window;
            this.comboSortBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSortBy.FormattingEnabled = true;
            this.comboSortBy.Items.AddRange(new object[] {
            "Sort by date added",
            "Sort by A-Z",
            "Sort by current learning step",
            "Sort by hardest to learn",
            "Sort by hardest to remember"});
            this.comboSortBy.Location = new System.Drawing.Point(13, 484);
            this.comboSortBy.Name = "comboSortBy";
            this.comboSortBy.Size = new System.Drawing.Size(449, 21);
            this.comboSortBy.TabIndex = 5;
            this.comboSortBy.SelectedIndexChanged += new System.EventHandler(this.comboSortBy_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 516);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Search:";
            // 
            // rtbDef
            // 
            this.rtbDef.BackColor = System.Drawing.SystemColors.Control;
            this.rtbDef.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbDef.Font = new System.Drawing.Font("Calibri", 12F);
            this.rtbDef.Location = new System.Drawing.Point(12, 0);
            this.rtbDef.Name = "rtbDef";
            this.rtbDef.ReadOnly = true;
            this.rtbDef.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbDef.Size = new System.Drawing.Size(610, 437);
            this.rtbDef.TabIndex = 20;
            this.rtbDef.Text = "";
            // 
            // buttSortOrder
            // 
            this.buttSortOrder.Location = new System.Drawing.Point(468, 482);
            this.buttSortOrder.Name = "buttSortOrder";
            this.buttSortOrder.Size = new System.Drawing.Size(145, 23);
            this.buttSortOrder.TabIndex = 22;
            this.buttSortOrder.Text = "Ascending";
            this.buttSortOrder.UseVisualStyleBackColor = true;
            this.buttSortOrder.Click += new System.EventHandler(this.buttSortOrder_Click);
            // 
            // checkSearchDefs
            // 
            this.checkSearchDefs.Location = new System.Drawing.Point(468, 511);
            this.checkSearchDefs.Name = "checkSearchDefs";
            this.checkSearchDefs.Size = new System.Drawing.Size(145, 23);
            this.checkSearchDefs.TabIndex = 23;
            this.checkSearchDefs.Text = "Search definitions too";
            this.checkSearchDefs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkSearchDefs.UseVisualStyleBackColor = true;
            this.checkSearchDefs.CheckedChanged += new System.EventHandler(this.checkSearchDefs_CheckedChanged);
            // 
            // formReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 549);
            this.Controls.Add(this.checkSearchDefs);
            this.Controls.Add(this.buttSortOrder);
            this.Controls.Add(this.rtbDef);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboSortBy);
            this.Controls.Add(this.textSearch);
            this.Controls.Add(this.rdbShowAll);
            this.Controls.Add(this.rdbShowLearned);
            this.Controls.Add(this.rdbShowUnlearned);
            this.Name = "formReview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Review";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formReview_FormClosing);
            this.Load += new System.EventHandler(this.formReview_Load);
            this.Resize += new System.EventHandler(this.formReview_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbShowUnlearned;
        private System.Windows.Forms.RadioButton rdbShowLearned;
        private System.Windows.Forms.RadioButton rdbShowAll;
        private System.Windows.Forms.TextBox textSearch;
        private System.Windows.Forms.ComboBox comboSortBy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbDef;
        private System.Windows.Forms.Button buttSortOrder;
        private System.Windows.Forms.CheckBox checkSearchDefs;
    }
}