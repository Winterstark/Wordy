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
using GenericForms;

namespace Wordy
{
    public partial class formMain : Form
    {
        const double VERSION = 1.21;
        const string UPDATE_URL = "https://raw.githubusercontent.com/Winterstark/Wordy/master/update/update.txt";

        List<Entry> words;
        Dictionary<string, List<Entry>> foreignWords;
        string[] extraWords, extraForeignWords;
        public List<WordOfTheDay> wotds;
        public Dictionary<string, string> Languages;
        public Preferences prefs;
        formOptions options;
        formAddLanguage addLanguage;
        public string Profile;
        public BackgroundWorker WotdWorker;
        public bool needWotDCheck = true;
        bool checkingWotDs = false;


        List<Entry> loadWords(string path)
        {
            List<Entry> words = new List<Entry>();
            StreamReader fRdr = new StreamReader(path);

            string line, word, defsTxt, synonyms, rhymes;
            DateTime created, learned, lastTest, nextTest;
            int learningPhase, nStudyAttempts, nRecallAttempts, nRecallSuccesses;
            bool archived;

            while (!fRdr.EndOfStream)
            {
                line = fRdr.ReadLine();
                while (line != "<entry>" && !fRdr.EndOfStream)
                    line = fRdr.ReadLine();
                if (fRdr.EndOfStream)
                    break;

                word = fRdr.ReadLine();

                line = fRdr.ReadLine();
                if (line != "[defs]")
                    throw new Exception("error while loading entries");

                defsTxt = "";
                line = fRdr.ReadLine();
                while (line != "[/defs]")
                {
                    defsTxt += line + Environment.NewLine;
                    line = fRdr.ReadLine();
                }

                defsTxt = defsTxt.Substring(0, defsTxt.Length - 2);
                synonyms = fRdr.ReadLine();
                rhymes = fRdr.ReadLine();

                created = DateTime.Parse(fRdr.ReadLine());
                learned = DateTime.Parse(fRdr.ReadLine());
                lastTest = DateTime.Parse(fRdr.ReadLine());
                nextTest = DateTime.Parse(fRdr.ReadLine());
                
                learningPhase = int.Parse(fRdr.ReadLine());
                nStudyAttempts = int.Parse(fRdr.ReadLine());
                nRecallAttempts = int.Parse(fRdr.ReadLine());
                nRecallSuccesses = int.Parse(fRdr.ReadLine());

                archived = bool.Parse(fRdr.ReadLine());

                line = fRdr.ReadLine();
                if (line != "</entry>")
                    throw new Exception("error while loading entries");

                words.Add(new Entry(word, new Definition(defsTxt), synonyms, rhymes, created, learned, lastTest, nextTest, learningPhase, nStudyAttempts, nRecallAttempts, nRecallSuccesses, archived));
            }

            fRdr.Close();

            return words;
        }

        void loadExtraForeignWords()
        {
            string path = Application.StartupPath + "\\languages\\random words\\" + Languages[Profile] + ".txt";
            if (File.Exists(path))
            {
                StreamReader file = new StreamReader(path);
                extraForeignWords = file.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                file.Close();
            }
            else
                extraForeignWords = new string[] { "ab", "fed", "nawat", "yyndl", "obl", "marat" };
        }

        public void LoadSubs(RunWorkerCompletedEventHandler wotdWorker_RunWorkerCompleted)
        {
            WotdWorker = new BackgroundWorker();
            WotdWorker.DoWork += new DoWorkEventHandler(wotdWorker_DoWork);
            WotdWorker.RunWorkerCompleted += wotdWorker_RunWorkerCompleted;

            checkingWotDs = true;
            WotdWorker.RunWorkerAsync(wotds);
        }

        void wotdWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            wotds = new List<WordOfTheDay>();

