using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NikSharp;
using NikSharp.Model;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Web;
using System.Windows.Forms;
using System.Net;

namespace Wordy
{
    public class Definition
    {
        public string[] defs;
        public string[] partsOfSpeech;
        bool[] learned;

        public Definition(WordnikDefinition[] wdDefs)
        {
            defs = new string[wdDefs.Length];
            partsOfSpeech = new string[wdDefs.Length];

            for (int i = 0; i < wdDefs.Length; i++)
            {
                defs[i] = wdDefs[i].Text;
                partsOfSpeech[i] = cleanUp(wdDefs[i].PartOfSpeech);
                
                if (defs[i].Contains(":  ")) // quote examples separated only by a colon
                    defs[i] = defs[i].Replace(":  ", " \"") + '"';
                if (defs[i].Contains('”')) // use standard quotes only
                    defs[i] = defs[i].Replace('”', '"');
                if (defs[i].Contains('—')) // replace em dash with standard hyphen
                    defs[i] = defs[i].Replace('—', '-');
                
                //moar cleanup
                int lb = defs[i].IndexOf("See Synonyms");

                while (lb != -1)
                {
                    int ub = defs[i].Length;

                    int altUB = defs[i].IndexOf(Environment.NewLine, lb);
                    if (altUB != -1 && altUB < ub)
                        ub = altUB;

                    altUB = defs[i].IndexOf(".", lb);
                    if (altUB != -1 && altUB < ub)
                        ub = altUB;

                    defs[i] = defs[i].Insert(ub, ")").Insert(lb, "(");

                    lb = defs[i].IndexOf("See Synonyms", ub);
                }
            }

            if (defs.Length > 0)
            {
                //append dictionary sources
                //insert the source in a new line after the definition
                //if multiple definitions come from the same source, insert the new line only once, after all of them
                List<string> newDefs = new List<string>();
                List<string> newParts = new List<string>();

                newDefs.Add(defs[0]);
                newParts.Add(partsOfSpeech[0]);

                string prevSource = wdDefs[0].SourceDictionary;

                for (int i = 1; i < defs.Length; i++)
                {
                    if (wdDefs[i].SourceDictionary != prevSource) //insert source if it's different than the previous definition
                    {
                        newDefs.Add(attribution(prevSource));
                        newParts.Add("Source");
                    }

                    prevSource = wdDefs[i].SourceDictionary;

                    //add definition
                    newDefs.Add(defs[i]);
                    newParts.Add(partsOfSpeech[i]);
                }

                //always insert source after last definition
                newDefs.Add(attribution(wdDefs[wdDefs.Length - 1].SourceDictionary));
                newParts.Add("Source");

                defs = newDefs.ToArray();
                partsOfSpeech = newParts.ToArray();
                learned = new bool[wdDefs.Length];
            }
        }

        string attribution(string source)
        {
            switch (source)
            {
                case "ahd-legacy":
                    return "(from The American Heritage® Dictionary of the English Language, 4th Edition)";
                case "century":
                    return "(from The Century Dictionary and Cyclopedia)";
                case "wiktionary":
                    return "(from Wiktionary, Creative Commons Attribution/Share-Alike License)";
                case "wordnet":
                    return "(from WordNet 3.0 Copyright 2006 by Princeton University. All rights reserved.)";
                case "gcide":
                    return "(from the GNU version of the Collaborative International Dictionary of English)";
                default:
                    return "(from unknown)";
            }
        }

        public Definition(string txtToParse)
        {
            Parse(txtToParse, true);
        }

        public Definition(string summary, bool wotd)
        {
            Parse(cleanUp(summary), false);
            learned = new bool[defs.Length];
        }

        public override string ToString()
        {
            string output = "";

            for (int i = 0; i < defs.Length; i++)
                if (partsOfSpeech[i] != null)
                    output += "(" + partsOfSpeech[i] + ") " + defs[i] + Environment.NewLine;
                else
                    output += defs[i] + Environment.NewLine;

            return output.Substring(0, output.Length - 2);
        }

        public string FormatEntry()
        {
            if (defs.Length > learned.Length) //fix learned array
            {
                List<bool> oldLearned = learned.ToList();

                learned = new bool[defs.Length];
                for (int i = 0; i < oldLearned.Count; i++)
                    learned[i] = oldLearned[i];
            }

            string output = "";

            for (int i = 0; i < defs.Length; i++)
                if (partsOfSpeech[i] != null)
                    output += "(" + partsOfSpeech[i] + ") " + defs[i] + "::" + learned[i].ToString() + Environment.NewLine;
                else
                    output += defs[i] + "::" + learned[i].ToString() + Environment.NewLine;

            return output.Substring(0, output.Length - 2);
        }

