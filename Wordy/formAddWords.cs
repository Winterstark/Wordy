using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using NikSharp;
using NikSharp.Model;
using FlickrNet;
using System.Diagnostics;
using GenericForms;

namespace Wordy
{
    public partial class formAddWords : Form
    {
        WordnikService wordnik = new WordnikService("b3bbd1f9103a01de7d00a0fd1300164c17bfcec03eb86a678");
        Flickr flickr = new Flickr("d2a2e14ee946139a8f0d2f0b626522f7");
        
        BackgroundWorker searchWordWorker, dlVisualsWorker, newWotDWorker;
        delegate void SetTextCallback(string text);

        Dictionary<string, Definition> newDefs = new Dictionary<string, Definition>();
        Dictionary<string, string> synonyms = new Dictionary<string, string>();
        Dictionary<string, string> rhymes = new Dictionary<string, string>();
        Dictionary<string, PhotoCollection> flickResults = new Dictionary<string, PhotoCollection>();
        Dictionary<string, List<Image>> visuals = new Dictionary<string, List<Image>>();
        Dictionary<string, string> links = new Dictionary<string, string>();

        List<string> corewords = new List<string>();
        Queue<Tuple<string, Definition>> wotdSearchQ;
        Queue<string> wordSearchQ;
        List<string> flickrSearchList;

        public formMain main;
        public bool chkNewWordsFile = false;
        int thumbIndex;


        string dlPage(string url)
        {
            string page;

            try
            {
                WebClient web = new WebClient();
                web.Headers.Add("user-agent", "c#");
                web.Headers[HttpRequestHeader.AcceptLanguage] = "en";

                page = web.DownloadString(url);
            }
            catch
            {
                page = "not_found";
            }

            return page;
        }

        string findSyns(string word)
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
                string rhymeList = "", rhymePg = dlPage("http://rhymebrain.com/en/What_rhymes_with_" + word + ".html");
                int count = 0;

