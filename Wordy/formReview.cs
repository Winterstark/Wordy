using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GenericForms;

namespace Wordy
{
    public partial class formReview : Form
    {
        public formMain main;
        public List<Entry> words;
        List<string> corewords = new List<string>();


        void showWords()
        {
            //grab the selected category of words
            Entry[] selectedWords;

            if (rdbShowUnlearned.Checked)
                selectedWords = words.Where(w => !w.archived).ToArray();
            else if (rdbShowLearned.Checked)
                selectedWords = words.Where(w => w.archived).ToArray();
            else
                selectedWords = words.ToArray();

            //filter words
            string query = textSearch.Text.ToLower();

            if (!checkSearchDefs.Checked)
                selectedWords = selectedWords.Where(w => w.ToString().ToLower().Contains(query)).ToArray();
            else
                selectedWords = selectedWords.Where(w => w.ToString().ToLower().Contains(query) || w.GetDefinition().ToLower().Contains(query)).ToArray();

            //sort
            switch (comboSortBy.Text)
            {
                case "Sort by date added":
                    if (buttSortOrder.Text == "Ascending")
                        selectedWords = selectedWords.OrderBy<Entry, DateTime>(w => w.GetCreationDate()).ToArray();
                    else
                        selectedWords = selectedWords.OrderByDescending<Entry, DateTime>(w => w.GetCreationDate()).ToArray();
                    break;
                case "Sort by A-Z":
                    if (buttSortOrder.Text == "Ascending")
                        selectedWords = selectedWords.OrderBy<Entry, string>(w => w.ToString()).ToArray();
                    else
                        selectedWords = selectedWords.OrderByDescending<Entry, string>(w => w.ToString()).ToArray();
                    break;
                case "Sort by current learning step":
                    if (buttSortOrder.Text == "Ascending")
                        selectedWords = selectedWords.OrderBy<Entry, int>(w => w.learningPhase).ToArray();
                    else
                        selectedWords = selectedWords.OrderByDescending<Entry, int>(w => w.learningPhase).ToArray();
                    break;
                case "Sort by hardest to learn":
                    if (buttSortOrder.Text == "Ascending")
                        selectedWords = selectedWords.OrderBy<Entry, int>(w => w.GetNStudyAttempts()).ToArray();
                    else
                        selectedWords = selectedWords.OrderByDescending<Entry, int>(w => w.GetNStudyAttempts()).ToArray();
                    break;
                case "Sort by hardest to remember":
                    if (buttSortOrder.Text == "Ascending")
                        selectedWords = selectedWords.OrderBy<Entry, float>(w => 1 - w.GetRecallSuccessRate()).ToArray();
                    else
                        selectedWords = selectedWords.OrderByDescending<Entry, float>(w => 1 - w.GetRecallSuccessRate()).ToArray();
                    break;
            }
            
            //set fonts
            Font heading = new Font("Candara", 20, FontStyle.Bold);
            Font body = new Font("Calibri", 12);

            //display
            rtbDef.Text = "";

            foreach (Entry word in selectedWords)
            {
                //word
                rtbDef.SelectionFont = heading;
                if (!word.archived)
                    rtbDef.SelectionColor = Color.Black;
                else
                    rtbDef.SelectionColor = Color.Green;
                rtbDef.AppendText(word.ToString() + Environment.NewLine);

                //definitions
                rtbDef.SelectionFont = body;
                Misc.AppendDefs(rtbDef, word.GetDefinition() + Environment.NewLine, corewords);

                //extra info
                rtbDef.SelectionColor = Color.Gray;

                switch (comboSortBy.Text)
                {
                    case "Sort by date added":
                        rtbDef.AppendText("Added on " + word.GetCreationDate() + Environment.NewLine);
                        break;
                    case "Sort by current learning step": ;
                        if (!word.archived)
                            rtbDef.AppendText("Current learning step: " + word.learningPhase + Environment.NewLine);
                        else
                            rtbDef.AppendText("Word already learned.");
                        break;
                    case "Sort by hardest to learn":
                        if (word.archived)
                            rtbDef.AppendText("Word learned in " + (word.GetNStudyAttempts() - 6).ToString() + " extra steps");
                        else
                            rtbDef.AppendText("Word not yet learned. Times tested: " + word.GetNStudyAttempts());
                        break;
                    case "Sort by hardest to remember":
                        if (word.GetRecallSuccessRate() != -1)
                            rtbDef.AppendText("Recall success rate: " + (int)(word.GetRecallSuccessRate() * 100) + "%" + Environment.NewLine);
                        else
                            rtbDef.AppendText("Word not yet tested (as a learned word).");
                        break;
                }

                //visual
                string imgPath = Application.StartupPath + "\\visuals\\" + word.ToString() + ".jpg";

                if (File.Exists(imgPath))
                {
                    Clipboard.SetImage(Image.FromFile(imgPath));

                    rtbDef.ReadOnly = false;
                    rtbDef.Paste();
                    rtbDef.ReadOnly = true;
                }

                rtbDef.AppendText(Environment.NewLine);
            }

            this.Text = "Review - Showing " + selectedWords.Length + " Words";
            this.Refresh();
        }