        public string GetUnlearnedDefs()
        {
            string output = "";

            for (int i = 0; i < defs.Length; i++)
                if (defs[i][0] != '"' && partsOfSpeech[i] != "Source" && !learned[i])
                {
                    if (partsOfSpeech[i] != null)
                        output += "(" + partsOfSpeech[i] + ") " + defs[i] + Environment.NewLine;
                    else
                        output += defs[i] + Environment.NewLine;
                }

            if (output.Length > 2)
                output.Substring(0, output.Length - 2);

            if (output == "")
            {
                //reset all to false
                for (int i = 0; i < learned.Length; i++)
                    learned[i] = false;

                output = GetUnlearnedDefs();
            }

            return output;
        }

        public void LearnedDefs(Answer[] answCorrectly)
        {
            if (answCorrectly == null)
                return;

            for (int i = 0; i < defs.Length; i++)
                if (defs[i][0] != '"' && partsOfSpeech[i] != "Source" && !learned[i])
                {
                    //find answer
                    foreach (Answer answer in answCorrectly)
                        if (answer.def == "(" + partsOfSpeech[i] + ") " + defs[i])
                        {
                            learned[i] = answer.correct;
                            break;
                        }
                }
        }

        public void Parse(string txt, bool parseLearned)
        {
            if (txt.Contains("\n") && !txt.Contains(Environment.NewLine))
                txt = txt.Replace("\n", Environment.NewLine);

            string[] lines = txt.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            
            defs = new string[lines.Length];
            partsOfSpeech = new string[lines.Length];
            if (parseLearned)
                learned = new bool[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i][0] == '(')
                {
                    defs[i] = lines[i].Substring(lines[i].IndexOf(") ") + 2);
                    partsOfSpeech[i] = lines[i].Substring(1, lines[i].IndexOf(')', 1) - 1);
                }
                else
                    defs[i] = lines[i];

