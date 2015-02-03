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
using System.Runtime.InteropServices;

namespace Wordy
{
    public partial class formTestRecall : Form
    {
        #region PlaySound DLL Imports
        [DllImport("WinMM.dll")]
        static extern bool PlaySound(string fname, int Mod, int flag);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        private int SND_ASYNC = 0x0001; 
        #endregion

        public formMain main;
        public List<Entry> words;
        public bool testUnlearned;

        string def;
        float lineHeight, animRatio;
        int nTests, nCorrectAnswers, correctPick, resetLearningPhase, startNotch, endNotch;
        bool noMoreWords;

        Entry testWord;
        Random rand;
        DateTime startTime, gotoNextQuestion;
        TimeSpan totalTime;

        List<MaskedTextBox> mtbDefs;
        List<List<Tuple<int, int>>> kwBounds;
        List<Tuple<int, int, Color, string>> answers;
        List<string> corewords = new List<string>();
        List<int> baitQuestionMarksIndices;
        string[] exampleSentences;
        Answer[] answCorrectly;
        WordnikService wordnik;
        int nNotches;


        void setupUI()
        {
            panelDef.Size = new Size(this.Width - 50, this.Height - 120);
            panelTestWord.Size = new Size(this.Width - 50, this.Height - 120);

            lblDef.MaximumSize = new Size(this.Width - 80, this.Height - 120);
            lblSynonyms.MaximumSize = new Size(this.Width - 60, lblSynonyms.Height);
            lblTestWordDef.MaximumSize = new Size(this.Width - 60, this.Height - 120);
            picVisual.MaximumSize = new Size(this.Width - 60, this.Height - 120);

            flowpanelPickAnswers.Size = new Size(this.Width - 176, flowpanelPickAnswers.Height);
            flowpanelPickAnswers.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;
            textTestWord.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;
            mtbTestWord.Width = this.Width - 60;
            textTestWord.Width = this.Width - 60;

            rtbDef.Size = new Size(this.Width - 90, this.Height - 120);
            
            chklistDefs.Size = new Size(this.Width - 80, this.Height - 120);
            chklistDefs.Height = 22 * chklistDefs.Items.Count;

            if (mtbDefs != null && mtbDefs.Count > 0)
            {
                foreach (MaskedTextBox mtbDef in mtbDefs)
                    mtbDef.Width = this.Width - 60;

                buttFinished.Top = mtbDefs[mtbDefs.Count - 1].Top + mtbDefs[mtbDefs.Count - 1].Height + 16;
                buttSkip.Top = buttFinished.Top;
            }

            if (testWord != null && testWord.learningPhase == 3)
            {
                buttFinishedLearned.Top = textTestWord.Top + textTestWord.Height + 16;
                buttSkipLearned.Top = buttFinishedLearned.Top;
            }

            //position pictures
            if (!picRight.Visible && !picWrong.Visible)
            {
                if (lblSynonyms.Text == "What word is represented by this picture?")
                {
                    //during testing learned word by visual association
                    picVisual.Visible = false; //hide visual to prevent image jumping when resizing window

                    picVisual.Size = new Size(Math.Min(this.Width - 60, 720), this.Height - 120 - (buttFinished.Top + buttFinished.Height + 16));
                    picVisual.Location = new Point(mtbTestWord.Left + mtbTestWord.Width / 2 - picVisual.Width / 2, buttFinished.Top + buttFinished.Height + 16);
                    picVisual.Visible = true;
                }
            }
            else
            {
                //after testing
                if (picVisual.Image == null)
                {
                    //only displaying picRight or picWrong
                    picRight.Left = lblSynonyms.Left + (this.Width - 60) / 2 - picRight.Width / 2;
                    picWrong.Left = picRight.Left;
                    picRight.Top = lblDef.Top + lblDef.Height + 12;
                    picWrong.Top = picRight.Top;

                    //position buttNext
                    buttNext.Left = picWrong.Left + picWrong.Width / 2 - buttNext.Width / 2;
                    buttNext.Top = picWrong.Top + picWrong.Height + 12;
                }
                else
                {
                    //displaying visual too
                    picVisual.Visible = false; //hide visual to prevent image jupming when resizing window

                    int top = lblDef.Top + lblDef.Height + 32;
                    int areaHeight = this.Height - 120 - top;
                    float picRatio = (float)picVisual.Image.Width / picVisual.Image.Height;

                    picVisual.Top = top;
                    picVisual.Height = areaHeight;
                    picVisual.Width = (int)(picRatio * picVisual.Height);

                    int maxVisualWidth = this.Width - 18 - 18 - buttNext.Width - 18;

                    if (picVisual.Width > maxVisualWidth)
                    {
                        //shrink visual so picRight/Wrong can be displayed properly
                        picVisual.Width = maxVisualWidth;
                        picVisual.Height = (int)(maxVisualWidth / picRatio);
                    }

                    //set Top values
                    picVisual.Top = top + areaHeight / 2 - picVisual.Height / 2;
                    picRight.Top = top + areaHeight / 2 - picRight.Height / 2;
                    picWrong.Top = picRight.Top - buttNext.Height;
                    buttNext.Top = picWrong.Top + picWrong.Height + 12;

                    if (buttNext.Visible)
                        //buttNext & picRight are both visible when the user made a typo or a similar minor error, but his answer was still accepted
                        picRight.Top = picWrong.Top;

                    //set Left values
                    int spacing = (this.Width - buttNext.Width - picVisual.Width) / 3;

                    buttNext.Left = spacing;
                    picRight.Left = picWrong.Left = spacing + (buttNext.Width - picRight.Width) / 2;
                    picVisual.Left = buttNext.Left + buttNext.Width + spacing;

                    picVisual.Visible = true; //show picVisual
                }
            }

            picWordnik.Left = this.Width - picWordnik.Width - 12;
            picWordnik.Top = this.Height - picWordnik.Height - 48;

            panelDef.Refresh();
        }

