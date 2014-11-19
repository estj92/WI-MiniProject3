using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataManipulator
{
    public class Reader
    {
        public Reader(string trainingFilesFolderPath)
        {
            TrainingFilesFolderPath = trainingFilesFolderPath;
        }

        public string TrainingFilesFolderPath { get; private set; }

        public Tuple<int, Dictionary<int, int>> ReadTrainingFile(string file)
        {
            var values = new Dictionary<int, int>();
            int index = -1;

            using (StreamReader reader = new StreamReader(file))
            {
                index = int.Parse(reader.ReadLine().Split(new char[] { ':' })[0]);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var split = line.Split(new char[] { ',' });
                    int user = int.Parse(split[0]);
                    int rating = int.Parse(split[1]);

                    values.Add(user, rating);
                }
            }

            return new Tuple<int, Dictionary<int, int>>(index, values);
        }

        public Dictionary<int, Dictionary<int, int>> ReadSeveralTrainingFiles(int n)
        {
            var filePaths = Directory.GetFiles(TrainingFilesFolderPath);
            if (n > filePaths.Length)
            {
                n = filePaths.Length;
            }

            var values = new Dictionary<int, Dictionary<int, int>>(n);

            for (int i = 0; i < n; i++)
            {
                var value = ReadTrainingFile(filePaths[i]);
                values.Add(value.Item1, value.Item2);
            }

            return values;
        }
    }
}
