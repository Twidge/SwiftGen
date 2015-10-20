using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftGen
{
    public class Pattern
    {
        string songName;
        public List<int> syllablePattern = new List<int>();
        public List<int> rhymePattern = new List<int>();
        public List<int> linePattern = new List<int>();

        public Pattern(string name, List<int> syllables, List<int> rhymes, List<int> lines)
        {
            songName = name;

            for(int i = 0; i < syllables.Count; i++)
            {
                syllablePattern.Add(syllables[i]);
            }

            for(int i = 0; i < rhymes.Count; i++)
            {
                rhymePattern.Add(rhymes[i]);
            }

            for(int i = 0; i < lines.Count; i++)
            {
                linePattern.Add(lines[i]);
            }
        }
    }
}
