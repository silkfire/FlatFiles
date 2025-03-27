﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class AsyncVsSyncTest
    {
        [Benchmark]
        public async Task<string> SyncTest()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new SampleData());
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
            mapper.CustomMapping(new GeoLocationColumn("GeoLocation"))
                .WithReader(static (d, v) => d.GeoLocation = (GeoLocation)v)
                .WithWriter(static d => d.GeoLocation);
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

            var textWriter = new StringWriter();

            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync("https://raw.githubusercontent.com/jehugaleahsa/FlatFiles/master/FlatFiles.Benchmark/TestFiles/SampleData.csv");

            using (var textReader = new StreamReader(stream))
            {
                var entities = mapper.Read(textReader, new DelimitedOptions { IsFirstRecordSchema = true });
                mapper.Write(textWriter, entities, new DelimitedOptions { IsFirstRecordSchema = true });
            }

            return textWriter.ToString();
        }

        [Benchmark]
        public async Task<string> AsyncTest()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new SampleData());
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
            mapper.CustomMapping(new GeoLocationColumn("GeoLocation"))
                .WithReader(static (d, v) => d.GeoLocation = (GeoLocation)v)
                .WithWriter(static d => d.GeoLocation);
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

            var textWriter = new StringWriter();

            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync("https://raw.githubusercontent.com/jehugaleahsa/FlatFiles/master/FlatFiles.Benchmark/TestFiles/SampleData.csv");

            using (var textReader = new StreamReader(stream))
            {
                var entities = mapper.ReadAsync(textReader, new DelimitedOptions { IsFirstRecordSchema = true });
                await mapper.WriteAsync(textWriter, entities, new DelimitedOptions { IsFirstRecordSchema = true });
            }

            return textWriter.ToString();
        }

        public class SampleData
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

            public GeoLocation GeoLocation { get; set; }

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

        public class GeoLocation
        {
            public decimal Latitude { get; set; }

            public decimal Longitude { get; set; }

            public override string ToString() => $"({Latitude}, {Longitude})";
        }

        public class GeoLocationColumn : ColumnDefinition<GeoLocation>
        {
            public GeoLocationColumn(string columnName) : base(columnName)
            {
            }

            protected override string OnFormat(IColumnContext context, GeoLocation value)
            {
                return value.ToString();
            }

            protected override GeoLocation OnParse(IColumnContext context, string value)
            {
                var parts = value.Substring(1, value.Length - 2).Split(',', 2);
                var result = new GeoLocation
                             {
                                 Latitude = Convert.ToDecimal(parts[0].Trim()),
                                 Longitude = Convert.ToDecimal(parts[1].Trim())
                             };

                return result;
            }
        }
    }
}
