using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    class FileOp
    {
        private readonly string path;

        public FileOp(string path)
        {
            this.path = path;
        }

        public void Write(int score)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(score);
            }
        }

        public void Read(ref int maxScore)
        {
            maxScore = 0;
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                List<int> list = new List<int>();

                foreach (var line in lines)
                {
                    list.Add(Int32.Parse(line));
                }

                if (list.Count != 0)
                    maxScore = list.Max();
            }
        }

    }
}
