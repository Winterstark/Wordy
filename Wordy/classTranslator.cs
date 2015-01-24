using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Web;
using System.IO;

namespace Wordy
{
    class Translator
    {
        const string clientID = "Wordy";
        const string clientSecret = "GXaFJEyzQC5uymFIq0QhQDMUjzmtDShMjef+AbHknpU=";

        BackgroundWorker searchWordWorker;
        Action<string, string> doneEvent;
        string headerValue, fromLanguage, toLanguage;


        public Translator(string fromLanguage, string toLanguage, Action<string, string> doneEvent)
        {
            this.fromLanguage = fromLanguage;
            this.toLanguage = toLanguage;
            this.doneEvent = doneEvent;

            //prepare worker
            searchWordWorker = new BackgroundWorker();
            searchWordWorker.DoWork += new DoWorkEventHandler(searchWordWorker_DoWork);
            searchWordWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(searchWordWorker_RunWorkerCompleted);

            //get access token
            string strTranslatorAccessURI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";

            String strRequestDetails = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientID), HttpUtility.UrlEncode(clientSecret));

            WebRequest webRequest = WebRequest.Create(strTranslatorAccessURI);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";

            byte[] bytes = Encoding.ASCII.GetBytes(strRequestDetails);
            webRequest.ContentLength = bytes.Length;

            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }

            WebResponse response = webRequest.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string content = reader.ReadToEnd();

            reader.Close();
            response.Close();

            int lb = content.IndexOf("\"access_token\":\"") + 16;
            int ub = content.IndexOf('"', lb);
            headerValue = "Bearer " + content.Substring(lb, ub - lb);
        }

        public void Translate(string txtToTranslate)
        {
            searchWordWorker.RunWorkerAsync(txtToTranslate);
        }

        void searchWordWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string txtToTranslate = (string)e.Argument;

            //search wordnik
            try
            {
                string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + HttpUtility.UrlEncode(txtToTranslate) + "&from=" + fromLanguage + "&to=" + toLanguage;
                WebRequest translationWebRequest = WebRequest.Create(uri);
                translationWebRequest.Headers.Add("Authorization", headerValue);
                WebResponse response = null;
                response = translationWebRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.GetEncoding("utf-8");
                StreamReader translatedStream = new StreamReader(stream, encode);
                System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();
                xTranslation.LoadXml(translatedStream.ReadToEnd());

                e.Result = new Tuple<string, string>(txtToTranslate, xTranslation.InnerText);
            }
            catch
            {
                e.Result = "error";
            }
        }

        void searchWordWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (Tuple<string, string>)e.Result;
            doneEvent(result.Item1, result.Item2);
        }

        public bool IsBusy()
        {
            return searchWordWorker.IsBusy;
        }
    }
}
