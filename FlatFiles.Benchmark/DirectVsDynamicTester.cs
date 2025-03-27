﻿using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class DirectVsDynamicTester
    {
        private readonly IDelimitedTypeMapper<Person> directMapper;
        private readonly IDynamicDelimitedTypeMapper dynamicMapper;
        private readonly Person[] people;

        public DirectVsDynamicTester()
        {
            var directMapper = DelimitedTypeMapper.Define(static () => new Person());
            directMapper.Property(static x => x.Name).ColumnName("Name");
            directMapper.Property(static x => x.IQ).ColumnName("IQ");
            directMapper.Property(static x => x.BirthDate).ColumnName("BirthDate");
            directMapper.Property(static x => x.TopSpeed).ColumnName("TopSpeed");
            directMapper.Property(static x => x.IsActive).ColumnName("IsActive");
            this.directMapper = directMapper;

            var dynamicMapper = DelimitedTypeMapper.DefineDynamic(typeof(Person));
            dynamicMapper.StringProperty("Name").ColumnName("Name");
            dynamicMapper.Int32Property("IQ").ColumnName("IQ");
            dynamicMapper.DateTimeProperty("BirthDate").ColumnName("BirthDate");
            dynamicMapper.DecimalProperty("TopSpeed").ColumnName("TopSpeed");
            dynamicMapper.BooleanProperty("IsActive").ColumnName("IsActive");
            this.dynamicMapper = dynamicMapper;

            people = Enumerable.Range(0, 10000).Select(static _ => new Person
                                                            {
                Name = "Susan",
                IQ = 132,
                BirthDate = new DateTime(1984, 3, 15),
                TopSpeed = 10.1m
            }).ToArray();
        }

        [Benchmark]
        public void Direct()
        {
            var writer = new StringWriter();
            directMapper.Write(writer, people);
            var peopleData = writer.ToString();

            var reader = new StringReader(peopleData);
            directMapper.Read(reader).ToList();
        }

        [Benchmark]
        public void Dynamic()
        {
            var writer = new StringWriter();
            dynamicMapper.Write(writer, people);
            var peopleData = writer.ToString();
            
            var reader = new StringReader(peopleData);
            dynamicMapper.Read(reader).ToList();
        }

        public class Person
        {
            public string Name { get; set; }

            public int? IQ { get; set; }

            public DateTime BirthDate { get; set; }

            public decimal TopSpeed { get; set; }

            public bool IsActive { get; set; }
        }
    }
}
