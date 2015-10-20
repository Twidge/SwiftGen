using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftGen
{
    enum direction { forwards, backwards };

    // The Writer class is responsible for the construction of the lyrics.

    class Writer
    {
        /*
         * failThreshold is used in the GetNextWord() and GetPreviousWord() methods. It's used to determine how long the program keeps
         * trying to find a suitable next/previous word before it gives up.
         */

        static int failThreshold = 100;

        public static List<string> writtenSong = new List<string>();

        static string previousWord = "";
        static string lastWordOfLastLine = "";

        static bool isFirstWord = true;

        static Random randomSeed = new Random();

        static string currentLine = "";
        static int remainingSyllables = 0;
        static int nextLineSyllables = 0;

        /*
         * OutputText() takes a song pattern (currently syllable count for each line + rhyme pattern), then creates the new song
         * and writes it to the console.
         * 
         * The chain of method calls goes like this:
         * 
         * OutputText --> GetNextLine --> GetNext/PreviousWord
         * 
         * Because the next word in the set of lyrics is determined probabilistically, it's quite possible the program will back
         * itself into a corner when no words exist with the required number of syllables that have been seen after/before the
         * previous word. In this situation, the program would just loop infinitely trying to find a solution that doesn't exist.
         * We get round the problem by passing booleans back up the chain that determine whether the program is likely to find
         * something that works. If OutputText returns false, the process begins again from the start.
         * 
         * Note: provided the song whose pattern we're using is contained in the input.txt file, a solution to the construction
         * problem exists, so this method is guaranteed to (eventually) output something. In practice, it generally outputs lyrics
         * fairly quickly.
         */


        public static bool OutputText(Pattern song, bool write)
        {
            writtenSong.Clear();

            for(int i = 0; i < song.syllablePattern.Count; i++)
            {
                remainingSyllables = song.syllablePattern[i];
                int failCounter = 0;

                if (i == 0)
                {
                    bool lineSucceeds = false;

                    while(!lineSucceeds)
                    {
                        lineSucceeds = GetNextLine(direction.forwards);

                        if(!lineSucceeds)
                        {
                            failCounter++;
                        }

                        if(failCounter >= failThreshold)
                        {
                            return false;
                        }
                    }
                }

                else
                {
                    int lineTypeToFind = song.rhymePattern[i];
                    int lyricTypeToFind = song.linePattern[i];
                    int lineToMimic = -1;
                    int lineToCopy = -1;

                    for(int j = 0; j < i; j++)
                    {
                        if(song.rhymePattern[j] == lineTypeToFind)
                        {
                            lineToMimic = j;
                        }

                        if(song.linePattern[j] == lyricTypeToFind)
                        {
                            lineToCopy = j;
                        }
                    }

                    if(lineToCopy != -1)
                    {
                        // Console.WriteLine("lineToCopy code is running.");

                        currentLine = writtenSong[lineToCopy];
                    }

                    else if(lineToMimic != -1)
                    {
                        // Console.WriteLine("lineToMimic code is running.");

                        bool lineSucceeds = false;

                        while (!lineSucceeds)
                        {
                            lineSucceeds = GetNextLine(direction.backwards, lineToMimic);

                            if (!lineSucceeds)
                            {
                                failCounter++;
                            }

                            if (failCounter >= failThreshold)
                            {
                                return false;
                            }
                        }
                    }

                    else
                    {
                        bool lineSucceeds = false;

                        while (!lineSucceeds)
                        {
                            lineSucceeds = GetNextLine(direction.forwards);

                            if (!lineSucceeds)
                            {
                                failCounter++;
                            }

                            if (failCounter >= failThreshold)
                            {
                                return false;
                            }
                        }
                    }

                    // Console.WriteLine("Loop complete for {0}", i);
                }

                writtenSong.Add(currentLine);
                isFirstWord = true;
            }

            if(write)
            {
                foreach (string x in writtenSong)
                {
                    Console.WriteLine(x);
                }
            }

            return true;
        }

        /*
         * GetNextLine() takes a direction (forwards or backwards) and an optional integer parameter specifying the line it should
         * rhyme with. Running it once puts the next line of the song into the writtenSong list.
         * 
         * If the direction is forwards, GetNextLine() constructs the line from the first word to the last. If backwards, it constructs
         * the line from the last word to the first.
         */

        static bool GetNextLine(direction x, int y = 0)
        {
            if (x == 0)
            {
                currentLine = "";

                previousWord = "";

                nextLineSyllables = remainingSyllables;

                while (remainingSyllables > 0)
                {
                    bool wordSucceeds = GetNextWord();

                    if (!wordSucceeds)
                    {
                        // Console.WriteLine("Line failed.");

                        previousWord = "";
                        remainingSyllables = nextLineSyllables;
                        currentLine = "";
                        isFirstWord = true;
                        return false;
                    }
                }

                return true;
            }

            else
            {
                lastWordOfLastLine = String.Copy(GetLastWordOfLine(y));
                previousWord = GetRhymingWord(lastWordOfLastLine);

                nextLineSyllables = remainingSyllables;

                remainingSyllables -= Syntax.CountSyllables(previousWord);

                currentLine = ' ' + previousWord;

                while(remainingSyllables > 0)
                {
                    bool wordSucceeds = GetPreviousWord();

                    if(!wordSucceeds)
                    {
                        // Console.WriteLine("Line failed.");

                        remainingSyllables = nextLineSyllables;
                        isFirstWord = true;
                        return false;
                    }
                }

                return true;
            }
        }

        // GetLastWordOfLine(x) gets the last word of line x and is used mostly for figuring out rhymes.

        static string GetLastWordOfLine(int x)
        {
            string lineToRead = writtenSong[x];

            int lastSpacePosition = 0;

            for (int i = 0; i < lineToRead.Length; i++)
            {
                if(lineToRead[i] == ' ')
                {
                    lastSpacePosition = i;
                }
            }

            return lineToRead.Substring(lastSpacePosition + 1);
        }

        // GetRhymingWord(word) gets a random word from the input dictionary that rhymes with word.

        static string GetRhymingWord(string word)
        {
            List<string> rhymingWords = Syntax.GetAllRhymingWords(word);
            
            if(rhymingWords.Count == 0)
            {
                return "";
            }

            double totalProbability = 0;

            List<double> probabilityList = new List<double>();

            foreach(string x in rhymingWords)
            {
                bool wordFound = false;

                foreach(SeenInfo y in Learner.backwardsTotalWords)
                {
                    if(y.previousWord == x)
                    {
                        wordFound = true;
                        probabilityList.Add(y.probability);
                        totalProbability += y.probability;
                    }

                    if(wordFound)
                    {
                        break;
                    }
                }
            }

            // Console.WriteLine("totalProbability is: {0}", totalProbability);

            double scalingFactor = 1 / totalProbability;

            for(int i = 0; i < probabilityList.Count; i++)
            {
                probabilityList[i] *= scalingFactor;
            }

            totalProbability = 0;

            foreach(double x in probabilityList)
            {
                totalProbability += x;
            }

            // Console.WriteLine("Scaled totalProbability is: {0}", totalProbability);
            // Console.ReadKey();

            while(true)
            {
                double randomGen = (double)(randomSeed.Next() % 100000) / (double)100000;

                totalProbability = 0;
                for(int i = 0; i < probabilityList.Count; i++)
                {
                    totalProbability += probabilityList[i];

                    if(totalProbability >= randomGen)
                    {
                        // Console.WriteLine("Returning: {0}", rhymingWords[i]);
                        // Console.ReadKey();
                        return rhymingWords[i];
                    }
                }
            }
        }

        // GetPreviousWord() is used in the backwards construction of the next line. It gets the "next" (so previous) word in the line.

        static bool GetPreviousWord()
        {
            double randomGen = 0;

            double probabilityTrack = 0;

            int loopCounter = 0;

            while (true)
            {
                foreach (SubseqInfo x in Learner.backwardsProbabilityArray)
                {
                    randomGen = ((double)(randomSeed.Next() % 100000)) / (double)100000;

                    if (previousWord == x.previousWord)
                    {
                        probabilityTrack += x.probability;

                        if (probabilityTrack >= randomGen)
                        {
                            if (Syntax.CountSyllables(x.nextWord) > remainingSyllables)
                            {
                                break;
                            }

                            else
                            {
                                bool badWord = true;

                                foreach (SeenInfo y in Learner.backwardsTotalWords)
                                {
                                    if (y.previousWord == x.nextWord)
                                    {
                                        foreach (SubseqInfo z in Learner.backwardsProbabilityArray)
                                        {
                                            if ((z.previousWord == y.previousWord) && (z.nextWord != ""))
                                            {
                                                badWord = false;
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }

                                if (badWord)
                                {
                                    break;
                                }

                                if(x.nextWord == " ")
                                {
                                    return false;
                                }

                                if(x.nextWord == "")
                                {
                                    return false;
                                }

                                string workString = String.Copy(x.nextWord);

                                remainingSyllables -= Syntax.CountSyllables(x.nextWord);

                                if (remainingSyllables != 0)
                                {
                                    currentLine = ' ' + workString + currentLine;
                                }

                                else
                                {
                                    foreach(SeenInfo y in Learner.backwardsTotalWords)
                                    {
                                        if(y.previousWord == x.nextWord)
                                        {
                                            if(!y.seenAtStart)
                                            {
                                                remainingSyllables += Syntax.CountSyllables(x.nextWord);

                                                return false;
                                            }
                                        }
                                    }

                                    currentLine = workString + currentLine;
                                }

                                previousWord = String.Copy(x.nextWord);
                                return true;
                            }
                        }
                    }
                }

                loopCounter++;

                if (loopCounter >= failThreshold)
                {
                    return false;
                }
            }

            // Console.WriteLine("Previous word chosen is: {0}", previousWord);
        }

        // GetNextWord() is used in the forwards construction of GetNextLine(). Unsurprisingly, it gets the next word in the line.

        static bool GetNextWord()
        {
            double randomGen = 0;

            double probabilityTrack = 0;

            int loopCounter = 0;

            while(true)
            {
                foreach (SubseqInfo x in Learner.probabilityArray)
                {
                    randomGen = ((double)(randomSeed.Next() % 100000)) / (double)100000;

                    if (previousWord == x.previousWord)
                    {
                        probabilityTrack += x.probability;

                        if (probabilityTrack >= randomGen)
                        {
                            if(Syntax.CountSyllables(x.nextWord) > remainingSyllables)
                            {
                                break;
                            }

                            else
                            {
                                bool badWord = true;

                                foreach(SeenInfo y in Learner.totalWords)
                                {
                                    if(y.previousWord == x.nextWord)
                                    {
                                        foreach(SubseqInfo z in Learner.probabilityArray)
                                        {
                                            if((z.previousWord == y.previousWord) && (z.nextWord != "0"))
                                            {
                                                badWord = false;
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }

                                if (badWord)
                                {
                                    break;
                                }

                                string workString = String.Copy(x.nextWord);

                                if(isFirstWord)
                                {
                                    isFirstWord = false;

                                    if(char.IsLower(workString[0]))
                                    {
                                        workString = char.ToUpper(workString[0]) + workString.Substring(1);
                                    }
                                }

                                else
                                {
                                    if(char.IsUpper(workString[0]))
                                    {
                                        if(!Syntax.CheckCapitalisationExceptions(workString))
                                        {
                                            workString = char.ToLower(workString[0]) + workString.Substring(1);
                                        }
                                    }
                                }

                                currentLine += workString;

                                remainingSyllables -= Syntax.CountSyllables(x.nextWord);
                                
                                if(remainingSyllables != 0)
                                {
                                    currentLine += ' ';
                                }

                                if(remainingSyllables == 0)
                                {
                                    foreach (SeenInfo y in Learner.backwardsTotalWords)
                                    {
                                        if (y.previousWord == x.nextWord)
                                        {
                                            if (!y.seenAtStart)
                                            {
                                                remainingSyllables += Syntax.CountSyllables(x.nextWord);

                                                return false;
                                            }
                                        }
                                    }

                                    lastWordOfLastLine = workString;
                                }

                                previousWord = String.Copy(x.nextWord);
                                return true;
                            }
                        }
                    }
                }

                loopCounter++;

                if(loopCounter >= failThreshold)
                {
                    return false;
                }
            }
        }
    }
}
