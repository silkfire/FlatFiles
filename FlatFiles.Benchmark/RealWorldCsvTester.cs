﻿using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using CsvHelper.Configuration;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class RealWorldCsvTester
    {
        [Benchmark]
        public void RunCsvHelper()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(directory, "TestFiles", "SampleData.csv");
            using var stream = File.OpenRead(path);
            using var textReader = new StreamReader(stream);
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };
            var csvReader = new CsvHelper.CsvReader(textReader, configuration);
            var map = csvReader.Context.AutoMap<SampleData>();
            map.Map(static x => x.YearStart).Name("YearStart").Index(0);
            map.Map(static x => x.YearEnd).Name("YearEnd").Index(1);
            map.Map(static x => x.LocationAbbreviation).Name("LocationAbbr").Index(2);
            map.Map(static x => x.LocationDescription).Name("LocationDesc").Index(3);
            map.Map(static x => x.DataSource).Name("DataSource").Index(4);
            map.Map(static x => x.Topic).Name("Topic").Index(5);
            map.Map(static x => x.Question).Name("Question").Index(6);
            map.Map(static x => x.Response).Name("Response").Index(7);
            map.Map(static x => x.DataValueUnit).Name("DataValueUnit").Index(8);
            map.Map(static x => x.DataValueType).Name("DataValueType").Index(9);
            map.Map(static x => x.DataValue).Name("DataValue").Index(10);
            map.Map(static x => x.AlternativeDataValue).Name("DataValueAlt").Index(11);
            map.Map(static x => x.DataValueFootnoteSymbol).Name("DataValueFootnoteSymbol").Index(12);
            map.Map(static x => x.DataValueFootnote).Name("DatavalueFootnote").Index(13);
            map.Map(static x => x.LowConfidenceLimit).Name("LowConfidenceLimit").Index(14);
            map.Map(static x => x.HighConfidenceLimit).Name("HighConfidenceLimit").Index(15);
            map.Map(static x => x.StratificationCategory1).Name("StratificationCategory1").Index(16);
            map.Map(static x => x.Stratification1).Name("Stratification1").Index(17);
            map.Map(static x => x.StratificationCategory2).Name("StratificationCategory2").Index(18);
            map.Map(static x => x.Stratification2).Name("Stratification2").Index(19);
            map.Map(static x => x.StratificationCategory3).Name("StratificationCategory3").Index(20);
            map.Map(static x => x.Stratification3).Name("Stratification3").Index(21);
            map.Map(static x => x.GeoLocation).Name("GeoLocation").Index(22);
            map.Map(static x => x.ResponseId).Name("ResponseID").Index(23);
            map.Map(static x => x.LocationId).Name("LocationID").Index(24);
            map.Map(static x => x.TopicId).Name("TopicID").Index(25);
            map.Map(static x => x.QuestionId).Name("QuestionID").Index(26);
            map.Map(static x => x.DataValueTypeId).Name("DataValueTypeID").Index(27);
            map.Map(static x => x.StratificationCategoryId1).Name("StratificationCategoryID1").Index(28);
            map.Map(static x => x.StratificationId1).Name("StratificationID1").Index(29);
            map.Map(static x => x.StratificationCategoryId2).Name("StratificationCategoryID2").Index(30);
            map.Map(static x => x.StratificationId2).Name("StratificationID2").Index(31);
            map.Map(static x => x.StratificationCategoryId3).Name("StratificationCategoryID3").Index(32);
            map.Map(static x => x.StratificationId3).Name("StratificationID3").Index(33);
            csvReader.Context.RegisterClassMap(map);
            csvReader.Read();
            csvReader.ReadHeader();
            var people = csvReader.GetRecords<SampleData>().ToArray();
        }

        [Benchmark]
        public void RunFlatFiles()
        {
            var mapper = DelimitedTypeMapper.Define<SampleData>();
            mapper.Property(static x => x.YearStart).ColumnName("YearStart");
            mapper.Property(static x => x.YearEnd).ColumnName("YearEnd");
            mapper.Property(static x => x.LocationAbbreviation).ColumnName("LocationAbbr");
            mapper.Property(static x => x.LocationDescription).ColumnName("LocationDesc");
            mapper.Property(static x => x.DataSource).ColumnName("DataSource");
            mapper.Property(static x => x.Topic).ColumnName("Topic");
            mapper.Property(static x => x.Question).ColumnName("Question");
            mapper.Property(static x => x.Response).ColumnName("Response");
            mapper.Property(static x => x.DataValueUnit).ColumnName("DataValueUnit");
            mapper.Property(static x => x.DataValueType).ColumnName("DataValueType");
            mapper.Property(static x => x.DataValue).ColumnName("DataValue");
            mapper.Property(static x => x.AlternativeDataValue).ColumnName("DataValueAlt");
            mapper.Property(static x => x.DataValueFootnoteSymbol).ColumnName("DataValueFootnoteSymbol");
            mapper.Property(static x => x.DataValueFootnote).ColumnName("DatavalueFootnote");
            mapper.Property(static x => x.LowConfidenceLimit).ColumnName("LowConfidenceLimit");
            mapper.Property(static x => x.HighConfidenceLimit).ColumnName("HighConfidenceLimit");
            mapper.Property(static x => x.StratificationCategory1).ColumnName("StratificationCategory1");
            mapper.Property(static x => x.Stratification1).ColumnName("Stratification1");
            mapper.Property(static x => x.StratificationCategory2).ColumnName("StratificationCategory2");
            mapper.Property(static x => x.Stratification2).ColumnName("Stratification2");
            mapper.Property(static x => x.StratificationCategory3).ColumnName("StratificationCategory3");
            mapper.Property(static x => x.Stratification3).ColumnName("Stratification3");
            mapper.Property(static x => x.GeoLocation).ColumnName("GeoLocation");
            mapper.Property(static x => x.ResponseId).ColumnName("ResponseID");
            mapper.Property(static x => x.LocationId).ColumnName("LocationID");
            mapper.Property(static x => x.TopicId).ColumnName("TopicID");
            mapper.Property(static x => x.QuestionId).ColumnName("QuestionID");
            mapper.Property(static x => x.DataValueTypeId).ColumnName("DataValueTypeID");
            mapper.Property(static x => x.StratificationCategoryId1).ColumnName("StratificationCategoryID1");
            mapper.Property(static x => x.StratificationId1).ColumnName("StratificationID1");
            mapper.Property(static x => x.StratificationCategoryId2).ColumnName("StratificationCategoryID2");
            mapper.Property(static x => x.StratificationId2).ColumnName("StratificationID2");
            mapper.Property(static x => x.StratificationCategoryId3).ColumnName("StratificationCategoryID3");
            mapper.Property(static x => x.StratificationId3).ColumnName("StratificationID3");

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(directory, "TestFiles", "SampleData.csv");
            using var stream = File.OpenRead(path);
            using var textReader = new StreamReader(stream);
            var people = mapper.Read(textReader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
        }

        private class SampleData
        {
            public int YearStart { get; set; }

            public int YearEnd { get; set; }

            public string LocationAbbreviation { get; set; }

            public string LocationDescription { get; set; }

            public string DataSource { get; set; }

            public string Topic { get; set; }

            public string Question { get; set; }

            public string Response { get; set; }

            public string DataValueUnit { get; set; }

            public string DataValueType { get; set; }

            public string DataValue { get; set; }

            public decimal? AlternativeDataValue { get; set; }

            public string DataValueFootnoteSymbol { get; set; }

            public string DataValueFootnote { get; set; }

            public decimal? LowConfidenceLimit { get; set; }

            public decimal? HighConfidenceLimit { get; set; }

            public string StratificationCategory1 { get; set; }

            public string Stratification1 { get; set; }

            public string StratificationCategory2 { get; set; }

            public string Stratification2 { get; set; }

            public string StratificationCategory3 { get; set; }

            public string Stratification3 { get; set; }

            public string GeoLocation { get; set; }

            public string ResponseId { get; set; }

            public string LocationId { get; set; }

            public string TopicId { get; set; }

            public string QuestionId { get; set; }

            public string DataValueTypeId { get; set; }

            public string StratificationCategoryId1 { get; set; }

            public string StratificationId1 { get; set; }

            public string StratificationCategoryId2 { get; set; }

            public string StratificationId2 { get; set; }

            public string StratificationCategoryId3 { get; set; }

            public string StratificationId3 { get; set; }
        }
    }
}