        void randomizeAnswers(List<string> answers, string correctAnswer)
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
                    buttPickWord1.Text = correctAnswer;
                    break;
                case 1:
                    buttPickWord2.Text = correctAnswer;
                    break;
                case 2:
                    buttPickWord3.Text = correctAnswer;
                    break;
                case 3:
                    buttPickWord4.Text = correctAnswer;
                    break;
                case 4:
                    buttPickWord5.Text = correctAnswer;
                    break;
                case 5:
                    buttPickWord6.Text = correctAnswer;
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
            lblDef.Visible = false;
            rtbDef.Visible = false;
            chklistDefs.Visible = false;
            buttAnotherExample.Visible = false;
            picWordnik.Enabled = false;

            lblWord.ForeColor = Color.Black;
            answCorrectly = null;

            if (picVisual.Image != null)
            {
                picVisual.Image.Dispose();
                picVisual.Image = null;
            }

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
                    picVisual.Image = Image.FromFile(Application.StartupPath + "\\visuals\\" + testWord + ".jpg");

                //prepare UI
                if (panelDef.Visible)
                    panelDef.Refresh();

                if (!testWord.archived)
                {
                    if (main.Profile == "English")
                    {
                        switch (testWord.learningPhase)
                        {
                            case 1: //multiple choice - random words
                                lblTestWordDef.Text = "Which word is defined by the following:" + Environment.NewLine + Environment.NewLine + removeWordInstances(def, testWord.ToString());
                                flowpanelPickAnswers.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;

                                randomizeAnswers(main.GetRandWords(5, testWord.ToString()), testWord.ToString());
                                break;
                            case 2: //multiple choice - rhymes
                                lblTestWordDef.Text = "Which word is defined by the following:" + Environment.NewLine + Environment.NewLine + removeWordInstances(def, testWord.ToString());

                                randomizeAnswers(testWord.GetRandomRhymes(), testWord.ToString());
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

                        prepareUIForNextWord(testWord.learningPhase);
                    }
                    else
                        prepareNonEnglishQuestion(testWord.learningPhase);
                }
                else
                {
                    int questionType = -1;

                    if (main.Profile == "English")
                    {
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
                                if (getExampleSentences())
                                    questionType = 4;
                                else
                                    questionType = -1;
                            }
                            else if (percent <= 90 && testWord.GetSynonyms() != "" && testWord.ToString().Length > 1)
                                questionType = 5;
                            else if (percent <= 100 && picVisual.Image != null)
                                questionType = 6;
                            else
                                questionType = -1;
                        }

                        string mask;
                        int offset;

                        mtbTestWord.Tag = false; //indicates mtbTestWord is NOT used to read the answer
                        mtbTestWord.TextAlign = HorizontalAlignment.Left;

                        switch (questionType)
                        {
                            case 0: // type word
                                lblTestWordDef.Text = "Type the word that is defined by the following:" + Environment.NewLine + Environment.NewLine + removeWordInstances(def, testWord.ToString());
                                textTestWord.Text = "";

                                textTestWord.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;
                                buttFinishedLearned.Top = textTestWord.Top + textTestWord.Height + 16;
                                buttSkipLearned.Top = buttFinishedLearned.Top;

                                textTestWord.Visible = true;
                                panelTestWord.Visible = true;
                                buttFinishedLearned.Visible = true;
                                buttSkipLearned.Visible = true;

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
                                bool removedWord = false;

                                foreach (string d in defs)
                                {
                                    chklistDefs.Items.Add(ind++ + ".  " + removeWordInstances(d, testWord.ToString()));
                                    if (chklistDefs.Items[chklistDefs.Items.Count - 1].ToString().Contains("???"))
                                        removedWord = true;
                                }

                                baitQuestionMarksIndices = new List<int>();
                                if (removedWord && rand.NextDouble() > 0.5)
                                {
                                    //insert "???" in a random line that isn't the correct definition for this word
                                    //otherwise the user would know that any line with "???" must be the correct definition
                                    int searchStartInd = rand.Next(chklistDefs.Items.Count);
                                    int i = searchStartInd;

                                    while (true)
                                    {
                                        string line = chklistDefs.Items[i].ToString();

                                        if (!line.Contains("???") && !testWord.GetDefinition().Contains(line))
                                        {
                                            if (line.Contains(" "))
                                                line = line.Insert(line.LastIndexOf(' '), " ???");
                                            else
                                                line += " ???";

                                            chklistDefs.Items[i] = line;
                                            baitQuestionMarksIndices.Add(i);
                                            break;
                                        }

                                        i++;
                                        if (i == chklistDefs.Items.Count)
                                            i = 0;
                                        if (i == searchStartInd)
                                            break;
                                    }
                                }

                                chklistDefs.Height = 22 * chklistDefs.Items.Count;
                                buttFinished.Top = chklistDefs.Top + chklistDefs.Height + 16;
                                buttSkip.Top = buttFinished.Top;

                                chklistDefs.Visible = true;
                                panelDef.Visible = true;
                                buttFinished.Visible = true;
                                buttSkip.Visible = true;

                                chklistDefs.Focus();

                                resetLearningPhase = 1;
                                break;
                            case 3: // recognize synonyms
                                lblDef.Text = "What word has these synonyms?";
                                mtbTestWord.Text = "";

                                mtbTestWord.Top = 17;
                                buttFinished.Top = lblDef.Top + lblDef.Height + 8;
                                buttSkip.Top = buttFinished.Top;

                                lblSynonyms.Text = removeWordInstances(lblSynonyms.Text, testWord.ToString()); //hide instances of test word in the synonyms list

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
                                buttFinished.Visible = true;
                                buttSkip.Visible = true;

                                mtbTestWord.Focus();
                                mtbTestWord.Tag = true; //indicates mtbTestWord is used to read the answer

                                resetLearningPhase = 5;
                                break;
                            case 4: // type word for example sentence
                                prepareQuestionWithExampleSentences();
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
                                    lblWord.Text = lblWord.Text.Substring(1, 1) + lblWord.Text.Substring(0, 1) + (lblWord.Text.Length > 2 ? lblWord.Text.Substring(2) : "");

                                lblDef.Text = "Unscramble this word.";

                                mtbTestWord.Text = "";
                                mtbTestWord.Mask = "";
                                mtbTestWord.TextAlign = HorizontalAlignment.Center;

                                mtbTestWord.Top = lblDef.Top + lblDef.Height + 32;
                                buttFinished.Top = mtbTestWord.Top + mtbTestWord.Height + 8;
                                buttSkip.Top = buttFinished.Top;

                                lblDef.Visible = true;
                                mtbTestWord.Visible = true;
                                panelDef.Visible = true;
                                buttFinished.Visible = true;
                                buttSkip.Visible = true;

                                mtbTestWord.Focus();
                                mtbTestWord.Tag = true; //indicates mtbTestWord is used to read the answer

                                resetLearningPhase = 6;
                                break;
                            case 6: // recognize visuals
                                lblSynonyms.Text = "What word is represented by this picture?";
                                lblDef.Text = "";

                                mtbTestWord.Text = "";
                                mtbTestWord.Mask = "";
                                mtbTestWord.TextAlign = HorizontalAlignment.Center;

                                lblSynonyms.Top = lblWord.Top + 16;
                                mtbTestWord.Top = lblSynonyms.Top + lblSynonyms.Height + 32;
                                buttFinished.Top = mtbTestWord.Top + mtbTestWord.Height + 8;
                                buttSkip.Top = buttFinished.Top;

                                setupUI(); //position visual

                                lblWord.Visible = false;
                                lblSynonyms.Visible = true;
                                mtbTestWord.Visible = true;
                                panelDef.Visible = true;
                                buttFinished.Visible = true;
                                buttSkip.Visible = true;

                                mtbTestWord.Focus();
                                mtbTestWord.Tag = true; //indicates mtbTestWord is used to read the answer

                                resetLearningPhase = 3;
                                break;
                        }
                    }
                    else
                    {
                        //non-English words
                        int percent = rand.Next(100);

                        percent = 81;
                        if (percent <= 15)
                            prepareNonEnglishQuestion(1);
                        else if (percent <= 30)
                            prepareNonEnglishQuestion(2);
                        else if (percent <= 55)
                            prepareNonEnglishQuestion(3);
                        else if (percent <= 80)
                            prepareNonEnglishQuestion(4);
                        else
                        {
                            if (getExampleSentences())
                                prepareQuestionWithExampleSentences();
                            else
                                prepareNonEnglishQuestion(rand.Next(4) + 1);
                        }
                    }
                }
            }
            else
            {
                noMoreWords = true;
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

            startTime = DateTime.Now; //start the clock
        }

        bool getExampleSentences()
        {
            try
            {
                if (wordnik == null)
                    wordnik = new WordnikService("b3bbd1f9103a01de7d00a0fd1300164c17bfcec03eb86a678");
                var exampleResults = wordnik.GetExamples(testWord.ToString());

                if (exampleResults.Examples == null)
                    return false;

                var examples = exampleResults.Examples.ToArray();
                exampleSentences = new string[examples.Length];

                for (int i = 0; i < examples.Length; i++)
                    exampleSentences[i] = removeWordInstances(examples[i].Text, testWord.ToString());

                lblDef.Text = exampleSentences[rand.Next(examples.Length)];

                return true;
            }
            catch
            {
                return false;
            }
        }

        void prepareQuestionWithExampleSentences()
        {
            lblSynonyms.Text = "What word completes this sentence:";
            mtbTestWord.Text = "";
            mtbTestWord.Top = 17;

            buttAnotherExample.Top = lblDef.Top + lblDef.Height + 16;
            buttFinished.Top = buttAnotherExample.Top + buttAnotherExample.Height + 8;
            buttSkip.Top = buttFinished.Top;

            buttAnotherExample.Visible = true;
            buttFinished.Visible = true;
            buttSkip.Visible = true;

            //reveal first letter and vowels
            string mask = createMask();
            int offset = 0;

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
            mtbTestWord.Tag = true; //indicates mtbTestWord is used to read the answer

            resetLearningPhase = 2;
        }

        void prepareNonEnglishQuestion(int questionType)
        {
            switch (questionType)
            {
                case 1: //multiple choice (english)
                    lblTestWordDef.Text = "What does this word mean?" + Environment.NewLine + Environment.NewLine + testWord.ToString();
                    flowpanelPickAnswers.Top = lblTestWordDef.Top + lblTestWordDef.Height + 32;
                    randomizeAnswers(main.GetRandWords(5, testWord.GetDefinition()), testWord.GetDefinition());
                    prepareUIForNextWord(1);
                    break;
                case 2: //multiple choice (foreign language)
                    lblTestWordDef.Text = "Translate this word:" + Environment.NewLine + Environment.NewLine + testWord.GetDefinition();
                    randomizeAnswers(main.GetRandForeignWords(5, testWord.ToString()), testWord.ToString());
                    prepareUIForNextWord(2);
                    break;
                case 3: //type word (english)
                    lblTestWordDef.Text = "What does this word mean?" + Environment.NewLine + Environment.NewLine + testWord.ToString();
                    textTestWord.Text = "";
                    prepareUIForNextWord(3);
                    break;
                case 4: //type word (foreign language)
                    lblTestWordDef.Text = "Translate this word:" + Environment.NewLine + Environment.NewLine + testWord.GetDefinition();
                    textTestWord.Text = "";
                    prepareUIForNextWord(3);
                    break;
            }
        }

        void prepareUIForNextWord(int step)
        {
            if (step <= 3)
            {
                panelTestWord.Visible = true;

                if (step <= 2)
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

                    buttFinishedLearned.Top = textTestWord.Top + textTestWord.Height + 16;
                    buttSkipLearned.Top = buttFinishedLearned.Top;
                    buttFinishedLearned.Visible = true;
                    buttSkipLearned.Visible = true;
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

        void finishFillDefinitions()
        {
            if (mtbDefs != null)
                foreach (var mtbDef in mtbDefs)
                    if (!mtbDef.MaskCompleted)
                    {
                        if (MessageBox.Show("You haven't filled out every blank.", "Skip question?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                            return;
                        break;
                    }

            answer(checkAnswers());
        }

        void finishSelectDefinitions()
        {
            chklistDefs.Visible = false;

            //remove indices from the items and replace "???" with test word
            for (int i = 0; i < chklistDefs.Items.Count; i++)
            {
                string item = chklistDefs.Items[i].ToString();

                if (baitQuestionMarksIndices.Contains(i))
                    chklistDefs.Items[i] = item.Substring(item.IndexOf('.') + 3).Replace(" ???", "");
                else
                    chklistDefs.Items[i] = item.Substring(item.IndexOf('.') + 3).Replace("???", testWord.ToString());
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

        bool checkAnswers()
        {
            answers = new List<Tuple<int, int, Color, string>>();
            answCorrectly = new Answer[mtbDefs.Count];
            int i = 0;
            bool typos = false;

            if (mtbDefs != null)
                while (mtbDefs.Count > 0)
                {
                    answCorrectly[i] = new Answer(mtbDefs[0].Tag.ToString());
                    answCorrectly[i].correct = true;

                    mtbDefs[0].TextMaskFormat = MaskFormat.IncludePromptAndLiterals;

                    for (int j = 0; j < kwBounds[0].Count; j++)
                    {
                        bool res;
                        Color answerColor = Color.Black;

                        if (mtbDefs[0].Text.Length < kwBounds[0][j].Item2)
                        {
                            res = false;
                            answerColor = Color.Red;
                        }
                        else
                        {
                            string answerGiven = mtbDefs[0].Text.Substring(kwBounds[0][j].Item1, kwBounds[0][j].Item2 - kwBounds[0][j].Item1).ToLower();
                            string correctAnswer = mtbDefs[0].Tag.ToString().Substring(kwBounds[0][j].Item1, kwBounds[0][j].Item2 - kwBounds[0][j].Item1).ToLower();

                            res = answerGiven == correctAnswer;

                            if (res)
                                //correct answer
                                answerColor = Color.Green;
                            else if (testWord.archived && isTypo(correctAnswer, answerGiven.Replace("_", "")))
                            {
                                //typo in answer
                                typos = true;
                                res = true;
                                answerColor = Color.LightGreen;
                            }
                            else
                            {
                                //wrong answer
                                res = false;
                                answerColor = Color.Red;
                            }
                        }

                        answCorrectly[i].correct = answCorrectly[i].correct && res;
                        answers.Add(new Tuple<int, int, Color, string>(lineOffset(i) + kwBounds[0][j].Item1, lineOffset(i) + kwBounds[0][j].Item2, answerColor, mtbDefs[0].Text));
                    }

                    mtbDefs[0].Dispose();
                    panelDef.Controls.Remove(mtbDefs[0]);
                    mtbDefs.RemoveAt(0);
                    kwBounds.RemoveAt(0);

                    i++;
                }

            if (typos)
                MessageBox.Show("It will be accepted anyway.", "You have a typo in your answer", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return answers.All(a => a.Item3 != Color.Red);
        }

        void answer(bool success)
        {
            if (timerProgressChange.Enabled) //disable answering between questions
                return;

            if (success)
                nCorrectAnswers++;

            lblSynonyms.Top = 83;
            lblWord.Text = testWord.ToString();

            panelTestWord.Visible = false;
            buttFinished.Visible = false;
            buttFinishedLearned.Visible = false;
            buttSkipLearned.Visible = false;
            buttSkip.Visible = false;
            buttAnotherExample.Visible = false;

            if (testWord.archived)
            {
                mtbTestWord.Visible = false;
                buttAnotherExample.Visible = false;
                lblWord.Visible = true;
            }

            string questionDef = lblDef.Text; //currently displayed definitions
            lblDef.Text = testWord.GetDefinition(); //display full list of definitions

            if (panelDef.Visible)
                lblSynonyms.Text = testWord.GetSynonyms();
            else
                panelDef.Visible = true;

            picWordnik.Enabled = true;

            //color correct/wrong answers
            if (main.Profile == "English" && //only applies to testing of English words
                (((!testWord.archived && testWord.learningPhase >= 4) || (testWord.archived && resetLearningPhase == 4)) && answers.Count > 0))
            {
                rtbDef.Text = "";
                rtbDef.AppendText(questionDef.Substring(0, answers[0].Item1));

                for (int i = 0; i < answers.Count - 1; i++)
                {
                    rtbDef.SelectionBackColor = answers[i].Item3;
                    rtbDef.AppendText(questionDef.Substring(answers[i].Item1, answers[i].Item2 - answers[i].Item1));
                    rtbDef.SelectionBackColor = SystemColors.Control;
                    rtbDef.AppendText(questionDef.Substring(answers[i].Item2, answers[i + 1].Item1 - answers[i].Item2));
                }

                rtbDef.SelectionBackColor = answers[answers.Count - 1].Item3;
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

            startNotch = testWord.learningPhase;
            animRatio = 0;

            //save test result
            if (success)
            {
                picRight.Visible = true;

                if (!testWord.archived)
                {
                    if (testWord.learningPhase == (main.Profile == "English" ? 6 : 4))
                    {
                        if (testWord.GetRecallSuccessRate() == -1)
                            lblWord.Text += " learned!";
                        else
                            lblWord.Text += " relearned!";

                        lblWord.ForeColor = Color.Green;
                    }

                    endNotch = testWord.learningPhase + 1;
                    timerProgressChange.Enabled = true;
                }
                else
                {
                    gotoNextQuestion = DateTime.Now.AddSeconds(2);
                    endNotch = startNotch;
                    timerProgressChange.Enabled = true;
                }
            }
            else
            {
                picWrong.Visible = true;

                if (!testWord.archived)
                {
                    buttNext.Visible = true;
                    endNotch = startNotch;
                    timerProgressChange.Enabled = true;
                }
                else
                {
                    //check if answer has a typo
                    string answerGiven;
                    if (mtbTestWord.Tag != null && (bool)mtbTestWord.Tag)
                        answerGiven = mtbTestWord.Text;
                    else
                        answerGiven = textTestWord.Text;

                    if (isTypo(getCorrectAnswer(), answerGiven))
                    {
                        acceptAnswer();
                        success = true;
                        MessageBox.Show("It will be accepted anyway.", "You have a typo in your answer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        //check if answer given is a synonym
                        if (testWord.GetSynonyms().ToLower().Split(new string[] { " / " }, StringSplitOptions.RemoveEmptyEntries).Contains(answerGiven))
                        {
                            acceptAnswer();
                            success = true;
                            MessageBox.Show("It will be accepted anyway.", "Your answer is a synonym of the correct answer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            lblWord.Text += " forgotten!";
                            lblWord.ForeColor = Color.Red;

                            if (main.Profile == "English")
                                endNotch = resetLearningPhase;
                            else
                                endNotch = 1;

                            timerProgressChange.Enabled = true;
                        }
                    }
                }
            }

            totalTime += DateTime.Now - startTime; //mark time

            testWord.LogTest(success, answCorrectly, resetLearningPhase, main.Profile == "English");
            main.SaveWords();

            //position pictures and other UI controls
            setupUI();

            if (buttNext.Visible)
                buttNext.Focus();

            if (main.prefs.PlaySounds)
            {
                //play sound
                if (success)
                    PlaySound(Application.StartupPath + "\\sounds\\correct.wav", 0, SND_ASYNC);
                else
                    PlaySound(Application.StartupPath + "\\sounds\\wrong.wav", 0, SND_ASYNC);
            }
        }

        bool isTypo(string correctAnswer, string answerGiven)
        {
            correctAnswer = correctAnswer.ToLower();
            answerGiven = answerGiven.ToLower();

            //check for extra letters
            for (int i = 0; i < answerGiven.Length; i++)
                if (answerGiven.Remove(i, 1) == correctAnswer)
                    return true;

            //check for missing letters
            for (int i = 0; i <= answerGiven.Length && i < correctAnswer.Length; i++)
                if (answerGiven.Insert(i, correctAnswer[i].ToString()) == correctAnswer)
                    return true;

            //check for a swapped pair of letters
            for (int i = 0; i < answerGiven.Length - 1; i++)
                if (answerGiven.Substring(0, i) + answerGiven[i + 1] + answerGiven[i] + (i + 2 < answerGiven.Length ? answerGiven.Substring(i + 2) : "") == correctAnswer)
                    return true;

            //check for a mistyped letter
            if (answerGiven.Length == correctAnswer.Length)
                for (int i = 0; i < answerGiven.Length; i++)
                    if (answerGiven.Substring(0, i) == correctAnswer.Substring(0, i) && answerGiven.Substring(i + 1) == correctAnswer.Substring(i + 1))
                        return true;

            return false;
        }

        void acceptAnswer()
        {
            picWrong.Visible = false;
            picRight.Visible = true;
            lblWord.ForeColor = Color.LightGreen;
            buttNext.Visible = true;

            endNotch = startNotch;
            timerProgressChange.Enabled = true;

            if (picVisual.Visible)
                //if the visual is displayed ensure all UI positions are accurate
                setupUI();

            nCorrectAnswers++;
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

                mtbDef.Left = lblDef.Left - 20;
                mtbDef.Top = top;
                mtbDef.Width = this.Width - 60;
                mtbDef.Font = lblDef.Font;
                mtbDef.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                mtbDef.TabIndex = i;
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
            //draw progress line
            int x1 = lblSynonyms.Left + 10, x2 = this.Width - 70;
            int y = lblWord.Top + lblWord.Height + 4;

            Graphics gfx = panelDef.CreateGraphics();
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Pen penGreen = new Pen(Color.Green, 5), penBlack = new Pen(Color.Black, 5), penRed = new Pen(Color.Red, 5);
            Brush brushGreen = new SolidBrush(Color.Green), brushBlack = new SolidBrush(Color.Black), brushRed = new SolidBrush(Color.Red);

            int startX = calcNotchX(startNotch);
            int endX = calcNotchX(endNotch);
            int curX = (int)(startX + animRatio * (endX - startX));

            gfx.DrawLine(penGreen, x1 + 5, y, curX, y);
            if (endX != startX || endNotch != nNotches + 1)
                gfx.DrawLine(endX >= startX ? penBlack : penRed, curX, y, x2 + 5, y);

            for (int i = 0; i <= nNotches; i++)
                if (x1 + (x2 - x1) / nNotches * i <= curX)
                    gfx.FillEllipse(brushGreen, x1 + (x2 - x1) / nNotches * i - 2, y - 5, 10, 10);
                else
                    gfx.FillEllipse(endX >= startX ? brushBlack : brushRed, x1 + (x2 - x1) / nNotches * i - 2, y - 5, 10, 10);

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
                    gfx.DrawImage(currAnsw == 1 ? picRight.Image : picWrong.Image, 10, rtbDef.Top + 20 * (i + skipLines) + 2, 20, 16);

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
            lb--;

            if (lb != -1)
                mtbDef.Select(lb, 0);
        }

        void mtbDef_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //enter
            {
                finishFillDefinitions();
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
            word = word.ToLower().Replace("’", "'");
            string correctAnswer = getCorrectAnswer().Replace("’", "'");

            bool success = word == correctAnswer;

            //when testing non-English words the correctAnswer may include several lines; only one needs to match the user's answer
            if (!success && correctAnswer.Contains(Environment.NewLine))
                foreach (string line in correctAnswer.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    if (line == word)
                    {
                        success = true;
                        break;
                    }

            //if testing recall ignore hyphen mistakes & diacritic letter mistakes
            if (!success && testWord.archived)
            {
                //hyphens
                success = word.ToLower().Replace("-", " ") == correctAnswer.Replace("-", " ") //hyphen or space mistake
                    || word.ToLower().Replace("-", "") == correctAnswer.Replace("-", ""); //no hyphen mistake

                if (success)
                    MessageBox.Show("Nevertheless your answer will be accepted.", "You messed up one of the hyphens.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    //diacritic letters
                    success = Misc.RemoveDiacritics(word.ToLower()) == Misc.RemoveDiacritics(correctAnswer);
                    if (success)
                        MessageBox.Show("Nevertheless your answer will be accepted.", "You messed up one of the diacritic letters.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        //BOTH???
                        success = Misc.RemoveDiacritics(word.ToLower().Replace("-", " ")) == Misc.RemoveDiacritics(correctAnswer.Replace("-", " "));
                        if (success)
                            MessageBox.Show("Nevertheless your answer will be accepted.", "You messed up one of the hyphens AND one of the diacritic letters.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            return success;
        }

        string getCorrectAnswer()
        {
            string correctAnswer = testWord.ToString().ToLower();
            if (lblTestWordDef.Text.Contains("What does this word mean?"))
                correctAnswer = testWord.GetDefinition();

            return correctAnswer;
        }

        void showResults(bool betweenQuestions)
        {
            int nRemaining = words.Count;
            if (!betweenQuestions)
                nRemaining++;

            string title;
            if (nRemaining == 0)
                title = "No more words ready for testing.";
            else
                title = nRemaining + (nRemaining != 1 ? " words remaining." : " word remaining.");

            double elapsedTime = totalTime.TotalSeconds;
            int nAnswered = nTests - nRemaining;

            string msg = "Correctly answered " + nCorrectAnswers + " / " + nAnswered + (nAnswered != 1 ? " questions in " : " question in ") + Misc.FormatTime(elapsedTime);

            if (nAnswered > 0)
            {
                msg += Environment.NewLine + "Success rate: " + (int)(100.0f * nCorrectAnswers / nAnswered) + "%.";
                msg += Environment.NewLine + "Average time spent per question: " + Misc.FormatTime(elapsedTime / nAnswered);
            }

            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        int calcNotchX(int notch)
        {
            return lblSynonyms.Left + 10 + (this.Width - 70 - lblSynonyms.Left - 10) / nNotches * (notch - 1);
        }


        public formTestRecall()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (timerProgressChange.Enabled)
                drawProgress();
        }

        private void formTestRecall_Load(object sender, EventArgs e)
        {
            setupUI();

            if (main.Profile == "English")
                nNotches = 6;
            else
                nNotches = 4;
            
            //icon & images
            if (File.Exists(Application.StartupPath + "\\Wordy.ico"))
                this.Icon = new Icon(Application.StartupPath + "\\Wordy.ico");

            if (File.Exists(Application.StartupPath + "\\ui\\1.png"))
                picRight.Image = new Bitmap(Application.StartupPath + "\\ui\\1.png");
            if (File.Exists(Application.StartupPath + "\\ui\\2.png"))
                picWrong.Image = new Bitmap(Application.StartupPath + "\\ui\\2.png");

            corewords = Misc.LoadCoreWords();

            lineHeight = lblDef.CreateGraphics().MeasureString("1", lblDef.Font).Height;

            //set volume to 25%
            waveOutSetVolume(IntPtr.Zero, 0x4444);

            //start testing
            noMoreWords = false;
            nTests = 0;
            rand = new Random((int)DateTime.Now.Ticks);
            totalTime = new TimeSpan(0);

            nextWord();
        }

        private void formTestRecall_Resize(object sender, EventArgs e)
        {
            setupUI();
        }

        private void formTestRecall_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool betweenQuestions = timerProgressChange.Enabled || buttNext.Visible || picRight.Visible || noMoreWords;
            
            timerProgressChange.Enabled = false;

            showResults(betweenQuestions);
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
            else if (char.IsDigit((char)e.KeyData))
            {
                int num = (char)e.KeyData - 49;

                if (buttPickWord1.Visible && num >= 0 && num < 6)
                {
                    answer(correctPick == num);
                    e.Handled = e.SuppressKeyPress = true;
                }
                else if (num >= 0 && num < chklistDefs.Items.Count)
                    chklistDefs.SetItemChecked(num, !chklistDefs.GetItemChecked(num));
            }
            else if (char.IsLetter((char)e.KeyData) && buttPickWord1.Visible)
            {
                //pressing Q W E is the same as pressing 4 5 6
                switch (e.KeyData)
                {
                    case Keys.Q:
                        answer(correctPick == 3);
                        break;
                    case Keys.W:
                        answer(correctPick == 4);
                        break;
                    case Keys.E:
                        answer(correctPick == 5);
                        break;
                }
            }
        }

        private void chklistDefs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) //don't call OnKeyDown when Enter is pressed because the function gets called twice
                this.OnKeyDown(e);
        }

        private void buttFinished_Click(object sender, EventArgs e)
        {
            if (mtbTestWord.Visible)
            {
                if (mtbTestWord.MaskCompleted || MessageBox.Show("You haven't filled out every blank.", "Skip question?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                    if (mtbTestWord.Text != "" || MessageBox.Show("You haven't entered any text.", "Skip question?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                        answer(checkEnteredWord(mtbTestWord.Text));
            }
            else if (!chklistDefs.Visible)
                finishFillDefinitions();
            else if (chklistDefs.CheckedItems.Count > 0 || MessageBox.Show("You haven't selected a single definition.", "Skip question?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                finishSelectDefinitions();
        }

        private void buttFinishedLearned_Click(object sender, EventArgs e)
        {
            if (textTestWord.Text != "" || MessageBox.Show("You haven't entered any text.", "Skip question?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                answer(checkEnteredWord(textTestWord.Text));
        }

        private void buttSkip_Click(object sender, EventArgs e)
        {
            if (chklistDefs.Visible)
                finishSelectDefinitions();
            else if (mtbTestWord.Visible)
                answer(checkEnteredWord(mtbTestWord.Text));
            else
                answer(checkAnswers());
        }

        private void buttSkipLearned_Click(object sender, EventArgs e)
        {
            answer(checkEnteredWord(textTestWord.Text));
        }

        private void buttAnotherExample_Click(object sender, EventArgs e)
        {
            if (buttAnotherExample.Visible) //only enabled during questions
            {
                //do we have any examples?
                if (exampleSentences == null || exampleSentences.Length == 0)
                    return;

                //find current example index
                int curExample;
                for (curExample = 0; curExample < exampleSentences.Length; curExample++)
                    if (exampleSentences[curExample] == lblDef.Text)
                        break;

                //display next example
                curExample = (curExample + 1) % exampleSentences.Length;
                lblDef.Text = exampleSentences[curExample];

                //readjust button positions
                buttAnotherExample.Top = lblDef.Top + lblDef.Height + 16;
                buttFinished.Top = buttAnotherExample.Top + buttAnotherExample.Height + 8;
                buttSkip.Top = buttFinished.Top;
            }
        }

        private void buttNext_Click(object sender, EventArgs e)
        {
            timerProgressChange.Enabled = false;
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
            this.Invalidate(); //draw progress indicator

            if (startNotch == endNotch) //if no animation
            {
                if (!buttNext.Visible)
                    //wait 2 seconds until automatically proceeding to next question
                    if (DateTime.Now >= gotoNextQuestion)
                    {
                        timerProgressChange.Enabled = false;
                        nextWord();
                    }
                //if buttNext IS visible then the user needs to click it to proceed
            }
            else
            {
                //progress change animation
                if (animRatio == 1)
                {
                    //animation ended
                    if (endNotch > startNotch)
                    {
                        //process to next question
                        timerProgressChange.Enabled = false;
                        nextWord();
                    }
                    else
                        //let the user click the button to proceed
                        buttNext.Visible = true;
                }
                else
                    //update animation
                    animRatio = Math.Min(animRatio + 0.03f, 1.0f);
            }
        }

        private void textTestWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //enter
            {
                buttFinishedLearned.PerformClick();
                e.Handled = true;
            }
        }

        private void mtbTestWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (mtbTestWord.Visible && e.KeyChar == 13) //still testing && enter pressed
            {
                buttFinished.PerformClick();
                e.Handled = true;
            }
        }

        private void picWordnik_Click(object sender, EventArgs e)
        {
            if (lblWord.Text != "")
                Process.Start("http://www.wordnik.com/words/" + lblWord.Text.Replace(" forgotten!", "").Replace(" learned!", "").Replace(" relearned!", ""));
        }
    }
}