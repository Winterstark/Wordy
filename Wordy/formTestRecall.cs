using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NikSharp;
using NikSharp.Model;
using System.Diagnostics;

namespace Wordy
{
    public partial class formTestRecall : Form
    {
        public formMain main;
        public List<Entry> words;
        public bool testUnlearned;

        string def;
        float lineHeight, curX, endX, deltaX;
        int nTests, nCorrectAnswers, correctPick, resetLearningPhase;

        Entry testWord;
        Random rand;
        List<MaskedTextBox> mtbDefs;
        List<List<Tuple<int, int>>> kwBounds;
        List<Tuple<int, int, bool, string>> answers;
        List<string> corewords = new List<string>();
        string[] exampleSentences;
        Answer[] answCorrectly;
        WordnikService wordnik;


        void setupUI()
        {
            panelDef.Size = new Size(this.Width - 50, this.Height - 120);
            panelTestWord.Size = new Size(this.Width - 50, this.Height - 120);

            lblDef.MaximumSize = new Size(this.Width - 60, this.Height - 120);
            lblSynonyms.MaximumSize = new Size(this.Width - 60, lblSynonyms.Height);
            picVisual.Size = new Size(this.Width - 60, this.Height - 120);

            lblTestWordDef.MaximumSize = new Size(this.Width - 60, this.Height - 120);
            flowpanelPickAnswers.Size = new Size(this.Width - 176, flowpanelPickAnswers.Height);
            flowpanelPickAnswers.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;
            textTestWord.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;
            mtbTestWord.Width = this.Width - 60;
            textTestWord.Width = this.Width - 60;

            rtbDef.Size = new Size(this.Width - 60, this.Height - 120);

            chklistDefs.Size = new Size(this.Width - 80, this.Height - 120);
            chklistDefs.Height = 22 * chklistDefs.Items.Count;

            if (mtbDefs != null && mtbDefs.Count > 0)
            {
                foreach (MaskedTextBox mtbDef in mtbDefs)
                    mtbDef.Width = this.Width - 60;

                buttFinished.Top = mtbDefs[mtbDefs.Count - 1].Top + mtbDefs[mtbDefs.Count - 1].Height + 16;
                buttSkip.Top = buttFinished.Top;
            }

            picRight.Left = lblSynonyms.Left + (this.Width - 70) / 2 - picRight.Width / 2;
            picWrong.Left = picRight.Left;
            picRight.Top = lblDef.Top + lblDef.Height + 12;
            picWrong.Top = picRight.Top;

            buttNext.Left = picWrong.Left + picWrong.Width / 2 - buttNext.Width / 2;
            buttNext.Top = picWrong.Top + picWrong.Height + 12;

            picWordnik.Left = this.Width - picWordnik.Width - 12;
            picWordnik.Top = this.Height - picWordnik.Height - 48;

            panelDef.Refresh();
        }

        void randomizeAnswers(List<string> answers)
        {
            while (answers.Count < 5) //add random words to complete set
                answers.AddRange(main.GetRandWords(5 - answers.Count, testWord.ToString()));

            buttPickWord1.Text = answers[0];
            buttPickWord2.Text = answers[1];
            buttPickWord3.Text = answers[2];
            buttPickWord4.Text = answers[3];
            buttPickWord5.Text = answers[4];

            correctPick = rand.Next(6);
            if (correctPick != 5)
                buttPickWord6.Text = answers[correctPick];

            switch (correctPick)
            {
                case 0:
                    buttPickWord1.Text = testWord.ToString();
                    break;
                case 1:
                    buttPickWord2.Text = testWord.ToString();
                    break;
                case 2:
                    buttPickWord3.Text = testWord.ToString();
                    break;
                case 3:
                    buttPickWord4.Text = testWord.ToString();
                    break;
                case 4:
                    buttPickWord5.Text = testWord.ToString();
                    break;
                case 5:
                    buttPickWord6.Text = testWord.ToString();
                    break;
            }

            buttPickWord1.Text = "1. " + buttPickWord1.Text;
            buttPickWord2.Text = "2. " + buttPickWord2.Text;
            buttPickWord3.Text = "3. " + buttPickWord3.Text;
            buttPickWord4.Text = "4. " + buttPickWord4.Text;
            buttPickWord5.Text = "5. " + buttPickWord5.Text;
            buttPickWord6.Text = "6. " + buttPickWord6.Text;
        }

