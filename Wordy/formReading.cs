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
        #region DLLImports
        [DllImport("user32.dll")]
        static extern uint GetDoubleClickTime();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, Int32 wParam, ref Point lParam);

        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        private const int WM_USER = 0x400;
        private const int EM_GETSCROLLPOS = WM_USER + 221;
        private const int EM_SETSCROLLPOS = WM_USER + 222;
        private const int SB_VERT = 0x1;
        #endregion


        const Keys ALT_KEY = Keys.MButton | Keys.Space | Keys.F17; //this is apparently the key combination for Alt (using UserActivityHook)

        public formMain main;
        public Translator Translator;
        public BackgroundWorker SearchWordWorker;
        public List<Entry> words;
        public Dictionary<string, Definition> newDefs;
        public Dictionary<string, string> synonyms, rhymes;
        public List<string> corewords, newWords, searchedWords, notFoundWords;
        public Graphics Gfx;
        public StringFormat MeasuringStringFormat;
        public Point PopupPos, DblClickPos;
        public string ActiveSearchWord, ActiveGooglingWord, Selection;
        public float LineH;
        public bool UsePrevPopupPos, MouseDownOnRtbDef;

        formPopup popup;
        UserActivityHook actHook;
        WordnikService wordnik;
        delegate void SetTextCallback(string text);
        DateTime prevClick;
        Rectangle dblClickArea;
        int rtbTextVScroll;
        bool initialized, colorizing, dblClick, altKey;


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

        public void ColorizeRtbText()
        {
            if (rtbText.Text == "" || rtbText.Text.Contains("Paste your text here..."))
                return;

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
            if (ActiveSearchWord != "")
                checkIfTextContainsWord(txt, ActiveSearchWord, knownWords, Color.DarkBlue);

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
            
            setVSCroll(popup.rtbDef, prevScroll);

            UsePrevPopupPos = true;
            rtbText.Select(selStart, selLen);

            UsePrevPopupPos = false;
            colorizing = false;
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

        void updateStatus(string status)
        {
            if (popup.rtbDef.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(updateStatus);
                this.Invoke(d, new object[] { status });
            }
            else
                popup.rtbDef.Text = status;
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

                    popup.SetPositionNextToPointer(popup.buttGoogle);
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
                showWordDefinition();
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

            ActiveSearchWord = "";
            ColorizeRtbText();
        }

        void doneEvent(string word, string translation)
        {
            if (translation == "error" || translation == "" || word.ToLower() == translation.ToLower())
            {
                popup.rtbDef.Text = "NOT FOUND!";
                notFoundWords.Add(word);

                popup.SetPositionNextToPointer(popup.buttGoogle);
            }
            else
            {
                //save word data
                newDefs.Add(word, new Definition(translation.ToLower(), true));
                synonyms.Add(word, "");
                rhymes.Add(word, "");

                searchedWords.Add(word);
                showWordDefinition();
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

            ActiveSearchWord = "";
            ColorizeRtbText();
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

        void showWordDefinition()
        {
            Misc.DisplayDefs(popup.rtbDef, newDefs[Selection].ToString(), corewords);

            popup.rtbDef.ReadOnly = false;
            popup.SetPositionNextToPointer(popup.rtbDef, popup.buttSave);
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

            ActiveSearchWord = "";
            ActiveGooglingWord = "";
            rtbTextVScroll = 0;
            colorizing = false;
            UsePrevPopupPos = false;

            Gfx = this.CreateGraphics();
            MeasuringStringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
            LineH = Gfx.MeasureString("A", rtbText.Font, int.MaxValue, MeasuringStringFormat).Height;

            //prepare popup window
            popup = new formPopup();

            popup.main = main;
            popup.reading = this;
            popup.Left = -1000; //hide popup
            popup.Show();

            //prepare language tools
            if (main.Profile == "English")
            {
                SearchWordWorker = new BackgroundWorker();
                SearchWordWorker.DoWork += new DoWorkEventHandler(searchWordWorker_DoWork);
                SearchWordWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(searchWordWorker_RunWorkerCompleted);
            }
            else
                Translator = new Translator(main.Languages[main.Profile], "en", doneEvent, main.prefs);
        }

        private void formReading_Activated(object sender, EventArgs e)
        {
            if (!initialized)
            {
                actHook = new UserActivityHook();
                actHook.OnMouseActivity += new MouseEventHandler(ActivityHook_OnMouseActivity);
                actHook.KeyDown += new KeyEventHandler(ActivityHook_KeyDown);
                actHook.KeyUp += new KeyEventHandler(ActivityHook_KeyUp);

                initialized = true;
            }
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

            try
            {
                actHook.Stop();
            }
            catch
            {
            }

            popup.Dispose();
            main.Show();
        }

        void ActivityHook_OnMouseActivity(object sender, MouseEventArgs e)
        {
            //after the user doubleclicks (using LMB) and then depresses the button, get the selected text of the front-most application (via the clipboard)
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.Delta != Misc.MOUSE_UP_CODE)
                {
                    if (!dblClick)
                    {
                        if ((DateTime.Now - prevClick).TotalMilliseconds <= SystemInformation.DoubleClickTime && dblClickArea.Contains(Cursor.Position))
                            dblClick = true;
                    }
                }
                else
                {
                    if (dblClick)
                    {
                        DblClickPos = Cursor.Position;
                        timerWaitUntilCopy.Enabled = true;

                        dblClick = false;
                    }
                    else
                    {
                        if (!MouseDownOnRtbDef)
                        {
                            timerPauseBeforeHiding.Enabled = true;

                            prevClick = DateTime.Now;
                            dblClickArea = new Rectangle(Cursor.Position.X - (SystemInformation.DoubleClickSize.Width / 2), Cursor.Position.Y - (SystemInformation.DoubleClickSize.Height / 2), SystemInformation.DoubleClickSize.Width, SystemInformation.DoubleClickSize.Height);
                        }
                        else
                            MouseDownOnRtbDef = false;
                    }
                }
            }
        }

        void ActivityHook_KeyDown(object sender, KeyEventArgs e)
        {
            bool handled = true;

            if (altKey && e.KeyCode == Keys.W)
            {
                DblClickPos = Cursor.Position;
                timerWaitUntilCopy.Enabled = true;
            }
            else if (e.KeyCode == ALT_KEY)
                altKey = true;
            else if (popup.buttAdd.Visible && altKey && e.KeyCode == Keys.A)
                popup.buttAdd.PerformClick();
            else if (popup.buttSearch.Visible && altKey && e.KeyCode == Keys.F)
                popup.buttSearch.PerformClick();
            else if (popup.buttGoogle.Visible && altKey && e.KeyCode == Keys.G)
                popup.buttGoogle.PerformClick();
            else if (popup.buttUpdateDefinition.Visible && altKey && e.KeyCode == Keys.D)
                popup.buttUpdateDefinition.PerformClick();
            else if (popup.buttSave.Visible && altKey && e.KeyCode == Keys.S)
                popup.buttSave.PerformClick();
            else
                handled = false;

            e.Handled = handled;
        }

        void ActivityHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == ALT_KEY)
                altKey = false;
        }

        private void rtbText_Enter(object sender, EventArgs e)
        {
            if (rtbText.Text.Contains("Paste your text here..."))
                rtbText.SelectAll();
        }

        private void rtbText_Click(object sender, EventArgs e)
        {
            if (rtbText.Text.Contains("Paste your text here..."))
                rtbText.SelectAll();
        }

        private void rtbText_TextChanged(object sender, EventArgs e)
        {
            if (!colorizing)
                ColorizeRtbText();
        }

        private void rtbText_VScroll(object sender, EventArgs e)
        {
            int currVScroll = getVScroll(rtbText);

            //move the popup UI accordingly
            int vertChange = rtbTextVScroll - currVScroll;

            if (popup.rtbDef.Visible)
                popup.rtbDef.Top += vertChange;
            if (popup.buttAdd.Visible)
                popup.buttAdd.Top += vertChange;
            if (popup.buttSearch.Visible)
                popup.buttSearch.Top += vertChange;
            if (popup.buttSave.Visible)
                popup.buttSave.Top += vertChange;
            if (popup.buttUpdateDefinition.Visible)
                popup.buttUpdateDefinition.Top += vertChange;

            rtbTextVScroll = currVScroll;
        }

        private void timerWaitUntilCopy_Tick(object sender, EventArgs e)
        {
            timerWaitUntilCopy.Enabled = false;

            SendKeys.Send("^c");
            timerGetClipboard.Enabled = true;
        }

        private void timerGetClipboard_Tick(object sender, EventArgs e)
        {
            timerGetClipboard.Enabled = false;

            //get selected text
            Selection = Clipboard.GetText();

            //remove extra spaces
            while (Selection.Length > 0 && Selection[0] == ' ')
                Selection = Selection.Substring(1);

            while (Selection.Length > 0 && Selection[Selection.Length - 1] == ' ')
                Selection = Selection.Substring(0, Selection.Length - 1);
            
            //show popup based on selection
            if (Selection != "")
            {
                string selectionLCase = Selection.ToLower();
                Entry selectedWord = words.Find(w => w.ToString().ToLower() == selectionLCase);

                if (selectedWord != null)
                {
                    //archived word
                    Misc.DisplayDefs(popup.rtbDef, selectedWord.GetDefinition(), corewords);
                    popup.rtbDef.ReadOnly = true;
                    popup.SetPositionNextToPointer(popup.rtbDef);
                }
                else if (Selection.ToLower() == ActiveGooglingWord.ToLower())
                    popup.SetPositionNextToPointer(popup.rtbDef, popup.buttSave, popup.buttUpdateDefinition); //word is waiting for new definition
                else if (newWords.Contains(Selection, StringComparer.OrdinalIgnoreCase))
                    popup.SetPositionNextToPointer(popup.buttSearch); //word-to-be-added
                else if (searchedWords.Contains(Selection, StringComparer.OrdinalIgnoreCase))
                    showWordDefinition(); //searched word
                else if (notFoundWords.Contains(Selection, StringComparer.OrdinalIgnoreCase))
                    popup.SetPositionNextToPointer(popup.buttGoogle); //nothing found for this word
                else
                    popup.SetPositionNextToPointer(popup.buttAdd, popup.buttSearch); //unknown word
            }
            else
                popup.HideAllControlsExcept();
        }

        private void timerPauseBeforeHiding_Tick(object sender, EventArgs e)
        {
            timerPauseBeforeHiding.Enabled = false;
            popup.Left = -1000; //hide popup
        }
    }
}
