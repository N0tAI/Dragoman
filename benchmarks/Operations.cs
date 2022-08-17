using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Dragoman.Benchmarks
{
    [MinColumn, MaxColumn]
    [Config(typeof(OperationsConfig))]
    public class Operations
    {
        private static readonly byte[] _addValues = { 0xFF, 0xF8, 0x22, 0xA3, 0x33, 0x01, 0x17, 0x41, 0xC2, 0xDF };
        public Operations()
        {

        }

        [Benchmark]
        public ulong AddValue()
        {
            ulong sum = 0;
            foreach (var byteValue in _addValues)
                sum += byteValue;
            return sum;
        }
        [Benchmark]
        public ulong OrValue()
        {
            ulong sum = 0;
            foreach (var byteValue in _addValues)
                sum |= byteValue;
            return sum;
        }

        private class OperationsConfig : ManualConfig
        {
            public OperationsConfig()
            {
                AddJob(Job.Default.WithIterationCount(200000));
            }
        }
    }
}