            StreamReader fRdr = new StreamReader(Application.StartupPath + "\\wotds.txt");
            while (!fRdr.EndOfStream)
                wotds.Add(new WordOfTheDay(fRdr.ReadLine(), fRdr.ReadLine(), bool.Parse(fRdr.ReadLine()), fRdr.ReadLine()));
            fRdr.Close();

            e.Result = wotds;
        }

        void wotdWorker_RunWorkerCompleted_ShowButton(object sender, RunWorkerCompletedEventArgs e)
        {
            wotds = (List<WordOfTheDay>)e.Result;

            checkWotDs();

            prefs.LastFeedCheck = DateTime.Now;
            prefs.Save();

            checkingWotDs = false;
            lblInfo.Text = "";
        }

        void wotdWorker_RunWorkerCompleted_ClickButton(object sender, RunWorkerCompletedEventArgs e)
        {
            wotdWorker_RunWorkerCompleted_ShowButton(sender, e); //save wotds
            buttNewWotD.PerformClick(); //click button to add new wotds
        }

        public void wotdWorker_RunWorkerCompleted_WotDOptions(object sender, RunWorkerCompletedEventArgs e)
        {
            wotdWorker_RunWorkerCompleted_ShowButton(sender, e); //save wotds

            if (options != null)
                options.DisplaySubs(); //display wotd options
        }

        public bool WordExists(string word)
        {
            return GetWords().Any(e => e.ToString() == word);
        }
        
        public void SaveWords()
        {
            if (Profile == "English")
            {
                StreamWriter fWrtr = new StreamWriter(Application.StartupPath + "\\words.txt");

                foreach (Entry word in words)
                    fWrtr.WriteLine(word.FormatEntry());

                fWrtr.Close();
            }
            else
            {
                StreamWriter fWrtr = new StreamWriter(Application.StartupPath + "\\languages\\words-" + Languages[Profile] + ".txt");

                foreach (Entry word in foreignWords[Profile])
                    fWrtr.WriteLine(word.FormatEntry());

                fWrtr.Close();
            }
        }

        public void SaveSubs()
        {
            StreamWriter fWrtr = new StreamWriter(Application.StartupPath + "\\wotds.txt");

            foreach (WordOfTheDay wotd in wotds)
                fWrtr.WriteLine(wotd.FormatWotD());

            fWrtr.Close();
        }

        public void AddNewWord(string word, Definition definitions, string synonyms, string rhymes)
        {
            GetWords().Add(new Entry(word, definitions, Entry.FormatCommas(synonyms), rhymes));
            SaveWords();
        }

        public void AddNewWords(Dictionary<string, Definition> newWords, Dictionary<string, string> synonyms, Dictionary<string, string> rhymes)
        {
            var relevantWords = GetWords();
            foreach (KeyValuePair<string, Definition> word in newWords)
                relevantWords.Add(new Entry(word.Key, word.Value, Entry.FormatCommas(synonyms[word.Key]), rhymes[word.Key]));

            SaveWords();
        }

        List<string> GetRandWords(int n, string exception, List<Entry> relevantWords, string[] relevantExtraWords)
        {
            string[] testWordDefs = null;
            if (relevantWords.Any(w => w.ToString() == exception))
                testWordDefs = relevantWords.Find(w => w.ToString() == exception).GetDefinition().ToLower().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            List<string> wordsCopy = new List<string>(), randWords = new List<string>();

            foreach (Entry word in relevantWords)
                if (word.ToString() != exception)
                {
                    //ignore words that are synonyms of the exception word
                    bool synonym = false;

                    if (testWordDefs != null)
                        foreach (string def in word.GetDefinition().ToLower().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                            if (testWordDefs.Contains(def))
                            {
                                synonym = true;
                                break;
                            }

                    if (!synonym)
                        wordsCopy.Add(word.ToString());
                }

            Random rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < n && wordsCopy.Count > 0; i++)
            {
                int ind = rand.Next(wordsCopy.Count);

                randWords.Add(wordsCopy[ind].ToString());
                wordsCopy.RemoveAt(ind);
            }

            //need more words?
            if (randWords.Count < n)
                for (int i = rand.Next(relevantExtraWords.Length); randWords.Count < n; i++)
                    randWords.Add(relevantExtraWords[i % relevantExtraWords.Length]);

            return randWords;
        }

