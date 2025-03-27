﻿using System;
using System.IO;
using System.Linq;
using System.Globalization;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the DelimitedParser class.
    /// </summary>
    [TestClass]
    public class DelimitedReaderTester
    {
        /// <summary>
        /// Setup for tests.
        /// </summary>
        public DelimitedReaderTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        public void TestCtor_NullWriter_NoSchema_Throws()
        {
            TextReader reader = null;
            Assert.ThrowsException<ArgumentNullException>(() => new DelimitedReader(reader));
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        public void TestCtor_NullWriter_WithSchema_Throws()
        {
            TextReader reader = null;
            var schema = new DelimitedSchema();
            Assert.ThrowsException<ArgumentNullException>(() => new DelimitedReader(reader, schema));
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        public void TestCtor_SchemaNull_Throws()
        {
            TextReader reader = new StringReader(String.Empty);
            DelimitedSchema schema = null;
            Assert.ThrowsException<ArgumentNullException>(() => new DelimitedReader(reader, schema));
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [TestMethod]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            const string text = "a,b,c";
            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader);
            var canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            object[] expected = ["a", "b", "c"];
            var actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual);
            canRead = parser.Read();
            Assert.IsFalse(canRead, "No more records should have been read.");
        }

        [TestMethod]
        public void TestRead_InvalidConversion_Throws()
        {
            const string text = "a";
            var stringReader = new StringReader(text);
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("First"));
            var parser = new DelimitedReader(stringReader, schema);
            Assert.ThrowsException<RecordProcessingException>(() => parser.Read());
        }

        /// <summary>
        /// If we skip a bad record, it should not result in a parsing error.
        /// </summary>
        [TestMethod]
        public void TestRead_SkipRecord_NoParsingError()
        {
            const string text = "a,b,c";
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("A"));
            schema.AddColumn(new DateTimeColumn("B"));
            schema.AddColumn(new GuidColumn("C"));

            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader, schema);
            var canRead = parser.Skip();
            Assert.IsTrue(canRead, "Could not skip the record.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "No more records should have been read.");
        }

        /// <summary>
        /// If we try to get the values before calling Read, an exception should be thrown.
        /// </summary>
        [TestMethod]
        public void TestRead_GetValuesWithoutReading_Throws()
        {
            var text = "a,b,c";
            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader);
            Assert.ThrowsException<InvalidOperationException>(parser.GetValues);
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [TestMethod]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            var text = "a,b,c";
            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader);
            var canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            object[] expected = ["a", "b", "c"];
            var actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual);
            actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If Read returns false, requesting the Values will cause an exception to be thrown.
        /// </summary>
        [TestMethod]
        public void TestRead_ValuesAfterEndOfFile_Throws()
        {
            var text = "a,b,c";
            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader);
            var canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "We should have reached the end of the file.");
            Assert.ThrowsException<InvalidOperationException>(parser.GetValues);
        }

        /// <summary>
        /// If a record contains a quote, it should still parse correctly.
        /// </summary>
        [TestMethod]
        public void TestRead_EmbeddedQuote_ParsesCorrectly()
        {
            var text = "123;Todd's Bait Shop;1/17/2014";
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DateTimeColumn("created"));
            var options = new DelimitedOptions
            {
                IsFirstRecordSchema = false,
                Separator = ";"
            };

            var stringReader = new StringReader(text);
            var reader = new DelimitedReader(stringReader, schema, options);

            var result = reader.Read();

            Assert.IsTrue(result, "Could not read the record.");
            object[] expected = [123, "Todd's Bait Shop", new DateTime(2014, 1, 17)];
            var actual = reader.GetValues();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If we do not explicitly say that the first record is the schema, we cannot retrieve it later.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_NotExtracted_Throws()
        {
            var text = "a,b,c";
            var stringReader = new StringReader(text);
            var options = new DelimitedOptions { IsFirstRecordSchema = false };
            IReader parser = new DelimitedReader(stringReader, options);
            var schema = parser.GetSchema();
            Assert.IsNull(schema, "No schema was provided or located in the file. Null should be returned.");
        }

        /// <summary>
        /// If we say that the first record is the schema, we can retrieve it later on.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_Extracted_ReturnsColumnNames()
        {
            var text = "a,b,c";
            var stringReader = new StringReader(text);
            var options = new DelimitedOptions { IsFirstRecordSchema = true };
            IReader parser = new DelimitedReader(stringReader, options);
            var schema = parser.GetSchema();
            Assert.IsTrue(schema.ColumnDefinitions.All(static d => d is StringColumn), "Not all of the columns were treated as strings.");
            var actual = schema.ColumnDefinitions.Select(static d => d.ColumnName).ToArray();
            string[] expected = ["a", "b", "c"];
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If we provide a schema and say the first record is the schema, our schema takes priority
        /// and we throw away the first record.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_FirstRecordSchema_SkipsFirstRecord()
        {
            const string text = "id,name,created";
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            var stringReader = new StringReader(text);
            var options = new DelimitedOptions { IsFirstRecordSchema = true };
            IReader parser = new DelimitedReader(stringReader, schema, options);
            var actual = parser.GetSchema();
            Assert.AreSame(schema, actual);
            Assert.IsFalse(parser.Read(), "The schema record was not skipped.");
        }

        /// <summary>
        /// If we provide a record filter, those records should be skipped while processing the file.
        /// </summary>
        [TestMethod]
        public void TestRead_WithSeparatedRecordFilter_SkipsRecordsMatchingCriteria()
        {
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            const string text = """
                                123,Bob Smith,4/21/2017
                                This is not a real record
                                234,Jay Smith,5/21/2017
                                """;
            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader, schema);
            parser.RecordRead += static (_, e) =>
            {
                e.IsSkipped = e.Values.Length < 3;
            };

            Assert.IsTrue(parser.Read(), "Could not read the first record.");
            var actual1 = parser.GetValues();
            CollectionAssert.AreEqual(new object[] { 123, "Bob Smith", new DateTime(2017, 04, 21) }, actual1);

            Assert.IsTrue(parser.Read(), "Could not read the second record.");
            var actual2 = parser.GetValues();
            CollectionAssert.AreEqual(new object[] { 234, "Jay Smith", new DateTime(2017, 05, 21) }, actual2);

            Assert.IsFalse(parser.Read(), "There should not be any more records.");
        }

        /// <summary>
        /// We should be able to inspect each raw record as we process a file.
        /// </summary>
        [TestMethod]
        public void TestRead_InspectRawRecords()
        {
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            const string text = """123,"Bob Smith",4/21/2017""";
            var stringReader = new StringReader(text);
            var reader = new DelimitedReader(stringReader, schema);
            reader.RecordRead += static (_, e) => {
                Assert.AreEqual("""123,"Bob Smith",4/21/2017""", e.RecordContext.Record);
                CollectionAssert.AreEqual(new[] { "123", "Bob Smith", "4/21/2017" }, e.RecordContext.Values);
            };
            reader.RecordParsed += static (_, e) => {
                Assert.AreEqual("""123,"Bob Smith",4/21/2017""", e.RecordContext.Record);
                CollectionAssert.AreEqual(new[] { "123", "Bob Smith", "4/21/2017" }, e.RecordContext.Values);
            };
            Assert.IsTrue(reader.Read());
            Assert.IsFalse(reader.Read());
        }

        internal class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Created { get; set; }

            public int? ParentId { get; set; }

            public bool? IsActive { get; set; }
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_ParsesValues()
        {
            const string text = "123,Bob,1/19/2013";
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader, schema);
            Assert.IsTrue(parser.Read(), "The first record was skipped.");
            var actual = parser.GetValues();
            object[] expected = [123, "Bob", new DateTime(2013, 1, 19)];
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values, also when columns are quoted.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_ParsesValues_Quoted()
        {
            const string text = "123,\"Bob\",1/19/2013";
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader, schema);
            Assert.IsTrue(parser.Read(), "The first record was skipped.");
            var actual = parser.GetValues();
            object[] expected = [123, "Bob", new DateTime(2013, 1, 19)];
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If we provide a schema, the records in the file must have a value for each column.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_WrongNumberOfColumns_Throws()
        {
            const string text = "123,Bob";
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            var stringReader = new StringReader(text);
            var parser = new DelimitedReader(stringReader, schema);
            Assert.ThrowsException<RecordProcessingException>(() => parser.Read());
        }

        /// <summary>
        /// If the first record is the schema, the records in the file must have the
        /// same number of columns.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_FirstRecordSchema_TooFewColumns_Throws()
        {
            const string text = """
                                id,name,created
                                123,Bob
                                """;
            var stringReader = new StringReader(text);
            var options = new DelimitedOptions { IsFirstRecordSchema = true };
            var parser = new DelimitedReader(stringReader, options);
            Assert.ThrowsException<RecordProcessingException>(() => parser.Read());
        }

        /// <summary>
        /// If the first record is the schema, the records in the file can have more columns that are ignored.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_FirstRecordSchema_TooManyColumns_IgnoresTrailing()
        {
            const string text = """
                                id,name,created
                                123,Bob,1/19/2013,Hello
                                """;
            var stringReader = new StringReader(text);
            var options = new DelimitedOptions { IsFirstRecordSchema = true };
            var parser = new DelimitedReader(stringReader, options);
            Assert.IsTrue(parser.Read(), "The record could not be read.");
            Assert.AreEqual(parser.GetSchema().ColumnDefinitions.Count, parser.GetValues().Length);
        }

        /// <summary>
        /// If a record has a blank value at the end of a line, null should be
        /// returned for the last column.
        /// </summary>
        [TestMethod]
        public void TestGetValues_BlankTrailingSection_ReturnsNull()
        {
            var options = new DelimitedOptions { IsFirstRecordSchema = true };
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id"))
                .AddColumn(new StringColumn("name"))
                .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" })
                .AddColumn(new StringColumn("trailing"));
            object[] sources = [123, "Bob", new DateTime(2013, 1, 19), ""];

            var stringWriter = new StringWriter();
            var builder = new DelimitedWriter(stringWriter, schema, options);
            builder.Write(sources);

            var stringReader = new StringReader(stringWriter.ToString());
            var parser = new DelimitedReader(stringReader, schema, options);
            Assert.IsTrue(parser.Read(), "No records were found.");
            var values = parser.GetValues();
            Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length);
            Assert.AreEqual(sources[0], values[0]);
            Assert.AreEqual(sources[1], values[1]);
            Assert.AreEqual(sources[2], values[2]);
            Assert.IsNull(values[3]);
            Assert.IsFalse(parser.Read(), "Too many records were found.");
        }

        /// <summary>
        /// If a record has a blank value in the middle of a line, null should be
        /// returned for that column.
        /// </summary>
        [TestMethod]
        public void TestGetValues_BlankMiddleSection_ReturnsNull()
        {
            using (var stream = new MemoryStream())
            {
                var options = new DelimitedOptions { IsFirstRecordSchema = true };
                var schema = new DelimitedSchema();
                schema.AddColumn(new Int32Column("id"))
                    .AddColumn(new StringColumn("name"))
                    .AddColumn(new StringColumn("middle"))
                    .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" });
                object[] sources = [123, "Bob", "", new DateTime(2013, 1, 19)];

                var stringWriter = new StringWriter();
                var builder = new DelimitedWriter(stringWriter, schema, options);
                builder.Write(sources);

                var stringReader = new StringReader(stringWriter.ToString());
                var parser = new DelimitedReader(stringReader, schema, options);
                Assert.IsTrue(parser.Read(), "No records were found.");
                var values = parser.GetValues();
                Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length);
                Assert.AreEqual(sources[0], values[0]);
                Assert.AreEqual(sources[1], values[1]);
                Assert.IsNull(values[2]);
                Assert.AreEqual(sources[3], values[3]);
                Assert.IsFalse(parser.Read(), "Too many records were found.");
            }
        }

        /// <summary>
        /// If a record has a blank value in the front of a line, null should be
        /// returned for that column.
        /// </summary>
        [TestMethod]
        public void TestGetValues_BlankLeadingSection_ReturnsNull()
        {
            using (var stream = new MemoryStream())
            {
                var options = new DelimitedOptions { IsFirstRecordSchema = true };
                var schema = new DelimitedSchema();
                schema.AddColumn(new StringColumn("leading"))
                    .AddColumn(new Int32Column("id"))
                    .AddColumn(new StringColumn("name"))
                    .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" });
                object[] sources = ["", 123, "Bob", new DateTime(2013, 1, 19)];

                var stringWriter = new StringWriter();
                var builder = new DelimitedWriter(stringWriter, schema, options);
                builder.Write(sources);

                var stringReader = new StringReader(stringWriter.ToString());
                var parser = new DelimitedReader(stringReader, schema, options);
                Assert.IsTrue(parser.Read(), "No records were found.");
                var values = parser.GetValues();
                Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length);
                Assert.IsNull(values[0]);
                Assert.AreEqual(sources[1], values[1]);
                Assert.AreEqual(sources[2], values[2]);
                Assert.AreEqual(sources[3], values[3]);
                Assert.IsFalse(parser.Read(), "Too many records were found.");
            }
        }

        [TestMethod]
        public void TestRead_ZeroLengthColumn()
        {
            //---- Arrange -----------------------------------------------------
            var text = "104\t20\t1000\t00\tLausanne\tLausanne\tVD\t2\t\t0\t130\t5586\t19880301";
            var options = new DelimitedOptions { IsFirstRecordSchema = false, Separator = "\t" };
            var schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("OnrpId"))
                .AddColumn(new Int32Column("Type"))
                .AddColumn(new Int32Column("ZipCode"))
                .AddColumn(new StringColumn("ZipCodeAddOn"))
                .AddColumn(new StringColumn("TownShortName"))
                .AddColumn(new StringColumn("TownOfficialName"))
                .AddColumn(new StringColumn("CantonAbbreviation"))
                .AddColumn(new Int16Column("MainLanguageCode"))
                .AddColumn(new Int16Column("OtherLanguageCode"))
                .AddColumn(new ByteColumn("HasSortfileData"))
                .AddColumn(new Int32Column("LetterServiceOnrpId"))
                .AddColumn(new Int32Column("MunicipalityId"))
                .AddColumn(new StringColumn("ValidFrom"));

            var stringReader = new StringReader(text);
            var testee = new DelimitedReader(stringReader, options);

            //---- Act ---------------------------------------------------------
            var result = testee.Read();

            //---- Assert ------------------------------------------------------
            Assert.IsTrue(result);
            Assert.AreEqual(schema.ColumnDefinitions.Count, testee.GetValues().Length);
        }

        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [TestMethod]
        public void TestTypeMapper_Roundtrip()
        {
            var mapper = DelimitedTypeMapper.Define<Person>();
            mapper.Property(static p => p.Id).ColumnName("id");
            mapper.Property(static p => p.Name).ColumnName("name");
            mapper.Property(static p => p.Created).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(static p => p.ParentId).ColumnName("parent_id");

            var bob = new Person { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19), ParentId = null };
            var options = new DelimitedOptions { IsFirstRecordSchema = true, Separator = "\t" };

            var stringWriter = new StringWriter();
            mapper.Write(stringWriter, [bob], options);

            var stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader, options).ToArray();
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.IsNotNull(person);
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
            Assert.AreEqual(bob.ParentId, person.ParentId);
        }

        /// <summary>
        /// We should be able to write and read values using a type mapper with a null value.
        /// </summary>
        [TestMethod]
        public void TestTypeMapper_RoundtripWithNull()
        {
            var mapper = DelimitedTypeMapper.Define<Person>();
            mapper.Property(static p => p.Id).ColumnName("id");
            mapper.Property(static p => p.Name).ColumnName("name");
            mapper.Property(static p => p.Created).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            var bob = new Person { Id = 123, Name = null, Created = new DateTime(2013, 1, 19) };
            var options = new DelimitedOptions { IsFirstRecordSchema = true, Separator = "\t" };

            var stringWriter = new StringWriter();
            mapper.Write(stringWriter, [bob], options);

            var stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader, options).ToArray();
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.IsNotNull(person);
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
        }

        /// <summary>
        /// We should be able to write and read values using a type mapper with a null value.
        /// </summary>
        [TestMethod]
        public void TestTypeMapper_IgnoredColumns_RoundTrips()
        {
            var mapper = DelimitedTypeMapper.Define<Person>();
            mapper.Property(static p => p.Id).ColumnName("id");
            mapper.Ignored();
            mapper.Ignored();
            mapper.Property(static p => p.Name).ColumnName("name");
            mapper.Ignored();
            mapper.Property(static p => p.Created).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            var bob = new Person { Id = 123, Name = "Bob Smith", Created = new DateTime(2013, 1, 19) };

            var stringWriter = new StringWriter();
            mapper.Write(stringWriter, [bob]);

            var stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.IsNotNull(person);
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
        }

        /// <summary>
        /// Test to make sure the sample CSV from http://www.creativyst.com/Doc/Articles/CSV/CSV01.htm works.
        /// </summary>
        [TestMethod]
        public void TestReader_creativyst_example()
        {
            const string text = """"
                                John,Doe,120 jefferson st.,Riverside, NJ, 08075
                                Jack,McGinnis,220 hobo Av.,Phila, PA,09119
                                "John ""Da Man""",Repici,120 Jefferson St.,Riverside, NJ,08075
                                Stephen,Tyler,"7452 Terrace ""At the Plaza"" road",SomeTown,SD, 91234
                                ,Blankman,,SomeTown, SD, 00298
                                "Joan ""the bone"", Anne",Jet,"9th, at Terrace plc",Desert City, CO,00123

                                """";
            var stringReader = new StringReader(text);
            var reader = new DelimitedReader(stringReader);
            Assert.IsTrue(reader.Read(), "Could not read the first record.");
            AssertValues(reader, "John", "Doe", "120 jefferson st.", "Riverside", "NJ", "08075");
            Assert.IsTrue(reader.Read(), "Could not read the second record.");
            AssertValues(reader, "Jack", "McGinnis", "220 hobo Av.", "Phila", "PA", "09119");
            Assert.IsTrue(reader.Read(), "Could not read the third record.");
            AssertValues(reader, "John \"Da Man\"", "Repici", "120 Jefferson St.", "Riverside", "NJ", "08075");
            Assert.IsTrue(reader.Read(), "Could not read the fourth record.");
            AssertValues(reader, "Stephen", "Tyler", "7452 Terrace \"At the Plaza\" road", "SomeTown", "SD", "91234");
            Assert.IsTrue(reader.Read(), "Could not read the fifth record.");
            AssertValues(reader, null, "Blankman",null, "SomeTown", "SD", "00298");
            Assert.IsTrue(reader.Read(), "Could not read the sixth record.");
            AssertValues(reader, "Joan \"the bone\", Anne", "Jet", "9th, at Terrace plc", "Desert City", "CO", "00123");
            Assert.IsFalse(reader.Read(), "Read too many records.");
        }

        private static void AssertValues(DelimitedReader reader, string firstName, string lastName, string street, string city, string state, string zip)
        {
            var values = reader.GetValues();
            Assert.AreEqual(6, values.Length);
            Assert.AreEqual(firstName, values[0]);
            Assert.AreEqual(lastName, values[1]);
            Assert.AreEqual(street, values[2]);
            Assert.AreEqual(city, values[3]);
            Assert.AreEqual(state, values[4]);
            Assert.AreEqual(zip, values[5]);
        }

        [TestMethod]
        public void TestTypeMapper_NullableBoolean_RoundTripsNull()
        {
            var mapper = DelimitedTypeMapper.Define<Person>();
            mapper.Property(static x => x.IsActive).ColumnName("is_active");

            var person = new Person { IsActive = null };

            var stringWriter = new StringWriter();
            mapper.Write(stringWriter, [person]);

            var stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var first = people.SingleOrDefault();
            Assert.IsNotNull(first);
            Assert.IsNull(first.IsActive);
        }

        [TestMethod]
        public void TestTypeMapper_NullableBoolean_RoundTripsFalse()
        {
            var mapper = DelimitedTypeMapper.Define<Person>();
            mapper.Property(static x => x.IsActive).ColumnName("is_active");

            var person = new Person { IsActive = false };

            var stringWriter = new StringWriter();
            mapper.Write(stringWriter, [person]);

            var stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var first = people.SingleOrDefault();
            Assert.IsNotNull(first);
            Assert.AreNotEqual(true, first.IsActive);
        }

        [TestMethod]
        public void TestTypeMapper_NullableBoolean_RoundTripsTrue()
        {
            var mapper = DelimitedTypeMapper.Define<Person>();
            mapper.Property(static x => x.IsActive).ColumnName("is_active");

            var person = new Person { IsActive = true };

            var stringWriter = new StringWriter();
            mapper.Write(stringWriter, [person]);

            var stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var first = people.SingleOrDefault();
            Assert.IsNotNull(first);
            Assert.AreEqual(true, first.IsActive);
        }

        [TestMethod]
        public void TestTypeMapper_BadRecordColumn_SkipError()
        {
            const string data = """
                                1,2017-06-11,John Smith
                                2,2017-12-32,Tom Stallon
                                3,2017-08-13,Walter Kay
                                """;
            var mapper = DelimitedTypeMapper.Define<Person>();
            mapper.Property(x => x.Id);
            mapper.Property(x => x.Created);
            mapper.Property(x => x.Name);

            var stringReader = new StringReader(data);
            List<int> errorRecords = [];
            var reader = mapper.GetReader(stringReader);
            reader.RecordError += (_, e) =>
            {
                errorRecords.Add(e.RecordContext.PhysicalRecordNumber);
                e.IsHandled = true;
            };
            var people = reader.ReadAll().ToArray();
            Assert.AreEqual(2, people.Length);
            Assert.AreEqual(1, errorRecords.Count);
            Assert.AreEqual(2, errorRecords[0]);
        }

        [TestMethod]
        public void TestReader_NullToDateTime_ProvidesUsefulErrorMessage()
        {
            const string rawData = "Hello,,Goodbye";
            var reader = new StringReader(rawData);

            var mapper = DelimitedTypeMapper.Define<ClassWithDate>();
            mapper.Ignored();
            mapper.Property(static x => x.DateTime);
            mapper.Ignored();

            try
            {
                var records = mapper.Read(reader).ToArray();
                Assert.IsTrue(false); // The line above should always fail.
            }
            catch (FlatFileException)
            {                
            }
        }

        [TestMethod]
        public void TestReader_DefaultRecordSeparator_HandlesLinuxNewline()
        {
            const string rawData = "a,b,c\nd,e,f\nh,i,j";
            var reader = new StringReader(rawData);
            var csvReader = new DelimitedReader(reader);
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "d", "e", "f" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "h", "i", "j" }, csvReader.GetValues());
            Assert.IsFalse(csvReader.Read());
        }

        [TestMethod]
        public void TestReader_DefaultRecordSeparator_HandlesMacNewline()
        {
            const string rawData = "a,b,c\rd,e,f\rh,i,j";
            var reader = new StringReader(rawData);
            var csvReader = new DelimitedReader(reader);
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "d", "e", "f" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "h", "i", "j" }, csvReader.GetValues());
            Assert.IsFalse(csvReader.Read());
        }

        [TestMethod]
        public void TestReader_DefaultRecordSeparator_HandlesWindowsNewline()
        {
            const string rawData = "a,b,c\r\nd,e,f\r\nh,i,j";
            var reader = new StringReader(rawData);
            var csvReader = new DelimitedReader(reader);
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "d", "e", "f" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "h", "i", "j" }, csvReader.GetValues());
            Assert.IsFalse(csvReader.Read());
        }

        [TestMethod]
        public void TestReader_DefaultRecordSeparator_HandlesMixedNewlines()
        {
            const string rawData = "a,b,c\rd,e,f\nh,i,j\r\nk,l,m";
            var reader = new StringReader(rawData);
            var csvReader = new DelimitedReader(reader);
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "d", "e", "f" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "h", "i", "j" }, csvReader.GetValues());
            Assert.IsTrue(csvReader.Read());
            CollectionAssert.AreEqual(new[] { "k", "l", "m" }, csvReader.GetValues());
            Assert.IsFalse(csvReader.Read());
        }

        internal class ClassWithDate
        {
            public DateTime DateTime { get; set; }
        }
    }
}
