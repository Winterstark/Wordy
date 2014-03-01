using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Globalization;

namespace Wordy
{
    class Misc
    {
        public static List<string> LoadCoreWords()
        {
            StreamReader coreFile = new StreamReader("corewords.txt");
            List<string> corewords = coreFile.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            coreFile.Close();

            return corewords;
        }

        public static void ToggleKeyword(List<string> corewords, string selection)
        {
            string changedWord = selection.ToLower();
            while (changedWord[0] == ' ')
                changedWord = changedWord.Substring(1);
            while (changedWord[changedWord.Length - 1] == ' ')
                changedWord = changedWord.Substring(0, changedWord.Length - 1);

            if (corewords.Contains(changedWord))
                corewords.Remove(changedWord);
            else
                corewords.Add(changedWord);
            
            StreamWriter coreFile = new StreamWriter("corewords.txt");
            for (int i = 0; i < corewords.Count; i++)
            {
                coreFile.Write(corewords[i]);
                if (i < corewords.Count - 1)
                    coreFile.WriteLine();
            }
            coreFile.Close();
        }

        public static void DisplayDefs(RichTextBox textDef, string defs, List<string> corewords)
        {
            textDef.Text = "";

            if (defs.Length > 0)
            {
                //find keywords
                var kwBounds = GetKeywordBounds("", defs, corewords);

                if (kwBounds.Count == 0)
                {
                    appendText(textDef, defs, true);
                    return;
                }

                //append text
                appendText(textDef, defs.Substring(0, kwBounds[0].Item1), false);

                for (int i = 0; i < kwBounds.Count - 1; i++)
                {
                    appendText(textDef, defs.Substring(kwBounds[i].Item1, kwBounds[i].Item2 - kwBounds[i].Item1), true);
                    appendText(textDef, defs.Substring(kwBounds[i].Item2, kwBounds[i + 1].Item1 - kwBounds[i].Item2), false);
                }

                appendText(textDef, defs.Substring(kwBounds[kwBounds.Count - 1].Item1, kwBounds[kwBounds.Count - 1].Item2 - kwBounds[kwBounds.Count - 1].Item1), true);
                appendText(textDef, defs.Substring(kwBounds[kwBounds.Count - 1].Item2, defs.Length - kwBounds[kwBounds.Count - 1].Item2), false);
            }
        }

        public static void AppendDefs(RichTextBox textDef, string defs, List<string> corewords)
        {
            if (defs.Length > 0)
            {
                //find keywords
                var kwBounds = GetKeywordBounds("", defs, corewords);

                if (kwBounds.Count == 0)
                {
                    appendText(textDef, defs, true);
                    return;
                }

                //append text
                appendText(textDef, defs.Substring(0, kwBounds[0].Item1), false);

                for (int i = 0; i < kwBounds.Count - 1; i++)
                {
                    appendText(textDef, defs.Substring(kwBounds[i].Item1, kwBounds[i].Item2 - kwBounds[i].Item1), true);
                    appendText(textDef, defs.Substring(kwBounds[i].Item2, kwBounds[i + 1].Item1 - kwBounds[i].Item2), false);
                }

                appendText(textDef, defs.Substring(kwBounds[kwBounds.Count - 1].Item1, kwBounds[kwBounds.Count - 1].Item2 - kwBounds[kwBounds.Count - 1].Item1), true);
                appendText(textDef, defs.Substring(kwBounds[kwBounds.Count - 1].Item2, defs.Length - kwBounds[kwBounds.Count - 1].Item2), false);
            }
        }

        public static List<Tuple<int, int>> GetKeywordBounds(string word, string defs, List<string> corewords)
        {
            List<Tuple<int, int>> kwBounds = new List<Tuple<int, int>>();
            int lb = 0, ub;

            while (true)
            {
                while (lb < defs.Length && !char.IsLetter(defs[lb]))
                {
                    if (defs[lb] == '(' && defs.IndexOf(')', lb + 1) != -1)
                        lb = defs.IndexOf(')', lb + 1) + 1;
                    else if (defs[lb] == '"' && defs.IndexOf('"', lb + 1) != -1)
                        lb = defs.IndexOf('"', lb + 1) + 1;
                    else
                        lb++;
                }
                if (lb >= defs.Length)
                    break;

                ub = lb + 1;
                while (ub < defs.Length && char.IsLetter(defs[ub]))
                    ub++;
                if (ub > defs.Length)
                    break;

                string seg = defs.Substring(lb, ub - lb).ToLower();
                if (!char.IsLetter(seg[0]))
                    seg = seg.Substring(1);
                if (seg != "" && !char.IsLetter(seg[seg.Length - 1]))
                    seg = seg.Substring(0, seg.Length - 1);

                if (seg != "" && seg != word.ToLower() && !corewords.Contains(seg))
                    kwBounds.Add(new Tuple<int, int>(lb, ub));

                //lb = ub + 1;
                lb = ub;
                if (lb >= defs.Length)
                    break;
            }

            return kwBounds;
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string ToUniversalString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm");
        }

        static void appendText(RichTextBox textDef, string txt, bool keyword)
        {
            textDef.SelectionColor = keyword ? Color.Blue : Color.Black;
            textDef.AppendText(txt);
        }
    }
}
