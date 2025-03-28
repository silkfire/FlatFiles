﻿using System;
using System.Globalization;
using System.IO;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class DelimitedMultipleSchemaTester
    {
        public DelimitedMultipleSchemaTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        [TestMethod]
        public void TestReader_ReadThreeTypes()
        {
            var stringWriter = new StringWriter();
            var injector = GetSchemaInjector();
            var writer = new DelimitedWriter(stringWriter, injector, new DelimitedOptions { RecordSeparator = "\n" });
            writer.Write(["First Batch", 2]);
            writer.Write([1, "Bob Smith", new DateTime(2018, 06, 04), 12.34m]);
            writer.Write([2, "Jane Doe", new DateTime(2018, 06, 05), 34.56m]);
            writer.Write([46.9m, 23.45m, true]);
            var output = stringWriter.ToString();
            Assert.AreEqual("""
                            First Batch,2
                            1,Bob Smith,20180604,12.34
                            2,Jane Doe,20180605,34.56
                            46.9,23.45,True

                            """, output);

            var stringReader = new StringReader(output);
            var selector = GetSchemaSelector();
            var reader = new DelimitedReader(stringReader, selector);

            Assert.IsTrue(reader.Read(), "The header record could not be read.");            
            var headerValues = reader.GetValues();
            Assert.AreEqual(2, headerValues.Length);
            Assert.AreEqual("First Batch", headerValues[0]);
            Assert.AreEqual(2, headerValues[1]);

            Assert.IsTrue(reader.Read(), "The first data record could not be read.");
            var dataValues1 = reader.GetValues();
            Assert.AreEqual(4, dataValues1.Length);
            Assert.AreEqual(1, dataValues1[0]);
            Assert.AreEqual("Bob Smith", dataValues1[1]);
            Assert.AreEqual(new DateTime(2018, 6, 4), dataValues1[2]);
            Assert.AreEqual(12.34m, dataValues1[3]);

            Assert.IsTrue(reader.Read(), "The second data record could not be read.");
            var dataValues2 = reader.GetValues();
            Assert.AreEqual(4, dataValues2.Length);
            Assert.AreEqual(2, dataValues2[0]);
            Assert.AreEqual("Jane Doe", dataValues2[1]);
            Assert.AreEqual(new DateTime(2018, 6, 5), dataValues2[2]);
            Assert.AreEqual(34.56m, dataValues2[3]);

            Assert.IsTrue(reader.Read(), "The footer record could not be read.");
            var footerValues = reader.GetValues();
            Assert.AreEqual(3, footerValues.Length);
            Assert.AreEqual(46.9m, footerValues[0]);
            Assert.AreEqual(23.45m, footerValues[1]);
            Assert.AreEqual(true, footerValues[2]);

            Assert.IsFalse(reader.Read());
        }

        private static DelimitedSchemaInjector GetSchemaInjector()
        {
            var injector = new DelimitedSchemaInjector();
            injector.When(static values => values.Length == 2).Use(GetHeaderSchema());
            injector.When(static values => values.Length == 3).Use(GetFooterSchema());
            injector.WithDefault(GetRecordSchema());
            return injector;
        }

        private static DelimitedSchemaSelector GetSchemaSelector()
        {
            var selector = new DelimitedSchemaSelector();
            selector.When(static values => values.Length == 2).Use(GetHeaderSchema());
            selector.When(static values => values.Length == 3).Use(GetFooterSchema());
            selector.WithDefault(GetRecordSchema());
            return selector;
        }

        private static DelimitedSchema GetHeaderSchema()
        {
            var mapper = GetHeaderTypeMapper();
            return mapper.GetSchema();
        }

        private static DelimitedSchema GetRecordSchema()
        {
            var mapper = GetRecordTypeMapper();
            return mapper.GetSchema();
        }

        private static DelimitedSchema GetFooterSchema()
        {
            var mapper = GetFooterTypeMapper();
            return mapper.GetSchema();
        }

        [TestMethod]
        public void TestTypeMapper_ReadThreeTypes()
        {
            var stringWriter = new StringWriter();
            var injector = GetTypeMapperInjector();
            var writer = injector.GetWriter(stringWriter, new DelimitedOptions { RecordSeparator = "\n" });
            writer.Write(new HeaderRecord { BatchName = "First Batch", RecordCount = 2 });
            writer.Write(new DataRecord { Id = 1, Name = "Bob Smith", CreatedOn = new DateTime(2018, 06, 04), TotalAmount = 12.34m });
            writer.Write(new DataRecord { Id = 2, Name = "Jane Doe", CreatedOn = new DateTime(2018, 06, 05), TotalAmount = 34.56m });
            writer.Write(new FooterRecord { TotalAmount = 46.9m, AverageAmount = 23.45m, IsCriteriaMet = true });
            var output = stringWriter.ToString();
            Assert.AreEqual("""
                            First Batch,2
                            1,Bob Smith,20180604,12.34
                            2,Jane Doe,20180605,34.56
                            46.9,23.45,True

                            """, output);

            var selector = GetTypeMapperSelector();
            var stringReader = new StringReader(output);
            var reader = selector.GetReader(stringReader);

            Assert.IsTrue(reader.Read(), "The header record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(HeaderRecord));
            Assert.AreEqual("First Batch", ((HeaderRecord)reader.Current).BatchName);
            Assert.AreEqual(2, ((HeaderRecord)reader.Current).RecordCount);

            Assert.IsTrue(reader.Read(), "The first data record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(DataRecord));
            Assert.AreEqual(1, ((DataRecord)reader.Current).Id);
            Assert.AreEqual("Bob Smith", ((DataRecord)reader.Current).Name);
            Assert.AreEqual(new DateTime(2018, 6, 4), ((DataRecord)reader.Current).CreatedOn);
            Assert.AreEqual(12.34m, ((DataRecord)reader.Current).TotalAmount);

            Assert.IsTrue(reader.Read(), "The second data record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(DataRecord));
            Assert.AreEqual(2, ((DataRecord)reader.Current).Id);
            Assert.AreEqual("Jane Doe", ((DataRecord)reader.Current).Name);
            Assert.AreEqual(new DateTime(2018, 6, 5), ((DataRecord)reader.Current).CreatedOn);
            Assert.AreEqual(34.56m, ((DataRecord)reader.Current).TotalAmount);

            Assert.IsTrue(reader.Read(), "The footer record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(FooterRecord));
            Assert.AreEqual(46.9m, ((FooterRecord)reader.Current).TotalAmount);
            Assert.AreEqual(23.45m, ((FooterRecord)reader.Current).AverageAmount);
            Assert.IsTrue(((FooterRecord)reader.Current).IsCriteriaMet);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void TestTypeMapper_ReadThreeTypes_WithMetadataRecord()
        {
            var stringWriter = new StringWriter();
            var injector = GetTypeMapperInjector();
            var writer = injector.GetWriter(stringWriter, new DelimitedOptions { RecordSeparator = "\n" });
            writer.Write(new HeaderRecord { BatchName = "First Batch", RecordCount = 2 });
            writer.Write(new DataRecord { Id = 1, Name = "Bob Smith", CreatedOn = new DateTime(2018, 06, 04), TotalAmount = 12.34m });
            writer.Write(new DataRecord { Id = 2, Name = "Jane Doe", CreatedOn = new DateTime(2018, 06, 05), TotalAmount = 34.56m });
            writer.Write(new FooterRecord { TotalAmount = 46.9m, AverageAmount = 23.45m, IsCriteriaMet = true });
            var output = stringWriter.ToString();
            Assert.AreEqual("""
                            First Batch,2
                            1,Bob Smith,20180604,12.34
                            2,Jane Doe,20180605,34.56
                            46.9,23.45,True

                            """, output);

            var selector = GetTypeMapperSelector(true);
            var stringReader = new StringReader(output);
            var reader = selector.GetReader(stringReader);

            Assert.IsTrue(reader.Read(), "The header record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(HeaderRecord));
            Assert.AreEqual("First Batch", ((HeaderRecord)reader.Current).BatchName);
            Assert.AreEqual(2, ((HeaderRecord)reader.Current).RecordCount);

            Assert.IsTrue(reader.Read(), "The first data record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(DataRecord));
            Assert.AreEqual(1, ((DataRecord)reader.Current).Id);
            Assert.AreEqual("Bob Smith", ((DataRecord)reader.Current).Name);
            Assert.AreEqual(new DateTime(2018, 6, 4), ((DataRecord)reader.Current).CreatedOn);
            Assert.AreEqual(12.34m, ((DataRecord)reader.Current).TotalAmount);

            Assert.IsTrue(reader.Read(), "The second data record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(DataRecord));
            Assert.AreEqual(2, ((DataRecord)reader.Current).Id);
            Assert.AreEqual("Jane Doe", ((DataRecord)reader.Current).Name);
            Assert.AreEqual(new DateTime(2018, 6, 5), ((DataRecord)reader.Current).CreatedOn);
            Assert.AreEqual(34.56m, ((DataRecord)reader.Current).TotalAmount);

            Assert.IsTrue(reader.Read(), "The footer record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(FooterRecord));
            Assert.AreEqual(46.9m, ((FooterRecord)reader.Current).TotalAmount);
            Assert.AreEqual(23.45m, ((FooterRecord)reader.Current).AverageAmount);
            Assert.IsTrue(((FooterRecord)reader.Current).IsCriteriaMet);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        [ExpectedException(typeof(RecordProcessingException))]
        public void TestReader_UnknownType()
        {
            var stringReader = new StringReader("What's this weird thing?");
            var selector = GetSchemaSelector();
            var reader = new DelimitedReader(stringReader, selector);

            reader.Read();
        }

        [TestMethod]
        public void TestReader_UnknownType_IgnoreUnknown_SkipsRecord()
        {
            var stringReader = new StringReader("What's this weird thing?");
            var selector = GetSchemaSelector();
            var reader = new DelimitedReader(stringReader, selector);
            reader.RecordError += static (_, e) => e.IsHandled = true;
            Assert.IsFalse(reader.Read());
        }

        private static DelimitedTypeMapperSelector GetTypeMapperSelector(bool hasMetadata = false)
        {
            var selector = new DelimitedTypeMapperSelector();
            selector.WithDefault(GetRecordTypeMapper(hasMetadata));
            selector.When(static x => x.Length == 2).Use(GetHeaderTypeMapper());
            selector.When(static x => x.Length == 3).Use(GetFooterTypeMapper());
            return selector;
        }

        private static DelimitedTypeMapperInjector GetTypeMapperInjector()
        {
            var selector = new DelimitedTypeMapperInjector();
            selector.WithDefault(GetRecordTypeMapper());
            selector.When<HeaderRecord>().Use(GetHeaderTypeMapper());
            selector.When<FooterRecord>().Use(GetFooterTypeMapper());
            return selector;
        }

        private static IDelimitedTypeMapper<HeaderRecord> GetHeaderTypeMapper()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new HeaderRecord());
            mapper.Property(static x => x.BatchName);
            mapper.Property(static x => x.RecordCount);
            return mapper;
        }

        private static IDelimitedTypeMapper<DataRecord> GetRecordTypeMapper(bool hasMetadata = false)
        {
            var mapper = DelimitedTypeMapper.Define(static () => new DataRecord());
            mapper.Property(static x => x.Id);
            mapper.Property(static x => x.Name);
            mapper.Property(static x => x.CreatedOn).InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(static x => x.TotalAmount);
            if (hasMetadata)
            {
                mapper.CustomMapping(new RecordNumberColumn("row_num")
                {
                    IncludeSchema = true,
                    IncludeSkippedRecords = true
                }).WithReader(static r => r.RecordNumber);
            }
            return mapper;
        }

        private static IDelimitedTypeMapper<FooterRecord> GetFooterTypeMapper()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new FooterRecord());
            mapper.Property(static x => x.TotalAmount);
            mapper.Property(static x => x.AverageAmount);
            mapper.Property(static x => x.IsCriteriaMet);
            return mapper;
        }

        public class HeaderRecord
        {
            public string BatchName { get; set; }

            public int RecordCount { get; set; }
        }

        public class FooterRecord
        {
            public decimal? TotalAmount { get; set; }

            public decimal? AverageAmount { get; set; }

            public bool IsCriteriaMet { get; set; }
        }

        public class DataRecord
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime? CreatedOn { get; set; }

            public decimal TotalAmount { get; set; }

            public int? RecordNumber { get; set; }
        }
    }
}
