using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FixedLengthTypeMapperFieldTester
    {
        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [TestMethod]
        public void TestTypeMapper_Roundtrip()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(static p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(static p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(static p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(static p => p.IsActive, new Window(5)).ColumnName("active");

            var bob = new Person { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19), IsActive = true };
            var options = new FixedLengthOptions { FillCharacter = '@' };

            var stringWriter = new StringWriter();
            mapper.Write(stringWriter, [bob], options);

            var stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader, options).ToArray();
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
        }

        internal class Person
        {
            public int Id;

            public string Name;

            public DateTime Created;

            public bool? IsActive;
        }
    }
}
