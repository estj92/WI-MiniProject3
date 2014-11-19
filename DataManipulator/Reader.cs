using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace DataManipulator
{
    public class Reader
    {
        public Reader(string dataFolder)
        {
            DataFolder = dataFolder;

        }

        public string DataFolder { get; private set; }
        public string TrainingFolder { get { return DataFolder + "/training"; } }

        /// <summary>
        /// Returns [movie, [user, rating]]
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Tuple<int, Dictionary<int, int>> ReadTrainingFile(string file)
        {
            var values = new Dictionary<int, int>();
            int index = -1;

            using (StreamReader reader = new StreamReader(file))
            {
                //index = int.Parse(reader.ReadLine().Split(new char[] { ':' })[0]);
                string first = reader.ReadLine();
                var val = first.TrimEnd(new char[] { ':' });
                index = int.Parse(val);

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

        /// <summary>
        /// Returns [movie, [user, rating]]
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Dictionary<int, Dictionary<int, int>> ReadSeveralTrainingFiles(int n)
        {
            var filePaths = Directory.GetFiles(TrainingFolder);
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

        public List<Movie> ReadMovies()
        {
            var movies = new List<Movie>(17771);

            using (StreamReader reader = new StreamReader(DataFolder + "/movie_titles.txt"))
            {
                while (!reader.EndOfStream)
                {
                    movies.Add(new Movie(reader.ReadLine()));
                }
            }

            return movies;
        }

        /// <summary>
        /// Returns [movie, [users]]
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<int>> ReadProbe()
        {
            var probes = new Dictionary<int, List<int>>();

            using (StreamReader reader = new StreamReader(DataFolder + "/probe.txt"))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line.Contains(':'))
                    {
                        var ind = line.TrimEnd(new char[] { ':' });
                        index = int.Parse(ind);
                        probes[index] = new List<int>();
                        line = reader.ReadLine();
                    }

                    probes[index].Add(int.Parse(line));
                }
            }

            return probes;
        }

        /// <summary>
        /// Returns [movie, user]
        /// </summary>
        /// <param name="probes"></param>
        /// <returns></returns>
        public Dictionary<Tuple<int, int>, int> ProbesToPairs(Dictionary<int, List<int>> probes, IEnumerable<int> users)
        {
            int n = probes.Select(p => p.Value.Count).Sum();
            var pairs = new Dictionary<Tuple<int, int>, int>(n);

            foreach (var probe in probes)
            {
                foreach (var user in probe.Value)
                {
                    if (users.Contains(user))
                    {
                        pairs.Add(new Tuple<int, int>(probe.Key, user), -1);
                    }
                }
            }

            return pairs;
        }

        public Dictionary<Tuple<int, int>, int> ReadProbesAsPair(IEnumerable<int> movies, IEnumerable<int> users)
        {
            int i = 0;
            var pairs = new Dictionary<Tuple<int, int>, int>();

            using (StreamReader reader = new StreamReader(DataFolder + "/probe.txt"))
            {
                var colonTrim = new char[] { ':' };
                var line = reader.ReadLine();
                i++;
                var ind = line.TrimEnd(colonTrim);
                int movie = int.Parse(ind);
                line = reader.ReadLine();
                i++;

                while (!reader.EndOfStream)
                {
                    Debug.WriteLine("i: " + i++);
                    if (!movies.Contains(movie))
                    {
                        line = reader.ReadLine();
                        while (!line.Contains(':') && !reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            i++;
                        }
                        ind = line.TrimEnd(colonTrim);
                        movie = int.Parse(ind);
                        line = reader.ReadLine();
                        i++;
                    }
                    else
                    {
                        while (!line.Contains(':') && !reader.EndOfStream)
                        {
                            int user = int.Parse(line);

                            if (users.Contains(user))
                            {
                                pairs.Add(new Tuple<int, int>(movie, user), -1);
                            }

                            line = reader.ReadLine();
                            i++;
                        }
                        movie = -1;
                    }
                }
            }

            return pairs;
        }
    }
}
