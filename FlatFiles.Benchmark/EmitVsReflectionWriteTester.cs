using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class EmitVsReflectionWriteTester
    {
        private readonly IDelimitedTypeMapper<Person> _mapper;
        private readonly Person[] _people;

        public EmitVsReflectionWriteTester()
        {
            var mapper = DelimitedTypeMapper.Define(() => new Person());
            mapper.Property(x => x.Name).ColumnName("Name");
            mapper.Property(x => x.IQ).ColumnName("IQ");
            mapper.Property(x => x.BirthDate).ColumnName("BirthDate");
            mapper.Property(x => x.TopSpeed).ColumnName("TopSpeed");
            _mapper = mapper;

            _people = Enumerable.Range(0, 10000).Select(i => new Person
                                                            {
                Name = "Susan",
                IQ = 132,
                BirthDate = new DateTime(1984, 3, 15),
                TopSpeed = 10.1m
            }).ToArray();
        }

        [Benchmark(Description = "SerializeEmit")]
        public string SerializeEmit()
        {
            _mapper.OptimizeMapping();
            var writer = new StringWriter();
            _mapper.Write(writer, _people);

            return writer.ToString();
        }

        [Benchmark(Description = "SerializeReflection")]
        public string SerializeReflection()
        {
            _mapper.OptimizeMapping(false);
            var writer = new StringWriter();
            _mapper.Write(writer, _people);

            return writer.ToString();
        }

        public class Person
        {
            public string Name { get; set; }

            public int? IQ { get; set; }

            public DateTime BirthDate { get; set; }

            public decimal TopSpeed { get; set; }
        }
    }
}
