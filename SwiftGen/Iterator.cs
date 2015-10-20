using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftGen
{
    /*
     * Once the first set of lyrics has been established, iterator methods are called to determine how much sense the lyrics make
     * (by comparing them to the input file in triplets of words instead of pairs). Probabilities are recalculated and the process is 
     * repeated for as long as the user desires.
     */

    class Iterator
    {
        public static List<string> wordsInOrder = new List<string>();

        /*
         * RecalculateProbabilities() takes the writtenSong list from the Writer class, checks to see if each triplet of words appears
         * in succession somewhere in the input, then adjusts the probabilities in the probabilityArray and backwardsProbabilityArray
         * by a chosen parameter.
         */

        public static void RecalculateProbabilities()
        {
            wordsInOrder.Clear();

            double seenInfoProbabilityChange = 0.005;
            double subseqInfoProbabilityChange = 0.005;

            // Populate wordsInOrder

            foreach(string x in Writer.writtenSong)
            {
                string word = "";

                for(int i = 0; i < x.Length; i++)
                {
                    if(x[i] == ' ')
                    {
                        wordsInOrder.Add(word);
                        word = "";
                    }

                    else if(i == x.Length - 1)
                    {
                        word += x[i];
                        wordsInOrder.Add(word);
                        word = "";
                    }

                    else
                    {
                        word += x[i];
                    }
                }
            }

            /*
             * For each triplet in wordsInOrder:
             * 1. Run Learner.FindTriplet() to see if it exists in the input file;
             * 2. Readjust probabilities (+/- small amount, redistribute remainder) in totalWords and backwardsTotalWords;
             * 3. Readjust probabilities (+/- small amount, redistribute remainder) in probabilityArray and backwardsProbabilityArray.
             */

            for(int i = 0; i < wordsInOrder.Count - 2; i++)
            {
                double totalProbabilityChange = 0;

                // Step 1

                bool tripletExists = Learner.FindTriplet(wordsInOrder[i], wordsInOrder[i + 1], wordsInOrder[i + 2], 0);
                
                // Step 2

                foreach(SeenInfo x in Learner.totalWords)
                {
                    if(x.previousWord == wordsInOrder[i] || x.previousWord == wordsInOrder[i + 1] || x.previousWord == wordsInOrder[i + 2])
                    {
                        if(tripletExists)
                        {
                            if(x.probability < 1 - seenInfoProbabilityChange)
                            {
                                x.probability += seenInfoProbabilityChange;
                                totalProbabilityChange += seenInfoProbabilityChange;
                            }
                        }

                        else
                        {
                            if(x.probability > seenInfoProbabilityChange)
                            {
                                x.probability -= seenInfoProbabilityChange;
                                totalProbabilityChange -= seenInfoProbabilityChange;
                            }
                        }
                    }
                }

                foreach(SeenInfo x in Learner.totalWords)
                {
                    x.probability += (totalProbabilityChange / (double)Learner.totalWords.Count);
                }

                totalProbabilityChange = 0;

                foreach (SeenInfo x in Learner.backwardsTotalWords)
                {
                    if (x.previousWord == wordsInOrder[i] || x.previousWord == wordsInOrder[i + 1] || x.previousWord == wordsInOrder[i + 2])
                    {
                        if (tripletExists)
                        {
                            if (x.probability < 1 - seenInfoProbabilityChange)
                            {
                                x.probability += seenInfoProbabilityChange;
                                totalProbabilityChange += seenInfoProbabilityChange;
                            }
                        }

                        else
                        {
                            if (x.probability > seenInfoProbabilityChange)
                            {
                                x.probability -= seenInfoProbabilityChange;
                                totalProbabilityChange -= seenInfoProbabilityChange;
                            }
                        }
                    }
                }

                foreach (SeenInfo x in Learner.backwardsTotalWords)
                {
                    x.probability += (totalProbabilityChange / (double)Learner.backwardsTotalWords.Count);
                }

                totalProbabilityChange = 0;

                // Step 3

                foreach (SubseqInfo x in Learner.probabilityArray)
                {
                    if ((x.previousWord == wordsInOrder[i] && x.nextWord == wordsInOrder[i + 1]) || (x.previousWord == wordsInOrder[i + 1] && x.nextWord == wordsInOrder[i + 2]))
                    {
                        if (tripletExists)
                        {
                            if (x.probability < 1 - subseqInfoProbabilityChange)
                            {
                                x.probability += subseqInfoProbabilityChange;
                                totalProbabilityChange += subseqInfoProbabilityChange;
                            }
                        }

                        else
                        {
                            if (x.probability > subseqInfoProbabilityChange)
                            {
                                x.probability -= subseqInfoProbabilityChange;
                                totalProbabilityChange -= subseqInfoProbabilityChange;
                            }
                        }
                    }
                }

                foreach (SubseqInfo x in Learner.probabilityArray)
                {
                    x.probability += (totalProbabilityChange / (double)Learner.probabilityArray.Count);
                }

                totalProbabilityChange = 0;

                foreach (SubseqInfo x in Learner.backwardsProbabilityArray)
                {
                    if ((x.previousWord == wordsInOrder[i + 1] && x.nextWord == wordsInOrder[i]) || (x.previousWord == wordsInOrder[i + 2] && x.nextWord == wordsInOrder[i + 1]))
                    {
                        if (tripletExists)
                        {
                            if (x.probability < 1 - subseqInfoProbabilityChange)
                            {
                                x.probability += subseqInfoProbabilityChange;
                                totalProbabilityChange += subseqInfoProbabilityChange;
                            }
                        }

                        else
                        {
                            if (x.probability > subseqInfoProbabilityChange)
                            {
                                x.probability -= subseqInfoProbabilityChange;
                                totalProbabilityChange -= subseqInfoProbabilityChange;
                            }
                        }
                    }
                }

                foreach (SubseqInfo x in Learner.backwardsProbabilityArray)
                {
                    x.probability += (totalProbabilityChange / (double)Learner.backwardsProbabilityArray.Count);
                }

                totalProbabilityChange = 0;

                /*Console.WriteLine("Loop done for {0}", i);
                Console.WriteLine("tripletExists was: {0}", tripletExists);
                Console.ReadKey();*/
            }
        }
    }
}
