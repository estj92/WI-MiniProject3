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
            Reader reader = new Reader(@"../../../netflix");

            var tf = reader.ReadSeveralTrainingFiles(1100);

            var trainingFiles = new Dictionary<int, Dictionary<int, int>>();

            //trainingFiles = tf.Take(10);
            //trainingFiles = trainingFiles.Union(tf.Skip(989).Take(10));

            foreach (var item in tf.Take(10))
            {
                trainingFiles.Add(item.Key, item.Value);
            }

            foreach (var item in tf.Skip(995).Take(10))
            {
                trainingFiles.Add(item.Key, item.Value);
            }


            var trainingUsers = DistinctUsers(trainingFiles);
            var trainingMovies = trainingFiles.Keys.ToList();


            // using probes and training data:
            // create a new probe file, with <movie, user, score>


            var movies = reader.ReadMovies();
            //var probes = reader.ReadProbe();
            //var pairs = reader.ProbesToPairs(probes, allUsers);
            var pairs = reader.ReadProbesAsPair(trainingMovies, trainingUsers);
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
