using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataManipulator;

namespace MainProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Reader reader = new Reader(@"../../../netflix");

            var files = reader.ReadSeveralTrainingFiles(10);
            var movies = reader.ReadMovies();
            var probes = reader.ReadProbe();
        }
    }
}
