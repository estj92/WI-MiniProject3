using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DataManipulator
{
    public class Reader
    {
        public Reader(string rootFolder)
        {
            RootFolder = rootFolder;
            TrainingFiles = Directory.GetFiles(TrainingFolder);

            Comma = new char[] { ',' };
            Colon = new char[] { ':' };
        }

        public string RootFolder { get; private set; }
        public string DataFolder { get { return RootFolder + "/netflix"; } }
        public string TrainingFolder { get { return DataFolder + "/training"; } }
        public string[] TrainingFiles { get; private set; }
        public string CreatedProbeFile { get { return RootFolder + "/createdprobe.txt"; } }

        private char[] Comma { get; set; }
        private char[] Colon { get; set; }

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
                var val = first.TrimEnd(Colon);
                index = int.Parse(val);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var split = line.Split(Comma);
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
            int i = 0;

            var values = new Dictionary<int, Dictionary<int, int>>(n);
            var filesToRead = TrainingFiles.Take(n).ToList();

            foreach (var file in filesToRead)
            {
                if (i % 100 == 0) { Debug.WriteLine(i); }
                var value = ReadTrainingFile(file);
                values.Add(value.Item1, value.Item2);
                i++;
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
        public Dictionary<int, List<int>> ReadProbeFile()
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
                        var ind = line.TrimEnd(Colon);
                        index = int.Parse(ind);
                        probes[index] = new List<int>();
                        line = reader.ReadLine();
                    }

                    probes[index].Add(int.Parse(line));
                }
            }

            return probes;
        }

        public Dictionary<int, List<Tuple<int, int>>> ReadOrCreateUserRatingsFromProbe()
        {
            if (!File.Exists(CreatedProbeFile))
            {
                CalcAndSaveUsersRatings();
            }

            return ReadUsersRatingsFromCreatedProbeFile();
        }

        public Dictionary<int, List<Tuple<int, int>>> ReadUsersRatingsFromCreatedProbeFile()
        {
            var result = new Dictionary<int, List<Tuple<int, int>>>();

            if (!File.Exists(CreatedProbeFile))
            {
                throw new FileNotFoundException("The created probe file was not found");
            }

            using (StreamReader reader = new StreamReader(CreatedProbeFile))
            {
                int movie = -1;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.EndsWith(":"))
                    {
                        movie = int.Parse(line.TrimEnd(Colon));
                        result.Add(movie, new List<Tuple<int, int>>());
                    }
                    else if (line == "")
                    {
                        continue;
                    }
                    else
                    {
                        var splitted = line.Split(Comma);
                        int mov = int.Parse(splitted[0]);
                        int rating = int.Parse(splitted[1]);

                        result[movie].Add(new Tuple<int, int>(mov, rating));
                    }
                }
            }

            return result;
        }

        public void CalcAndSaveUsersRatings()
        {
            var probes = ReadProbeFile();
            var output = new ConcurrentBag<string>();

            Parallel.ForEach(probes.Take(100000), probe =>
            {
                output.Add(CalcAndSaveUsersRatingsHelper(probe));
            });

            if (File.Exists(CreatedProbeFile))
            {
                File.Delete(CreatedProbeFile);
            }
            using (StreamWriter writer = new StreamWriter(CreatedProbeFile))
            {
                foreach (var line in output.OrderBy(o => o.Length))
                {
                    writer.WriteLine(line);
                }
            }
        }

        private string CalcAndSaveUsersRatingsHelper(KeyValuePair<int, List<int>> probe)
        {
            int movie = probe.Key;
            StringBuilder movieUsersRatings = new StringBuilder(movie.ToString() + ":" + Environment.NewLine);
            var usersWatched = probe.Value;
            var trainingFile = TrainingFolder + "/mv_" + movie.ToString().PadLeft(7, '0') + ".txt";

            var ratingsFromFile = ReadTrainingFile(trainingFile);
            if (ratingsFromFile.Item1 != movie)
            {
                throw new Exception();
            }

            foreach (var userWithRating in ratingsFromFile.Item2)
            {
                if (probe.Value.Contains(userWithRating.Key))
                {
                    movieUsersRatings.Append(userWithRating.Key + "," + userWithRating.Value + Environment.NewLine);
                }
                //else
                //{
                //    movieUsersRatings.Append(userWithRating.Key + ",-1" + Environment.NewLine);
                //}
            }

            return movieUsersRatings.ToString();
        }
    }
}