                if (rhymePg.IndexOf("<span class=wordpanel>") != -1)
                {
                    rhymePg = rhymePg.Substring(rhymePg.IndexOf("<span class=wordpanel>"));
                    int lb = rhymePg.IndexOf("<span class=wordpanel>") + 22, ub;

                    while (lb != 21 && count < 10)
                    {
                        ub = rhymePg.IndexOf(" </span>", lb);

                        if (ub != -1)
                            rhymeList += rhymePg.Substring(lb, ub - lb) + ", ";

                        lb = rhymePg.IndexOf("<span class=wordpanel>", ub) + 22;
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

        PhotoCollection findFlickrImages(string word)
        {
            try
            {
                updateStatus("Searching visuals for '" + word + "' ...");

                PhotoSearchOptions opts = new PhotoSearchOptions();
                opts.Tags = word;
                opts.SortOrder = PhotoSearchSortOrder.InterestingnessDescending;
                opts.Licenses.Add(LicenseType.AttributionCC);
                opts.Licenses.Add(LicenseType.AttributionNoDerivativesCC);
                opts.Licenses.Add(LicenseType.AttributionShareAlikeCC);
                opts.Licenses.Add(LicenseType.NoKnownCopyrightRestrictions);
                opts.SafeSearch = SafetyLevel.Safe;
                opts.ContentType = ContentTypeSearch.PhotosOnly;
                opts.MediaType = MediaType.Photos;
                opts.Extras = PhotoSearchExtras.OwnerName;

                return flickr.PhotosSearch(opts);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error while accessing Flickr." + Environment.NewLine + exc.Message);
                return null;
            }
        }

        void loadVisuals(string word)
        {
            
        }

        void toggleVisuals()
        {
            if (buttToggleVisuals.Text == "No Visuals <<")
            {
                this.Width = 650;
                lblVisuals.Visible = false;

                buttToggleVisuals.Text = "Find Visuals >>";
            }
            else
            {
                this.Width = 900;
                lblVisuals.Visible = true;

                buttToggleVisuals.Text = "No Visuals <<";
            }
        }

        /// <summary>
        /// Function to download Image from website
        /// </summary>
        /// <param name="_URL">URL address to download image</param>
        /// <returns>Image</returns>
        Image downloadImage(string _URL)
        {
            Image _tmpImage = null;

            try
            {
                // Open a connection
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                _HttpWebRequest.AllowWriteStreamBuffering = true;

                // You can also specify additional header values like the user agent or the referer: (Optional)
                _HttpWebRequest.UserAgent = "c#";
                _HttpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 20 seconds (Optional)
                _HttpWebRequest.Timeout = 20000;

                // Request response:
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                // Open data stream:
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();

                // convert webstream to image
                _tmpImage = Image.FromStream(_WebStream);

                // Cleanup
                _WebResponse.Close();
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }

            return _tmpImage;
        }

        void genThumbnails()
        {
            if (buttToggleVisuals.Text.Contains("<<") && visuals.ContainsKey(listFoundWords.Text) && visuals[listFoundWords.Text].Count > 0)
            {
                lblVisuals.Enabled = true;
                buttLoadMoreVisuals.Visible = true;
                buttReloadVisuals.Visible = true;

                Bitmap thumbs = new Bitmap(visuals[listFoundWords.Text].Count * 32, 32);
                Graphics gfx = Graphics.FromImage(thumbs);

                for (int i = 0; i < visuals[listFoundWords.Text].Count; i++)
                    gfx.DrawImage(visuals[listFoundWords.Text][i], i * 32, 0, 32, 32);

                picThumbnails.Image = thumbs;

                //enlarge selected thumbnail
                if (visuals[listFoundWords.Text].Count <= thumbIndex)
                    thumbIndex = 0;

                picVisual.Image = visuals[listFoundWords.Text][thumbIndex];
            }
            else
            {
                picThumbnails.Image = null;
                picVisual.Image = null;

                if (!visuals.ContainsKey(listFoundWords.Text))
                    buttLoadMoreVisuals.Visible = false;
            }

            //enable loading visuals
            if (listFoundWords.SelectedIndex != -1)
                buttReloadVisuals.Visible = true;
        }

        int getThumbIndex(int x)
        {
            return Math.Min((int)(x / 32), Math.Max(visuals[listFoundWords.Text].Count - 1, 0));
        }

        void updateStatus(string status)
        {
            if (labelProgress.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(updateStatus);
                this.Invoke(d, new object[] { status });
            }
            else
                labelProgress.Text = status;
        }

        void removeWord()
        {
            newDefs.Remove(listFoundWords.Text);
            synonyms.Remove(listFoundWords.Text);
            rhymes.Remove(listFoundWords.Text);

            flickResults.Remove(listFoundWords.Text);
            visuals.Remove(listFoundWords.Text);

            listFoundWords.Items.RemoveAt(listFoundWords.SelectedIndex);

            lblVisuals.Enabled = false;
            buttLoadMoreVisuals.Visible = false;
            buttReloadVisuals.Visible = false;

            if (newDefs.Count == 0)
            {
                lblRecognizedWords.Enabled = false;
                listFoundWords.Enabled = false;
                buttUpdateDef.Enabled = false;
                buttRemoveWord.Enabled = false;
                buttAcceptWords.Enabled = false;
                buttOpenWotD.Enabled = false;
            }
        }

        public void LoadWotDs()
        {
            if (main.wotds.Count > 0)
            {
                flickrSearchList = new List<string>();
                wotdSearchQ = new Queue<Tuple<string, Definition>>();

                //load WotDs data
                foreach (WordOfTheDay wotd in main.wotds)
                    if (wotd.active && wotd.AnyNewPosts())
                    {
                        //prepare links to WotD webpages
                        foreach (var link in wotd.getNewWordsLinks())
                            if (!links.ContainsKey(link.Key))
                                links.Add(link.Key, link.Value);

                        //prepare a queue of new WotD words
                        foreach (var wotdDef in wotd.getNewWords())
                            if (main.WordExists(wotdDef.Key))
                                MessageBox.Show("The following words already exist in Wordy's database and will not be added: " + wotdDef.Key);
                            else if (!newDefs.ContainsKey(wotdDef.Key))
                                wotdSearchQ.Enqueue(new Tuple<string, Definition>(wotdDef.Key, wotdDef.Value));
                    }

                //prepare worker to get synonyms and rhymes data
                newWotDWorker = new BackgroundWorker();
                newWotDWorker.DoWork += new DoWorkEventHandler(newWotDWorker_DoWork);
                newWotDWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(newWotDWorker_RunWorkerCompleted);

                if (wotdSearchQ.Count > 0)
                    newWotDWorker.RunWorkerAsync(wotdSearchQ.Dequeue()); //process 1st word
            }
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
                updateStatus("Searching definitions for '" + word + "' ..."); //may need invoke?
                WordnikDefinition[] wdDefs = wordnik.GetDefinitions(word).ToArray();

                if (wdDefs.Length > 0)
                {
                    definitions = new Definition(wdDefs);
                    synonyms = findSyns(word);
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
            if (e.Result is string)
            {
                string result = (string)e.Result;

                if (result.Length > 10 && result.Contains(" not found"))
                {
                    string word = result.Substring(0, result.Length - 10);

                    //mark the word as not found
                    int lb = textNewWords.Text.IndexOf(word);

                    if (lb != -1)
                    {
                        int ub = textNewWords.Text.IndexOf(Environment.NewLine, lb);
                        if (ub == -1)
                            ub = textNewWords.Text.Length;

                        textNewWords.Text = textNewWords.Text.Insert(ub, " <- NOT FOUND");
                    }

                    //and remove it from flickr search list (if present)
                    if (flickrSearchList.Contains(word))
                        flickrSearchList.Remove(word);
                }
            }
            else
            {
                var result = (Tuple<string, Definition, string, string>)e.Result;
                string word = result.Item1;

                //save word data
                newDefs.Add(word, result.Item2);
                synonyms.Add(word, result.Item3);
                rhymes.Add(word, result.Item4);

                //display word data
                listFoundWords.Items.Add(word);

                //enable UI elements
                lblRecognizedWords.Enabled = true;
                listFoundWords.Enabled = true;
                buttAcceptWords.Enabled = true;

                if (textNewWords.Text.Contains(word))
                {
                    //remove word from first text box and cleanup if necessary
                    if (textNewWords.Text.Substring(textNewWords.Text.Length - word.Length, word.Length) == word)
                        textNewWords.Text = textNewWords.Text.Substring(0, textNewWords.Text.Length - word.Length);
                    else
                        textNewWords.Text = textNewWords.Text.Replace(word + Environment.NewLine, Environment.NewLine);

                    if (textNewWords.Text == Environment.NewLine)
                        textNewWords.Text = "";
                }
            }

            //next word
            if (wordSearchQ.Count > 0)
                searchWordWorker.RunWorkerAsync(wordSearchQ.Dequeue());
            else
            {
                if (flickrSearchList != null && flickrSearchList.Count > 0 && !dlVisualsWorker.IsBusy) //if need to search Flickr and visuals worker isn't busy
                {
                    string nextWord = flickrSearchList[0];
                    flickrSearchList.RemoveAt(0);

                    dlVisualsWorker.RunWorkerAsync(nextWord); //start downloading visuals for the first word
                }
                else
                {
                    updateStatus("Done");
                    buttFindDefs.Enabled = textNewWords.Text != "";
                }
            }
        }

        void dlVisualsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //extract args
            string word;
            PhotoCollection flickResult;
            int existingVisuals;

            if (e.Argument is string)
            {
                //initial Flickr search, or user clicked Reload All (visuals)
                word = (string)e.Argument;
                existingVisuals = 0;
                
                flickResult = findFlickrImages(word);
            }
            else
            {
                //loading more visuals
                var args = (Tuple<string, PhotoCollection, int>)e.Argument;

                word = args.Item1;
                flickResult = args.Item2;
                existingVisuals = args.Item3;
            }

            //download images
            List<Image> imgs = new List<Image>();
            int nVis = Math.Min(flickResult.Count, 6) - existingVisuals; //how many visuals to load

            for (int i = 0; i < nVis; i++)
            {
                updateStatus("Downloading visuals for '" + word + "' (" + (i + 1).ToString() + "/" + nVis.ToString() + ") ...");

                imgs.Add(downloadImage(flickResult[0].SmallUrl));
                imgs[imgs.Count - 1].Tag = flickResult[0].WebUrl;

                flickResult.RemoveAt(0);
            }

            e.Result = new Tuple<string, PhotoCollection, List<Image>>(word, flickResult, imgs);
        }

        void dlVisualsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //extract and save Flickr search and downloaded imgs
            var result = (Tuple<string, PhotoCollection, List<Image>>)e.Result;
            string word = result.Item1;

            if (flickResults.ContainsKey(word))
                flickResults.Remove(word);
            flickResults.Add(word, result.Item2);

            if (visuals.ContainsKey(word))
                visuals[word].AddRange(result.Item3);
            else
                visuals.Add(word, result.Item3);

            if (flickrSearchList != null && flickrSearchList.Count > 0)
            {
                string firstWord = flickrSearchList[0];
                flickrSearchList.RemoveAt(0);

                dlVisualsWorker.RunWorkerAsync(firstWord); //start downloading visuals for the next word
            }
            else
            {
                updateStatus("Done");
                buttFindDefs.Enabled = textNewWords.Text != "";
            }

            if (listFoundWords.SelectedIndex != -1)
                genThumbnails(); //display new visuals
        }

        void newWotDWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (Tuple<string, Definition>)e.Argument;
            string word = args.Item1;

            try
            {
                //search for word synonyms and rhymes
                e.Result = new Tuple<string, Definition, string, string>(word, args.Item2, findSyns(word), findRhymes(word));
            }
            catch
            {
                e.Result = false; //error
            }
        }