        public formReview()
        {
            InitializeComponent();
        }

        private void formReview_Load(object sender, EventArgs e)
        {
            //icon
            string iconPath = Application.StartupPath + "\\Wordy.ico";

            if (File.Exists(iconPath))
                this.Icon = new Icon(iconPath);

            //load core words
            corewords = Misc.LoadCoreWords();

            //init controls
            comboSortBy.SelectedIndex = 0;

            this.BackColor = Color.FromArgb(211, 211, 211);
            rtbDef.BackColor = this.BackColor;

            //display
            showWords();

            //tutorial
            new Tutorial(Application.StartupPath + "\\tutorials\\review.txt", this);
        }

        private void formReview_Resize(object sender, EventArgs e)
        {
            //resize controls
            rdbShowUnlearned.Width = (this.Width - 30 - 4 * 6) / 3;
            rdbShowLearned.Width = rdbShowUnlearned.Width;
            rdbShowAll.Width = rdbShowUnlearned.Width;

            comboSortBy.Width = this.Width - 30 - 3 * 6 - buttSortOrder.Width;

            textSearch.Width = this.Width - 30 - 4 * 6 - label1.Width - checkSearchDefs.Width;

            //recalculate controls' Top values
            label1.Top = this.Height - 71;
            textSearch.Top = label1.Top - 3;
            checkSearchDefs.Top = textSearch.Top - 2;

            comboSortBy.Top = this.Height - 103;
            buttSortOrder.Top = comboSortBy.Top - 2;

            rdbShowUnlearned.Top = this.Height - 132;
            rdbShowLearned.Top = rdbShowUnlearned.Top;
            rdbShowAll.Top = rdbShowUnlearned.Top;

            //recalculate controls' Left values
            rdbShowLearned.Left = rdbShowUnlearned.Left + rdbShowUnlearned.Width + 6;
            rdbShowAll.Left = rdbShowLearned.Left + rdbShowLearned.Width + 6;

            buttSortOrder.Left = comboSortBy.Left + comboSortBy.Width + 6;

            checkSearchDefs.Left = textSearch.Left + textSearch.Width + 6;

            //resize richtextbox
            rtbDef.Width = this.Width - 31;
            rtbDef.Height = rdbShowUnlearned.Top - 6;
        }

        private void formReview_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Show();
        }
        
        private void rdbShowUnlearned_Click(object sender, EventArgs e)
        {
            showWords();
        }

        private void rdbShowLearned_Click(object sender, EventArgs e)
        {
            showWords();
        }

        private void rdbShowAll_Click(object sender, EventArgs e)
        {
            showWords();
        }

        private void comboSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            showWords();
        }

        private void buttSortOrder_Click(object sender, EventArgs e)
        {
            if (buttSortOrder.Text == "Descending")
                buttSortOrder.Text = "Ascending";
            else
                buttSortOrder.Text = "Descending";

            showWords();
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            showWords();
        }

        private void checkSearchDefs_CheckedChanged(object sender, EventArgs e)
        {
            showWords();
        }
    }
}
