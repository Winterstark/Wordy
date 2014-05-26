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
        const double VERSION = 1.0;

        List<Entry> words;
        public List<WordOfTheDay> wotds;
        public Preferences prefs;
        public bool needWotDCheck = true;


        void loadWords()
        {
            words = new List<Entry>();

            StreamReader fRdr = new StreamReader(Application.StartupPath + "\\words.txt");

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
        }

        public void LoadSubs()
        {
            wotds = new List<WordOfTheDay>();

            StreamReader fRdr = new StreamReader(Application.StartupPath + "\\wotds.txt");
            while (!fRdr.EndOfStream)
                wotds.Add(new WordOfTheDay(fRdr.ReadLine(), fRdr.ReadLine(), bool.Parse(fRdr.ReadLine()), fRdr.ReadLine()));
            fRdr.Close();
        }

        public bool WordExists(string word)
        {
            return words.Any(e => e.ToString() == word);
        }
        
        public void SaveWords()
        {
            StreamWriter fWrtr = new StreamWriter(Application.StartupPath + "\\words.txt");

            foreach (Entry word in words)
                fWrtr.WriteLine(word.FormatEntry());

            fWrtr.Close();
        }

        public void SaveSubs()
        {
            StreamWriter fWrtr = new StreamWriter(Application.StartupPath + "\\wotds.txt");

            foreach (WordOfTheDay wotd in wotds)
                fWrtr.WriteLine(wotd.FormatWotD());

            fWrtr.Close();
        }

        public void AddNewWords(Dictionary<string, Definition> newWords, Dictionary<string, string> synonyms, Dictionary<string, string> rhymes)
        {
            foreach (KeyValuePair<string, Definition> word in newWords)
                words.Add(new Entry(word.Key, word.Value, Entry.FormatCommas(synonyms[word.Key]), rhymes[word.Key]));

            SaveWords();
        }

        public List<string> GetRandWords(int n, string exception)
        {
            List<string> wordsCopy = new List<string>(), randWords = new List<string>();

            foreach (Entry word in words)
                if (word.ToString() != exception)
                    wordsCopy.Add(word.ToString());

            Random rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < n && wordsCopy.Count > 0; i++)
            {
                int ind = rand.Next(wordsCopy.Count);

                randWords.Add(wordsCopy[ind].ToString());
                wordsCopy.RemoveAt(ind);
            }

            //need more words?
            if (randWords.Count < n)
            {
                string[] constWords = { "defenestration", "palimpsest", "sagittipotent", "slayer", "rarity", "skald" };

                for (int i = randWords.Count; i < n; i++)
                    randWords.Add(constWords[i % constWords.Length]);
            }

            return randWords;
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

            loadWords();

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

            if (File.Exists(Application.StartupPath + "\\ui\\review.png"))
                buttReview.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\review.png");

            if (File.Exists(Application.StartupPath + "\\ui\\options.png"))
                buttOptions.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\options.png");

            if (File.Exists(Application.StartupPath + "\\ui\\about.png"))
                buttAbout.Image = Bitmap.FromFile(Application.StartupPath + "\\ui\\about.png");
            

            //show tutorial
            new Tutorial(Application.StartupPath + "\\tutorials\\main.txt", this);

            //check for update
            bool[] askPermissions = new bool[3] { true, true, true };
            for (int i = 0; i < prefs.UpdateNotifs; i++)
                askPermissions[i] = false;

            Updater.Update(VERSION, "https://raw.github.com/Winterstark/Wordy/master/update/update.txt", askPermissions, prefs.ShowChangelog);
        }

        private void formMain_Activated(object sender, EventArgs e)
        {
            if (needWotDCheck)
            {
                needWotDCheck = false;

                lblInfo.Text = "Checking for new Words of the Day" + Environment.NewLine + "Please wait a moment...";
                this.Refresh();

                LoadSubs();
                checkWotDs();

                prefs.LastFeedCheck = DateTime.Now;
                prefs.Save();

                lblInfo.Text = "";
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
            formOptions wordlist = new formOptions();

            wordlist.main = this;
            wordlist.words = words;
            wordlist.wotds = wotds;

            wordlist.Show();
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
            if (words.Any(w => !w.archived && w.CanTest()))
            {
                formTestRecall test = new formTestRecall();

                test.main = this;
                test.newWords = true;
                test.words = copyList(words.Where(w => !w.archived && w.CanTest()).ToList());
                test.Text = "Study New Words";

                test.Show();
                this.Hide();
            }
            else if (words.Any(w => !w.archived))
                MessageBox.Show("No new words that haven't been tested recently.");
            else
                MessageBox.Show("No new words.");
        }

        private void buttRecall_Click(object sender, EventArgs e)
        {
            if (words.Any(w => w.archived && w.CanTest()))
            {
                formTestRecall test = new formTestRecall();

                test.main = this;
                test.newWords = false;
                test.words = copyList(words.Where(w => w.archived && w.CanTest()).ToList());

                test.Text = "Test Learned Words";

                test.Show();
                this.Hide();
            }
            else
                MessageBox.Show("No learned words that haven't been tested recently.");
        }

        private void buttNewWotD_Click(object sender, EventArgs e)
        {
            if (wotds == null)
                LoadSubs();

            prefs.NewWotDs = false;
            prefs.Save();

            formAddWords addWords = new formAddWords();
            addWords.main = this;

            addWords.Show();
            this.Hide();

            foreach (WordOfTheDay wotd in wotds)
                if (wotd.active && wotd.AnyNewPosts())
                    addWords.loadWotDs(wotd.getNewWordsLinks(), wotd.getNewWords());

            SaveSubs();
            buttNewWotD.Visible = false;
        }

        private void buttReview_Click(object sender, EventArgs e)
        {
            if (words.Count > 0)
            {
                formReview review = new formReview();

                review.main = this;
                review.words = words;
                review.Show();
                this.Hide();
            }
            else
                MessageBox.Show("You have no words! Use the Add New Words button first.");
        }

        private void button_MouseLeave(object sender, EventArgs e)
        {
            lblInfo.Text = "";
        }

        private void buttAdd_MouseEnter(object sender, EventArgs e)
        {
            //get number of new words in text file
            int n = Misc.LoadNewWordsFromFile(prefs.NewWordsPath).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;

            if (n == 0)
                setInfo("Add new words that you want to study.");
            else
                setInfo("Add new words that you want to study." + Environment.NewLine + "You have " + n + " new words to add from your designated text file.");
        }

        private void buttStudyWords_MouseEnter(object sender, EventArgs e)
        {
            displayWordCount(copyList(words.Where(w => !w.archived && w.CanTest()).ToList()));
        }

        private void buttRecall_MouseEnter(object sender, EventArgs e)
        {
            displayWordCount(copyList(words.Where(w => w.archived && w.CanTest()).ToList()));
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
    }
}
