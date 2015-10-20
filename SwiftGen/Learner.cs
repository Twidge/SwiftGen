using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SwiftGen
{
    // The Learner class stores the input and dictionary files in memory and figures out probabilities of words appearing.

    class Learner
    {
        public static List<SubseqInfo> probabilityArray = new List<SubseqInfo>();
        public static List<SubseqInfo> backwardsProbabilityArray = new List<SubseqInfo>();

        public static List<SeenInfo> totalWords = new List<SeenInfo>();
        public static List<SeenInfo> backwardsTotalWords = new List<SeenInfo>();

        public static List<DictInfo> dictionary = new List<DictInfo>();

        public static void LearnDictionary(string path)
        {
            StreamReader fileRead = new StreamReader(path);

            string currentWord = "";
            string currentDictWord = "";
            List<string> currentDictPhonemes = new List<string>();

            bool isFirstWord = true;
            bool again = true;

            while(again)
            {
                int nextChar = fileRead.Read();

                if(nextChar == 32)
                {
                    if(isFirstWord)
                    {
                        isFirstWord = false;

                        currentDictWord = String.Copy(currentWord);
                        currentWord = "";
                    }

                    else
                    {
                        currentDictPhonemes.Add(currentWord);
                        currentWord = "";
                    }
                }

                else if(nextChar == 13)
                {
                    fileRead.Read();

                    currentDictPhonemes.Add(currentWord);
                    currentWord = "";
                    isFirstWord = true;

                    bool shouldBeAdded = false;

                    foreach(SeenInfo x in Learner.totalWords)
                    {
                        if(currentDictWord == x.previousWord)
                        {
                            shouldBeAdded = true;
                        }
                    }

                    foreach(SeenInfo x in Learner.backwardsTotalWords)
                    {
                        if(currentDictWord == x.previousWord)
                        {
                            shouldBeAdded = true;
                        }
                    }

                    if(shouldBeAdded)
                    {
                        dictionary.Add(new DictInfo(currentDictWord, currentDictPhonemes));
                    }

                    currentDictPhonemes.Clear();
                }

                else if (nextChar == -1)
                {
                    currentDictPhonemes.Add(currentWord);
                    currentWord = "";
                    isFirstWord = true;
                    again = false;

                    bool shouldBeAdded = false;

                    foreach (SeenInfo x in Learner.totalWords)
                    {
                        if (currentDictWord == x.previousWord)
                        {
                            shouldBeAdded = true;
                        }
                    }

                    foreach (SeenInfo x in Learner.backwardsTotalWords)
                    {
                        if (currentDictWord == x.previousWord)
                        {
                            shouldBeAdded = true;
                        }
                    }

                    if (shouldBeAdded)
                    {
                        dictionary.Add(new DictInfo(currentDictWord, currentDictPhonemes));
                    }

                    dictionary.Add(new DictInfo(currentDictWord, currentDictPhonemes));
                    currentDictPhonemes.Clear();
                }

                else
                {
                    currentWord += (char)nextChar;
                }
            }

            fileRead.Close();
        }

        // LearnInput method is responsible for crawling the input.txt file forwards and backwards

        public static void LearnInput()
        {
            StreamReader fileRead = new StreamReader("input.txt");

            bool again = true;
            bool isStartOfLine = true;

            double totalWordAmount = 0;

            string lastWord = "";
            string currentWord = "";

            while(again)
            {
                int nextChar = fileRead.Read();
                
                if(nextChar != -1)
                {
                    if(nextChar == 32)
                    {
                        AddToPArray(lastWord, currentWord);
                        
                        if(isStartOfLine)
                        {
                            AddToBPArray(currentWord, lastWord, true);
                            isStartOfLine = false;

                            totalWordAmount++;
                        }

                        else
                        {
                            AddToBPArray(currentWord, lastWord);

                            totalWordAmount++;
                        }

                        lastWord = String.Copy(currentWord);
                        currentWord = "";
                    }

                    else if(nextChar == 13)
                    {
                        nextChar = fileRead.Read();

                        AddToPArray(lastWord, currentWord, true);
                        totalWordAmount++;
                        
                        if(isStartOfLine)
                        {
                            AddToBPArray(currentWord, lastWord, true);
                        }

                        else
                        {
                            AddToBPArray(currentWord, lastWord);
                        }

                        lastWord = "";
                        currentWord = "";
                        isStartOfLine = true;
                    }

                    else
                    {
                        currentWord += (char)nextChar;
                    }
                }

                else
                {
                    AddToPArray(lastWord, currentWord);
                    totalWordAmount++;

                    if(isStartOfLine)
                    {
                        AddToBPArray(currentWord, lastWord, true);
                        isStartOfLine = false;
                    }

                    else
                    {
                        AddToBPArray(currentWord, lastWord);
                    }

                    again = false;
                }
            }

            foreach(SubseqInfo x in probabilityArray)
            {
                bool shouldContinue = true;

                foreach(SeenInfo y in totalWords)
                {
                    if(x.previousWord == y.previousWord)
                    {
                        x.probability = x.numberOfTimesSeen / y.totalTimesSeen;

                        break;
                    }
                }

                if(!shouldContinue)
                {
                    break;
                }
            }

            foreach(SubseqInfo x in backwardsProbabilityArray)
            {
                bool shouldContinue = true;

                foreach(SeenInfo y in backwardsTotalWords)
                {
                    if(x.previousWord == y.previousWord)
                    {
                        x.probability = x.numberOfTimesSeen / y.totalTimesSeen;

                        break;
                    }
                }

                if(!shouldContinue)
                {
                    break;
                }
            }

            foreach(SeenInfo x in totalWords)
            {
                x.probability = x.totalTimesSeen / totalWordAmount;
            }

            foreach(SeenInfo x in backwardsTotalWords)
            {
                x.probability = x.totalTimesSeen / totalWordAmount;
            }

            fileRead.Close();
        }

        public static void TagFirstAndLast()
        {
            StreamReader fileRead = new StreamReader("input.txt");

            bool isFirstWord = true;
            string currentWord = "";

            int nextChar = -1;

            do
            {
                nextChar = fileRead.Read();

                if (nextChar == 32)
                {
                    if (isFirstWord)
                    {
                        foreach (SeenInfo x in totalWords)
                        {
                            if (x.previousWord == currentWord)
                            {
                                x.seenAtStart = true;
                            }
                        }

                        foreach (SeenInfo x in backwardsTotalWords)
                        {
                            if (x.previousWord == currentWord)
                            {
                                x.seenAtEnd = true;
                            }
                        }

                        isFirstWord = false;
                    }

                    currentWord = "";
                }

                else if (nextChar == 13)
                {
                    fileRead.Read();

                    foreach (SeenInfo x in totalWords)
                    {
                        if (x.previousWord == currentWord)
                        {
                            x.seenAtEnd = true;
                        }
                    }

                    foreach (SeenInfo x in backwardsTotalWords)
                    {
                        if (x.previousWord == currentWord)
                        {
                            x.seenAtStart = true;
                        }
                    }

                    isFirstWord = true;

                    currentWord = "";
                }

                else if (nextChar == -1)
                {
                    foreach (SeenInfo x in totalWords)
                    {
                        if (x.previousWord == currentWord)
                        {
                            x.seenAtEnd = true;
                        }
                    }

                    foreach (SeenInfo x in backwardsTotalWords)
                    {
                        if (x.previousWord == currentWord)
                        {
                            x.seenAtStart = true;
                        }
                    }

                    currentWord = "";
                }

                else
                {
                    currentWord += (char)nextChar;
                }

            } while (nextChar != -1);
        }

        public static void AddToPArray(string oldWord, string newWord, bool isEndOfLine = false)
        {
            bool seenBefore = false;

            foreach(SubseqInfo x in probabilityArray)
            {
                if((x.previousWord == oldWord) && (x.nextWord == newWord))
                {
                    foreach(SeenInfo y in totalWords)
                    {
                        if(y.previousWord == oldWord)
                        {
                            y.totalTimesSeen++;
                            break;
                        }
                    }

                    if(isEndOfLine)
                    {
                        AddToPArray(newWord, "0");
                    }

                    x.numberOfTimesSeen++;
                    seenBefore = true;
                    break;
                }
            }

            if(!seenBefore)
            {
                probabilityArray.Add(new SubseqInfo(oldWord, newWord));

                bool alsoSeenBefore = false;

                foreach(SeenInfo y in totalWords)
                {
                    if(y.previousWord == oldWord)
                    {
                        y.totalTimesSeen++;

                        alsoSeenBefore = true;
                        break;
                    }
                }

                if(isEndOfLine)
                {
                    AddToPArray(newWord, "0");
                }

                if(!alsoSeenBefore)
                {
                    totalWords.Add(new SeenInfo(oldWord));
                }
            }
        }

        public static void AddToBPArray(string oldWord, string newWord, bool isStartOfLine = false)
        {
            bool seenBefore = false;

            foreach (SubseqInfo x in backwardsProbabilityArray)
            {
                if ((x.previousWord == oldWord) && (x.nextWord == newWord))
                {
                    foreach (SeenInfo y in backwardsTotalWords)
                    {
                        if (y.previousWord == oldWord)
                        {
                            y.totalTimesSeen++;
                            break;
                        }
                    }

                    if (isStartOfLine)
                    {
                        AddToBPArray(newWord, "0");
                    }

                    x.numberOfTimesSeen++;
                    seenBefore = true;
                    break;
                }
            }

            if (!seenBefore)
            {
                backwardsProbabilityArray.Add(new SubseqInfo(oldWord, newWord));

                bool alsoSeenBefore = false;

                foreach (SeenInfo y in backwardsTotalWords)
                {
                    if (y.previousWord == oldWord)
                    {
                        y.totalTimesSeen++;

                        alsoSeenBefore = true;
                        break;
                    }
                }

                if (isStartOfLine)
                {
                    AddToBPArray(newWord, "0");
                }

                if (!alsoSeenBefore)
                {
                    backwardsTotalWords.Add(new SeenInfo(oldWord));
                }
            }
        }

        // FindTriplet(A, B, C) determines if the succession of words A, B, C appears anywhere in the input.txt file.

        /*
         * NOTE: This method is horrible because my understanding is StreamReaders are forwards-only, so we need an extra integer
         * parameter to keep track of different modes for the method based on what words were read correctly.
         * 
         * FOR EXAMPLE: Say we're looking for the triplet "oh oh my" (standard Taylor Swift fare). Say there's a group of words in the
         * input like "oh my oh oh my". (I can't think how that would ever happen, but anyway...) if you naively run forward checking
         * words, the SR sees "oh" then checks "my" and then checks the second "oh". It determines this triplet is not "oh oh my", but
         * it's forwards-only so it can't go back and re-read the second "oh", thereby missing out on the actual triplet unless we
         * restart the reader.
         * 
         * Obviously we don't want to re-read the entire file thousands of times, so the integer parameter instead keeps track of how
         * many words were correct and thus what words are left to be checked.
         */

        public static bool FindTriplet(string wordA, string wordB, string wordC, int x = 0)
        {
            StreamReader fileRead = new StreamReader("input.txt");

            bool EOFFlag = false;

            string wordACandidate = "";
            string wordBCandidate = "";
            string wordCCandidate = "";

            if(x == 0)
            {
                wordACandidate = ReadNextWord(fileRead, ref EOFFlag);

                if(wordACandidate == wordA)
                {
                    wordBCandidate = ReadNextWord(fileRead, ref EOFFlag);

                    if(wordBCandidate == wordB)
                    {
                        wordCCandidate = ReadNextWord(fileRead, ref EOFFlag);

                        if(wordCCandidate == wordC)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static string ReadNextWord(StreamReader sr, ref bool EOFFlag)
        {
            int nextChar = 0;
            string word = "";

            do
            {
                nextChar = sr.Read();

                if (nextChar == 32)
                {
                    return word;
                }

                else if (nextChar == 13)
                {
                    sr.Read();
                    return word;
                }

                else if (nextChar == -1)
                {
                    EOFFlag = true;
                    return word;
                }

                else
                {
                    word += (char)nextChar;
                }

            } while (true);
        }
    }

    class SubseqInfo
    {
        public string previousWord;
        public string nextWord;
        public double probability = 0;
        public double numberOfTimesSeen = 1;

        public SubseqInfo(string x, string y)
        {
            previousWord = x;
            nextWord = y;
        }
    }

    class SeenInfo
    {
        public string previousWord;
        public double totalTimesSeen = 1;
        public double probability = 0;
        public bool seenAtStart = false;
        public bool seenAtEnd = false;

        public SeenInfo(string x)
        {
            previousWord = x;
        }
    }

    class DictInfo
    {
        public string word;
        public List<string> phonemes = new List<string>();

        public DictInfo(string x, List<string> y)
        {
            word = x;

            foreach(string z in y)
            {
                phonemes.Add(z);
            }
        }
    }
}
