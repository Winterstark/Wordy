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

        WordnikService wordnik;
        BackgroundWorker searchWordWorker;
        delegate void SetTextCallback(string text);
        Dictionary<string, Definition> newDefs;
        Dictionary<string, string> synonyms, rhymes;
        Graphics gfx;
        List<string> corewords, searchedWords, notFoundWords;
        string[] newWords;
        string activeSearchWord;
        float lineH;
        int rtbTextVScroll;
        bool colorizing;


        void loadNewwords()
        {
            string list = Misc.LoadNewWordsFromFile(main.prefs.NewWordsPath);
            newWords = list.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
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
                checkIfTextContainsWord(txt, word.ToString(), knownWords, Color.Orange);

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
                checkIfTextContainsWord(txt, word, knownWords, Color.Yellow);
            
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
            rtbText.Select(selStart, selLen);

            colorizing = false;
        }

        void resizeRtbDef()
        {
            rtbDef.Height = (int)(gfx.MeasureString(rtbDef.Text, rtbDef.Font, 233).Height + lineH);
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

        void setPositionNextToSelectedText(Control control)
        {
            //move control next to selected text
            int ub = rtbText.SelectionStart;
            int lb = rtbText.Text.LastIndexOf("\n", ub) + 1;

            string prevText = rtbText.Text.Substring(0, lb);
            string currLine = "";
            if (lb < ub)
                currLine = rtbText.Text.Substring(lb, ub - lb);

            //calculate Top
            string[] lines = prevText.Split('\n');
            float top = lines.Length * lineH;

            foreach (string line in lines)
                if (gfx.MeasureString(line, rtbText.Font, rtbText.ClientSize.Width).Height > lineH)
                    top += gfx.MeasureString(line, rtbText.Font, rtbText.ClientSize.Width).Height - lineH; //check if line is displayed in more than one row

            top -= getVScroll(rtbText); //adjust for vertical scroll of control
            top += 1.5f * lineH; //don't obscure the selected text

            if (top > rtbText.Top + rtbText.ClientSize.Height - control.Height - 8 && control.Height < rtbText.Height) //if there is not enough space below the selected word (&& control isn't unreasonably big)
                top -= 2 * lineH + control.Height; //display the definitions above the selected word

            control.Top = (int)top;

            //calculate Left
            int left = (int)gfx.MeasureString(currLine, rtbText.Font).Width;
            left += (int)(gfx.MeasureString(rtbText.SelectedText, rtbText.Font).Width * 0.75f); //right-align, more or less

            left = Math.Min(left, rtbText.Left + rtbText.ClientSize.Width - control.Width - 8);
            left = Math.Max(left, 8);

            control.Left = left;
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
                word = word.ToLower();

                StreamWriter file = new StreamWriter(main.prefs.NewWordsPath);
                foreach (string newWord in newWords)
                    if (newWord.ToLower() != word)
                        file.WriteLine(newWord);
                file.Close();

                loadNewwords();
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
            lineH = gfx.MeasureString("A", rtbDef.Font).Height;

            activeSearchWord = "";
            rtbTextVScroll = 0;
            colorizing = false;

            //prepare worker
            searchWordWorker = new BackgroundWorker();
            searchWordWorker.DoWork += new DoWorkEventHandler(searchWordWorker_DoWork);
            searchWordWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(searchWordWorker_RunWorkerCompleted);
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
                rtbDef.Visible = false;
                buttAdd.Visible = false;
                buttSearch.Visible = false;
                buttSave.Visible = false;
                buttUpdateDefinition.Visible = false;
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
                    setPositionNextToSelectedText(rtbDef);

                    rtbDef.Visible = true;
                    buttAdd.Visible = false;
                    buttSearch.Visible = false;
                    buttSave.Visible = false;
                    buttUpdateDefinition.Visible = false;
                }
                else if (newWords.Contains(selection, StringComparer.OrdinalIgnoreCase))
                {
                    //word-to-be-added
                    setPositionNextToSelectedText(buttSearch);

                    rtbDef.Visible = false;
                    buttAdd.Visible = false;
                    buttSearch.Visible = true;
                    buttSave.Visible = false;
                    buttUpdateDefinition.Visible = false;
                }
                else if (searchedWords.Contains(selection, StringComparer.OrdinalIgnoreCase))
                {
                    //searched word
                    Misc.DisplayDefs(rtbDef, newDefs[selection].ToString(), corewords);
                    resizeRtbDef();
                    rtbDef.Enabled = true;
                    setPositionNextToSelectedText(rtbDef);

                    buttSave.Top = rtbDef.Top + rtbDef.Height - buttUpdateDefinition.Height;
                    buttSave.Left = rtbDef.Left + rtbDef.Width + 6;

                    rtbDef.Visible = true;
                    buttAdd.Visible = false;
                    buttSearch.Visible = false;
                    buttSave.Visible = true;
                    buttUpdateDefinition.Visible = false;
                }
                else if (notFoundWords.Contains(selection, StringComparer.OrdinalIgnoreCase))
                {
                    //nothing found for this word
                    rtbDef.Visible = false;
                    buttAdd.Visible = false;
                    buttSearch.Visible = false;
                    buttSave.Visible = false;
                    buttUpdateDefinition.Visible = false;
                }
                else
                {
                    //unknown word
                    setPositionNextToSelectedText(buttAdd);
                    buttSearch.Top = buttAdd.Top;
                    buttSearch.Left = buttAdd.Left + 38;

                    rtbDef.Visible = false;
                    buttAdd.Visible = true;
                    buttSearch.Visible = true;
                    buttSave.Visible = false;
                    buttUpdateDefinition.Visible = false;
                }
            }
            else
            {
                rtbDef.Visible = false;
                buttAdd.Visible = false;
                buttSearch.Visible = false;
                buttSave.Visible = false;
                buttUpdateDefinition.Visible = false;
            }
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

        private void buttAdd_Click(object sender, EventArgs e)
        {
            StreamWriter file = new StreamWriter(main.prefs.NewWordsPath, true);
            file.WriteLine(getSelection());
            file.Close();

            loadNewwords();
            colorizeRtbText();
        }

        private void buttSearch_Click(object sender, EventArgs e)
        {
            activeSearchWord = getSelection();
            rtbDef.Text = "Looking up word. Please wait...";
            setPositionNextToSelectedText(rtbDef);

            searchWordWorker.RunWorkerAsync(activeSearchWord);

            rtbDef.Visible = true;
            buttAdd.Visible = false;
            buttSearch.Visible = false;
        }

        private void rtbDef_TextChanged(object sender, EventArgs e)
        {
            if (newDefs.ContainsKey(getSelection()))
            {
                if (rtbDef.Text != newDefs[getSelection()].ToString().Replace("\r\n", "\n"))
                {
                    buttUpdateDefinition.Left = buttSave.Left;
                    buttUpdateDefinition.Top = buttSave.Top - 6 - buttUpdateDefinition.Height;
                    buttUpdateDefinition.Visible = true;
                }
                else
                    buttUpdateDefinition.Visible = false;
            }
        }

        private void buttUpdateDefinition_Click(object sender, EventArgs e)
        {
            newDefs[getSelection()].Parse(rtbDef.Text, false);
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
