namespace Wordy
{
    partial class formPopup
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
            this.rtbDef = new System.Windows.Forms.RichTextBox();
            this.buttGoogle = new System.Windows.Forms.Button();
            this.buttUpdateDefinition = new System.Windows.Forms.Button();
            this.buttSave = new System.Windows.Forms.Button();
            this.buttAdd = new System.Windows.Forms.Button();
            this.buttSearch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbDef
            // 
            this.rtbDef.BackColor = System.Drawing.SystemColors.Window;
            this.rtbDef.Location = new System.Drawing.Point(12, 12);
            this.rtbDef.Name = "rtbDef";
            this.rtbDef.ReadOnly = true;
            this.rtbDef.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbDef.Size = new System.Drawing.Size(254, 165);
            this.rtbDef.TabIndex = 7;
            this.rtbDef.Text = "";
            this.rtbDef.Visible = false;
            this.rtbDef.VScroll += new System.EventHandler(this.rtbDef_VScroll);
            this.rtbDef.Click += new System.EventHandler(this.rtbDef_Click);
            this.rtbDef.TextChanged += new System.EventHandler(this.rtbDef_TextChanged);
            this.rtbDef.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtbDef_MouseDown);
            // 
            // buttGoogle
            // 
            this.buttGoogle.Location = new System.Drawing.Point(88, 183);
            this.buttGoogle.Name = "buttGoogle";
            this.buttGoogle.Size = new System.Drawing.Size(32, 32);
            this.buttGoogle.TabIndex = 13;
            this.buttGoogle.UseVisualStyleBackColor = true;
            this.buttGoogle.Visible = false;
            this.buttGoogle.Click += new System.EventHandler(this.buttGoogle_Click);
            // 
            // buttUpdateDefinition
            // 
            this.buttUpdateDefinition.Location = new System.Drawing.Point(310, 145);
            this.buttUpdateDefinition.Name = "buttUpdateDefinition";
            this.buttUpdateDefinition.Size = new System.Drawing.Size(32, 32);
            this.buttUpdateDefinition.TabIndex = 9;
            this.buttUpdateDefinition.UseVisualStyleBackColor = true;
            this.buttUpdateDefinition.Visible = false;
            this.buttUpdateDefinition.Click += new System.EventHandler(this.buttUpdateDefinition_Click);
            // 
            // buttSave
            // 
            this.buttSave.Location = new System.Drawing.Point(272, 145);
            this.buttSave.Name = "buttSave";
            this.buttSave.Size = new System.Drawing.Size(32, 32);
            this.buttSave.TabIndex = 10;
            this.buttSave.UseVisualStyleBackColor = true;
            this.buttSave.Visible = false;
            this.buttSave.Click += new System.EventHandler(this.buttSave_Click);
            // 
            // buttAdd
            // 
            this.buttAdd.Location = new System.Drawing.Point(12, 183);
            this.buttAdd.Name = "buttAdd";
            this.buttAdd.Size = new System.Drawing.Size(32, 32);
            this.buttAdd.TabIndex = 11;
            this.buttAdd.UseVisualStyleBackColor = true;
            this.buttAdd.Visible = false;
            this.buttAdd.Click += new System.EventHandler(this.buttAdd_Click);
            // 
            // buttSearch
            // 
            this.buttSearch.Location = new System.Drawing.Point(50, 183);
            this.buttSearch.Name = "buttSearch";
            this.buttSearch.Size = new System.Drawing.Size(32, 32);
            this.buttSearch.TabIndex = 12;
            this.buttSearch.UseVisualStyleBackColor = true;
            this.buttSearch.Visible = false;
            this.buttSearch.Click += new System.EventHandler(this.buttSearch_Click);
            // 
            // formPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(481, 239);
            this.Controls.Add(this.buttGoogle);
            this.Controls.Add(this.buttUpdateDefinition);
            this.Controls.Add(this.buttSave);
            this.Controls.Add(this.buttAdd);
            this.Controls.Add(this.buttSearch);
            this.Controls.Add(this.rtbDef);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "formPopup";
            this.ShowInTaskbar = false;
            this.Text = "Wordy";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Load += new System.EventHandler(this.formPopup_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox rtbDef;
        public System.Windows.Forms.Button buttGoogle;
        public System.Windows.Forms.Button buttUpdateDefinition;
        public System.Windows.Forms.Button buttSave;
        public System.Windows.Forms.Button buttAdd;
        public System.Windows.Forms.Button buttSearch;

    }
}