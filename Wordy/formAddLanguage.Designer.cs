namespace Wordy
{
    partial class formAddLanguage
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
            this.buttCancel = new System.Windows.Forms.Button();
            this.chklistLanguages = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // buttAdd
            // 
            this.buttAdd.Enabled = false;
            this.buttAdd.Location = new System.Drawing.Point(12, 337);
            this.buttAdd.Name = "buttAdd";
            this.buttAdd.Size = new System.Drawing.Size(100, 23);
            this.buttAdd.TabIndex = 1;
            this.buttAdd.Text = "Add";
            this.buttAdd.UseVisualStyleBackColor = true;
            this.buttAdd.Click += new System.EventHandler(this.buttAdd_Click);
            // 
            // buttCancel
            // 
            this.buttCancel.Location = new System.Drawing.Point(124, 337);
            this.buttCancel.Name = "buttCancel";
            this.buttCancel.Size = new System.Drawing.Size(100, 23);
            this.buttCancel.TabIndex = 2;
            this.buttCancel.Text = "Cancel";
            this.buttCancel.UseVisualStyleBackColor = true;
            this.buttCancel.Click += new System.EventHandler(this.buttCancel_Click);
            // 
            // chklistLanguages
            // 
            this.chklistLanguages.FormattingEnabled = true;
            this.chklistLanguages.Location = new System.Drawing.Point(12, 12);
            this.chklistLanguages.Name = "chklistLanguages";
            this.chklistLanguages.Size = new System.Drawing.Size(212, 319);
            this.chklistLanguages.TabIndex = 3;
            this.chklistLanguages.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chklistLanguages_ItemCheck);
            // 
            // formAddLanguage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 367);
            this.Controls.Add(this.chklistLanguages);
            this.Controls.Add(this.buttCancel);
            this.Controls.Add(this.buttAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "formAddLanguage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Language";
            this.Load += new System.EventHandler(this.formAddLanguage_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttAdd;
        private System.Windows.Forms.Button buttCancel;
        private System.Windows.Forms.CheckedListBox chklistLanguages;
    }
}