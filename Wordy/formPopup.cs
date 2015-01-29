using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Wordy
{
    public partial class formPopup : Form
    {
        public formMain main;
        public formReading reading;


        public void SetPositionNextToPointer(params Control[] controls)
        {
            //position form
            if (reading.UsePrevPopupPos)
                this.Location = reading.PopupPos;
            else
            {
                Point pos = reading.DblClickPos;
                pos.Offset(0, (int)reading.LineH);

                this.Location = reading.PopupPos = pos;
            }

            //resize rtbDef & window
            if (controls.Contains(rtbDef))
            {
                resizeRtbDef();
                this.Height = rtbDef.Height + 6 + buttAdd.Height;

                if (this.Top + this.Height > Screen.PrimaryScreen.Bounds.Height)
                    this.Top = Math.Max(0, reading.PopupPos.Y - (int)reading.LineH - this.Height);

                if (this.Left + this.Width > Screen.PrimaryScreen.Bounds.Width)
                {
                    int totalW = 0;
                    foreach (Control control in controls)
                        totalW += control.Width; //sum of controls' width
                    if (controls.Length > 1)
                        totalW += 6 * (controls.Length - 1); //sum of width of gaps between controls

                    this.Left = Math.Max(0, Screen.PrimaryScreen.Bounds.Width - totalW);
                }
            }

            //position controls
            if (controls.Length > 0)
            {
                controls[0].Location = new Point(0, 0);

                if (controls.Length > 1)
                    for (int i = 1; i < controls.Length; i++)
                    {
                        controls[i].Left = controls[i - 1].Left + controls[i - 1].Width + 6;
                        controls[i].Top = controls[i - 1].Top + controls[i - 1].Height - controls[i].Height;
                    }
            }

            //set Visible values
            HideAllControlsExcept(controls);
        }

        public void HideAllControlsExcept(params Control[] exceptions)
        {
            rtbDef.Visible = exceptions.Contains(rtbDef);
            
            buttAdd.Visible = exceptions.Contains(buttAdd);
            buttSearch.Visible = exceptions.Contains(buttSearch);
            buttGoogle.Visible = exceptions.Contains(buttGoogle);
            buttSave.Visible = exceptions.Contains(buttSave);
            buttUpdateDefinition.Visible = exceptions.Contains(buttUpdateDefinition);
            
            //this.Visible = exceptions.Length > 0;
        }

        void resizeRtbDef()
        {
            SizeF size = reading.Gfx.MeasureString(rtbDef.Text, rtbDef.Font, 233, reading.MeasuringStringFormat);

            rtbDef.Width = (int)(1.5f * size.Width);
            rtbDef.Height = Math.Min(400, (int)(size.Height + reading.LineH));

            //if (rtbDef.Height == 400)
            //    rtbDef.ScrollBars = RichTextBoxScrollBars.Vertical;
            //else
            //    rtbDef.ScrollBars = RichTextBoxScrollBars.None;
        }


        public formPopup()
        {
            InitializeComponent();
        }

        private void formPopup_Load(object sender, EventArgs e)
        {
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
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        private void rtbDef_Click(object sender, EventArgs e)
        {
            reading.timerPauseBeforeHiding.Enabled = false;

            if (rtbDef.Text == "Enter new definition...")
                rtbDef.SelectAll();
        }

        private void rtbDef_TextChanged(object sender, EventArgs e)
        {
            if (reading.newDefs.ContainsKey(reading.Selection))
            {
                if (rtbDef.Text != reading.newDefs[reading.Selection].ToString().Replace("\r\n", "\n"))
                {
                    buttUpdateDefinition.Left = buttSave.Left + buttSave.Width + 6;
                    buttUpdateDefinition.Top = buttSave.Top;

                    buttUpdateDefinition.Visible = true;
                }
                else
                    buttUpdateDefinition.Visible = false;
            }
        }

        private void rtbDef_MouseDown(object sender, MouseEventArgs e)
        {
            reading.MouseDownOnRtbDef = true;
        }

        private void rtbDef_VScroll(object sender, EventArgs e)
        {
            reading.timerPauseBeforeHiding.Enabled = false;
        }

        private void buttAdd_Click(object sender, EventArgs e)
        {
            reading.timerPauseBeforeHiding.Enabled = false;
            reading.newWords.Add(reading.Selection);

            StreamWriter file = new StreamWriter(main.GetNewWordsPath());
            for (int i = 0; i < reading.newWords.Count; i++)
                file.Write(reading.newWords[i] + (i < reading.newWords.Count - 1 ? Environment.NewLine : ""));
            file.Close();

            reading.ColorizeRtbText();
            SetPositionNextToPointer(buttSearch);
        }

        private void buttSearch_Click(object sender, EventArgs e)
        {
            reading.ActiveSearchWord = reading.Selection.ToLower();

            rtbDef.Text = "Looking up word. Please wait...";
            resizeRtbDef();
            rtbDef.Location = reading.PopupPos;

            if (main.Profile == "English")
                reading.SearchWordWorker.RunWorkerAsync(reading.ActiveSearchWord);
            else
                reading.Translator.Translate(reading.ActiveSearchWord);

            rtbDef.Visible = true;
            buttAdd.Visible = false;
            buttSearch.Visible = false;
        }

        private void buttGoogle_Click(object sender, EventArgs e)
        {
            reading.timerPauseBeforeHiding.Enabled = false;
            reading.ActiveGooglingWord = reading.Selection;

            if (main.Profile != "English")
                Process.Start("https://translate.google.com/#" + main.Languages[main.Profile] + "/en/" + reading.Selection);
            else
                Process.Start("https://www.google.com/search?q=" + reading.Selection);

            rtbDef.ReadOnly = false;
            rtbDef.Text = "Enter new definition...";

            SetPositionNextToPointer(rtbDef, buttUpdateDefinition);
        }

        private void buttUpdateDefinition_Click(object sender, EventArgs e)
        {
            reading.timerPauseBeforeHiding.Enabled = false;

            string word = reading.Selection;

            if (reading.newDefs.ContainsKey(word))
                reading.newDefs[word].Parse(rtbDef.Text, false);
            else if (rtbDef.Text != "Enter new definition...")
            {
                //user adding definition found via Google
                reading.notFoundWords.Remove(reading.notFoundWords.Find(w => w.ToLower() == word.ToLower()));

                reading.searchedWords.Add(word);
                reading.newDefs.Add(word, new Definition(rtbDef.Text, true));
                reading.synonyms.Add(word, "");
                reading.rhymes.Add(word, "");

                reading.ColorizeRtbText();
            }

            reading.ActiveGooglingWord = "";
            buttUpdateDefinition.Visible = false;
        }

        private void buttSave_Click(object sender, EventArgs e)
        {
            reading.timerPauseBeforeHiding.Enabled = false;

            string word = reading.Selection;
            main.AddNewWord(word, reading.newDefs[word], reading.synonyms[word], reading.rhymes[word]);

            //remove word from lists
            reading.searchedWords.Remove(reading.searchedWords.Find(w => w.ToLower() == word.ToLower()));
            reading.newDefs.Remove(word);
            reading.synonyms.Remove(word);
            reading.rhymes.Remove(word);

            reading.ColorizeRtbText();
            buttSave.Visible = false;
        }
    }
}