        public List<string> GetRandWords(int n, string exception)
        {
            if (Profile == "English")
                return GetRandWords(n, exception, words, extraWords);
            else
            {
                //for non-English tests use the English words that appear in that word archive (so it is more difficult for the user to eliminate wrong answers)
                List<string> defs = new List<string>();

                foreach (Entry word in foreignWords[Profile])
                    defs.AddRange(word.GetDefinition().ToLower().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

                string[] exceptions = exception.ToLower().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                defs.RemoveAll(w => exceptions.Contains(w));

                List<string> randDefs = new List<string>();
                Random rand = new Random((int)DateTime.Now.Ticks);

                for (int i = 0; i < n; i++)
                {
                    int ind = rand.Next(defs.Count);
                    
                    randDefs.Add(defs[ind]);
                    defs.RemoveAt(ind);
                }

                //need more words?
                if (randDefs.Count < 9)
                    randDefs.AddRange(GetRandWords(9 - randDefs.Count, exception, words, extraWords));

                return randDefs;
            }
        }

        public List<string> GetRandForeignWords(int n, string exception)
        {
            return GetRandWords(n, exception, GetWords(), extraForeignWords);
        }

        public List<string> GetRandDefs(int n, string ignoreWord)
        {
            //create a list of words from which definitions can be taken
            List<string> availableWords = new List<string>();

            foreach (Entry word in words)
                if (word.ToString() != ignoreWord)
                    availableWords.Add(word.ToString());

            //create a list of backup definitions (in case there are not enough words in Wordy's archive to fulfill the request)
            List<string> randDefs = new List<string>();

            randDefs.Add("(n.) Confused, rambling, or incoherent discourse; nonsense.");
            randDefs.Add("(n.) A sneaker or rubber overshoe.");
            randDefs.Add("(adj.) Heavy with or as if with moisture or water.");
            randDefs.Add("(adj.) Catlike; stealthy.");
            randDefs.Add("(adv.) without partiality; fairly.");
            randDefs.Add("(adv.) From side to side; crosswise or transversely.");
            randDefs.Add("(v.) To censure scathingly.");
            randDefs.Add("(v.) To complain or protest with great hostility.");
            randDefs.Add("(n.) A calculated move.");
            randDefs.Add("(n.) A piece of wood driven into a wall to act as an anchor for nails.");

            //grab random definitions
            Random rand = new Random((int)DateTime.Now.Ticks);
            List<string> defs = new List<string>();
            int ind;

            for (int i = 0; i < n; i++)
            {
                if (availableWords.Count > 0)
                {
                    //take 1 definition per word
                    ind = rand.Next(availableWords.Count);

                    string[] defList = words.Find(w => w.ToString() == availableWords[ind]).GetDefinition().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(d => d[0] != '"' && d.Substring(0, 8) != "(Source)").ToArray();
                    defs.Add(defList[rand.Next(defList.Length)]);

                    availableWords.RemoveAt(ind);
                }
                else
                {
                    //take a definition from the backup defs
                    ind = rand.Next(randDefs.Count);

                    defs.Add(randDefs[ind]);
                    randDefs.RemoveAt(ind);
                }
            }

            return defs;
        }

        public List<Entry> GetWordsReadyForTesting(bool archived)
        {
            if (!archived)
                return copyList(GetWords().Where(w => !w.archived && w.CanTest()).ToList());
            else
                return copyList(GetWords().Where(w => w.archived && w.CanTest()).ToList());
        }

        List<Entry> copyList(List<Entry> orig)
        {
            List<Entry> copy = new List<Entry>();

            foreach (Entry word in orig)
                copy.Add(word);

            return copy;
        }

        void checkWotDs()
        {
            foreach (WordOfTheDay wotd in wotds)
                if (wotd.active && wotd.AnyNewPosts())
                {
                    prefs.NewWotDs = true;
                    break;
                }

            if (Profile == "English")
                buttNewWotD.Visible = prefs.NewWotDs;
            prefs.Save();
        }

        void setInfo(string info)
        {
            if (!buttNewWotD.Visible)
                lblInfo.Text = info;
        }

        void displayWordCount(List<Entry> wordList)
        {
            //display how many words are ready to be tested
            int n = wordList.Count;

            if (n == 0)
                setInfo("You have no words ready to be tested.");
            else if (n == 1)
                setInfo("You have 1 word ready to be tested.");
            else
                setInfo("You have " + n + " words ready to be tested.");
        }

        public void LoadActiveLanguages()
        {
            ComboLanguage.Items.Clear();
            ComboLanguage.Items.Add("English");
            foreach (var lang in Languages)
                if (File.Exists(Application.StartupPath + "\\languages\\words-" + lang.Value + ".txt"))
                    ComboLanguage.Items.Add(lang.Key);
            ComboLanguage.Items.Add("Add another language...");
            ComboLanguage.SelectedIndex = 0;

            Profile = "English";

            foreignWords = new Dictionary<string, List<Entry>>();
            foreach (var lang in Languages)
            {
                string path = Application.StartupPath + "\\languages\\words-" + lang.Value + ".txt";
                if (File.Exists(path))
                    foreignWords.Add(lang.Key, loadWords(path));
            }
        }

        public string GetNewWordsPath()
        {
            if (Profile == "English")
                return prefs.NewWordsPath;
            else
                return Application.StartupPath + "\\languages\\newwords-" + Languages[Profile] + ".txt";
        }

        public List<Entry> GetWords()
        {
            if (Profile == "English")
                return words;
            else
                return foreignWords[Profile];
        }


        public formMain()
        {
            InitializeComponent();
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            prefs = new Preferences();
            
            needWotDCheck = DateTime.Now.DayOfYear != prefs.LastFeedCheck.DayOfYear;
            if (!needWotDCheck)
                buttNewWotD.Visible = prefs.NewWotDs;

            words = loadWords(Application.StartupPath + "\\words.txt");

            //load extra English words
            string pathExtraWords = Application.StartupPath + "\\languages\\random words\\en.txt";
            if (File.Exists(pathExtraWords))
            {
                StreamReader fileExtraWords = new StreamReader(pathExtraWords);
                extraWords = fileExtraWords.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                fileExtraWords.Close();
            }
            else
                extraWords = new string[] { "instauration", "spindrift", "chouse", "alight", "amanuensis", "vitriform" };

            //app icon
            if (File.Exists(Application.StartupPath + "\\Wordy.ico"))
                this.Icon = new Icon(Application.StartupPath + "\\Wordy.ico");

            //ui icons
            if (File.Exists(Application.StartupPath + "\\ui\\add.png"))
                buttAdd.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\add.png");

            if (File.Exists(Application.StartupPath + "\\ui\\study new.png"))
                buttStudyWords.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\study new.png");

            if (File.Exists(Application.StartupPath + "\\ui\\test recall.png"))
                buttRecall.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\test recall.png");

            if (File.Exists(Application.StartupPath + "\\ui\\new wotd.png"))
                buttNewWotD.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\new wotd.png");

            if (File.Exists(Application.StartupPath + "\\ui\\reading.png"))
                buttReading.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\reading.png");

            if (File.Exists(Application.StartupPath + "\\ui\\review.png"))
                buttReview.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\review.png");

            if (File.Exists(Application.StartupPath + "\\ui\\options.png"))
                buttOptions.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\options.png");

            if (File.Exists(Application.StartupPath + "\\ui\\about.png"))
                buttAbout.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\about.png");

            //setup other languages
            Languages = new Dictionary<string, string>();
            StreamReader file = new StreamReader(Application.StartupPath + "\\languages\\!available languages.txt");

            while (!file.EndOfStream)
            {
                string[] langKeyValue = file.ReadLine().Split(new string[] { " -> " }, StringSplitOptions.RemoveEmptyEntries);
                Languages.Add(langKeyValue[1], langKeyValue[0]); //Language name -> code
            }

            file.Close();

            LoadActiveLanguages();

            //check for updates
            Updater.Update(VERSION, UPDATE_URL);

            //show tutorial
            new Tutorial(Application.StartupPath + "\\tutorials\\main.txt", this);
        }

        private void formMain_Activated(object sender, EventArgs e)
        {
            if (needWotDCheck)
            {
                needWotDCheck = false;

                lblInfo.Text = "Checking for new Words of the Day" + Environment.NewLine + "Please wait a moment...";
                this.Refresh();

                LoadSubs(wotdWorker_RunWorkerCompleted_ShowButton);
            }
        }

        private void buttAdd_Click(object sender, EventArgs e)
        {
            formAddWords addWords = new formAddWords();

            addWords.main = this;
            addWords.chkNewWordsFile = true;

            addWords.Show();
            this.Hide();
        }

        private void buttOptions_Click(object sender, EventArgs e)
        {
            options = new formOptions();

            options.main = this;
            options.words = GetWords();
            options.wotds = wotds;
            options.CurrentVersion = VERSION;
            options.DefaultUpdateURL = UPDATE_URL;

            options.Show();
            this.Hide();
        }

        private void buttAbout_Click(object sender, EventArgs e)
        {
            formAbout about = new formAbout();

            about.main = this;

            about.lblVersion.Text = "v" + VERSION.ToString().Replace(',', '.');
            if (!about.lblVersion.Text.Contains("."))
                about.lblVersion.Text += ".0";

            about.Show();
            this.Hide();
        }
        
        private void buttStudyWords_Click(object sender, EventArgs e)
        {
            var relevantWords = GetWords();

            if (relevantWords.Any(w => !w.archived && w.CanTest()))
            {
                formTestRecall test = new formTestRecall();

                test.main = this;
                test.testUnlearned = true;

                test.Show();
                this.Hide();
            }
            else if (relevantWords.Any(w => !w.archived))
                MessageBox.Show("No new words that haven't been tested recently.");
            else
                MessageBox.Show("No new words.");
        }

        private void buttRecall_Click(object sender, EventArgs e)
        {
            if (GetWords().Any(w => w.archived && w.CanTest()))
            {
                formTestRecall test = new formTestRecall();

                test.main = this;
                test.testUnlearned = false;

                test.Show();
                this.Hide();
            }
            else
                MessageBox.Show("No learned words that haven't been tested recently.");
        }

        private void buttNewWotD_Click(object sender, EventArgs e)
        {
            if (wotds == null)
                LoadSubs(wotdWorker_RunWorkerCompleted_ClickButton);
            else
            {
                prefs.NewWotDs = false;
                prefs.Save();

                formAddWords addWords = new formAddWords();
                addWords.main = this;
                addWords.LoadWotDs();
                addWords.Show();

                this.Hide();
                buttNewWotD.Visible = false;
            }
        }

        private void buttReading_Click(object sender, EventArgs e)
        {
            formReading reading = new formReading();
            reading.main = this;
            reading.words = GetWords();
            reading.Show();
            this.Hide();
        }

        private void buttReview_Click(object sender, EventArgs e)
        {
            if (words.Count > 0)
            {
                formReview review = new formReview();

                review.main = this;
                review.words = GetWords();
                review.Show();
                this.Hide();
            }
            else
                MessageBox.Show("You have no words! Use the Add New Words button first.");
        }

        private void control_MouseLeave(object sender, EventArgs e)
        {
            if (!checkingWotDs)
                lblInfo.Text = "";
            else
                lblInfo.Text = "Checking for new Words of the Day" + Environment.NewLine + "Please wait a moment..."; ;
        }

        private void buttAdd_MouseEnter(object sender, EventArgs e)
        {
            //get number of new words in text file
            int n = Misc.LoadNewWordsFromFile(GetNewWordsPath()).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;

            if (n == 0)
                setInfo("Add new words that you want to study.");
            else
                setInfo("Add new words that you want to study." + Environment.NewLine + "You have " + n + " new words to add from your designated text file.");
        }

        private void buttStudyWords_MouseEnter(object sender, EventArgs e)
        {
            List<Entry> availableWords = GetWordsReadyForTesting(false);

            if (availableWords.Count == 0)
            {
                //find out which word will become available first
                DateTime earliest = new DateTime(2305, 7, 13);

                foreach (Entry word in GetWords())
                    if (!word.archived && word.GetLastTest() < earliest)
                        earliest = word.GetLastTest();

                if (earliest.Year != 2305)
                    setInfo("The next word will become available for testing in " + Misc.FormatTime(earliest.AddHours(22).Subtract(DateTime.Now).TotalSeconds));
                else
                    setInfo("You have no words ready to be tested.");
            }
            else
                displayWordCount(availableWords);
        }

        private void buttRecall_MouseEnter(object sender, EventArgs e)
        {
            List<Entry> availableWords = GetWordsReadyForTesting(true);

            if (availableWords.Count == 0)
            {
                //find out which word will become available first
                DateTime earliest = new DateTime(2305, 7, 13);

                foreach (Entry word in GetWords())
                    if (word.archived && word.GetNextTest() < earliest)
                        earliest = word.GetNextTest();

                if (earliest.Year != 2305)
                    setInfo("The next word will become available for testing in " + Misc.FormatTime(earliest.Subtract(DateTime.Now).TotalSeconds));
                else
                    setInfo("You have no words ready to be tested.");
            }
            else
                displayWordCount(availableWords);
        }

        private void buttReading_MouseEnter(object sender, EventArgs e)
        {
            setInfo("Read a text extract with the help of quick word lookups.");
        }

        private void buttReview_MouseEnter(object sender, EventArgs e)
        {
            setInfo("View a list of all the words that you have added.");
        }

        private void buttOptions_MouseEnter(object sender, EventArgs e)
        {
            setInfo("Edit your word collection, Word of the Day subscriptions, and other options.");
        }

        private void buttAbout_MouseEnter(object sender, EventArgs e)
        {
            setInfo("Display information about Wordy.");
        }

        private void comboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboLanguage.Text == "Add another language...")
            {
                if (addLanguage == null || addLanguage.IsDisposed)
                {
                    addLanguage = new formAddLanguage();
                    addLanguage.main = this;
                }

                addLanguage.Show();

                ComboLanguage.Text = Profile;
            }
            else if (ComboLanguage.SelectedIndex != -1)
            {
                string flagPath;
                if (ComboLanguage.Text == "English")
                    flagPath = Application.StartupPath + "\\languages\\flags\\en.png";
                else
                    flagPath = Application.StartupPath + "\\languages\\flags\\" + Languages[ComboLanguage.Text] + ".png";

                if (File.Exists(flagPath))
                    picFlag.ImageLocation = flagPath;
                else
                    picFlag.ImageLocation = "";

                Profile = ComboLanguage.Text;

                if (Profile == "English")
                    buttNewWotD.Visible = prefs.NewWotDs;
                else
                {
                    loadExtraForeignWords();
                    buttNewWotD.Visible = false;
                }
            }
        }

        private void comboLanguage_MouseEnter(object sender, EventArgs e)
        {
            setInfo("Change current language profile.");
        }
    }
}
