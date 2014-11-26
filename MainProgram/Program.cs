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

            var calcProbe = reader.ReadOrCreateUserRatingsFromProbe();
            reader.PreProcessing_2B(calcProbe);

            Console.WriteLine("DONE");
            Console.ReadKey();
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
