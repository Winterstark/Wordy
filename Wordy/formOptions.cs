using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Diagnostics;
using GenericForms;

namespace Wordy
{
    public partial class formOptions : Form
    {
        public formMain main;
        public List<Entry> words;
        public List<WordOfTheDay> wotds;
        public double CurrentVersion;
        public string DefaultUpdateURL;

        UpdateConfig updateConfig;
        List<string> corewords = new List<string>();
        bool populatingWordList;


        void delWord()
        {
            if (chklistWords.SelectedIndex != -1 && MessageBox.Show("Are you sure you want to permanently delete \"" + chklistWords.Text + "\"?", "Delete Word?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                //del visual
                if (File.Exists("visuals\\" + chklistWords.Text + ".jpg"))
                    File.Delete("visuals\\" + chklistWords.Text + ".jpg");

                words.RemoveAt(getSelWordInd());
                main.SaveWords();

                chklistWords.Items.RemoveAt(chklistWords.SelectedIndex);
            }
        }

        void renameWord()
        {
            if (chklistWords.SelectedIndex != -1)
            {
                string word = chklistWords.Text;
                if (InputBox.Show("Rename Word", "Set new word:", ref word) == DialogResult.OK)
                {
                    words[getSelWordInd()].Rename(word);
                    main.SaveWords();

                    int ind = chklistWords.SelectedIndex;
                    bool isChecked = chklistWords.GetItemChecked(ind);
                    chklistWords.Items.RemoveAt(ind);
                    chklistWords.Items.Insert(ind, word);
                    chklistWords.SetItemChecked(ind, isChecked);
                }
            }
        }

        void delSub()
        {
            if (MessageBox.Show("Are you sure you want to permanently delete subscription: " + chklistSubscriptions.Text + "?", "Delete WotD Subscription?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
            {
                wotds.RemoveAt(chklistSubscriptions.SelectedIndex);
                main.SaveSubs();

                chklistSubscriptions.Items.RemoveAt(chklistSubscriptions.SelectedIndex);
            }
        }

        int getSelWordInd()
        {
            return words.FindIndex(w => w.ToString() == chklistWords.Text);
        }

        void updateWordCount()
        {
            lblWords.Text = "Learned " + words.Count<Entry>(w => w.archived) + " of " + words.Count + " words:";
        }

        void checkIfValidFeed()
        {
            bool valid = textNewSubAddress.Text != "";

            if (valid)
                foreach (WordOfTheDay wotd in wotds)
                    if (wotd.URI == textNewSubAddress.Text || wotd.title == textNewSubTitle.Text)
                    {
                        valid = false;
                        break;
                    }

            buttAddSub.Enabled = valid;
        }

        void displayWords()
        {
            populatingWordList = true;

            chklistWords.Items.Clear();
            string filter = textFilter.Text.ToLower();

            foreach (Entry word in words)
                if (word.ToString().ToLower().Contains(filter))
                    chklistWords.Items.Add(word.ToString(), word.archived);

            populatingWordList = false;
        }

        void checkForDefinitionChanges()
        {
            if (chklistWords.SelectedIndex == -1)
                buttUpdateDef.Enabled = false;
            else
                buttUpdateDef.Enabled = textDef.Text != words[getSelWordInd()].GetDefinition().Replace("\r\n", "\n")
                    || textSynonyms.Text != words[getSelWordInd()].GetSynonyms().Replace(" / ", ", ");
        }

        public void DisplaySubs()
        {
            wotds = main.wotds;

            chklistSubscriptions.Items.Clear();
            foreach (WordOfTheDay wotd in wotds)
                chklistSubscriptions.Items.Add(wotd.title, wotd.active);
        }


        public formOptions()
        {
            InitializeComponent();
        }

        private void formOptions_Load(object sender, EventArgs e)
        {
            //icon
            if (File.Exists(Application.StartupPath + "\\Wordy.ico"))
                this.Icon = new Icon(Application.StartupPath + "\\Wordy.ico");

            //load core words
            corewords = Misc.LoadCoreWords();

            //display preferences
            checkAutoVisuals.Checked = main.prefs.AutoVisuals;
            checkPlaySounds.Checked = main.prefs.PlaySounds;
            textNewWordsPath.Text = main.prefs.NewWordsPath;

            displayWords();

            updateWordCount();
            
            textFilter.Left = lblWords.Width + 12;
            textFilter.Width = 341 - textFilter.Left;

            if (main.Profile != "English")
            {
                //hide English-only UI elements
                lblSyns.Visible = false;
                textSynonyms.Visible = false;
                picWordnik.Visible = false;
            }
        }

        private void formWordlist_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save new words file path
            main.prefs.NewWordsPath = textNewWordsPath.Text;
            main.prefs.Save();

            main.Show();
        }

        private void chklistWords_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!populatingWordList) //don't track itemchecks when Wordy is adding words to the listbox
            {
                int wordInd = getSelWordInd();

                if (chklistWords.SelectedIndex != -1)
                {
                    bool newState = !chklistWords.GetItemChecked(chklistWords.SelectedIndex);
                    words[wordInd].archived = newState;

                    if (words[wordInd].archived)
                        words[wordInd].learningPhase = 7;
                    else
                    {
                        words[wordInd].learningPhase = 1;
                        words[wordInd].ResetLearned();
                    }

                    main.SaveWords();
                    updateWordCount();
                }
            }
        }