        void nextWord()
        {
            //reset UI
            panelDef.Visible = false;
            picRight.Visible = false;
            picWrong.Visible = false;
            picVisual.Visible = false;
            buttNext.Visible = false;
            lblVisualTrigger.Visible = false;
            lblDef.Visible = false;
            rtbDef.Visible = false;
            chklistDefs.Visible = false;
            buttAnotherExample.Visible = false;
            picWordnik.Enabled = false;

            lblWord.ForeColor = Color.Black;
            answCorrectly = null;

            //get/refresh word list (some words might've only just become available for testing)
            int prevWordCount = 0;
            if (words != null)
                prevWordCount = words.Count;

            words = main.GetWordsReadyForTesting(!testUnlearned);

            if (words.Count != prevWordCount) //if the wordcount changed...
                nTests += words.Count - prevWordCount; //...update nTests value accordingly

            //load word
            if (words.Count > 0)
            {
                testWord = words[rand.Next(words.Count)];
                words.Remove(testWord);

                lblWord.Text = testWord.ToString();
                lblSynonyms.Text = testWord.GetSynonyms();
                lblDef.Text = testWord.GetUnlearnedDefs();
                def = lblDef.Text;

                //load visual
                if (File.Exists("visuals\\" + testWord + ".jpg"))
                {
                    picVisual.ImageLocation = "visuals\\" + testWord + ".jpg";
                    lblVisualTrigger.Left = lblWord.Width + 18;
                }
                else
                    picVisual.ImageLocation = "";

                //prepare UI
                if (panelDef.Visible)
                    panelDef.Refresh();

                if (!testWord.archived)
                {
                    switch (testWord.learningPhase)
                    {
                        case 1: //multiple choice - random words
                            lblTestWordDef.Text = "Which word is defined by the following:" + Environment.NewLine + Environment.NewLine + removeWordInstances(def, testWord.ToString());
                            flowpanelPickAnswers.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;

                            randomizeAnswers(main.GetRandWords(5, testWord.ToString()));
                            break;
                        case 2: //multiple choice - rhymes
                            lblTestWordDef.Text = "Which word is defined by the following:" + Environment.NewLine + Environment.NewLine + removeWordInstances(def, testWord.ToString());

                            randomizeAnswers(testWord.GetRandomRhymes());
                            break;
                        case 3: //type word
                            lblTestWordDef.Text = "Type the word that is defined by the following:" + Environment.NewLine + Environment.NewLine + removeWordInstances(def, testWord.ToString());
                            textTestWord.Text = "";
                            break;
                        case 4: //type 1 keyword per def.
                            setupDefFillIns(testWord.learningPhase);
                            break;
                        case 5: //type half keywords per def.
                            setupDefFillIns(testWord.learningPhase);
                            break;
                        case 6: //type all keywords
                            setupDefFillIns(testWord.learningPhase);
                            break;
                    }

                    if (testWord.learningPhase <= 3)
                    {
                        panelTestWord.Visible = true;

                        buttFinished.Visible = false;
                        buttSkip.Visible = false;

                        if (testWord.learningPhase <= 2)
                        {
                            flowpanelPickAnswers.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;
                            flowpanelPickAnswers.Visible = true;
                            textTestWord.Visible = false;
                        }
                        else
                        {
                            flowpanelPickAnswers.Visible = false;
                            textTestWord.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;
                            textTestWord.Visible = true;
                            textTestWord.Focus();
                        }
                    }
                    else
                    {
                        panelDef.Visible = true;
                        buttFinished.Visible = true;
                        buttSkip.Visible = true;

                        lblSynonyms.Text = "Fill in the blanks of the following definitions:";

                        buttFinished.Top = mtbDefs[mtbDefs.Count - 1].Top + mtbDefs[mtbDefs.Count - 1].Height + 16;
                        buttSkip.Top = buttFinished.Top;

                        if (mtbDefs != null)
                            mtbDefs[0].Focus();
                    }
                }
                else
                {
                    int questionType = -1;
                    
                    while (questionType == -1)
                    {
                        int percent = rand.Next(100);
                        
                        if (percent <= 25)
                            questionType = 0;
                        else if (percent <= 40)
                            questionType = 1;
                        else if (percent <= 50)
                            questionType = 2;
                        else if (percent <= 60 && testWord.GetSynonyms() != "")
                            questionType = 3;
                        else if (percent <= 85)
                        {
                            try
                            {
                                if (wordnik == null)
                                    wordnik = new WordnikService("b3bbd1f9103a01de7d00a0fd1300164c17bfcec03eb86a678");
                                var exampleResults = wordnik.GetExamples(testWord.ToString());

                                if (exampleResults.Examples == null)
                                    continue;

                                var examples = exampleResults.Examples.ToArray();
                                exampleSentences = new string[examples.Length];

                                for (int i = 0; i < examples.Length; i++)
                                    exampleSentences[i] = removeWordInstances(examples[i].Text, testWord.ToString());
                                
                                lblDef.Text = exampleSentences[rand.Next(examples.Length)];
                                questionType = 4;
                            }
                            catch
                            {
                                questionType = -1;
                            }
                        }
                        else if (percent <= 90 && testWord.GetSynonyms() != "" && testWord.ToString().Length > 1)
                            questionType = 5;
                        else if (percent <= 100 && picVisual.ImageLocation != "")
                            questionType = 6;
                        else
                            questionType = -1;
                    }

                    string mask;
                    int offset;

                    mtbTestWord.Tag = false; //indicates mtbTestWord is NOT used to read the answer
                    
                    switch (questionType)
                    {
                        case 0: // type word
                            lblTestWordDef.Text = "Type the word that is defined by the following:" + Environment.NewLine + Environment.NewLine + removeWordInstances(def, testWord.ToString());
                            textTestWord.Text = "";

                            textTestWord.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;

                            textTestWord.Visible = true;
                            panelTestWord.Visible = true;

                            textTestWord.Focus();

                            resetLearningPhase = 3;
                            break;
                        case 1: // type 1 keywords per def.
                            setupDefFillIns(4);
                            lblSynonyms.Text = "Fill in the blanks of the following definitions:";

                            buttFinished.Top = mtbDefs[mtbDefs.Count - 1].Top + mtbDefs[mtbDefs.Count - 1].Height + 16;
                            buttSkip.Top = buttFinished.Top;
                            
                            buttFinished.Visible = true;
                            buttSkip.Visible = true;
                            panelDef.Visible = true;

                            if (mtbDefs != null)
                                mtbDefs[0].Focus();

                            resetLearningPhase = 4;
                            break;
                        case 2: // select correct definitions
                            lblSynonyms.Text = "Select only the correct definitions for this word.";

                            List<string> defs = new List<string>();
                            defs.AddRange(getLineDefs());
                            for (int i = rand.Next((int)(defs.Count * 0.75)) - 1; i >= 0; i--)
                                defs.RemoveAt(rand.Next(defs.Count));

                            List<string> wrongDefs = main.GetRandDefs(1 + rand.Next(5), testWord.ToString());
                            while (wrongDefs.Count > 0)
                            {
                                int next = rand.Next(wrongDefs.Count);

                                defs.Insert(rand.Next(defs.Count + 1), wrongDefs[next]);
                                wrongDefs.RemoveAt(next);
                            }

                            chklistDefs.Items.Clear();
                            int ind = 1;

                            foreach (string d in defs)
                                chklistDefs.Items.Add(ind++ + ".  " + d);

                            chklistDefs.Height = 22 * chklistDefs.Items.Count;
                            buttFinished.Top = chklistDefs.Top + chklistDefs.Height + 16;

                            chklistDefs.Visible = true;
                            panelDef.Visible = true;
                            buttFinished.Visible = true;

                            chklistDefs.Focus();

                            resetLearningPhase = 1;
                            break;
                        case 3: // recognize synonyms
                            lblDef.Text = "What word has these synonyms?";
                            mtbTestWord.Text = "";
                            mtbTestWord.Top = 17;
                                
                            //reveal every other letter
                            mask = createMask();
                            offset = 0;

                            for (int i = 2; i < testWord.ToString().Length; i += 2)
                            {
                                offset += (mask[i + offset - 2] == '\\' ? 1 : 0) + (mask[i + offset - 1] == '\\' ? 1 : 0);

                                mask = mask.Remove(i + offset, 1);
                                mask = mask.Insert(i + offset, convToLiterals(testWord.ToString().Substring(i, 1)));
                            }

                            mtbTestWord.Mask = mask;
                            
                            lblWord.Visible = false;
                            lblDef.Visible = true;
                            mtbTestWord.Visible = true;
                            panelDef.Visible = true;

                            mtbTestWord.Focus();

                            resetLearningPhase = 5;
                            break;
                        case 4: // type word for example sentence
                            lblSynonyms.Text = "What word completes this sentence:";
                            mtbTestWord.Text = "";
                            mtbTestWord.Top = 17;

                            buttAnotherExample.Top = lblDef.Top + lblDef.Height + 16;
                            buttAnotherExample.Visible = true;

                            //reveal first letter and vowels
                            mask = createMask();
                            offset = 0;
                            
                            for (int i = 1; i < testWord.ToString().Length; i++)
                            {
                                offset += mask[i + offset - 1] == '\\' ? 1 : 0;

                                if (testWord.ToString()[i] == 'a' || testWord.ToString()[i] == 'e' || testWord.ToString()[i] == 'i' || testWord.ToString()[i] == 'o' || testWord.ToString()[i] == 'u' || testWord.ToString()[i] == 'y')
                                {
                                    mask = mask.Remove(i + offset, 1);
                                    mask = mask.Insert(i + offset, convToLiterals(testWord.ToString().Substring(i, 1)));
                                }
                            }

                            mtbTestWord.Mask = mask;

                            lblWord.Visible = false;
                            lblDef.Visible = true;
                            mtbTestWord.Visible = true;
                            panelDef.Visible = true;

                            mtbTestWord.Focus();

                            resetLearningPhase = 2;
                            break;
                        case 5: // unscramble letters
                            lblWord.Text = "";
                            List<char> letters = testWord.ToString().ToList();

                            while (letters.Count > 0)
                            {
                                int next = rand.Next(letters.Count);
                                lblWord.Text += letters[next];
                                letters.RemoveAt(next);
                            }

                            if (lblWord.Text == testWord.ToString()) //extra check
                                lblWord.Text = lblWord.Text.Substring(1,1) + lblWord.Text.Substring(0,1) + (lblWord.Text.Length > 2 ? lblWord.Text.Substring(2) : "");

                            lblDef.Text = "Unscramble this word.";
                            mtbTestWord.Text = "";
                            mtbTestWord.Mask = "";
                            mtbTestWord.Top = lblDef.Top + lblDef.Height + 32;

                            lblDef.Visible = true;
                            mtbTestWord.Visible = true;
                            panelDef.Visible = true;

                            mtbTestWord.Focus();

                            resetLearningPhase = 6;
                            break;
                        case 6: // recognize visuals
                            lblSynonyms.Text = "What word is represented by this picture?";
                            lblDef.Text = "";
                            mtbTestWord.Text = "";
                            mtbTestWord.Mask = "";

                            lblVisualTrigger.Left = lblWord.Left;
                            mtbTestWord.Top = lblSynonyms.Top + lblSynonyms.Height + 32;

                            lblWord.Visible = false;
                            lblVisualTrigger.Visible = true;
                            lblSynonyms.Visible = true;
                            mtbTestWord.Visible = true;
                            panelDef.Visible = true;

                            mtbTestWord.Focus();
                            mtbTestWord.Tag = true; //indicates mtbTestWord is used to read the answer

                            resetLearningPhase = 3;
                            break;
                    }
                }
            }
            else
            {
                if (testUnlearned)
                    MessageBox.Show("No more unlearned words.");
                else
                    MessageBox.Show("No more learned words.");

                this.Close();
            }

            //display how many words remain to be tested and success rate so far
            if (testUnlearned)
                this.Text = "Study New Words (";
            else
                this.Text = "Test Learned Words (";

            this.Text += (nTests - words.Count) + " / " + nTests + ")";

            if (prevWordCount != 0) //display the success rate only if this isn't the first test
                this.Text += " - " + (int)(100.0f * nCorrectAnswers / (nTests - words.Count - 1)) + "%";
        }

