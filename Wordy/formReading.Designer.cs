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
            this.rtbText = new System.Windows.Forms.RichTextBox();
            this.rtbDef = new System.Windows.Forms.RichTextBox();
            this.buttSearch = new System.Windows.Forms.Button();
            this.buttSave = new System.Windows.Forms.Button();
            this.buttUpdateDefinition = new System.Windows.Forms.Button();
            this.buttAdd = new System.Windows.Forms.Button();
            this.buttGoogle = new System.Windows.Forms.Button();
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
            this.rtbText.Text = "Paste your text here...";
            this.rtbText.SelectionChanged += new System.EventHandler(this.rtbText_SelectionChanged);
            this.rtbText.VScroll += new System.EventHandler(this.rtbText_VScroll);
            this.rtbText.Click += new System.EventHandler(this.rtbText_Click);
            this.rtbText.TextChanged += new System.EventHandler(this.rtbText_TextChanged);
            this.rtbText.Enter += new System.EventHandler(this.rtbText_Enter);
            this.rtbText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbText_KeyDown);
            // 
            // rtbDef
            // 
            this.rtbDef.BackColor = System.Drawing.SystemColors.Window;
            this.rtbDef.Location = new System.Drawing.Point(342, 130);
            this.rtbDef.Name = "rtbDef";
            this.rtbDef.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtbDef.Size = new System.Drawing.Size(254, 165);
            this.rtbDef.TabIndex = 6;
            this.rtbDef.Text = "";
            this.rtbDef.Visible = false;
            this.rtbDef.Click += new System.EventHandler(this.rtbDef_Click);
            this.rtbDef.TextChanged += new System.EventHandler(this.rtbDef_TextChanged);
            this.rtbDef.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbDef_KeyDown);
            // 
            // buttSearch
            // 
            this.buttSearch.Location = new System.Drawing.Point(380, 301);
            this.buttSearch.Name = "buttSearch";
            this.buttSearch.Size = new System.Drawing.Size(32, 32);
            this.buttSearch.TabIndex = 7;
            this.buttSearch.UseVisualStyleBackColor = true;
            this.buttSearch.Visible = false;
            this.buttSearch.Click += new System.EventHandler(this.buttSearch_Click);
            // 
            // buttSave
            // 
            this.buttSave.Location = new System.Drawing.Point(602, 263);
            this.buttSave.Name = "buttSave";
            this.buttSave.Size = new System.Drawing.Size(32, 32);
            this.buttSave.TabIndex = 7;
            this.buttSave.UseVisualStyleBackColor = true;
            this.buttSave.Visible = false;
            this.buttSave.Click += new System.EventHandler(this.buttSave_Click);
            // 
            // buttUpdateDefinition
            // 
            this.buttUpdateDefinition.Location = new System.Drawing.Point(602, 225);
            this.buttUpdateDefinition.Name = "buttUpdateDefinition";
            this.buttUpdateDefinition.Size = new System.Drawing.Size(32, 32);
            this.buttUpdateDefinition.TabIndex = 7;
            this.buttUpdateDefinition.UseVisualStyleBackColor = true;
            this.buttUpdateDefinition.Visible = false;
            this.buttUpdateDefinition.Click += new System.EventHandler(this.buttUpdateDefinition_Click);
            // 
            // buttAdd
            // 
            this.buttAdd.Location = new System.Drawing.Point(342, 301);
            this.buttAdd.Name = "buttAdd";
            this.buttAdd.Size = new System.Drawing.Size(32, 32);
            this.buttAdd.TabIndex = 7;
            this.buttAdd.UseVisualStyleBackColor = true;
            this.buttAdd.Visible = false;
            this.buttAdd.Click += new System.EventHandler(this.buttAdd_Click);
            // 
            // buttGoogle
            // 
            this.buttGoogle.Location = new System.Drawing.Point(418, 301);
            this.buttGoogle.Name = "buttGoogle";
            this.buttGoogle.Size = new System.Drawing.Size(32, 32);
            this.buttGoogle.TabIndex = 8;
            this.buttGoogle.UseVisualStyleBackColor = true;
            this.buttGoogle.Visible = false;
            this.buttGoogle.Click += new System.EventHandler(this.buttGoogle_Click);
            // 
            // formReading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 559);
            this.Controls.Add(this.buttGoogle);
            this.Controls.Add(this.buttUpdateDefinition);
            this.Controls.Add(this.buttSave);
            this.Controls.Add(this.buttAdd);
            this.Controls.Add(this.buttSearch);
            this.Controls.Add(this.rtbDef);
            this.Controls.Add(this.rtbText);
            this.Name = "formReading";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Assisted Reading";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formReading_FormClosing);
            this.Load += new System.EventHandler(this.formReading_Load);
            this.Resize += new System.EventHandler(this.formReading_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbText;
        private System.Windows.Forms.RichTextBox rtbDef;
        private System.Windows.Forms.Button buttSearch;
        private System.Windows.Forms.Button buttSave;
        private System.Windows.Forms.Button buttUpdateDefinition;
        private System.Windows.Forms.Button buttAdd;
        private System.Windows.Forms.Button buttGoogle;
    }
}