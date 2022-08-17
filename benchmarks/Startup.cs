using BenchmarkDotNet.Running;

namespace Dragoman.Benchmarks
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Operations>();
            while (Console.ReadLine() != "enter") ;
        }
    }
}