        void newWotDWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Result is bool)) //if not error
            {
                //extract and save result
                var result = (Tuple<string, Definition, string, string>)e.Result;
                string word = result.Item1;

                if (newDefs.ContainsKey(word))
                    newDefs.Remove(word);
                if (synonyms.ContainsKey(word))
                    synonyms.Remove(word);
                if (rhymes.ContainsKey(word))
                    rhymes.Remove(word);

                newDefs.Add(word, result.Item2);
                synonyms.Add(word, result.Item3);
                rhymes.Add(word, result.Item4);

                //display word in list
                listFoundWords.Items.Add(word);

                if (listFoundWords.Enabled == false)
                {
                    listFoundWords.Enabled = true;
                    lblRecognizedWords.Enabled = true;
                    buttAcceptWords.Enabled = true;
                    buttOpenWotD.Visible = true;
                }

                flickrSearchList.Add(word); //remember to search visuals for this word
            }

            //process the next WotD word
            if (wotdSearchQ.Count > 0)
                newWotDWorker.RunWorkerAsync(wotdSearchQ.Dequeue());
            else
            {
                //no more subscriptions
                main.SaveSubs();

                //start flickr search
                if (flickrSearchList.Count > 0)
                {
                    string firstWord = flickrSearchList[0];
                    flickrSearchList.RemoveAt(0);

                    dlVisualsWorker.RunWorkerAsync(firstWord); //start downloading visuals for the next word
                }
                else
                    updateStatus("Done");
            }
        }


        public formAddWords()
        {
            InitializeComponent();
        }

        private void formAddWords_Load(object sender, EventArgs e)
        {
            if (!main.prefs.AutoVisuals)
                toggleVisuals();

            //icon
            if (File.Exists(Application.StartupPath + "\\Wordy.ico"))
                this.Icon = new Icon(Application.StartupPath + "\\Wordy.ico");

            //prepare workers
            searchWordWorker = new BackgroundWorker();
            searchWordWorker.DoWork += new DoWorkEventHandler(searchWordWorker_DoWork);
            searchWordWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(searchWordWorker_RunWorkerCompleted);

            dlVisualsWorker = new BackgroundWorker();
            dlVisualsWorker.DoWork += new DoWorkEventHandler(dlVisualsWorker_DoWork);
            dlVisualsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(dlVisualsWorker_RunWorkerCompleted);

            //load core words
            corewords = Misc.LoadCoreWords();

            //load new words
            if (chkNewWordsFile)
                textNewWords.Text = Misc.LoadNewWordsFromFile(main.prefs.NewWordsPath);

            //cleanup
            while (textNewWords.Text.Length >= 2 && textNewWords.Text.Substring(0, 2) == Environment.NewLine)
                textNewWords.Text = textNewWords.Text.Remove(0, 2);

            new Tutorial(Application.StartupPath + "\\tutorials\\add words.txt", this);
        }

        private void textNewWords_TextChanged(object sender, EventArgs e)
        {
            if (textNewWords.Text != "")
            {
                buttFindDefs.Enabled = !searchWordWorker.IsBusy;
                lblNext.Enabled = true;
            }
            else
            {
                buttFindDefs.Enabled = false;
                lblNext.Enabled = false;
            }
        }

        private void textDef_TextChanged(object sender, EventArgs e)
        {
            if (listFoundWords.SelectedIndex == -1)
                buttUpdateDef.Enabled = false;
            else
                buttUpdateDef.Enabled = textDef.Text.Replace("\n", Environment.NewLine) != newDefs[listFoundWords.Text].ToString();
        }

        private void buttFindDefs_Click(object sender, EventArgs e)
        {
            if (searchWordWorker.IsBusy)
            {
                //already searching
                MessageBox.Show("Please wait for the search to finish before starting a new one.", "Wordy is already searching word definitions", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            List<string> allNewWords = new List<string>(textNewWords.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            int initialCount = allNewWords.Count;

            //remove any blank lines
            for (int i = 0; i < allNewWords.Count; i++)
                if (string.IsNullOrWhiteSpace(allNewWords[i]))
                    allNewWords.RemoveAt(i--);

            //remove any duplicates from the list
            for (int i = 0; i < allNewWords.Count - 1; i++)
                for (int j = i + 1; j < allNewWords.Count; j++)
                    if (allNewWords[j].ToLower() == allNewWords[i].ToLower())
                        allNewWords.RemoveAt(j--);

            if (allNewWords.Count < initialCount)
            {
                //edit text box to include only unique words
                textNewWords.Text = "";

                for (int i = 0; i < allNewWords.Count; i++)
                    textNewWords.Text += allNewWords[i] + (i < allNewWords.Count - 1 ? Environment.NewLine : "");
            }

            //check if words already exist in database (or have already been searched)
            string duplicates = "";
            string[] foundDuplicates = new string[0];

            for (int i = 0; i < allNewWords.Count; i++)
                if (main.WordExists(allNewWords[i]))
                {
                    duplicates += Environment.NewLine + allNewWords[i];
                    allNewWords.RemoveAt(i--);
                }
                else if (listFoundWords.Items.Contains(allNewWords[i]))
                {
                    textNewWords.Text = textNewWords.Text.Replace(allNewWords[i], "").Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                    allNewWords.RemoveAt(i--);
                }

            if (duplicates != "")
            {
                MessageBox.Show("The following words already exist in Wordy's database and will not be added:" + duplicates);
                foundDuplicates = duplicates.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }

            wordSearchQ = new Queue<string>(allNewWords); //process only words that have no duplicates

            if (buttToggleVisuals.Text.Contains("<<")) //if flickr search is enabled
                flickrSearchList = new List<string>(allNewWords);

            if (wordSearchQ.Count > 0)
                searchWordWorker.RunWorkerAsync(wordSearchQ.Dequeue()); //call worker for first word

            buttFindDefs.Enabled = false; //prevent user from starting a new search while a search is running
        }

        private void listFoundWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listFoundWords.SelectedIndex == -1)
            {
                buttRemoveWord.Enabled = false;
                picWordnik.Enabled = false;
                textDef.Enabled = false;
                lblDef.Enabled = false;
                buttOpenWotD.Enabled = false;
                lblSyns.Enabled = false;
                textSynonyms.Enabled = false;

                textDef.Text = "";
                textSynonyms.Text = "";
            }
            else
            {
                buttRemoveWord.Enabled = true;
                picWordnik.Enabled = true;
                lblDef.Enabled = true;
                textDef.Enabled = true;
                buttOpenWotD.Enabled = true;
                lblSyns.Enabled = true;
                textSynonyms.Enabled = true;

                textSynonyms.Text = synonyms[listFoundWords.Text];
                Misc.DisplayDefs(textDef, newDefs[listFoundWords.Text].ToString(), corewords);
            }

            if (buttToggleVisuals.Text.Contains("<<"))
                genThumbnails();
        }

        private void buttUpdateDef_Click(object sender, EventArgs e)
        {
            newDefs[listFoundWords.Text].Parse(textDef.Text, false);
            Misc.DisplayDefs(textDef, newDefs[listFoundWords.Text].ToString(), corewords);

            buttUpdateDef.Enabled = false;
        }

        private void buttRemoveWord_Click(object sender, EventArgs e)
        {
            removeWord();
        }

        private void buttAcceptWords_Click(object sender, EventArgs e)
        {
            //add words
            main.AddNewWords(newDefs, synonyms, rhymes);

            if (buttToggleVisuals.Text == "No Visuals <<")
            {
                //save visuals
                Graphics gfx;
                Bitmap visMosaic;
                int nW, nH, ind;

                foreach (var visual in visuals)
                    if (visual.Value.Count > 0)
                    {
                        nW = 1 + (visual.Value.Count >= 2 ? 1 : 0) + (visual.Value.Count >= 5 ? 1 : 0);
                        nH = 1 + (visual.Value.Count >= 3 ? 1 : 0);

                        visMosaic = new Bitmap(nW * 240, nH * 240);

                        ind = 0;

                        for (int y = 0; y < nH; y++)
                            for (int x = 0; x < nW; x++)
                            {
                                if (ind == visual.Value.Count)
                                    break;

                                visMosaic.SetResolution(visual.Value[ind].HorizontalResolution, visual.Value[ind].VerticalResolution);
                                gfx = Graphics.FromImage(visMosaic);

                                gfx.DrawImage(visual.Value[ind], x * 240 + (240 - visual.Value[ind].Width) / 2, y * 240 + (240 - visual.Value[ind].Height) / 2);

                                ind++;
                            }

                        visMosaic.Save("visuals\\" + visual.Key + ".jpg");
                    }
            }

            this.Close();
        }

        private void formAddWords_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save changes to new words
            while (textNewWords.Text.Contains(Environment.NewLine + Environment.NewLine))
                textNewWords.Text = textNewWords.Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            if (chkNewWordsFile && File.Exists(main.prefs.NewWordsPath))
            {
                StreamWriter file = new StreamWriter(main.prefs.NewWordsPath);
                file.Write(textNewWords.Text);
                file.Close();
            }

            main.Show();
        }

        private void buttToggleVisuals_Click(object sender, EventArgs e)
        {
            toggleVisuals();

            if (listFoundWords.SelectedIndex != -1 && buttToggleVisuals.Text.Contains("<<"))
                dlVisualsWorker.RunWorkerAsync(listFoundWords.Text);

            genThumbnails();
        }

        private void picThumbnails_MouseMove(object sender, MouseEventArgs e)
        {
            if (!visuals.ContainsKey(listFoundWords.Text))
                return;

            thumbIndex = getThumbIndex(e.X);

            if (listFoundWords.SelectedIndex != -1 && visuals[listFoundWords.Text].Count > 0)
                picVisual.Image = visuals[listFoundWords.Text][thumbIndex];
        }

        private void picThumbnails_MouseDown(object sender, MouseEventArgs e)
        {
            if (!visuals.ContainsKey(listFoundWords.Text) || visuals[listFoundWords.Text].Count == 0)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                visuals[listFoundWords.Text].RemoveAt(thumbIndex);

                thumbIndex = getThumbIndex(e.X);
                genThumbnails();
            }
            else
                Process.Start(visuals[listFoundWords.Text][thumbIndex].Tag.ToString());
        }

        private void buttLoadMoreVisuals_Click(object sender, EventArgs e)
        {
            string word = listFoundWords.Text;

            if (visuals[word].Count == 6)
                MessageBox.Show("Already loaded maximum of 6 visuals. Delete some of them by right-clicking on their thumbnail.", "Can't load more visuals", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else if (flickResults[word].Count == 0)
                MessageBox.Show("There are no more visuals.", "Can't load more visuals", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else if (dlVisualsWorker.IsBusy)
                MessageBox.Show("Please wait for the search to finish before starting a new one.", "Wordy is already searching Flickr for images", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                dlVisualsWorker.RunWorkerAsync(new Tuple<string, PhotoCollection, int>(word, flickResults[word], visuals[word].Count));
        }

        private void buttReloadVisuals_Click(object sender, EventArgs e)
        {
            if (dlVisualsWorker.IsBusy)
                MessageBox.Show("Please wait for the search to finish before starting a new one.", "Wordy is already searching Flickr for images", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                if (visuals.ContainsKey(listFoundWords.Text))
                    visuals[listFoundWords.Text].Clear();

                dlVisualsWorker.RunWorkerAsync(listFoundWords.Text);
            }
        }

        private void picVisual_Click(object sender, EventArgs e)
        {
            if (!visuals.ContainsKey(listFoundWords.Text) || visuals[listFoundWords.Text].Count == 0)
                return;

            Process.Start(visuals[listFoundWords.Text][thumbIndex].Tag.ToString());
        }

        private void listFoundWords_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listFoundWords.SelectedIndex != -1)
                removeWord();
        }

        private void textDef_DoubleClick(object sender, EventArgs e)
        {
            if (textDef.SelectedText != "")
            {
                string newWord = textDef.SelectedText;

                if (newWord[newWord.Length - 1] == '.')
                    newWord = newWord.Substring(0, newWord.Length - 1);

                if (textNewWords.Text == "" || (textNewWords.Text.Length > 2 && textNewWords.Text.Substring(textNewWords.Text.Length - 2, 2) == Environment.NewLine))
                    textNewWords.Text += newWord;
                else
                    textNewWords.Text += Environment.NewLine + newWord;
            }
        }

        private void buttOpenWotD_Click(object sender, EventArgs e)
        {
            Process.Start(links[listFoundWords.Text]);
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
            Misc.DisplayDefs(textDef, newDefs[listFoundWords.Text].ToString(), corewords);
        }

        private void menuSurroundWQuotes_Click(object sender, EventArgs e)
        {
            int ub = textDef.SelectionStart + textDef.SelectionLength;
            while (textDef.Text[ub - 1] == '\n' || textDef.Text[ub - 1] == '\r' || textDef.Text[ub - 1] == ' ')
                ub--;

            if (textDef.Text[textDef.SelectionStart] == ' ')
                textDef.SelectionStart++;

            newDefs[listFoundWords.Text].Parse(textDef.Text.Insert(ub, "\"").Insert(textDef.SelectionStart, "\""), false);
            Misc.DisplayDefs(textDef, newDefs[listFoundWords.Text].ToString(), corewords);
        }

        private void menuSurroundWParentheses_Click(object sender, EventArgs e)
        {
            int ub = textDef.SelectionStart + textDef.SelectionLength;
            while (textDef.Text[ub - 1] == '\n' || textDef.Text[ub - 1] == '\r' || textDef.Text[ub - 1] == ' ')
                ub--;

            if (textDef.Text[textDef.SelectionStart] == ' ')
                textDef.SelectionStart++;

            newDefs[listFoundWords.Text].Parse(textDef.Text.Insert(ub, ")").Insert(textDef.SelectionStart, "("), false);
            Misc.DisplayDefs(textDef, newDefs[listFoundWords.Text].ToString(), corewords);
        }

        private void textSynonyms_KeyUp(object sender, KeyEventArgs e)
        {
            if (listFoundWords.Text != "")
                synonyms[listFoundWords.Text] = textSynonyms.Text;
        }

        private void picWordnik_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.wordnik.com/words/" + listFoundWords.Text);
        }
    }
}
