﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class CoreBenchmarkSuite
    {
        private readonly string data;
        private readonly string quotedData;

        public CoreBenchmarkSuite()
        {
            string[] headers =
            [
                "FirstName", "LastName", "Age", "Street1", "Street2", "City", "State", "Zip", "FavoriteColor", "FavoriteFood", "FavoriteSport", "CreatedOn", "IsActive"
            ];
            string[] values =
            [
                "John", "Smith", "29", "West Street Rd", "Apt 23", "Lexington", "DE", "001569", "Blue", "Cheese and Crackers", "Soccer", "2017-01-01", "true"
            ];
            var header = String.Join(",", headers);
            var record = String.Join(",", values);
            data = String.Join(Environment.NewLine, (new[] { header }).Concat(Enumerable.Repeat(0, 10000).Select(_ => record)));

            string[] quotedValues =
            [
                "Joe", "Smith", "29", "\"West Street Rd, Apt. 23\"", "ATTN: Will Smith", "Lexington", "DE", "001569", "Blue", "\"Cheese, and Crackers\"", "Soccer", "2017-01-01", "true"
            ];
            var quotedRecord = String.Join(",", quotedValues);
            quotedData = String.Join(Environment.NewLine, (new[] { header }).Concat(Enumerable.Repeat(0, 10000).Select(_ => record)));
        }

        [Benchmark]
        public void RunFlatFiles_NoSchema()
        {
            var reader = new StringReader(data);
            var csvReader = new DelimitedReader(reader);
            var people = new List<object[]>();
            while (csvReader.Read())
            {
                people.Add(csvReader.GetValues());
            }
        }

        [Benchmark]
        public void RunFlatFiles_TypeMapper()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new Person());
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

            var reader = new StringReader(data);
            var people = mapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public async Task RunFlatFiles_TypeMapper_Async()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new Person());
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

            var textReader = new StringReader(data);
            var reader = mapper.GetReader(textReader, new DelimitedOptions { IsFirstRecordSchema = true });
            var people = new List<Person>();
            await foreach (var person in reader.ReadAllAsync().ConfigureAwait(false))
            {
                people.Add(person);
            }
        }

        [Benchmark]
        public void RunFlatFiles_TypeMapper_Quoted()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new Person());
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

            var reader = new StringReader(quotedData);
            var people = mapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public void RunFlatFiles_TypeMapper_Fields()
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

            var reader = new StringReader(data);
            var people = mapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public void RunFlatFiles_TypeMapper_Unoptimized()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new Person());
            mapper.OptimizeMapping(false);
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

            var reader = new StringReader(data);
            var people = mapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public void RunFlatFiles_TypeMapper_CustomMapping()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new Person());
            mapper.CustomMapping(new StringColumn("FirstName")).WithReader(static p => p.FirstName);
            mapper.CustomMapping(new StringColumn("LastName")).WithReader(static p => p.LastName);
            mapper.CustomMapping(new Int32Column("Age")).WithReader(static p => p.Age);
            mapper.CustomMapping(new StringColumn("Street1")).WithReader(static p => p.Street1);
            mapper.CustomMapping(new StringColumn("Street2")).WithReader(static p => p.Street2);
            mapper.CustomMapping(new StringColumn("City")).WithReader(static p => p.City);
            mapper.CustomMapping(new StringColumn("State")).WithReader(static p => p.State);
            mapper.CustomMapping(new StringColumn("Zip")).WithReader(static p => p.Zip);
            mapper.CustomMapping(new StringColumn("FavoriteColor")).WithReader(static p => p.FavoriteColor);
            mapper.CustomMapping(new StringColumn("FavoriteFood)")).WithReader(static p => p.FavoriteFood);
            mapper.CustomMapping(new StringColumn("FavoriteSport")).WithReader(static p => p.FavoriteSport);
            mapper.CustomMapping(new DateTimeColumn("CreatedOn")).WithReader(static p => p.CreatedOn);
            mapper.CustomMapping(new BooleanColumn("IsActive")).WithReader(static p => p.IsActive);

            var reader = new StringReader(data);
            var people = mapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public void RunFlatFiles_TypeMapper_CustomMapping_Unoptimized()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new Person());
            mapper.OptimizeMapping(false);
            mapper.CustomMapping(new StringColumn("FirstName")).WithReader(static p => p.FirstName);
            mapper.CustomMapping(new StringColumn("LastName")).WithReader(static p => p.LastName);
            mapper.CustomMapping(new Int32Column("Age")).WithReader(static p => p.Age);
            mapper.CustomMapping(new StringColumn("Street1")).WithReader(static p => p.Street1);
            mapper.CustomMapping(new StringColumn("Street2")).WithReader(static p => p.Street2);
            mapper.CustomMapping(new StringColumn("City")).WithReader(static p => p.City);
            mapper.CustomMapping(new StringColumn("State")).WithReader(static p => p.State);
            mapper.CustomMapping(new StringColumn("Zip")).WithReader(static p => p.Zip);
            mapper.CustomMapping(new StringColumn("FavoriteColor")).WithReader(static p => p.FavoriteColor);
            mapper.CustomMapping(new StringColumn("FavoriteFood)")).WithReader(static p => p.FavoriteFood);
            mapper.CustomMapping(new StringColumn("FavoriteSport")).WithReader(static p => p.FavoriteSport);
            mapper.CustomMapping(new DateTimeColumn("CreatedOn")).WithReader(static p => p.CreatedOn);
            mapper.CustomMapping(new BooleanColumn("IsActive")).WithReader(static p => p.IsActive);

            var reader = new StringReader(data);
            var people = mapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public void RunFlatFiles_AutoMapped()
        {
            var reader = new StringReader(data);
            var csvReader = DelimitedTypeMapper.GetAutoMappedReader<Person>(reader);
            var people = new List<Person>();
            foreach (var person in csvReader.ReadAll())
            {
                people.Add(person);
            }
        }

        [Benchmark]
        public async Task RunFlatFiles_AutoMapped_Async()
        {
            var reader = new StringReader(data);
            var csvReader = await DelimitedTypeMapper.GetAutoMappedReaderAsync<Person>(reader);
            var people = new List<Person>();
            while (await csvReader.ReadAsync().ConfigureAwait(false))
            {
                people.Add(csvReader.Current);
            }
        }

        [Benchmark]
        public void RunFlatFiles_FlatFileDataReader_ByPosition()
        {
            var reader = new StringReader(data);
            var schema = GetSchema();
            var csvReader = new DelimitedReader(reader, schema, new DelimitedOptions { IsFirstRecordSchema = true });
            var dataReader = new FlatFileDataReader(csvReader);
            var people = new List<Person>();
            while (dataReader.Read())
            {
                var person = new Person
                             {
                    FirstName = dataReader.GetString(0),
                    LastName = dataReader.GetString(1),
                    Age = dataReader.GetInt32(2),
                    Street1 = dataReader.GetString(3),
                    Street2 = dataReader.GetString(4),
                    City = dataReader.GetString(5),
                    State = dataReader.GetString(6),
                    Zip = dataReader.GetString(7),
                    FavoriteColor = dataReader.GetString(8),
                    FavoriteFood = dataReader.GetString(9),
                    FavoriteSport = dataReader.GetString(10),
                    CreatedOn = dataReader.GetDateTime(11),
                    IsActive = dataReader.GetBoolean(12)
                };
                people.Add(person);
            }
        }

        [Benchmark]
        public void RunFlatFiles_FlatFileDataReader_ByName()
        {
            var reader = new StringReader(data);
            var schema = GetSchema();
            var csvReader = new DelimitedReader(reader, schema, new DelimitedOptions { IsFirstRecordSchema = true });
            var dataReader = new FlatFileDataReader(csvReader);
            var people = new List<Person>();
            while (dataReader.Read())
            {
                var person = new Person
                             {
                    FirstName = dataReader.GetString("FirstName"),
                    LastName = dataReader.GetString("LastName"),
                    Age = dataReader.GetInt32("Age"),
                    Street1 = dataReader.GetString("Street1"),
                    Street2 = dataReader.GetString("Street2"),
                    City = dataReader.GetString("City"),
                    State = dataReader.GetString("State"),
                    Zip = dataReader.GetString("Zip"),
                    FavoriteColor = dataReader.GetString("FavoriteColor"),
                    FavoriteFood = dataReader.GetString("FavoriteFood"),
                    FavoriteSport = dataReader.GetString("FavoriteSport"),
                    CreatedOn = dataReader.GetDateTime("CreatedOn"),
                    IsActive = dataReader.GetBoolean("IsActive")
                };
                people.Add(person);
            }
        }

        [Benchmark]
        public void RunFlatFiles_FlatFileDataReader_GetValue()
        {
            var reader = new StringReader(data);
            var schema = GetSchema();
            var csvReader = new DelimitedReader(reader, schema, new DelimitedOptions { IsFirstRecordSchema = true });
            var dataReader = new FlatFileDataReader(csvReader);
            var people = new List<Person>();
            while (dataReader.Read())
            {
                var person = new Person
                             {
                    FirstName = dataReader.GetValue<string>(0),
                    LastName = dataReader.GetValue<string>(1),
                    Age = dataReader.GetValue<int>(2),
                    Street1 = dataReader.GetValue<string>(3),
                    Street2 = dataReader.GetValue<string>(4),
                    City = dataReader.GetValue<string>(5),
                    State = dataReader.GetValue<string>(6),
                    Zip = dataReader.GetValue<string>(7),
                    FavoriteColor = dataReader.GetValue<string>(8),
                    FavoriteFood = dataReader.GetValue<string>(9),
                    FavoriteSport = dataReader.GetValue<string>(10),
                    CreatedOn = dataReader.GetValue<DateTime?>(11),
                    IsActive = dataReader.GetValue<bool>(12)
                };
                people.Add(person);
            }
        }

        [Benchmark]
        public void RunFlatFiles_DataTable()
        {
            var reader = new StringReader(data);
            var schema = GetSchema();
            var csvReader = new DelimitedReader(reader, schema, new DelimitedOptions { IsFirstRecordSchema = true });
            var dataTable = new DataTable();
            dataTable.ReadFlatFile(csvReader);
        }

        private static DelimitedSchema GetSchema()
        {
            var schema = new DelimitedSchema();
            schema.AddColumn(new StringColumn("FirstName"));
            schema.AddColumn(new StringColumn("LastName"));
            schema.AddColumn(new Int32Column("Age"));
            schema.AddColumn(new StringColumn("Street1"));
            schema.AddColumn(new StringColumn("Street2"));
            schema.AddColumn(new StringColumn("City"));
            schema.AddColumn(new StringColumn("State"));
            schema.AddColumn(new StringColumn("Zip"));
            schema.AddColumn(new StringColumn("FavoriteColor"));
            schema.AddColumn(new StringColumn("FavoriteFood"));
            schema.AddColumn(new StringColumn("FavoriteSport"));
            schema.AddColumn(new DateTimeColumn("CreatedOn"));
            schema.AddColumn(new BooleanColumn("IsActive"));
            return schema;
        }

        [Benchmark]
        public void RunCsvHelper()
        {
            var reader = new StringReader(data);
            var csvReader = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
            var people = new List<Person>();
            foreach (var person in csvReader.GetRecords<Person>())
            {
                people.Add(person);
            }
        }

        [Benchmark]
        public async Task RunCsvHelper_Async()
        {
            var reader = new StringReader(data);
            var csvReader = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
            var people = new List<Person>();
            await csvReader.ReadAsync().ConfigureAwait(false);
            csvReader.ReadHeader();
            while (await csvReader.ReadAsync().ConfigureAwait(false))
            {
                people.Add(csvReader.GetRecord<Person>());
            }
        }

        [Benchmark]
        public void RunCsvHelper_Quoted()
        {
            var reader = new StringReader(quotedData);
            var csvReader = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
            var people = new List<Person>();
            foreach (var person in csvReader.GetRecords<Person>())
            {
                people.Add(person);
            }
        }

        [Benchmark]
        public void RunStringSplit()
        {
            var lines = data.Split(Environment.NewLine);
            var records = lines.Skip(1).Select(static l => l.Split(",").ToArray());
            var people = new List<Person>();
            foreach (var record in records)
            {
                var person = new Person
                             {
                    FirstName = record[0],
                    LastName = record[1],
                    Age = Int32.Parse(record[2]),
                    Street1 = record[3],
                    Street2 = record[4],
                    City = record[5],
                    State = record[6],
                    Zip = record[7],
                    FavoriteColor = record[8],
                    FavoriteFood = record[9],
                    FavoriteSport = record[10],
                    CreatedOn = DateTime.Parse(record[11]),
                    IsActive = Boolean.Parse(record[12])
                };
                people.Add(person);
            }
        }
        
        public class Person
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