        void finish()
        {
            if (mtbDefs != null)
                foreach (var mtbDef in mtbDefs)
                    if (!mtbDef.MaskCompleted)
                    {
                        if (MessageBox.Show("Skip question?", "You haven't filled out every blank.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                            return;
                        break;
                    }

            answer(checkAnswers());
        }

        void answer(bool success)
        {
            if (success)
                nCorrectAnswers++;

            if (timerProgressChange.Enabled || timerWait.Enabled || buttNext.Visible) //disable answering between questions
                return;
            
            panelTestWord.Visible = false;
            lblWord.Text = testWord.ToString();
            
            if (testWord.archived)
            {
                mtbTestWord.Visible = false;
                buttAnotherExample.Visible = false;
                lblWord.Visible = true;
            }

            string questionDef = lblDef.Text; //currently displayed definitions
            lblDef.Text = testWord.GetDefinition(); //display full list of definitions

            if (picVisual.ImageLocation != "")
            {
                lblVisualTrigger.Visible = true;
                lblVisualTrigger.Left = lblWord.Width + 18;
            }
            if (panelDef.Visible)
                lblSynonyms.Text = testWord.GetSynonyms();
            else
                panelDef.Visible = true;

            picWordnik.Enabled = true;

            picRight.Left = lblSynonyms.Left + (this.Width - 60) / 2 - picRight.Width / 2;
            picWrong.Left = picRight.Left;
            picRight.Top = lblDef.Top + lblDef.Height + 12;
            picWrong.Top = picRight.Top;

            buttNext.Left = picWrong.Left + picWrong.Width / 2 - buttNext.Width / 2;
            buttNext.Top = picWrong.Top + picWrong.Height + 12;

            curX = lblSynonyms.Left + 10 + (this.Width - 70 - lblSynonyms.Left - 10) / 6 * (testWord.learningPhase - 1);

            if (((!testWord.archived && testWord.learningPhase >= 4) || (testWord.archived && resetLearningPhase == 4)) && answers.Count > 0)
            {
                rtbDef.Text = "";
                rtbDef.AppendText(questionDef.Substring(0, answers[0].Item1));

                for (int i = 0; i < answers.Count - 1; i++)
                {
                    rtbDef.SelectionBackColor = answers[i].Item3 ? Color.Green : Color.Red;
                    rtbDef.AppendText(questionDef.Substring(answers[i].Item1, answers[i].Item2 - answers[i].Item1));
                    rtbDef.SelectionBackColor = SystemColors.Control;
                    rtbDef.AppendText(questionDef.Substring(answers[i].Item2, answers[i + 1].Item1 - answers[i].Item2));
                }

                rtbDef.SelectionBackColor = answers[answers.Count - 1].Item3 ? Color.Green : Color.Red;
                rtbDef.AppendText(questionDef.Substring(answers[answers.Count - 1].Item1, answers[answers.Count - 1].Item2 - answers[answers.Count - 1].Item1));
                rtbDef.SelectionBackColor = SystemColors.Control;
                rtbDef.AppendText(questionDef.Substring(answers[answers.Count - 1].Item2));

                //display any other definitions that may be hidden (such as definitions that weren't part of the test or definition sources)
                rtbDef.SelectionBackColor = SystemColors.Control;
                string[] defLines = lblDef.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                
                for (int i = 0; i < defLines.Length; i++)
                {
                    if (!rtbDef.Text.Contains(defLines[i]))
                    {
                        //find the index of the previous definition
                        int pos;

                        if (i == 0)
                            pos = 0; //insert def at the beginning
                        else if (rtbDef.Text.Contains(defLines[i - 1]))
                            pos = rtbDef.Text.IndexOf(defLines[i - 1]) + defLines[i - 1].Length + 1; //insert new def after the previous def
                        else
                            pos = rtbDef.Text.Length; //insert def at the end

                        //the new lines need to be inserted like this to preserve colors in the richtextbox
                        rtbDef.Select(pos, 0);
                        rtbDef.SelectedText = defLines[i] + Environment.NewLine;
                    }
                }

                rtbDef.Visible = true;
            }
            else
                lblDef.Visible = true;

            if (success)
            {
                picRight.Visible = true;

                if (!testWord.archived)
                {
                    if (testWord.learningPhase == 6)
                    {
                        if (testWord.GetRecallSuccessRate() == -1)
                            lblWord.Text += " learned!";
                        else
                            lblWord.Text += " relearned!";

                        lblWord.ForeColor = Color.Green;
                        lblVisualTrigger.Left = lblWord.Width + 18;
                    }

                    endX = curX + (this.Width - 70 - lblSynonyms.Left - 10) / 6;
                    deltaX = 3;
                    timerProgressChange.Enabled = true;
                }
                else
                {
                    drawProgress();
                    timerWait.Enabled = true;
                }
            }
            else
            {
                picWrong.Visible = true;

                if (!testWord.archived)
                {
                    buttNext.Visible = true;
                    drawProgress();
                }
                else
                {
                    //check if answer given is a synonym
                    string answerGiven;
                    if ((bool)mtbTestWord.Tag)
                        answerGiven = mtbTestWord.Text;
                    else
                        answerGiven = textTestWord.Text;

                    if (testWord.GetSynonyms().Split(new string[] { " / " }, StringSplitOptions.RemoveEmptyEntries).Contains(answerGiven))
                    {
                        picWrong.Visible = false;
                        picRight.Visible = true;
                        lblWord.ForeColor = Color.Yellow;
                        buttNext.Visible = true;
                        drawProgress();

                        success = true;

                        MessageBox.Show("Although it's technically not the right answer, it will be accepted.", "Your answer is a synonym of the correct answer.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblWord.Text += " forgotten!";
                        lblWord.ForeColor = Color.Red;
                        lblVisualTrigger.Left = lblWord.Width + 18;

                        endX = lblSynonyms.Left + 10 + (this.Width - 70 - lblSynonyms.Left - 10) / 6 * (resetLearningPhase - 1);
                        deltaX = -9;
                        timerProgressChange.Enabled = true;
                    }
                }
            }

            testWord.LogTest(success, answCorrectly);
            main.SaveWords();

            if (buttNext.Visible)
                buttNext.Focus();
        }

        bool checkAnswers()
        {
            buttFinished.Visible = false;
            buttSkip.Visible = false;
            buttAnotherExample.Visible = false;

            answers = new List<Tuple<int, int, bool, string>>();
            answCorrectly = new Answer[mtbDefs.Count];
            int i = 0;

            if (mtbDefs != null)
                while (mtbDefs.Count > 0)
                {
                    answCorrectly[i] = new Answer(mtbDefs[0].Tag.ToString());
                    answCorrectly[i].correct = true;

                    mtbDefs[0].TextMaskFormat = MaskFormat.IncludePromptAndLiterals;

                    for (int j = 0; j < kwBounds[0].Count; j++)
                    {
                        bool res = mtbDefs[0].Text.Length >= kwBounds[0][j].Item2 && mtbDefs[0].Text.Substring(kwBounds[0][j].Item1, kwBounds[0][j].Item2 - kwBounds[0][j].Item1).ToLower() == mtbDefs[0].Tag.ToString().Substring(kwBounds[0][j].Item1, kwBounds[0][j].Item2 - kwBounds[0][j].Item1).ToLower();
                        answCorrectly[i].correct = answCorrectly[i].correct && res;

                        answers.Add(new Tuple<int, int, bool, string>(lineOffset(i) + kwBounds[0][j].Item1, lineOffset(i) + kwBounds[0][j].Item2, res, mtbDefs[0].Text));
                    }

                    mtbDefs[0].Dispose();
                    panelDef.Controls.Remove(mtbDefs[0]);
                    mtbDefs.RemoveAt(0);
                    kwBounds.RemoveAt(0);

                    i++;
                }

            return answers.All(a => a.Item3);
        }

        string removeWordInstances(string txt, string word)
        {
            word = word.ToLower();

            int lb = 0, ub;
            while (!char.IsLetter(txt[lb]))
                lb++;

            while (true)
            {
                ub = lb + 1;

                while (ub < txt.Length && char.IsLetter(txt[ub]))
                    ub++;

                string seg = txt.Substring(lb, ub - lb).ToLower();
                if (!char.IsLetter(seg[0]))
                {
                    seg = seg.Substring(1);
                    lb++;
                }
                if (seg != "" && !char.IsLetter(seg[seg.Length - 1]))
                {
                    seg = seg.Substring(0, seg.Length - 1);
                    ub--;
                }

                if (seg != "" && seg.ToLower() == word)
                    txt = txt.Substring(0, lb) + "???" + txt.Substring(ub);

                if (ub >= txt.Length)
                    break;

                lb = ub + 1;
                if (lb == txt.Length)
                    break;
            }

            if (word.Contains('-') || word.Contains(' ')) //special check for composite words
            {
                lb = txt.IndexOf(word);
                
                while (lb != -1)
                {
                    if (lb == 0 || !char.IsLetter(txt[lb - 1]))
                        if (lb + word.Length == txt.Length || !char.IsLetter(txt[lb + word.Length]))
                            txt = txt.Remove(lb, word.Length).Insert(lb, "???");

                    lb = txt.IndexOf(word);
                }
            }

            //inside quotes replace any instance of the word
            lb = txt.IndexOf('"');

            while (lb != -1)
            {
                ub = txt.IndexOf('"', lb + 1);
                if (ub == -1)
                    break;

                if (txt.Substring(lb, ub - lb).Contains(word))
                    txt = txt.Substring(0, lb) + txt.Substring(lb, ub - lb).Replace(word, "???") + txt.Substring(ub);

                if (ub + 1 >= txt.Length)
                    break;
                lb = txt.IndexOf('"', ub + 1);
            }

            return txt;
        }

        string convToLiterals(string txt)
        {
            string maskChars = @"\09#L?&CAa.,:/$<>|";

            foreach (char maskChar in maskChars)
                txt = txt.Replace(maskChar.ToString(), "\\" + maskChar.ToString());

            return txt;
        }

        void setupDefFillIns(int learningPhase)
        {
            string[] defs = getLineDefs();

            mtbDefs = new List<MaskedTextBox>();
            int top = lblDef.Top;

            for (int i = 0; i < defs.Length; i++)
            {
                MaskedTextBox mtbDef = new MaskedTextBox();

                mtbDef.Left = lblDef.Left;
                mtbDef.Top = top;
                mtbDef.Width = this.Width - 60;
                mtbDef.Font = lblDef.Font;
                mtbDef.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                mtbDef.Tag = defs[i];
                mtbDef.Click += new EventHandler(mtbDef_Enter);
                mtbDef.GotFocus += new EventHandler(mtbDef_Enter);
                mtbDef.KeyPress += new KeyPressEventHandler(mtbDef_KeyPress);
                mtbDef.KeyUp += new KeyEventHandler(mtbDef_KeyUp);
                mtbDef.Tag = defs[i];

                panelDef.Controls.Add(mtbDef);
                mtbDefs.Add(mtbDef);

                mtbDef.BringToFront();

                top += mtbDef.Height + 8;
            }

            kwBounds = new List<List<Tuple<int, int>>>();

            for (int i = 0; i < defs.Length; i++)
            {
                kwBounds.Add(new List<Tuple<int, int>>());
                kwBounds[i] = Misc.GetKeywordBounds(testWord.ToString(), defs[i], corewords);

                int toRemove = 0;
                switch (learningPhase)
                {
                    case 4:
                        toRemove = kwBounds[i].Count - 1;
                        break;
                    case 5:
                        toRemove = kwBounds[i].Count / 2;
                        break;
                    case 6:
                        //remove none
                        break;
                }

                for (int j = 0; j < toRemove; j++)
                    kwBounds[i].RemoveAt(rand.Next(kwBounds[i].Count));
            }

            dispDefs();
        }

        void dispDefs()
        {
            string[] defs = getLineDefs();

            for (int i = 0; i < defs.Length; i++)
            {
                if (kwBounds[i].Count == 0)
                {
                    mtbDefs[i].Mask = convToLiterals(defs[i]);
                    continue;
                }

                mtbDefs[i].Mask = convToLiterals(defs[i].Substring(0, kwBounds[i][0].Item1 + 1));
                for (int j = 0; j < kwBounds[i].Count - 1; j++)
                    mtbDefs[i].Mask += "".PadRight(defs[i].Substring(kwBounds[i][j].Item1, kwBounds[i][j].Item2 - kwBounds[i][j].Item1).Length - 1, '&') + convToLiterals(defs[i].Substring(kwBounds[i][j].Item2, kwBounds[i][j + 1].Item1 - kwBounds[i][j].Item2 + 1));
                mtbDefs[i].Mask += "".PadRight(defs[i].Substring(kwBounds[i][kwBounds[i].Count - 1].Item1, kwBounds[i][kwBounds[i].Count - 1].Item2 - kwBounds[i][kwBounds[i].Count - 1].Item1).Length - 1, '&') + convToLiterals(defs[i].Substring(kwBounds[i][kwBounds[i].Count - 1].Item2));
            }
        }

        void drawProgress()
        {
            int x1 = lblSynonyms.Left + 10, x2 = this.Width - 70;
            int y = lblWord.Top + lblWord.Height + 4;

            Graphics gfx = panelDef.CreateGraphics();
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Pen penGreen = new Pen(Color.Green, 5), penBlack = new Pen(Color.Black, 5), penRed = new Pen(Color.Red, 5);
            Brush brushGreen = new SolidBrush(Color.Green), brushBlack = new SolidBrush(Color.Black), brushRed = new SolidBrush(Color.Red);

            gfx.DrawLine(penGreen, x1 + 5, y, curX, y);
            gfx.DrawLine(deltaX >= 0 ? penBlack : penRed, curX, y, x2 + 5, y);

            for (int i = 0; i <= 6; i++)
                if (x1 + (x2 - x1) / 6 * i <= curX)
                    gfx.FillEllipse(brushGreen, x1 + (x2 - x1) / 6 * i - 2, y - 5, 10, 10);
                else
                    gfx.FillEllipse(deltaX >= 0 ? brushBlack : brushRed, x1 + (x2 - x1) / 6 * i - 2, y - 5, 10, 10);

            if (answCorrectly == null)
                return;

            //draw check marks
            string[] defs = getLineDefs();
            int skipLines = 0;

            for (int i = 0; i < defs.Length; i++)
            {
                int currAnsw = 0;

                foreach (Answer answer in answCorrectly)
                    if (answer.def == defs[i])
                    {
                        if (answer.correct)
                            currAnsw = 1; //correct
                        else
                            currAnsw = 2; //wrong
                        break;
                    }

                if (currAnsw != 0)
                    gfx.DrawImage(currAnsw == 1 ? picRight.Image : picWrong.Image, lblDef.Left, rtbDef.Top + 20 * (i + skipLines) + 2, 20, 16);

                skipLines += (int)(gfx.MeasureString(defs[i], rtbDef.Font).Width / rtbDef.Width);
            }
        }

        void mtbDef_Enter(object sender, EventArgs e)
        {
            MaskedTextBox mtbDef = (MaskedTextBox)sender;

            if (!mtbDef.Mask.Contains('&'))
                return;

            int lb = mtbDef.Mask.IndexOf('&');
            int n = mtbDef.Text.Length;
            
            while (n > 0)
            {
                lb = mtbDef.Mask.IndexOf('&', lb + 1);
                
                if (lb == -1)
                {
                    lb = mtbDef.Text.Length - 1;
                    break;
                }

                n--;
            }

            int escapeChars = mtbDef.Mask.Substring(0, lb).Count(c => c == '\\');
            lb -= escapeChars;

            if (lb != -1)
                mtbDef.Select(lb, 0);
        }

        void mtbDef_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //enter
            {
                finish();
                e.Handled = true;
            }
        }

        private void mtbDef_KeyUp(object sender, KeyEventArgs e)
        {
            if (char.IsLetterOrDigit((char)e.KeyValue))
            {
                int i = mtbDefs.IndexOf((MaskedTextBox)sender);

                for (int j = 0; j < kwBounds[i].Count - 1; j++)
                    if (mtbDefs[i].SelectionStart == kwBounds[i][j].Item2)
                    {
                        mtbDefs[i].SelectionStart = kwBounds[i][j + 1].Item1;
                        break;
                    }
            }
        }

        int lineOffset(int line)
        {
            int offset = 0;

            for (int i = 0; i < line; i++)
                offset = lblDef.Text.IndexOf(Environment.NewLine, offset + 1) + 2;

            return offset;
        }

        string[] getLineDefs()
        {
            return lblDef.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(d => d[0] != '"').ToArray();
        }

        string createMask()
        {
            string mask = convToLiterals(testWord.ToString().Substring(0, 1)) + "".PadRight(testWord.ToString().Length - 1, '&');
            int offset = mask[0] == '\\' ? 1 : 0;

            for (int i = 1; i < testWord.ToString().Length; i++)
                if (!char.IsLetter(testWord.ToString()[i]))
                {
                    mask = mask.Remove(i + offset, 1);
                    mask = mask.Insert(i + offset, convToLiterals(testWord.ToString().Substring(i, 1)));

                    offset += mask[i] == '\\' ? 1 : 0;
                }

            return mask;
        }

        bool checkEnteredWord(string word)
        {
            word = word.ToLower();

            bool success = word == testWord.ToString().ToLower();

            //if testing recall ignore hyphen mistakes & diacritic letter mistakes
            if (!success && testWord.archived)
            {
                //hyphens
                success = word.ToLower().Replace("-", " ") == testWord.ToString().ToLower().Replace("-", " ") //hyphen or space mistake
                    || word.ToLower().Replace("-", "") == testWord.ToString().ToLower().Replace("-", ""); //no hyphen mistake

                if (success)
                    MessageBox.Show("Nevertheless your answer will be accepted.", "You messed up one of the hyphens.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    //diacritic letters
                    success = Misc.RemoveDiacritics(word.ToLower()) == Misc.RemoveDiacritics(testWord.ToString().ToLower());
                    if (success)
                        MessageBox.Show("Nevertheless your answer will be accepted.", "You messed up one of the diacritic letters.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        //BOTH???
                        success = Misc.RemoveDiacritics(word.ToLower().Replace("-", " ")) == Misc.RemoveDiacritics(testWord.ToString().ToLower().Replace("-", " "));
                        if (success)
                            MessageBox.Show("Nevertheless your answer will be accepted.", "You messed up one of the hyphens AND one of the diacritic letters.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            return success;
        }
        

        public formTestRecall()
        {
            InitializeComponent();
        }

        private void formTestRecall_Load(object sender, EventArgs e)
        {
            setupUI();
            
            //icon & images
            if (File.Exists(Application.StartupPath + "\\Wordy.ico"))
                this.Icon = new Icon(Application.StartupPath + "\\Wordy.ico");

            if (File.Exists(Application.StartupPath + "\\ui\\1.png"))
                picRight.Image = new Bitmap(Application.StartupPath + "\\ui\\1.png");
            if (File.Exists(Application.StartupPath + "\\ui\\2.png"))
                picWrong.Image = new Bitmap(Application.StartupPath + "\\ui\\2.png");

            corewords = Misc.LoadCoreWords();

            lblDef.MaximumSize = new Size(this.Width - 60, this.Height - 120);
            picVisual.MaximumSize = new Size(this.Width - 60, this.Height - 120);

            lineHeight = lblDef.CreateGraphics().MeasureString("1", lblDef.Font).Height;

            //start testing
            nTests = 0;
            rand = new Random((int)DateTime.Now.Ticks);

            nextWord();
        }

        private void formTestRecall_Resize(object sender, EventArgs e)
        {
            setupUI();
        }

        private void formTestRecall_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.Show();
        }

        private void formTestRecall_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13) //enter
            {
                if (picWrong.Visible && !timerProgressChange.Enabled && !buttNext.Visible)
                    nextWord();
                else if (chklistDefs.Visible)
                    buttFinished.PerformClick();
            }
            else if (e.KeyData == Keys.Tab)
            {
                //disable tab jumping when between tests
                if (timerProgressChange.Enabled || buttNext.Visible)
                    e.Handled = true;
            }
            else if (char.IsDigit((char)e.KeyData))
            {
                int num = (char)e.KeyData - 49;

                if (buttPickWord1.Visible)
                    answer(correctPick == num);
                else if (num >= 0 && num < chklistDefs.Items.Count)
                    chklistDefs.SetItemChecked(num, !chklistDefs.GetItemChecked(num));
            }
        }

        private void lblVisualTrigger_MouseEnter(object sender, EventArgs e)
        {
            lblDef.Visible = false;
            picVisual.Visible = true;
        }

        private void lblVisualTrigger_MouseLeave(object sender, EventArgs e)
        {
            picVisual.Visible = false;

            if (lblVisualTrigger.Visible)
                lblDef.Visible = true;
        }

        private void buttFinished_Click(object sender, EventArgs e)
        {
            if (!chklistDefs.Visible)
                finish();
            else
            {
                buttFinished.Visible = false;
                chklistDefs.Visible = false;

                //remove indices from the items
                for (int i = 0; i < chklistDefs.Items.Count; i++)
                {
                    string item = chklistDefs.Items[i].ToString();
                    chklistDefs.Items[i] = item.Substring(item.IndexOf('.') + 3);
                }

                //check answers
                for (int i = 0; i < chklistDefs.Items.Count; i++)
                    if (chklistDefs.GetItemChecked(i) != testWord.GetDefinition().Contains(chklistDefs.Items[i].ToString()))
                    {
                        answer(false);
                        return;
                    }

                answer(true);
            }
        }

        private void buttSkip_Click(object sender, EventArgs e)
        {
            answer(checkAnswers());
        }

        private void buttAnotherExample_Click(object sender, EventArgs e)
        {
            //do we have any examples?
            if (exampleSentences == null || exampleSentences.Length == 0)
                return;

            //find current example index
            int curExample;
            for (curExample = 0; curExample <= exampleSentences.Length; curExample++)
                if (exampleSentences[curExample] == lblDef.Text)
                    break;

            //display next example
            curExample = (curExample + 1) % exampleSentences.Length;
            lblDef.Text = exampleSentences[curExample];

            //readjust button position
            buttAnotherExample.Top = lblDef.Top + lblDef.Height + 16;
        }

        private void buttNext_Click(object sender, EventArgs e)
        {
            nextWord();
        }

        private void buttPickWord1_Click(object sender, EventArgs e)
        {
            answer(correctPick == 0);
        }

        private void buttPickWord2_Click(object sender, EventArgs e)
        {
            answer(correctPick == 1);
        }

        private void buttPickWord3_Click(object sender, EventArgs e)
        {
            answer(correctPick == 2);
        }

        private void buttPickWord4_Click(object sender, EventArgs e)
        {
            answer(correctPick == 3);
        }

        private void buttPickWord5_Click(object sender, EventArgs e)
        {
            answer(correctPick == 4);
        }

        private void buttPickWord6_Click(object sender, EventArgs e)
        {
            answer(correctPick == 5);
        }

        private void timerProgressChange_Tick(object sender, EventArgs e)
        {
            drawProgress();

            curX += deltaX;
            if ((deltaX > 0 && curX >= endX) || (deltaX < 0 && curX <= endX))
            {
                curX = endX;

                timerProgressChange.Enabled = false;

                if (deltaX > 0)
                    nextWord();
                else
                {
                    buttNext.Visible = true;
                    drawProgress();
                }
            }
        }

        private void timerWait_Tick(object sender, EventArgs e)
        {
            timerWait.Enabled = false;
            nextWord();
        }

        private void textTestWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //enter
            {
                answer(checkEnteredWord(textTestWord.Text));
                e.Handled = true;
            }
        }

        private void mtbTestWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //enter
            {
                answer(checkEnteredWord(mtbTestWord.Text));
                e.Handled = true;
            }
        }

        private void chklistDefs_KeyDown(object sender, KeyEventArgs e)
        {
            formTestRecall_KeyDown(sender, e);
        }

        private void picWordnik_Click(object sender, EventArgs e)
        {
            if (lblWord.Text != "")
                Process.Start("http://www.wordnik.com/words/" + lblWord.Text);
        }
    }
}