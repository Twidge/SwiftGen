using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftGen
{
    class Program
    {
        static void Main(string[] args)
        {
            SongLibrary library = new SongLibrary();
            bool songSucceeds = false;

            Learner.LearnInput();

            Console.WriteLine("Taylor Swift songs learnt.");
            Console.WriteLine();

            Learner.TagFirstAndLast();

            Console.WriteLine("First and last words of each line tagged.");
            Console.WriteLine();

            Learner.LearnDictionary("dictionary.txt");

            Console.WriteLine("CMU Pronunciation Dictionary learnt.");
            Console.WriteLine();

            Console.WriteLine("Ready to begin song construction; enter an amount of times to iterate:");
            int loopBound = Int32.Parse(Console.ReadLine());

            for(int i = 0; i < loopBound; i++)
            {
                do
                {
                    songSucceeds = Writer.OutputText(library.YouBelongWithMe, true);

                } while (!songSucceeds);

                Iterator.RecalculateProbabilities();

                Console.WriteLine();
                Console.WriteLine("Loop {0} done", i + 1);
                Console.ReadKey();
            }

            do
            {
                songSucceeds = Writer.OutputText(library.YouBelongWithMe, true);

            } while (!songSucceeds);

            Console.ReadKey();
        }
    }
}
