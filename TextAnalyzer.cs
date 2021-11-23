using OpenNLP.Tools.SentenceDetect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Readability
{
    public class TextAnalyzer
    {
        // Constructors -------------------------------------------------------------------------------------
        public TextAnalyzer() { }
        public TextAnalyzer(string text)
            => Text = text;


        // Private helper variables -------------------------------------------------------------------------
        private static readonly EnglishMaximumEntropySentenceDetector SentenceDetector = new EnglishMaximumEntropySentenceDetector(Settings.Default.SentenceModelLocation);
        
        // Dale-Chall list of easy words
        private static readonly string[] EasyWords = Resources.ChallEasyWords.Split('\n').Select(s => s.Trim()).ToArray();
        // Suffixes to try appending to words in easyWords for a more complete list
        private static readonly string[] Suffixes = Resources.WordSuffixes.Split('\n').Select(s => s.Trim()).ToArray();
        // Full english dictionary -- only words found in this dictionary will be considered for difficult words
        private static readonly string[] RealWords = File.ReadAllLines("./Resources/words.txt");
        
        // Public variables ---------------------------------------------------------------------------------
        public string Text { get; set; }

        public int Words { get => WordsInText.Length; }
        public string[] WordsInText 
        { get => Regex.Matches(Text, @"(?:\b)\w+(?:\b)").Cast<Match>().Select(m => m.Value).ToArray(); }
        public int Sentences 
        { get => SentenceDetector.SentenceDetect(Text).Length; }
        public double FleschKincaidGrade 
        {
            get
            {
                Debug.WriteLine($"{Words} {Sentences} {WordsInText.Sum(w => SyllableCount(w))} {WordsInText.Count(w => SyllableCount(w) >= 3)}");
                return 0.39 * Words / Sentences + 11.8 * WordsInText.Sum(w => SyllableCount(w)) / Words - 15.59;
            }
        }
        // TODO: Optimize, write docs
        public double DaleChallScore
        {
            get
            {
                int difficultWords = WordsInText.Where(w => RealWords.Contains(w)).Count(s =>
                {
                    s = s.ToLower();
                    foreach(string suffix in Suffixes)
                        foreach(string w in EasyWords)
                            if(s.Equals(w + suffix))
                                return false;
                    return true;
                });

                double score = 0.1579 * difficultWords / Words * 100 + 0.0496 * Words / Sentences;
                if((double)difficultWords / Words > 0.05)
                    score += 3.6365;
                return score;
            }
        }
        // TODO: Verify implementation
        public double DaleChallGrade 
        {
            get => DaleChallGradeLevel(DaleChallScore);
        }
        public double GunningFog 
        { get => 0.4 * (Words / Sentences + 100 * WordsInText.Count(w => SyllableCount(w) >= 3) / Words); }

        // Helper methods -----------------------------------------------------------------------------------
        // TODO: Improve accuracy
        public static int SyllableCount(string word)
        {
            // https://codereview.stackexchange.com/questions/9972/syllable-counting-function
            word = word.ToLower().Trim();
            bool lastWasVowel = false;
            char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };

            int count = 0;
            for(int i = 0; i < word.Length; ++i)
            {
                char c = word[i];
                if(vowels.Contains(c))
                {
                    if(!lastWasVowel)
                        count++;
                    lastWasVowel = true;

                    if(c == 'y' && i != 0)
                        lastWasVowel = false;
                }
                else
                    lastWasVowel = false;
            }

            if(count > 1 && (word.EndsWith("e") || word.EndsWith("es") || word.EndsWith("ed")) && !word.EndsWith("le"))
                count--;

            if(word.EndsWith("ia") || word.EndsWith("io"))
                count++;

            return count;
        }

        public static double DaleChallGradeLevel(double score)
        {
            if(score > 25 || score < -10) // Chosen arbitrarily
                throw new ArgumentException($"Value \"{score}\" outside of tolerated range");

            if(score < 5)  return 4;
            if(score > 10) return 15;
            return 2 * score - 5;
        }
    }
}
