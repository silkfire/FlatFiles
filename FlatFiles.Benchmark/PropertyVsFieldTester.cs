﻿using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class PropertyVsFieldTester
    {
        private readonly PropertyPerson[] propertyPeople;
        private readonly FieldPerson[] fieldPeople;

        public PropertyVsFieldTester()
        {
            var propertyPerson = new PropertyPerson
                                 {
                                     FirstName = "John",
                                     LastName = "Smith",
                                     Age = 29,
                                     Street1 = "West Street Rd",
                                     Street2 = "Apt 23",
                                     City = "Lexington",
                                     State = "DE",
                                     Zip = "001569",
                                     FavoriteColor = "Blue",
                                     FavoriteFood = "Cheese and Crackers",
                                     FavoriteSport = "Soccer",
                                     CreatedOn = new DateTime(2017, 01, 01),
                                     IsActive = true
                                 };
            propertyPeople = Enumerable.Repeat(0, 10000).Select(_ => propertyPerson).ToArray();

            var fieldPerson = new FieldPerson
                              {
                                  FirstName = "John",
                                  LastName = "Smith",
                                  Age = 29,
                                  Street1 = "West Street Rd",
                                  Street2 = "Apt 23",
                                  City = "Lexington",
                                  State = "DE",
                                  Zip = "001569",
                                  FavoriteColor = "Blue",
                                  FavoriteFood = "Cheese and Crackers",
                                  FavoriteSport = "Soccer",
                                  CreatedOn = new DateTime(2017, 01, 01),
                                  IsActive = true
                              };
            fieldPeople = Enumerable.Repeat(0, 10000).Select(_ => fieldPerson).ToArray();
        }

        [Benchmark]
        public void RunPropertyTest()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new PropertyPerson());
            mapper.Property(static x => x.FirstName);
            mapper.Property(static x => x.LastName);
            mapper.Property(static x => x.Age);
            mapper.Property(static x => x.Street1);
            mapper.Property(static x => x.Street2);
            mapper.Property(static x => x.City);
            mapper.Property(static x => x.State);
            mapper.Property(static x => x.Zip);
            mapper.Property(static x => x.FavoriteColor);
            mapper.Property(static x => x.FavoriteFood);
            mapper.Property(static x => x.FavoriteSport);
            mapper.Property(static x => x.CreatedOn);
            mapper.Property(static x => x.IsActive);

            var writer = new StringWriter();
            mapper.Write(writer, propertyPeople);
            var serialized = writer.ToString();

            var reader = new StringReader(serialized);
            var deserialized = mapper.Read(reader).ToArray();
        }

        [Benchmark]
        public void RunFieldTest()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new FieldPerson());
            mapper.Property(static x => x.FirstName);
            mapper.Property(static x => x.LastName);
            mapper.Property(static x => x.Age);
            mapper.Property(static x => x.Street1);
            mapper.Property(static x => x.Street2);
            mapper.Property(static x => x.City);
            mapper.Property(static x => x.State);
            mapper.Property(static x => x.Zip);
            mapper.Property(static x => x.FavoriteColor);
            mapper.Property(static x => x.FavoriteFood);
            mapper.Property(static x => x.FavoriteSport);
            mapper.Property(static x => x.CreatedOn);
            mapper.Property(static x => x.IsActive);

            var writer = new StringWriter();
            mapper.Write(writer, fieldPeople);
            var serialized = writer.ToString();

            var reader = new StringReader(serialized);
            var deserialized = mapper.Read(reader).ToArray();
        }
        
        public class PropertyPerson
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }

            public string Street1 { get; set; }

            public string Street2 { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }

            public string FavoriteColor { get; set; }

            public string FavoriteFood { get; set; }

            public string FavoriteSport { get; set; }
            
            public DateTime? CreatedOn { get; set; }

            public bool IsActive { get; set; }
        }

        public class FieldPerson
        {
            public string FirstName;
            public string LastName;
            public int Age;
            public string Street1;
            public string Street2;
            public string City;
            public string State;
            public string Zip;
            public string FavoriteColor;
            public string FavoriteFood;
            public string FavoriteSport;
            public DateTime? CreatedOn;
            public bool IsActive;
        }
    }
}
