using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataManipulator
{
    public class Movie
    {
        public Movie(string fromFile)
        {
            var values = fromFile.Split(new char[] { ',' });
            ID = int.Parse(values[0]);
            Year = values[1] == "NULL" ? -1 : int.Parse(values[1]);
            Title = values[2];
        }

        public int ID { get; private set; }
        public int Year { get; private set; }
        public string Title { get; private set; }

        public override string ToString()
        {
            return ID.ToString().PadLeft(5) + " - " + Year.ToString().PadLeft(4) + " - " + Title;
        }
    }
}
