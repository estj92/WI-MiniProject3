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

        public Dictionary<int, int> ReadTrainingFile(string file)
        {
            var values = new Dictionary<int, int>();

            using (StreamReader reader = new StreamReader(file))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var split = line.Split(new char[] { ',' });
                    int user = int.Parse(split[0]);
                    int rating = int.Parse(split[1]);

                    values.Add(user, rating);
                }
            }

            return values;
        }


    }
}
