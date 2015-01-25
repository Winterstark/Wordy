using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NikSharp;
using NikSharp.Model;

namespace Wordy
{
    public partial class formReading : Form
    {
        #region DLLImport to get/set Scroll position in a RichTextBox
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, Int32 wParam, ref Point lParam);

        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        private const int WM_USER = 0x400;
        private const int EM_GETSCROLLPOS = WM_USER + 221;
        private const int EM_SETSCROLLPOS = WM_USER + 222;
        private const int SB_VERT = 0x1;
        #endregion


        public formMain main;
        public List<Entry> words;

        Translator translator;
        WordnikService wordnik;
        BackgroundWorker searchWordWorker;
        delegate void SetTextCallback(string text);
        Dictionary<string, Definition> newDefs;
        Dictionary<string, string> synonyms, rhymes;
        Graphics gfx;
        StringFormat measuringStringFormat;
        List<string> corewords, newWords, searchedWords, notFoundWords;
        string activeSearchWord;
        float lineH;
        Point popupPos;
        int rtbTextVScroll;
        bool colorizing, usePrevPopupPos;


        void loadNewwords()
        {
            string list = Misc.LoadNewWordsFromFile(main.GetNewWordsPath());
            newWords = list.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        void checkIfTextContainsWord(string txt, string word, List<Tuple<int, int, Color>> knownWords, Color color)
        {
            int i = txt.IndexOf(word.ToLower());

            while (i != -1)
            {
                //check if word is delimited by non-letters
                if ((i == 0 || !char.IsLetter(txt[i - 1]))
                    && (i + word.Length == txt.Length || !char.IsLetter(txt[i + word.Length])))
                    knownWords.Add(new Tuple<int, int, Color>(i, word.Length, color));

                i = txt.IndexOf(word.ToLower(), i + 1);
            }
        }

        void colorizeRtbText()
        {
            colorizing = true;

            int prevScroll = rtbTextVScroll;
            int selStart = rtbText.SelectionStart, selLen = rtbText.SelectionLength;

            List<Tuple<int, int, Color>> knownWords = new List<Tuple<int, int, Color>>();
            string txt = rtbText.Text.ToLower();

            //find learned words
            foreach (Entry word in words.Where(w => !w.archived))
                checkIfTextContainsWord(txt, word.ToString(), knownWords, Color.Yellow);

            //find learned words
            foreach (Entry word in words.Where(w => w.archived))
                checkIfTextContainsWord(txt, word.ToString(), knownWords, Color.Green);

            //find words-to-add
            foreach (string word in newWords)
                checkIfTextContainsWord(txt, word, knownWords, Color.Purple);

            //find activeSearchWord
            if (activeSearchWord != "")
                checkIfTextContainsWord(txt, activeSearchWord, knownWords, Color.DarkBlue);

            //find words with new definitions
            foreach (string word in searchedWords)
                checkIfTextContainsWord(txt, word, knownWords, Color.Orange);
            
            //find words with failed searches
            foreach (string word in notFoundWords)
                checkIfTextContainsWord(txt, word, knownWords, Color.Red);

            if (knownWords.Count > 0)
            {
                //sort
                knownWords.Sort();

                //check for overlaps
                for (int i = 0; i < knownWords.Count - 1; i++)
                    if (knownWords[i].Item1 + knownWords[i].Item2 > knownWords[i + 1].Item1)
                    {
                        knownWords.RemoveAt(i + 1);
                        i--; //check again with the next one
                    }

                //rebuild text and color known word chunks
                txt = rtbText.Text;
                rtbText.Text = "";

                rtbText.SelectionColor = Color.Black;
                rtbText.AppendText(txt.Substring(0, knownWords[0].Item1));

                for (int i = 0; i < knownWords.Count - 1; i++)
                {
                    rtbText.SelectionColor = knownWords[i].Item3;
                    rtbText.AppendText(txt.Substring(knownWords[i].Item1, knownWords[i].Item2));
                    rtbText.SelectionColor = Color.Black;
                    rtbText.AppendText(txt.Substring(knownWords[i].Item1 + knownWords[i].Item2, knownWords[i + 1].Item1 - (knownWords[i].Item1 + knownWords[i].Item2)));
                }

                rtbText.SelectionColor = knownWords[knownWords.Count - 1].Item3;
                rtbText.AppendText(txt.Substring(knownWords[knownWords.Count - 1].Item1, knownWords[knownWords.Count - 1].Item2));
                rtbText.SelectionColor = Color.Black;
                rtbText.AppendText(txt.Substring(knownWords[knownWords.Count - 1].Item1 + knownWords[knownWords.Count - 1].Item2));
            }
            
            setVSCroll(rtbDef, prevScroll);

            usePrevPopupPos = true;
            rtbText.Select(selStart, selLen);

            usePrevPopupPos = false;
            colorizing = false;
        }

        void resizeRtbDef()
        {
            SizeF size = gfx.MeasureString(rtbDef.Text, rtbDef.Font, 233, measuringStringFormat);

            rtbDef.Width = (int)(1.5f * size.Width);
            rtbDef.Height = (int)(size.Height + lineH);
        }

        void hideAllControlsExcept(params Control[] exceptions)
        {
            rtbDef.Visible = exceptions.Contains(rtbDef);

            buttAdd.Visible = exceptions.Contains(buttAdd);
            buttSearch.Visible = exceptions.Contains(buttSearch);
            buttGoogle.Visible = exceptions.Contains(buttGoogle);
            buttSave.Visible = exceptions.Contains(buttSave);
            buttUpdateDefinition.Visible = exceptions.Contains(buttUpdateDefinition);
        }

        string getSelection()
        {
            string selection = rtbText.SelectedText;

            //remove extra spaces
            while (selection.Length > 0 && selection[0] == ' ')
                selection = selection.Substring(1);

            while (selection.Length > 0 && selection[selection.Length - 1] == ' ')
                selection = selection.Substring(0, selection.Length - 1);

            return selection;
        }

        int getVScroll(RichTextBox rtb)
        {
            Point pt = Point.Empty;
            SendMessage(rtb.Handle, EM_GETSCROLLPOS, 0, ref pt);
            return pt.Y;
        }

        void setVSCroll(RichTextBox rtb, int value)
        {
            SetScrollPos((IntPtr)rtb.Handle, SB_VERT, value, true);

            Point pt = new Point(0, value);
            SendMessage(rtbText.Handle, EM_SETSCROLLPOS, 0, ref pt);
        }

        void setPositionNextToPointer(Control control)
        {
            if (usePrevPopupPos)
                control.Location = popupPos;
            else
            {
                Point pos = this.PointToClient(Cursor.Position);
                pos.Offset(0, (int)lineH);

                if (pos.X + control.Width + 6 > this.ClientSize.Width)
                    pos.X = this.ClientSize.Width - 6 - control.Width;

                if (pos.Y + control.Height + 6 > this.ClientSize.Height)
                    pos.Y = Math.Max(this.ClientSize.Height - 6 - control.Height, 6);

                control.Location = popupPos = pos;
            }
        }

        void updateStatus(string status)
        {
            if (rtbDef.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(updateStatus);
                this.Invoke(d, new object[] { status });
            }
            else
                rtbDef.Text = status;
        }

        void searchWordWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string word = (string)e.Argument;

            //prepare result variables
            Definition definitions = null;
            string synonyms = "";
            string rhymes = "";

            //search wordnik
            try
            {
                updateStatus("Searching definitions for '" + word + "' ...");
                WordnikDefinition[] wdDefs = wordnik.GetDefinitions(word).ToArray();

                if (wdDefs.Length > 0)
                {
                    definitions = new Definition(wdDefs);
                    synonyms = findSynonyms(word);
                    rhymes = findRhymes(word);

                    e.Result = new Tuple<string, Definition, string, string>(word, definitions, synonyms, rhymes);
                }
                else
                    e.Result = word + " not found";
            }
            catch
            {
                e.Result = "error";
                MessageBox.Show("Your Internet connection might be disrupted, or the Wordnik service might be temporarily down.", "Error while finding word definition for " + word, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void searchWordWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string word = "";

            if (e.Result is string)
            {
                string result = (string)e.Result;

                if (result.Length > 10 && result.Contains(" not found"))
                {
                    word = result.Substring(0, result.Length - 10);

                    //mark the word as not found
                    updateStatus("NOT FOUND!");
                    notFoundWords.Add(word);
                }
            }
            else
            {
                var result = (Tuple<string, Definition, string, string>)e.Result;
                word = result.Item1;

                //save word data
                newDefs.Add(word, result.Item2);
                synonyms.Add(word, result.Item3);
                rhymes.Add(word, result.Item4);

                searchedWords.Add(word);

                //display word data
                Misc.DisplayDefs(rtbDef, result.Item2.ToString(), corewords);
            }
            
            if (newWords.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                //remove word from words-to-add
                newWords.Remove(newWords.Find(w => w.ToLower() == word.ToLower()));

                StreamWriter file = new StreamWriter(main.GetNewWordsPath());
                foreach (string newWord in newWords)
                    file.WriteLine(newWord);
                file.Close();
            }

            activeSearchWord = "";
            colorizeRtbText();
        }

        void doneEvent(string word, string translation)
        {
            if (translation == "error" || translation == "" || word.ToLower() == translation.ToLower())
            {
                rtbDef.Text = "NOT FOUND!";
                notFoundWords.Add(word);
            }
            else
            {
                //save word data
                newDefs.Add(word, new Definition(translation, true));
                synonyms.Add(word, "");
                rhymes.Add(word, "");

                searchedWords.Add(word);

                //display word data
                Misc.DisplayDefs(rtbDef, translation, corewords);
            }

            if (newWords.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                //remove word from words-to-add
                newWords.Remove(newWords.Find(w => w.ToLower() == word.ToLower()));

                StreamWriter file = new StreamWriter(main.GetNewWordsPath());
                foreach (string newWord in newWords)
                    file.WriteLine(newWord);
                file.Close();
            }

            activeSearchWord = "";
            colorizeRtbText();
        }

        string findSynonyms(string word)
        {
            try
            {
                updateStatus("Finding synonyms for '" + word + "' ...");

                List<string> synList = new List<string>();

                foreach (var relation in wordnik.GetRelatedWords(word))
                    synList.AddRange(relation.Words);

                string syns = ", ";

                foreach (string syn in synList)
                    if (!syns.Contains(", " + syn + ", "))
                        syns += syn + ", ";

                if (syns.Length > 4)
                    syns = syns.Substring(2, syns.Length - 4);

                return syns;
            }
            catch
            {
                return "";
            }
        }

        string findRhymes(string word)
        {
            try
            {
                updateStatus("Finding rhymes for '" + word + "' ...");
                string rhymeList = "", rhymePg = Misc.DlPage("http://rhymebrain.com/talk?function=getRhymes&word=" + word);
                int count = 0;

                if (rhymePg != "not_found")
                {
                    int lb = rhymePg.IndexOf("\"word\":\"") + 8, ub;

                    while (lb != 7 && count < 10)
                    {
                        ub = rhymePg.IndexOf("\",\"", lb);

                        if (ub != -1)
                            rhymeList += rhymePg.Substring(lb, ub - lb) + ", ";

                        lb = rhymePg.IndexOf("\"word\":\"", ub) + 8;
                        count++;
                    }
                }

                if (rhymeList.Length > 2)
                    rhymeList = rhymeList.Substring(0, rhymeList.Length - 2);

                return rhymeList;
            }
            catch
            {
                return "";
            }
        }

        bool performHotkey(KeyEventArgs e)
        {
            bool hotkeyPerformed = true;

            if (buttAdd.Visible && e.Control && e.KeyCode == Keys.A)
                buttAdd.PerformClick();
            else if (buttSearch.Visible && e.Control && e.KeyCode == Keys.F)
                buttSearch.PerformClick();
            else if (buttGoogle.Visible && e.Control && e.KeyCode == Keys.G)
                buttGoogle.PerformClick();
            else if (buttUpdateDefinition.Visible && e.Control && e.KeyCode == Keys.D)
                buttUpdateDefinition.PerformClick();
            else if (buttSave.Visible && e.Control && e.KeyCode == Keys.S)
                buttSave.PerformClick();
            else
                hotkeyPerformed = false;

            return hotkeyPerformed;
        }


        public formReading()
        {
            InitializeComponent();
        }

        private void formReading_Load(object sender, EventArgs e)
        {
            //icon
            string iconPath = Application.StartupPath + "\\Wordy.ico";

            if (File.Exists(iconPath))
                this.Icon = new Icon(iconPath);

            //button icons
            string iconDir = Application.StartupPath + "\\ui\\";

            if (File.Exists(iconDir + "add.png"))
                buttAdd.Image = Image.FromFile(iconDir + "add.png");

            if (File.Exists(iconDir + "search.png"))
                buttSearch.Image = Image.FromFile(iconDir + "search.png");

            if (File.Exists(iconDir + "google.png"))
                buttGoogle.Image = Image.FromFile(iconDir + "google.png");

            if (File.Exists(iconDir + "save.png"))
                buttSave.Image = Image.FromFile(iconDir + "save.png");

            if (File.Exists(iconDir + "pencil.png"))
                buttUpdateDefinition.Image = Image.FromFile(iconDir + "pencil.png");

            //set back colors
            this.BackColor = Color.FromArgb(211, 211, 211);
            rtbText.BackColor = this.BackColor;

            //load/init arrays & stuff
            corewords = Misc.LoadCoreWords();
            loadNewwords();

            wordnik = new WordnikService("b3bbd1f9103a01de7d00a0fd1300164c17bfcec03eb86a678");

            newDefs = new Dictionary<string, Definition>(StringComparer.OrdinalIgnoreCase);
            synonyms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            rhymes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            searchedWords = new List<string>();
            notFoundWords = new List<string>();

            gfx = this.CreateGraphics();
            measuringStringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
            lineH = gfx.MeasureString("A", rtbDef.Font, int.MaxValue, measuringStringFormat).Height;

            activeSearchWord = "";
            rtbTextVScroll = 0;
            colorizing = false;
            usePrevPopupPos = false;

            //prepare language tools
            if (main.Profile == "English")
            {
                searchWordWorker = new BackgroundWorker();
                searchWordWorker.DoWork += new DoWorkEventHandler(searchWordWorker_DoWork);
                searchWordWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(searchWordWorker_RunWorkerCompleted);
            }
            else
                translator = new Translator(main.Languages[main.Profile], "en", doneEvent, main.prefs);
        }

        private void formReading_Resize(object sender, EventArgs e)
        {
            //resize richtextbox
            rtbText.Width = this.ClientSize.Width - 24;
            rtbText.Height = this.ClientSize.Height - 24;
        }

        private void formReading_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (searchedWords.Count > 0)
            {
                string msg = "Do you want to save these words:";
                foreach (string word in searchedWords)
                    msg += Environment.NewLine + word;

                DialogResult answer = MessageBox.Show(msg, "You haven't saved all of the looked-up words.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                switch (answer)
                {
                    case DialogResult.Yes:
                        //add words
                        main.AddNewWords(newDefs, synonyms, rhymes);
                        break;
                    case DialogResult.No:
                        //do nothing
                        break;
                    case DialogResult.Cancel:
                        //stop window from closing
                        e.Cancel = true;
                        return;
                }
            }

            main.Show();
        }

        private void rtbText_Enter(object sender, EventArgs e)
        {
            if (rtbText.Text == "Paste your text here...")
                rtbText.SelectAll();
        }

        private void rtbText_Click(object sender, EventArgs e)
        {
            if (rtbText.Text == "Paste your text here...")
                rtbText.SelectAll();
        }

        private void rtbText_TextChanged(object sender, EventArgs e)
        {
            if (!colorizing)
            {
                if (rtbText.Text == "")
                {
                    rtbText.Text = "Paste your text here...";
                    rtbText.SelectAll();
                    return;
                }

                colorizeRtbText();
            }
        }

        private void rtbText_SelectionChanged(object sender, EventArgs e)
        {
            if (rtbText.Text == "Paste your text here..." || rtbText.SelectionLength == 0 || string.IsNullOrWhiteSpace(rtbText.SelectedText))
            {
                hideAllControlsExcept();
                return;
            }

            string selection = getSelection();

            if (selection != "")
            {
                string selectionLCase = selection.ToLower();
                Entry selectedWord = words.Find(w => w.ToString().ToLower() == selectionLCase);

                if (selectedWord != null)
                {
                    //archived word
                    Misc.DisplayDefs(rtbDef, selectedWord.GetDefinition(), corewords);
                    resizeRtbDef();
                    rtbDef.Enabled = false;
                    setPositionNextToPointer(rtbDef);

                    hideAllControlsExcept(rtbDef);
                }
                else if (newWords.Contains(selection, StringComparer.OrdinalIgnoreCase))
                {
                    //word-to-be-added
                    setPositionNextToPointer(buttSearch);
                    hideAllControlsExcept(buttSearch);
                }
                else if (searchedWords.Contains(selection, StringComparer.OrdinalIgnoreCase))
                {
                    //searched word
                    Misc.DisplayDefs(rtbDef, newDefs[selection].ToString(), corewords);
                    resizeRtbDef();
                    rtbDef.Enabled = true;
                    setPositionNextToPointer(rtbDef);

                    buttSave.Top = Math.Max(rtbDef.Top + rtbDef.Height - buttSave.Height, rtbDef.Top);
                    buttSave.Left = rtbDef.Left + rtbDef.Width + 6;

                    hideAllControlsExcept(rtbDef, buttSave);
                }
                else if (notFoundWords.Contains(selection, StringComparer.OrdinalIgnoreCase))
                {
                    //nothing found for this word
                    setPositionNextToPointer(buttGoogle);
                    hideAllControlsExcept(buttGoogle);
                }
                else
                {
                    //unknown word
                    setPositionNextToPointer(buttAdd);
                    buttAdd.Left -= buttSearch.Width + 6;
                    buttSearch.Top = buttAdd.Top;
                    buttSearch.Left = buttAdd.Left + 38;

                    hideAllControlsExcept(buttAdd, buttSearch);
                }
            }
            else
                hideAllControlsExcept();
        }

        private void rtbText_KeyDown(object sender, KeyEventArgs e)
        {
            if (performHotkey(e))
                e.SuppressKeyPress = true;
        }

        private void rtbText_VScroll(object sender, EventArgs e)
        {
            int currVScroll = getVScroll(rtbText);

            //move the popup UI accordingly
            int vertChange = rtbTextVScroll - currVScroll;

            if (rtbDef.Visible)
                rtbDef.Top += vertChange;
            if (buttAdd.Visible)
                buttAdd.Top += vertChange;
            if (buttSearch.Visible)
                buttSearch.Top += vertChange;
            if (buttSave.Visible)
                buttSave.Top += vertChange;
            if (buttUpdateDefinition.Visible)
                buttUpdateDefinition.Top += vertChange;

            rtbTextVScroll = currVScroll;
        }

        private void rtbDef_Click(object sender, EventArgs e)
        {
            if (rtbDef.Text == "Enter new definition...")
                rtbDef.SelectAll();
        }

        private void rtbDef_KeyDown(object sender, KeyEventArgs e)
        {
            if (performHotkey(e))
                e.SuppressKeyPress = true;
        }

        private void rtbDef_TextChanged(object sender, EventArgs e)
        {
            if (newDefs.ContainsKey(getSelection()))
            {
                if (rtbDef.Text != newDefs[getSelection()].ToString().Replace("\r\n", "\n"))
                {
                    buttUpdateDefinition.Left = buttSave.Left + buttSave.Width + 6;
                    buttUpdateDefinition.Top = buttSave.Top;

                    buttUpdateDefinition.Visible = true;
                }
                else
                    buttUpdateDefinition.Visible = false;
            }
        }

        private void buttAdd_Click(object sender, EventArgs e)
        {
            newWords.Add(getSelection());

            StreamWriter file = new StreamWriter(main.GetNewWordsPath());
            for (int i = 0; i < newWords.Count; i++)
                file.Write(newWords[i] + (i < newWords.Count - 1 ? Environment.NewLine : ""));
            file.Close();

            colorizeRtbText();
        }

        private void buttSearch_Click(object sender, EventArgs e)
        {
            activeSearchWord = getSelection().ToLower();
            
            rtbDef.Text = "Looking up word. Please wait...";
            resizeRtbDef();
            rtbDef.Location = popupPos;

            if (main.Profile == "English")
                searchWordWorker.RunWorkerAsync(activeSearchWord);
            else
                translator.Translate(activeSearchWord);

            rtbDef.Visible = true;
            buttAdd.Visible = false;
            buttSearch.Visible = false;
        }

        private void buttGoogle_Click(object sender, EventArgs e)
        {
            if (main.Profile != "English")
                Process.Start("https://translate.google.com/#" + main.Languages[main.Profile] + "/en/" + getSelection());
            else
                Process.Start("https://www.google.com/search?q=" + getSelection());

            rtbDef.Enabled = true;
            rtbDef.Text = "Enter new definition...";

            rtbDef.Location = buttGoogle.Location;
            buttUpdateDefinition.Top = Math.Max(rtbDef.Top + rtbDef.Height - buttSave.Height, rtbDef.Top);
            buttUpdateDefinition.Left = rtbDef.Left + rtbDef.Width + 6;

            hideAllControlsExcept(rtbDef, buttUpdateDefinition);
        }

        private void buttUpdateDefinition_Click(object sender, EventArgs e)
        {
            string word = getSelection();

            if (newDefs.ContainsKey(word))
                newDefs[word].Parse(rtbDef.Text, false);
            else if (rtbDef.Text != "Enter new definition...")
            {
                //user adding definition found via Google
                notFoundWords.Remove(notFoundWords.Find(w => w.ToLower() == word.ToLower()));

                searchedWords.Add(word);
                newDefs.Add(word, new Definition(rtbDef.Text, true));
                synonyms.Add(word, "");
                rhymes.Add(word, "");

                colorizeRtbText();
            }

            buttUpdateDefinition.Visible = false;
        }

        private void buttSave_Click(object sender, EventArgs e)
        {
            string word = getSelection();
            main.AddNewWord(word, newDefs[word], synonyms[word], rhymes[word]);

            //remove word from lists
            searchedWords.Remove(word);
            newDefs.Remove(word);
            synonyms.Remove(word);
            rhymes.Remove(word);

            colorizeRtbText();
        }
    }
}
