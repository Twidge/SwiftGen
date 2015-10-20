using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftGen
{
    // Misc contains any other random methods, usually for debugging purposes.

    class Misc
    {
        public static void OutputProbabilities()
        {
            foreach (SubseqInfo x in Learner.probabilityArray)
            {
                Console.WriteLine("The probability of {0} following {1} is: {2}", x.nextWord, x.previousWord, x.probability);
            }

            foreach (SubseqInfo x in Learner.backwardsProbabilityArray)
            {
                Console.WriteLine("The probability of {0} being preceded by {1} is: {2}", x.nextWord, x.previousWord, x.probability);
            }
        }

        public static void OutputDictionary()
        {
            foreach(DictInfo x in Learner.dictionary)
            {
                string foo = x.word + ' ';

                for(int i = 0; i < x.phonemes.Count - 1; i++)
                {
                    foo += x.phonemes[i] + ' ';
                }

                foo += x.phonemes[x.phonemes.Count - 1];

                Console.WriteLine(foo);
            }
        }
    }
}
