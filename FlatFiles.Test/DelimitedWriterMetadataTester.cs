using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class DelimitedWriterMetadataTester
    {
        [TestMethod]
        public void TestWriter_WithSchema_SchemaNotCounted()
        {
            var outputMapper = DelimitedTypeMapper.Define(static () => new Person());
            outputMapper.Property(static x => x.Name);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber"))
                .WithReader(static (p, v) => p.RecordNumber = (int)v)
                .WithWriter(static p => p.RecordNumber);
            outputMapper.Property(static x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            var writer = new StringWriter();
            outputMapper.Write(writer, people, new DelimitedOptions { IsFirstRecordSchema = true });
            var output = writer.ToString();

            var inputMapper = DelimitedTypeMapper.Define(static () => new Person());
            inputMapper.Property(static x => x.Name);
            inputMapper.Property(static x => x.RecordNumber);
            inputMapper.Property(static x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            var reader = new StringReader(output);
            var results = inputMapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(3, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [TestMethod]
        public void TestWriter_WithSchema_SchemaCounted()
        {
            var outputMapper = DelimitedTypeMapper.Define(static () => new Person());
            outputMapper.Property(static x => x.Name);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = true })
                .WithReader(static (p, v) => p.RecordNumber = (int)v)
                .WithWriter(static p => p.RecordNumber);
            outputMapper.Property(static x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            var writer = new StringWriter();
            outputMapper.Write(writer, people, new DelimitedOptions { IsFirstRecordSchema = true });
            var output = writer.ToString();

            var inputMapper = DelimitedTypeMapper.Define(static () => new Person());
            inputMapper.Property(static x => x.Name);
            inputMapper.Property(static x => x.RecordNumber);
            inputMapper.Property(static x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            var reader = new StringReader(output);
            var results = inputMapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(4, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [TestMethod]
        public void TestWriter_NoSchema_SchemaNotCounted()
        {
            var outputMapper = DelimitedTypeMapper.Define(static () => new Person());
            outputMapper.Property(static x => x.Name);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = false })
                .WithReader(static (p, v) => p.RecordNumber = (int)v)
                .WithWriter(static p => p.RecordNumber);
            outputMapper.Property(static x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            var writer = new StringWriter();
            outputMapper.Write(writer, people, new DelimitedOptions { IsFirstRecordSchema = true });
            var output = writer.ToString();

            var inputMapper = DelimitedTypeMapper.Define(static () => new Person());
            inputMapper.Property(static x => x.Name);
            inputMapper.Property(static x => x.RecordNumber);
            inputMapper.Property(static x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            var reader = new StringReader(output);
            var results = inputMapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(3, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }
        
        [TestMethod]
        public void TestWriter_WithSchema_WithIgnoredColumn()
        {
            var outputMapper = DelimitedTypeMapper.Define(static () => new Person());
            outputMapper.Property(static x => x.Name);
            outputMapper.Ignored();
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = true })
                .WithReader(static (p, v) => p.RecordNumber = (int)v)
                .WithWriter(static p => p.RecordNumber);
            outputMapper.Ignored();
            outputMapper.Property(static x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            var writer = new StringWriter();
            outputMapper.Write(writer, people, new DelimitedOptions { IsFirstRecordSchema = true });
            var output = writer.ToString();

            var inputMapper = DelimitedTypeMapper.Define(static () => new Person());
            inputMapper.Property(static x => x.Name);
            inputMapper.Ignored();
            inputMapper.Property(static x => x.RecordNumber);
            inputMapper.Ignored();
            inputMapper.Property(static x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            var reader = new StringReader(output);
            var results = inputMapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(4, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [TestMethod]
        public void TestWriter_WriteOnlyColumn_WithIgnoredColumn()
        {
            var outputMapper = DelimitedTypeMapper.Define(static () => new Person());
            outputMapper.Property(static x => x.Name);
            outputMapper.Ignored();
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = true });
            outputMapper.Ignored();
            outputMapper.Property(static x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            var writer = new StringWriter();
            outputMapper.Write(writer, people, new DelimitedOptions { IsFirstRecordSchema = true });
            var output = writer.ToString();

            var inputMapper = DelimitedTypeMapper.Define(static () => new Person());
            inputMapper.Property(static x => x.Name);
            inputMapper.Ignored();
            inputMapper.Property(static x => x.RecordNumber);
            inputMapper.Ignored();
            inputMapper.Property(static x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            var reader = new StringReader(output);
            var results = inputMapper.Read(reader, new DelimitedOptions { IsFirstRecordSchema = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(4, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        public class Person
        {
            public string Name { get; set; }

            public int RecordNumber { get; set; }

            public DateTime CreatedOn { get; set; }
        }
    }
}
