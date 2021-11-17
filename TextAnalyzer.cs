using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Readability
{
    public class TextAnalyzer
    {
        

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                
            }
        }

        public int Words { get => ; }
        public int Sentences { get => ; }
        public double FleschKincaidGrade 
        {
            get => 0.39 * Words / Sentences + 11.8 * TotalSyllables / Words - 15.59;
        }
        public double DaleChallScore
        {
            get
            {
                string[] easyWords = Resources.ChallEasyWords.Split();
                string[] suffixes = Resources.WordSuffixes.Split();
                int difficultWords = WordsInText.Count(s =>
                {
                    foreach(string suffix in suffixes)
                        if(easyWords.Contains(s + suffix))
                            return true;
                    return false;
                });
                
                double score = 0.1579 * difficultWords / Words * 100 + 0.0496 * Words / Sentences;
                if((double)difficultWords / Words > 0.05)
                    score += 3.6365;
                return score;
            }
        }
        public double DaleChallGrade
        {
            get
            {
                for(int i = 5; i <= 10; ++i)
                    if(DaleChallScore <= i + 0.5)
                        return (i - 5) * 2 + i;
                    else if(DaleChallScore < i + 1)
                        return (i - 5) * 2 + i + 1;
                return 4;
            }
        }
        public double GunningFog
        {
            get => 0.4 * (Words / Sentences + 100 * WordsWith3OrMoreSyllables) / Words);
        }

        public TextAnalyzer(string text)
        {
            Text = text;
            
        }

        private int SyllableCount(string word)
        {
            // https://codereview.stackexchange.com/questions/9972/syllable-counting-function
            word = word.ToLower().Trim();
            bool lastWasVowel = false;
            char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
            int count = 0;

            foreach(char c in word)
                if(vowels.Contains(c))
                {
                    if(!lastWasVowel)
                        count++;
                    lastWasVowel = true;
                }
                else
                    lastWasVowel = false;

            if((word.EndsWith("e") || (word.EndsWith("es") || word.EndsWith("ed")))
                  && !word.EndsWith("le"))
                count--;

            return count;
        }
    }
}