                if (parseLearned)
                {
                    learned[i] = bool.Parse(defs[i].Substring(defs[i].LastIndexOf("::") + 2));
                    defs[i] = defs[i].Substring(0, defs[i].LastIndexOf("::"));
                }
            }
        }

        public void ResetLearned()
        {
            for (int i = 0; i < learned.Length; i++)
                learned[i] = true;
        }

        string cleanUp(string txt)
        {
            //load shortcuts list
            StreamReader fRdr = new StreamReader("shortcuts.txt");
            string[] shortcuts = fRdr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            fRdr.Close();

            //apply shortcuts
            foreach (string shortcut in shortcuts)
                txt = txt.Replace(shortcut.Substring(0, shortcut.IndexOf("->")), shortcut.Substring(shortcut.IndexOf("->") + 2));

            //remove html tags
            txt = txt.Replace("<p>", Environment.NewLine);

            int lb, ub;

            while (txt.Contains('<'))
            {
                lb = txt.IndexOf('<');
                ub = txt.IndexOf('>');

                txt = txt.Remove(lb, ub - lb + 1);
            }

            //cleanup special chars
            txt = HttpUtility.HtmlDecode(txt);
            
            //misc removals
            txt = txt.Replace("Discuss\n", "");

            //check for redundant newlines
            while (txt.Length > 1 && (txt[0] == '\n' || txt[0] == '\r'))
                txt = txt.Remove(0, 1);

            //convert quotes
            txt = txt.Replace('"', '\'');

            if (txt.Contains("Merriam-Webster")) //special cleanup for Merriam-Webster
            {
                txt = txt.Substring(txt.IndexOf("\\", txt.IndexOf("\\") + 1) + 5);

                string wordType = "(" + txt.Substring(0, txt.IndexOf("\n")) + ")";
                txt = wordType + txt.Substring(txt.IndexOf("\n") + 1);

                for (char c = 'a'; c <= 'z' && txt.Contains(" " + c.ToString() + " : "); c++)
                    txt = txt.Replace(" " + c.ToString() + " : ", Environment.NewLine + wordType + " ");
                for (char c = '2'; c <= '9' && txt.Contains(" " + c.ToString() + "     : "); c++)
                    txt = txt.Replace(" " + c.ToString() + "     : ", Environment.NewLine + wordType + " ");

                lb = txt.IndexOf("Examples") - 1;
                ub = txt.IndexOf("\t", lb + 1) + 1;
                while (lb >= 0 && isJunkChar(txt[lb]))
                    lb--;

                txt = txt.Substring(0, lb + 1) + Environment.NewLine + '"' + txt.Substring(ub, txt.IndexOf("\n", ub) - ub) + '"';

                lb = txt.IndexOf(')') + 2;
                if (txt[lb - 1] != ' ')
                    txt = txt.Insert(lb - 1, " ");

                //return txt;
                while (lb != 1)
                {
                    ub = lb;
                    while (ub < txt.Length && isJunkChar(txt[ub]))
                        ub++;

                    if (lb != ub)
                        txt = txt.Remove(lb, ub - lb);

                    if (txt.IndexOf(Environment.NewLine, lb) == -1)
                        break;
                    lb = txt.IndexOf(')', txt.IndexOf(Environment.NewLine, lb)) + 2;
                }

                ub = txt.IndexOf(Environment.NewLine);
                while (ub != -1)
                {
                    lb = ub;
                    while (lb >= 0 && isJunkChar(txt[lb]))
                        lb--;

                    if (lb + 1 != ub)
                        txt = txt.Remove(lb + 1, ub - lb - 1);

                    ub = txt.IndexOf(Environment.NewLine, lb + 2);
                }
            }
            else if (txt.Length > 12 && txt.Substring(0, 12) == "Definition: ") //special cleanup for The Free Dictionary
            {
                txt = txt.Substring(12);

                lb = txt.IndexOf("\n");
                ub = txt.IndexOf("Usage: ") + 7;
                txt = txt.Substring(0, lb) + Environment.NewLine + '"' + txt.Substring(ub);

                ub = txt.Length - 1;
                while (isJunkChar(txt[ub]))
                    ub--;
                if (txt[++ub] == '.')
                    ub++;
                txt = txt.Substring(0, ub) + '"';
            }

            return txt;
        }

        bool isJunkChar(char c)
        {
            return !(char.IsLetter(c) || c == '"' || c == '(' || c == ')');
        }
    }

    public class Entry
    {
        string word;
        Definition def;
        string[] synonyms, rhymes;
        DateTime created, learned, lastTest, nextTest;
        int nStudyAttempts, nRecallAttempts, nRecallSuccesses;
        public int learningPhase;
        public bool archived;

        public Entry(string word, Definition def, string synList, string rhymeList)
        {
            this.word = word;
            this.def = def;

            synonyms = processList(synList);
            rhymes = processList(rhymeList);
            
            created = DateTime.Now;
            learned = DateTime.MinValue;
            lastTest = DateTime.Now;

            learningPhase = 1;
            nStudyAttempts = 0;
            nRecallAttempts = 0;
            nRecallSuccesses = 0;

            archived = false;
        }

        public Entry(string word, Definition def, string synList, string rhymeList, DateTime created, DateTime learned, DateTime lastTest, DateTime nextTest, int learningPhase, int nStudyAttempts, int nRecallAttempts, int nRecallSuccesses, bool archived)
        {
            this.word = word;
            this.def = def;
            synonyms = processList(synList);
            rhymes = processList(rhymeList);
            this.created = created;
            this.learned = learned;
            this.lastTest = lastTest;
            this.nextTest = nextTest;
            this.learningPhase = learningPhase;
            this.nStudyAttempts = nStudyAttempts;
            this.nRecallAttempts = nRecallAttempts;
            this.nRecallSuccesses = nRecallSuccesses;
            this.archived = archived;
        }

        public override string ToString()
        {
            return word;
        }

        public string FormatEntry()
        {
            string txt = "<entry>" + Environment.NewLine + word + Environment.NewLine + "[defs]" + Environment.NewLine + def.FormatEntry() + Environment.NewLine + "[/defs]" + Environment.NewLine;

            foreach (string syn in synonyms)
                txt += syn + ", ";

            txt += Environment.NewLine;

            foreach (string rhyme in rhymes)
                txt += rhyme + ", ";

            txt += Environment.NewLine;

            txt += Misc.ToUniversalString(created) + Environment.NewLine + Misc.ToUniversalString(learned) + Environment.NewLine + Misc.ToUniversalString(lastTest) + Environment.NewLine + Misc.ToUniversalString(nextTest) + Environment.NewLine + learningPhase.ToString() + Environment.NewLine + nStudyAttempts.ToString() + Environment.NewLine + nRecallAttempts.ToString() + Environment.NewLine + nRecallSuccesses.ToString() + Environment.NewLine + archived.ToString() + Environment.NewLine + "</entry>" + Environment.NewLine;

            return txt;
        }

        public string GetDefinition()
        {
            return def.ToString();
        }

        public string GetUnlearnedDefs()
        {
            return def.GetUnlearnedDefs();
        }

        public string GetSynonyms()
        {
            if (synonyms.Length == 0)
                return "";

            string syns = "";

            foreach (string syn in synonyms)
                syns += syn + " / ";

            return syns.Substring(0, syns.Length - 3);
        }

        public List<string> GetRandomSynonyms()
        {
            List<string> syns = new List<string>();

            if (synonyms.Length == 0)
                return syns;

            List<string> synsCopy = new List<string>();
            foreach (string syn in synonyms)
                synsCopy.Add(syn);

            Random rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 5 && synsCopy.Count > 0; i++)
            {
                int ind = rand.Next(synsCopy.Count);

                syns.Add(synsCopy[ind]);
                synsCopy.RemoveAt(ind);
            }

            return syns;
        }

        public List<string> GetRandomRhymes()
        {
            List<string> randRhymes = new List<string>();

            if (rhymes.Length == 0)
                return randRhymes;

            List<string> rhymesCopy = new List<string>();
            foreach (string rhyme in rhymes)
                rhymesCopy.Add(rhyme);

            Random rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 5 && rhymesCopy.Count > 0; i++)
            {
                int ind = rand.Next(rhymesCopy.Count);

                randRhymes.Add(rhymesCopy[ind]);
                rhymesCopy.RemoveAt(ind);
            }

            return randRhymes;
        }

        public void SetDefinition(string newDef)
        {
            def.Parse(newDef, false);
        }

        public void SetSynonyms(string synList)
        {
            synonyms = processList(FormatCommas(synList));
        }

        public string GetInfo()
        {
            string info = "Added on: " + created.ToString("d. MMMM yyyy.");

            if (archived)
                info += Environment.NewLine + "Learned on: " + learned.ToString("d. MMMM yyyy.");

            if (lastTest.Ticks != 0)
                info += Environment.NewLine + "Last test on: " + lastTest.ToString("d. MMMM yyyy.");

            DateTime nextTestUnlearned;
            if (archived)                   //learned
                nextTestUnlearned = nextTest;
            else if (lastTest.Ticks != 0)   //unlearned
                nextTestUnlearned = lastTest.AddDays(1);
            else                            //never tested
                nextTestUnlearned = DateTime.Now;

            if (nextTestUnlearned < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) //correct when next test scheduled on a date in the past
                nextTestUnlearned = DateTime.Now;

            info += Environment.NewLine + "Next test on: " + nextTestUnlearned.ToString("d. MMMM yyyy.");

            if (archived)
                info += Environment.NewLine + "Word learned in " + (nStudyAttempts - 6).ToString() + " extra steps.";
            else
                info += Environment.NewLine + "Current learning step: " + learningPhase.ToString();

            if (nRecallAttempts > 0)
                info += Environment.NewLine + "Recall success: " + Math.Round((float)nRecallSuccesses / nRecallAttempts * 100.0f, 2).ToString() + "%.";
            
            return info;
        }

        public DateTime GetCreationDate()
        {
            return created;
        }

        public int GetNStudyAttempts()
        {
            return nStudyAttempts;
        }

        public float GetRecallSuccessRate()
        {
            if (nRecallAttempts == 0)
                return -1;
            else
                return (float)nRecallSuccesses / nRecallAttempts;
        }

        public DateTime GetLastTest()
        {
            return lastTest;
        }

        public DateTime GetNextTest()
        {
            return nextTest;
        }

        public void LogTest(bool success, Answer[] answCorrectly, int resetLearningPhase)
        {
            if (!archived)
            {
                nStudyAttempts++;

                if (success)
                {
                    learningPhase++;

                    if (learningPhase == 7)
                    {
                        archived = true;
                        nextTest = DateTime.Now.AddDays(7);
                    }
                }
            }
            else
            {
                nRecallAttempts++;

                if (success)
                {
                    nRecallSuccesses++;

                    if (DateTime.Now.Subtract(lastTest).TotalDays < 1) //in case of a bug where last test occured today (probably caused when the user tests a learned word, fails the test, and then in Options checks the word as learned again)
                        nextTest = DateTime.Now.AddDays(7);
                    else
                        nextTest = DateTime.Now.AddDays(nextTest.Subtract(lastTest).Days + 7);
                }
                else
                {
                    archived = false;
                    learningPhase = resetLearningPhase;
                }
            }

            lastTest = DateTime.Now;

            def.LearnedDefs(answCorrectly);
        }

        public bool CanTest()
        {
            if (!archived)
                return (int)(DateTime.Now - lastTest).TotalHours >= 22;
            else
                return DateTime.Now >= nextTest;
        }

        private string[] processList(string synList)
        {
            if (synList != "")
                return synList.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            else
                return new string[0];
        }

        public static string FormatCommas(string synList)
        {
            int ind = synList.IndexOf(',');

            while (synList.Contains("  "))
                synList = synList.Replace("  ", " ");

            while (ind != -1)
            {
                if (ind > 0 && synList[ind - 1] == ' ')
                    synList = synList.Remove(ind-- - 1, 1);

                if (ind == synList.Length - 1 || synList[ind + 1] != ' ')
                    synList = synList.Insert(ind + 1, " ");

                ind = synList.IndexOf(',', ind + 1);
            }

            return synList;
        }

        public void ResetLearned()
        {
            def.ResetLearned();
        }
    }

    public class Preferences
    {
        public DateTime LastFeedCheck;
        public string NewWordsPath;
        public bool AutoVisuals, NewWotDs;


        public Preferences()
        {
            StreamReader fRdr = new StreamReader("prefs.txt");
            AutoVisuals = bool.Parse(fRdr.ReadLine());
            LastFeedCheck = DateTime.Parse(fRdr.ReadLine());
            NewWotDs = bool.Parse(fRdr.ReadLine());
            NewWordsPath = fRdr.ReadLine();
            fRdr.Close();
        }

        public void Save()
        {
            StreamWriter fWrtr = new StreamWriter("prefs.txt");
            fWrtr.WriteLine(AutoVisuals.ToString());
            fWrtr.WriteLine(Misc.ToUniversalString(LastFeedCheck));
            fWrtr.WriteLine(NewWotDs.ToString());
            fWrtr.WriteLine(NewWordsPath);
            fWrtr.Close();
        }
    }

    public class WordOfTheDay
    {
        public string title, URI;
        string lastWord;
        SyndicationFeed feed;
        public bool active;

        public WordOfTheDay(string title, string URI, bool active, string lastWord)
        {
            if (active)
            {
                try
                {
                    feed = SyndicationFeed.Load(XmlReader.Create(URI));
                }
                catch
                {
                    MessageBox.Show("Feed: " + URI + Environment.NewLine + "Check your Internet connection and your RSS feed address.", "Error while loading new words of the day", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //int x = 9;
                }
            }

            if (title == "")
            {
                title = URI.Replace("http://", "").Replace("www.", "");

                if (title.Contains('/'))
                    title = title.Substring(0, title.IndexOf('/'));
            }

            this.URI = URI;
            this.title = title;
            this.active = active;
            this.lastWord = lastWord;
        }

        public string FormatWotD()
        {
            return title + Environment.NewLine + URI + Environment.NewLine + active.ToString() + Environment.NewLine + lastWord;
        }

        public bool AnyNewPosts()
        {
            if (feed == null)
                return false;
            else
                return feed.Items.First().Title.Text != lastWord;
        }

        public Dictionary<string, Definition> getNewWords()
        {
            Dictionary<string, Definition> newWords = new Dictionary<string, Definition>();
            string word;

            foreach (var item in feed.Items)
            {
                word = parseWord(item.Title.Text);

                if (word == lastWord)
                    break;

                string summary = item.Summary.Text;
                if (summary.Length > word.Length + 2 && summary.Substring(0, word.Length + 2) == word + ": ") //special cleanup for Dictionary.com
                    summary = summary.Substring(word.Length + 2);
                if (feed.Title.Text.Contains("Wordsmith")) //special cleanup for Wordsmith
                {
                    summary = summary.Replace("1. ", "");

                    for (int i = 2; summary.Contains(i.ToString() + ". "); i++)
                        summary = summary.Replace(i.ToString() + ". ", Environment.NewLine);
                }

                newWords.Add(word, new Definition(summary, true));
            }

            lastWord = feed.Items.First().Title.Text;

            return newWords;
        }

        public Dictionary<string, string> getNewWordsLinks()
        {
            Dictionary<string, string> links = new Dictionary<string, string>();
            string word;

            foreach (var item in feed.Items)
            {
                word = parseWord(item.Title.Text);

                if (word == lastWord)
                    break;

                links.Add(word, item.Links[0].Uri.ToString());
            }

            return links;
        }

        static string parseWord(string word)
        {
            word = WebUtility.HtmlDecode(word);

            int ub = word.IndexOf(": ");

            if (ub == -1)
                ub = word.IndexOf(" - ");

            if (ub != -1)
                return word.Substring(0, ub);
            else
                return word;
        }
    }

    public class Answer
    {
        public string def;
        public bool correct;

        public Answer(string def)
        {
            this.def = def;
        }
    }
}
