using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Wordy
{
    public partial class formAddLanguage : Form
    {
        public formMain main;


        public formAddLanguage()
        {
            InitializeComponent();
        }

        private void formAddLanguage_Load(object sender, EventArgs e)
        {
            chklistLanguages.Items.Clear();

            foreach (var lang in main.Languages)
                if (!File.Exists(Application.StartupPath + "\\languages\\" + "words-" + lang.Value + ".txt"))
                    chklistLanguages.Items.Add(lang.Key);
        }

        private void chklistLanguages_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            buttAdd.Enabled = chklistLanguages.CheckedItems.Count + (e.NewValue == CheckState.Checked ? 1 : -1) > 0;
        }

        private void buttAdd_Click(object sender, EventArgs e)
        {
            bool change = false;

            foreach (var item in chklistLanguages.CheckedItems)
            {
                string filePath = Application.StartupPath + "\\languages\\words-" + main.Languages[item.ToString()] + ".txt";

                if (!File.Exists(filePath))
                {
                    //create blank files
                    StreamWriter file = new StreamWriter(filePath);
                    file.Close();

                    file = new StreamWriter(filePath.Replace("\\words-", "\\newwords-"));
                    file.Close();

                    change = true;
                }
            }

            if (change)
                main.LoadActiveLanguages();

            this.Close();
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
