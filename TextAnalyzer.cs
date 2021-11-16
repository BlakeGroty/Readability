using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Readability
{
    class TextAnalyzer
    {
        private Document Doc { get; set; }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                UpdateDoc();
            }
        }

        public int Words { get => Doc.Words.Count; }
        public int Sentences { get => Doc.Sentences.Count; }
        public int Syllables 
        {
            get
            {
                // https://codereview.stackexchange.com/questions/9972/syllable-counting-function
                string text = Text.ToLower().Trim();
                bool lastWasVowel = false;
                char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
                int count = 0;

                //a string is an IEnumerable<char>; convenient.
                foreach(var c in text)
                {
                    if(vowels.Contains(c))
                    {
                        if(!lastWasVowel)
                            count++;
                        lastWasVowel = true;
                    }
                    else
                        lastWasVowel = false;
                }

                if((text.EndsWith("e") || (text.EndsWith("es") || text.EndsWith("ed")))
                      && !text.EndsWith("le"))
                    count--;

                return count;
            }
        }
        public double FleschKincaidGrade 
        {
            get => 0.39 * Words / Sentences + 11.8 * Syllables / Words - 15.59;
        }
        public double DaleChallGrade
        {
            get
            {
                string[] easyWords = File.ReadAllLines("./chall-easy-words.txt");
                int difficultWords = Doc.Words.Cast<string>().Count(s => !easyWords.Contains(s));
                double score = 0.1579 * difficultWords / Words * 100 + 0.0496 * Words / Sentences;

                if((double)difficultWords / Words > 0.05)
                    score += 3.6365;
            }
        }

        public TextAnalyzer(string text)
        {
            Text = text;
            UpdateDoc();
        }

        private void UpdateDoc()
        {
            Application word = new Application();
            word.Visible = false;
            object missing = System.Reflection.Missing.Value;
            Doc = word.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            Doc.Content.Text = Text;
        }
    }
}