        private void chklistWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chklistWords.SelectedIndex == -1)
            {
                buttDelete.Enabled = false;
                buttRenameWord.Enabled = false;
                picWordnik.Enabled = false;

                textDef.Text = "";
                lblInfo.Text = "";
                textDef.Enabled = false;
                lblDef.Enabled = false;
                textSynonyms.Enabled = false;
                lblSyns.Enabled = false;
            }
            else
            {
                buttDelete.Enabled = true;
                buttRenameWord.Enabled = true;
                picWordnik.Enabled = true;

                textDef.Enabled = true;
                lblDef.Enabled = true;
                textSynonyms.Enabled = true;
                lblSyns.Enabled = true;

                int ind = getSelWordInd();

                Misc.DisplayDefs(textDef, words[ind].GetDefinition(), corewords);
                textSynonyms.Text = words[getSelWordInd()].GetSynonyms().Replace(" / ", ", ");
                lblInfo.Text = words[ind].GetInfo(main.Profile == "English");

                if (File.Exists("visuals\\" + chklistWords.Text + ".jpg"))
                    lblVisualTrigger.Visible = true;
                else
                    lblVisualTrigger.Visible = false;
            }
        }

        private void chklistWords_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    delWord();
                    break;
                case Keys.F2:
                    renameWord();
                    break;
            }
        }

        private void buttDelete_Click(object sender, EventArgs e)
        {
            delWord();
        }

        private void buttRenameWord_Click(object sender, EventArgs e)
        {
            renameWord();
        }

        private void textDef_TextChanged(object sender, EventArgs e)
        {
            checkForDefinitionChanges();
        }

        private void textSynonyms_TextChanged(object sender, EventArgs e)
        {
            checkForDefinitionChanges();
        }

        private void buttUpdateDef_Click(object sender, EventArgs e)
        {
            words[getSelWordInd()].SetDefinition(textDef.Text);
            words[getSelWordInd()].SetSynonyms(textSynonyms.Text);

            main.SaveWords();

            buttUpdateDef.Enabled = false;

            Misc.DisplayDefs(textDef, words[getSelWordInd()].GetDefinition(), corewords);
        }

        private void checkAutoVisuals_CheckedChanged(object sender, EventArgs e)
        {
            main.prefs.AutoVisuals = checkAutoVisuals.Checked;
            main.prefs.Save();
        }

        private void checkPlaySounds_CheckedChanged(object sender, EventArgs e)
        {
            main.prefs.PlaySounds = checkPlaySounds.Checked;
            main.prefs.Save();
        }

        private void numDaysUnlearned_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void numDaysArchived_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void buttAddSub_Click(object sender, EventArgs e)
        {
            if (textNewSubAddress.Text == "")
                MessageBox.Show("You need to enter the subscription's address first.");
            else 
            {
                try
                {
                    WordOfTheDay newSub = new WordOfTheDay(textNewSubTitle.Text, textNewSubAddress.Text, true, "");

                    chklistSubscriptions.Items.Add(newSub.title, newSub.active);

                    wotds.Add(newSub);
                    main.SaveSubs();
                    main.needWotDCheck = true;

                    textNewSubAddress.Text = "";
                    textNewSubTitle.Text = "";
                }
                catch
                {
                     MessageBox.Show("Cannot add subscription. The entered address doesn't seem to point to an RSS service");
                }
            }
        }

        private void chklistSubscriptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chklistSubscriptions.SelectedIndex != -1)
            {
                textNewSubAddress.Text = wotds[chklistSubscriptions.SelectedIndex].URI;
                textNewSubTitle.Text = wotds[chklistSubscriptions.SelectedIndex].title;

                buttDelSub.Enabled = true;
            }
            else
            {
                textNewSubAddress.Text = "";
                textNewSubTitle.Text = "";

                buttDelSub.Enabled = false;
            }
        }

        private void chklistSubscriptions_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (chklistSubscriptions.SelectedIndex != -1)
            {
                wotds[chklistSubscriptions.SelectedIndex].active = !chklistSubscriptions.GetItemChecked(chklistSubscriptions.SelectedIndex);
                main.SaveSubs();
                
                main.needWotDCheck = true;
            }
        }

        private void chklistSubscriptions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                delSub();
        }

        private void buttDelSub_Click(object sender, EventArgs e)
        {
            delSub();
        }

        private void textNewSubAddress_TextChanged(object sender, EventArgs e)
        {
            checkIfValidFeed();
        }

        private void textNewSubTitle_TextChanged(object sender, EventArgs e)
        {
            checkIfValidFeed();
        }

        private void buttSortName_Click(object sender, EventArgs e)
        {
            chklistWords.Sorted = true;
            buttSortName.Visible = false;
        }

        private void buttBrowse_Click(object sender, EventArgs e)
        {
            openDiag.InitialDirectory = Application.StartupPath;
            openDiag.ShowDialog();

            if (openDiag.FileName != "")
                textNewWordsPath.Text = openDiag.FileName;
        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabs.SelectedIndex == 2)
            {
                if (wotds == null && (main.WotdWorker == null || !main.WotdWorker.IsBusy))
                    main.LoadSubs(main.wotdWorker_RunWorkerCompleted_WotDOptions);
                else
                    DisplaySubs();
            }
        }

        private void textDef_SelectionChanged(object sender, EventArgs e)
        {
            menuToggleKeyword.Enabled = textDef.SelectedText != "";
            menuSurroundWQuotes.Enabled = menuToggleKeyword.Enabled;
            menuSurroundWParentheses.Enabled = menuToggleKeyword.Enabled;
        }

        private void menuToggleKeyword_Click(object sender, EventArgs e)
        {
            Misc.ToggleKeyword(corewords, textDef.SelectedText);
            Misc.DisplayDefs(textDef, words[getSelWordInd()].GetDefinition(), corewords);
        }

        private void menuSurroundWQuotes_Click(object sender, EventArgs e)
        {
            int ub = textDef.SelectionStart + textDef.SelectionLength;
            while (textDef.Text[ub - 1] == '\n' || textDef.Text[ub - 1] == '\r' || textDef.Text[ub - 1] == ' ')
                ub--;

            if (textDef.Text[textDef.SelectionStart] == ' ')
                textDef.SelectionStart++;

            words[getSelWordInd()].SetDefinition(textDef.Text.Insert(ub, "\"").Insert(textDef.SelectionStart, "\""));
            Misc.DisplayDefs(textDef, words[getSelWordInd()].GetDefinition(), corewords);
            buttUpdateDef.Enabled = true;
        }

        private void menuSurroundWParentheses_Click(object sender, EventArgs e)
        {
            int ub = textDef.SelectionStart + textDef.SelectionLength;
            while (textDef.Text[ub - 1] == '\n' || textDef.Text[ub - 1] == '\r' || textDef.Text[ub - 1] == ' ')
                ub--;

            if (textDef.Text[textDef.SelectionStart] == ' ')
                textDef.SelectionStart++;

            words[getSelWordInd()].SetDefinition(textDef.Text.Insert(ub, ")").Insert(textDef.SelectionStart, "("));
            Misc.DisplayDefs(textDef, words[getSelWordInd()].GetDefinition(), corewords);
            buttUpdateDef.Enabled = true;
        }

        private void lblVisualTrigger_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", " /select, " + "visuals\\" + chklistWords.Text + ".jpg");
        }

        private void textFilter_TextChanged(object sender, EventArgs e)
        {
            displayWords();
        }

        private void picWordnik_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.wordnik.com/words/" + chklistWords.Text);
        }

        private void buttUpdateOptions_Click(object sender, EventArgs e)
        {
            if (updateConfig == null || updateConfig.IsDisposed)
            {
                updateConfig = new UpdateConfig();
                updateConfig.CurrentVersion = CurrentVersion;
                updateConfig.DefaultUpdateURL = DefaultUpdateURL;
                updateConfig.Show();
            }
        }
    }
}
