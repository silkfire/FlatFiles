﻿using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class DelimitedTypeMapperMixedMemberTester
    {
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
            mapper.Property(static p => p.IsActive).ColumnName("active");

            var bob = new Person { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19), IsActive = true };

            var stringWriter = new StringWriter();
            mapper.Write(stringWriter, [bob]);

            var stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
        }

        internal class Person
        {
            public int Id;

            public string Name { get; set; }

            public DateTime Created;

            public bool? IsActive { get; set; }
        }
    }
}
