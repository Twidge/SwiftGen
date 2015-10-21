using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftGen
{
    // The Syntax class contains any grammatical/lexicographical methods, like checking if two words rhyme.

    class Syntax
    {
        /*
         * RhymeCheck() checks if two words rhyme, though it's not perfect - there'll always be exceptions to any rule-based
         * method for checking if words rhyme.
         */

        public static bool RhymeCheck(string wordA, string wordB)
        {
            bool doesRhyme = false;

            List<DictInfo> wordATypes = GetDictionaryEntries(wordA);
            List<DictInfo> wordBTypes = GetDictionaryEntries(wordB);

            for(int i = 0; i < wordATypes.Count; i++)
            {
                for (int j = 0; j < wordBTypes.Count; j++)
                {
                    bool xIsThreeOrMorePhonemes = true;
                    bool yIsThreeOrMorePhonemes = true;

                    if (wordATypes[i].phonemes.Count <= 2)
                    {
                        xIsThreeOrMorePhonemes = false;
                    }

                    if (wordBTypes[j].phonemes.Count <= 2)
                    {
                        yIsThreeOrMorePhonemes = false;
                    }

                    if (!xIsThreeOrMorePhonemes || !yIsThreeOrMorePhonemes)
                    {
                        if (wordATypes[i].phonemes[wordATypes[i].phonemes.Count - 1] == wordBTypes[j].phonemes[wordBTypes[j].phonemes.Count - 1])
                        {
                            doesRhyme = true;
                            break;
                        }
                    }

                    else
                    {
                        if ((wordATypes[i].phonemes[wordATypes[i].phonemes.Count - 1] == wordBTypes[j].phonemes[wordBTypes[j].phonemes.Count - 1]) && (wordATypes[i].phonemes[wordATypes[i].phonemes.Count - 2] == wordBTypes[j].phonemes[wordBTypes[j].phonemes.Count - 2]))
                        {
                            doesRhyme = true;
                            break;
                        }
                    }

                    if (doesRhyme)
                    {
                        break;
                    }
                }
            }

            return doesRhyme;
        }

        // GetAllRhymingWords() grabs everything that rhymes with a certain word.

        public static List<string> GetAllRhymingWords(string word)
        {
            List<string> output = new List<string>();

            foreach(DictInfo x in Learner.dictionary)
            {
                if(RhymeCheck(x.word, word))
                {
                    output.Add(x.word);
                }
            }

            return output;
        }

        public static List<DictInfo> GetDictionaryEntries(string word)
        {
            List<DictInfo> output = new List<DictInfo>();

            foreach(DictInfo x in Learner.dictionary)
            {
                if(x.word == word)
                {
                    output.Add(x);
                }
            }

            return output;
        }

        // This method is not my own work.

        public static int CountSyllables(string word)
        {
            char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
            string currentWord = word;
            int numVowels = 0;
            bool lastWasVowel = false;

            foreach (char wc in currentWord)
            {
                bool foundVowel = false;

                foreach (char v in vowels)
                {
                    // Forget diphthongs

                    if (v == wc && lastWasVowel)
                    {
                        foundVowel = true;
                        lastWasVowel = true;
                        break;
                    }

                    else if (v == wc && !lastWasVowel)
                    {
                        numVowels++;
                        foundVowel = true;
                        lastWasVowel = true;
                        break;
                    }
                }

                // If we go through a full cycle and no vowel was found, set lastWasVowel to false;

                if (!foundVowel)
                {
                    lastWasVowel = false;
                }
            }

            // Remove es, it's usually silent

            if (currentWord.Length > 2 && currentWord.Substring(currentWord.Length - 2) == "es")
            {
                numVowels--;
            }

            // Remove silent e

            else if (currentWord.Length > 1 && currentWord.Substring(currentWord.Length - 1) == "e")
            {
                numVowels--;
            }

            if(numVowels == 0)
            {
                numVowels++;
            }

            return numVowels;
        }

        public static bool CheckCapitalisationExceptions(string word)
        {
            if (word == "I")
            {
                return true;
            }

            else if (word == "I'm")
            {
                return true;
            }

            else if (word == "Drew")
            {
                return true;
            }

            else if(word == "Tim")
            {
                return true;
            }

            else if(word == "McGraw")
            {
                return true;
            }

            else if(word == "Cory")
            {
                return true;
            }

            else if(word == "Cory's")
            {
                return true;
            }

            return false;
        }
    }
}
