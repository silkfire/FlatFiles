using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class CustomMappingTester
    {
        [TestMethod]
        public void ShouldManuallyReadWriteEntity_WithReflection()
        {
            var mapper = GetTypeMapper();
            mapper.OptimizeMapping(false);

            var writer = new StringWriter();
            var data = new[]
            {
                new Person { Id = 1, Name = "Bob", CreatedOn = new DateTime(2018, 6, 28), Amount = 12.34m },
                new Person { Id = 2, Name = "John", CreatedOn = new DateTime(2018, 6, 29), Amount = 23.45m },
                new Person { Id = 3, Name = "Susan", CreatedOn= new DateTime(2018, 6, 30), Amount  = null }
            };
            mapper.Write(writer, data);
            var output = writer.ToString();
            var reader = new StringReader(output);
            var people = mapper.Read(reader).ToArray();
            Assert.AreEqual(3, people.Length, "The wrong number of entities were read.");
            AssertPeopleEqual(data, people, 0);
            AssertPeopleEqual(data, people, 1);
            AssertPeopleEqual(data, people, 2);
        }

        [TestMethod]
        public void ShouldManuallyReadWriteEntity_WithEmit()
        {
            var mapper = GetTypeMapper();

            var writer = new StringWriter();
            var data = new[]
            {
                new Person { Id = 1, Name = "Bob", CreatedOn = new DateTime(2018, 6, 28), Amount = 12.34m },
                new Person { Id = 2, Name = "John", CreatedOn = new DateTime(2018, 6, 29), Amount = 23.45m },
                new Person { Id = 3, Name = "Susan", CreatedOn= new DateTime(2018, 6, 30), Amount  = null }
            };
            mapper.Write(writer, data);
            var output = writer.ToString();
            var reader = new StringReader(output);
            var people = mapper.Read(reader).ToArray();
            Assert.AreEqual(3, people.Length, "The wrong number of entities were read.");
            AssertPeopleEqual(data, people, 0);
            AssertPeopleEqual(data, people, 1);
            AssertPeopleEqual(data, people, 2);
        }

        private static IDelimitedTypeMapper<Person> GetTypeMapper()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new Person());
            mapper.CustomMapping(new Int32Column("Id")).WithReader(static (_, person, value) =>
            {
                person.Id = (int)value;
            }).WithWriter(static (ctx, person, values) =>
            {
                values[ctx.LogicalIndex] = person.Id;
            });
            mapper.CustomMapping(new StringColumn("Name")).WithReader(static (person, value) =>
            {
                person.Name = (string)value;
            }).WithWriter(static p => p.Name);
            mapper.CustomMapping(new DateTimeColumn("CreatedOn")).WithReader(static p => p.CreatedOn).WithWriter(static p => p.CreatedOn);
            mapper.CustomMapping(new DecimalColumn("Amount")).WithReader(static (_, person, value) =>
            {
                person.Amount = value == null ? (decimal?)null : (decimal)value;
            }).WithWriter(static (ctx, person, values) =>
            {
                values[ctx.LogicalIndex] = person.Amount;
            });
            return mapper;
        }

        private static void AssertPeopleEqual(Person[] data, Person[] people, int offset)
        {
            Assert.AreEqual(data[offset].Id, people[offset].Id, $"Person {offset} ID is wrong.");
            Assert.AreEqual(data[offset].Name, people[offset].Name, $"Person {offset} Name is wrong.");
            Assert.AreEqual(data[offset].CreatedOn, people[offset].CreatedOn, $"Person {offset} CreatedOn is wrong.");
            Assert.AreEqual(data[offset].Amount, people[offset].Amount, $"Person {offset} Amount is wrong.");
        }

        internal class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime CreatedOn { get; set; }

            public decimal? Amount { get; set; }
        }

        [TestMethod]
        public void ShouldManuallyReadWriteEntityWithCollection_WithReflection()
        {
            var mapper = GetCollectionTypeMapper();
            mapper.OptimizeMapping(false);

            var data = GetContacts();
            var writer = new StringWriter();
            mapper.Write(writer, data);
            var output = writer.ToString();
            var reader = new StringReader(output);
            var contacts = mapper.Read(reader).ToArray();
            Assert.AreEqual(3, contacts.Length, "The wrong number of entities were read.");
            AssertContactEqual(data, contacts, 0);
            AssertContactEqual(data, contacts, 1);
            Assert.AreEqual(2, contacts[2].Emails.Count); // The extra email is lost
            data[2].Emails.RemoveAt(data[2].Emails.Count - 1); // Remove the last email for comparison
            AssertContactEqual(data, contacts, 2);
        }

        [TestMethod]
        public void ShouldManuallyReadWriteEntityWithCollection_WithEmit()
        {
            var mapper = GetCollectionTypeMapper();

            var data = GetContacts();
            var writer = new StringWriter();
            mapper.Write(writer, data);
            var output = writer.ToString();
            var reader = new StringReader(output);
            var contacts = mapper.Read(reader).ToArray();
            Assert.AreEqual(3, contacts.Length, "The wrong number of entities were read.");
            AssertContactEqual(data, contacts, 0);
            AssertContactEqual(data, contacts, 1);
            Assert.AreEqual(2, contacts[2].Emails.Count); // The extra email is lost
            data[2].Emails.RemoveAt(data[2].Emails.Count - 1); // Remove the last email for comparison
            AssertContactEqual(data, contacts, 2);
        }

        private static void AssertContactEqual(Contact[] data, Contact[] contact, int offset)
        {
            Assert.AreEqual(data[offset].Id, contact[offset].Id, $"Contact {offset} ID is wrong.");
            Assert.AreEqual(data[offset].Name, contact[offset].Name, $"Contact {offset} Name is wrong.");
            CollectionAssert.AreEqual(data[offset].PhoneNumbers, contact[offset].PhoneNumbers, $"Contact {offset} has different phone numbers.");
            CollectionAssert.AreEqual(data[offset].Emails, contact[offset].Emails, $"Contact {offset} has different emails.");
        }

        private static Contact[] GetContacts()
        {
            var data = new[]
            {
                new Contact
                {
                    Id = 1,
                    Name = "Bob",
                    PhoneNumbers = ["555-1111", "555-2222"],
                    Emails = ["bob@x.com"]
                },
                new Contact
                {
                    Id = 2,
                    Name = "John",
                    PhoneNumbers = ["555-3333"],
                    Emails = ["john@x.com", "john@y.com"]
                },
                new Contact
                {
                    Id = 3,
                    Name = "Susan",
                    PhoneNumbers = [],
                    Emails = ["Susan@x.com", "Susan@y.com", "susan@z.com"]
                }
            };
            return data;
        }

        private static IFixedLengthTypeMapper<Contact> GetCollectionTypeMapper()
        {
            var mapper = FixedLengthTypeMapper.Define(static () => new Contact());
            mapper.CustomMapping(new Int32Column("Id"), 10).WithReader(static c => c.Id).WithWriter(static c => c.Id);
            mapper.CustomMapping(new StringColumn("Name"), 10).WithReader(static c => c.Name).WithWriter(static c => c.Name);
            mapper.CustomMapping(new StringColumn("Phone1"), 12).WithReader(static (c, phone1) =>
            {
                if (phone1 != null)
                {
                    c.PhoneNumbers.Add((string)phone1);
                }
            }).WithWriter(static c => c.PhoneNumbers.Count > 0 ? c.PhoneNumbers[0] : null);
            mapper.CustomMapping(new StringColumn("Phone2"), 12).WithReader(static (c, phone2) =>
            {
                if (phone2 != null)
                {
                    c.PhoneNumbers.Add((string)phone2);
                }
            }).WithWriter(static c => c.PhoneNumbers.Count > 1 ? c.PhoneNumbers[1] : null);
            mapper.CustomMapping(new StringColumn("Phone3"), 12).WithReader(static (c, phone3) =>
            {
                if (phone3 != null)
                {
                    c.PhoneNumbers.Add((string)phone3);
                }
            }).WithWriter(static c => c.PhoneNumbers.Count > 2 ? c.PhoneNumbers[2] : null);
            mapper.CustomMapping(new StringColumn("Email1"), 15).WithReader(static (_, c, email1) =>
            {
                if (email1 != null)
                {
                    c.Emails.Add((string)email1);
                }
            }).WithWriter(static (ctx, c, values) =>
            {
                values[ctx.LogicalIndex] = c.Emails.Count > 0 ? c.Emails[0] : null;
            });
            mapper.CustomMapping(new StringColumn("Email2"), 15).WithReader(static (_, c, email2) =>
            {
                if (email2 != null)
                {
                    c.Emails.Add((string)email2);
                }
            }).WithWriter(static (ctx, c, values) =>
            {
                values[ctx.LogicalIndex] = c.Emails.Count > 1 ? c.Emails[1] : null;
            });
            return mapper;
        }

        internal class Contact
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public List<string> PhoneNumbers { get; set; } = [];

            public List<string> Emails { get; set; } = [];
        }

        [TestMethod]
        public void ShouldManuallyReadWriteEntityWithNestedMember_WithReflection()
        {
            var mapper = GetNestedTypeMapper();
            mapper.OptimizeMapping(false);

            var data = GetRealtyProperties();
            var writer = new StringWriter();
            mapper.Write(writer, data);
            var output = writer.ToString();
            var reader = new StringReader(output);
            var properties = mapper.Read(reader).ToArray();
            Assert.AreEqual(2, properties.Length, "The wrong number of entities were read.");
            AssertPropertyEqual(data, properties, 0);
            AssertPropertyEqual(data, properties, 1);
        }

        [TestMethod]
        public void ShouldManuallyReadWriteEntityWithNestedMember_WithEmit()
        {
            var mapper = GetNestedTypeMapper();

            var data = GetRealtyProperties();
            var writer = new StringWriter();
            mapper.Write(writer, data);
            var output = writer.ToString();
            var reader = new StringReader(output);
            var properties = mapper.Read(reader).ToArray();
            Assert.AreEqual(2, properties.Length, "The wrong number of entities were read.");
            AssertPropertyEqual(data, properties, 0);
            AssertPropertyEqual(data, properties, 1);
        }

        private static IDelimitedTypeMapper<RealtyProperty> GetNestedTypeMapper()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new RealtyProperty
                                                          {
                Address = new Address(),
                Coordinates = new Geolocation()
            });
            mapper.CustomMapping(new Int32Column("Id")).WithReader(static x => x.Id).WithWriter(static x => x.Id);
            mapper.CustomMapping(new DecimalColumn("Longitude"))
                .WithReader(static (x, v) => x.Coordinates.Longitude = (decimal)v)
                .WithWriter(static x => x.Coordinates.Longitude);
            mapper.CustomMapping(new DecimalColumn("Latitude"))
                .WithReader(static x => x.Coordinates.Latitude)
                .WithWriter(static x => x.Coordinates.Latitude);
            mapper.CustomMapping(new StringColumn("Street1"))
                .WithReader(static (x, v) => x.Address.Street = (string)v)
                .WithWriter(static x => x.Address.Street);
            mapper.CustomMapping(new StringColumn("City"))
                .WithReader(static (x, v) => x.Address.City = (string)v)
                .WithWriter(static x => x.Address.City);
            mapper.CustomMapping(new StringColumn("State"))
                .WithReader(static (x, v) => x.Address.State = (string)v)
                .WithWriter(static x => x.Address.State);
            mapper.CustomMapping(new StringColumn("Zip"))
                .WithReader(static (x, v) => x.Address.Zip = (string)v)
                .WithWriter(static x => x.Address.Zip);
            return mapper;
        }

        private static RealtyProperty[] GetRealtyProperties()
        {
            return
            [
                new RealtyProperty
                {
                    Id = 1,
                    Coordinates = new Geolocation
                                  {
                        Latitude = 90,
                        Longitude = 120
                    },
                    Address = new Address
                              {
                        Street = "123 Ardvark Ln",
                        City = "Los Alimos",
                        State = "NM",
                        Zip = "55555"
                    }
                },
                new RealtyProperty
                {
                    Id = 2,
                    Coordinates = new Geolocation
                                  {
                        Latitude = 33,
                        Longitude = 80
                    },
                    Address = new Address
                              {
                        Street = "23 Hamburger Rd",
                        City = "Greenwich",
                        State = "NY",
                        Zip = "11111"
                    }
                }
            ];
        }

        private void AssertPropertyEqual(RealtyProperty[] expected, RealtyProperty[] actual, int id)
        {
            Assert.AreEqual(expected[id].Id, actual[id].Id, $"Property {id} ID is wrong.");
            Assert.AreEqual(expected[id].Coordinates.Longitude, actual[id].Coordinates.Longitude, $"Property {id} Longitude is wrong.");
            Assert.AreEqual(expected[id].Coordinates.Latitude, actual[id].Coordinates.Latitude, $"Property {id} Latitude is wrong.");
            Assert.AreEqual(expected[id].Address.Street, actual[id].Address.Street, $"Property {id} Street is wrong.");
            Assert.AreEqual(expected[id].Address.City, actual[id].Address.City, $"Property {id} City is wrong.");
            Assert.AreEqual(expected[id].Address.State, actual[id].Address.State, $"Property {id} State is wrong.");
            Assert.AreEqual(expected[id].Address.Zip, actual[id].Address.Zip, $"Property {id} Zip is wrong.");
        }

        internal class Address
        {
            public string Street { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }
        }

        internal class Geolocation
        {
            public decimal Longitude { get; set; }

            public decimal Latitude { get; set; }
        }

        internal class RealtyProperty
        {
            public int Id { get; set; }

            public Address Address { get; set; }

            public Geolocation Coordinates { get; set; }
        }

        [TestMethod]
        public void ShouldConvertLongToTimeSpan()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new Session());
            mapper.CustomMapping(new Int64Column("Duration")).WithReader(static (s, d) => s.Duration = TimeSpan.FromSeconds((long)d));

            var reader = new StringReader($"{24 * 60 * 60}"); // 24 hours
            var csvReader = mapper.GetReader(reader);
            Assert.IsTrue(csvReader.Read(), "The first record was not read.");
            Assert.AreEqual(TimeSpan.FromDays(1), csvReader.Current.Duration, "The wrong duration was read.");
            Assert.IsFalse(csvReader.Read(), "Too many records were read.");
        }

        internal class Session
        {
            public TimeSpan Duration { get; set; }
        }
    }
}
