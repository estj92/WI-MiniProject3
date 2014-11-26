using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataManipulator;
using System.Diagnostics;

namespace MainProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Reader reader = new Reader(@"../../..");

            //var trainingFiles = reader.ReadSeveralTrainingFiles(200000);
                        
            // using probes and training data:
            // create a new probe file, with <movie, user, score>

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //reader.CalcAndSaveUsersRatings();
            //watch.Stop();
            //Console.WriteLine(watch.Elapsed);

            var calcProbe = reader.ReadOrCreateUserRatingsFromProbe();

            Console.ReadKey();

            //var probes = reader.ReadProbeFile();


            //var movies = reader.ReadMovies();
            //var probes = reader.ReadProbe();
            //var pairs = reader.ProbesToPairs(probes, allUsers);
            //var pairs = reader.ReadProbesAsPair(trainingMovies, trainingUsers);
        }

        private static IEnumerable<int> DistinctUsers(Dictionary<int, Dictionary<int, int>> trainingFiles)
        {
            int i = 0;
            var allUsers = Enumerable.Empty<int>();
            foreach (var users in trainingFiles.Values)
            {
                allUsers = allUsers.Union(users.Keys);
                //Debug.WriteLine("unioning " + i++);
            }
            allUsers = allUsers.OrderByDescending(u => u);


            return allUsers;
            //return new List<int>(allUsers);
        }
    }
}
