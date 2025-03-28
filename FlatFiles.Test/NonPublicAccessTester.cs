﻿using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class NonPublicAccessTester
    {
        [TestMethod]
        public void MapInternalClass()
        {
            var mapper = DelimitedTypeMapper.Define<InternalClass>();

            mapper.Property(static x => x.Identifier);
            mapper.Property(static x => x.Status);
            mapper.Property(static x => x.EffectiveDate).InputFormat("yyyyMMdd");
            mapper.Property(static x => x.ModificationDate).InputFormat("yyyyMMddHHmmss");
            mapper.Property(static x => x.IsInternal);

            var rawData = "ABC123,Doing Fine,20180115,20180115145100,true";
            var reader = new StringReader(rawData);
            var data = mapper.Read(reader, new DelimitedOptions
                                           {
                IsFirstRecordSchema = false,
                RecordSeparator = "\n",
                Separator = ",",
                Quote = '"'
            }).ToArray();

            Assert.AreEqual(1, data.Length);
            var result = data[0];
            Assert.AreEqual("ABC123", result.Identifier);
            Assert.AreEqual("Doing Fine", result.Status);
            Assert.AreEqual(new DateTime(2018, 1, 15), result.EffectiveDate);
            Assert.AreEqual(new DateTime(2018, 1, 15, 14, 51, 00), result.ModificationDate);
            Assert.IsTrue(result.IsInternal);
        }

        [TestMethod]
        public void MapInternalClass_Dynamic()
        {
            var mapper = DelimitedTypeMapper.DefineDynamic(typeof(InternalClass));

            mapper.StringProperty("Identifier");
            mapper.StringProperty("Status");
            mapper.DateTimeProperty("EffectiveDate").InputFormat("yyyyMMdd");
            mapper.DateTimeProperty("ModificationDate").InputFormat("yyyyMMddHHmmss");
            mapper.BooleanProperty("IsInternal");

            var rawData = "ABC123,Doing Fine,20180115,20180115145100,true";
            var reader = new StringReader(rawData);
            var data = mapper.Read(reader, new DelimitedOptions
                                           {
                IsFirstRecordSchema = false,
                RecordSeparator = "\n",
                Separator = ",",
                Quote = '"'
            }).ToArray();

            Assert.AreEqual(1, data.Length);
            dynamic result = data[0];
            Assert.AreEqual("ABC123", result.Identifier);
            Assert.AreEqual("Doing Fine", result.Status);
            Assert.AreEqual(new DateTime(2018, 1, 15), result.EffectiveDate);
            Assert.AreEqual(new DateTime(2018, 1, 15, 14, 51, 00), result.ModificationDate);
            Assert.AreEqual(true, result.IsInternal);
        }

        //[TestMethod]
        //public void MapPrivateClass()
        //{
        //    var mapper = DelimitedTypeMapper.Define<PrivateClass>();

        //    mapper.Property(x => x.Identifier);
        //    mapper.Property(x => x.Status);
        //    mapper.Property(x => x.EffectiveDate).InputFormat("yyyyMMdd");
        //    mapper.Property(x => x.ModificationDate).InputFormat("yyyyMMddHHmmss");

        //    string rawData = @"ABC123,Doing Fine,20180115,20180115145100";
        //    StringReader reader = new StringReader(rawData);
        //    var data = mapper.Read(reader, new DelimitedOptions()
        //    {
        //        IsFirstRecordSchema = false,
        //        RecordSeparator = "\n",
        //        Separator = ",",
        //        Quote = '"'
        //    }).ToArray();

        //    Assert.AreEqual(1, data.Length);
        //    var result = data[0];
        //    Assert.AreEqual("ABC123", result.Identifier);
        //    Assert.AreEqual("Doing Fine", result.Status);
        //    Assert.AreEqual(new DateTime(2018, 1, 15), result.EffectiveDate);
        //    Assert.AreEqual(new DateTime(2018, 1, 15, 14, 51, 00), result.ModificationDate);
        //}

        //[TestMethod]
        //public void MapPrivateClass_Dynamic()
        //{
        //    var mapper = DelimitedTypeMapper.DefineDynamic(typeof(PrivateClass));

        //    mapper.StringProperty("Identifier");
        //    mapper.StringProperty("Status");
        //    mapper.DateTimeProperty("EffectiveDate").InputFormat("yyyyMMdd");
        //    mapper.DateTimeProperty("ModificationDate").InputFormat("yyyyMMddHHmmss");

        //    string rawData = @"ABC123,Doing Fine,20180115,20180115145100";
        //    StringReader reader = new StringReader(rawData);
        //    var data = mapper.Read(reader, new DelimitedOptions()
        //    {
        //        IsFirstRecordSchema = false,
        //        RecordSeparator = "\n",
        //        Separator = ",",
        //        Quote = '"'
        //    }).ToArray();

        //    Assert.AreEqual(1, data.Length);
        //    dynamic result = data[0];
        //    Assert.AreEqual("ABC123", result.Identifier);
        //    Assert.AreEqual("Doing Fine", result.Status);
        //    Assert.AreEqual(new DateTime(2018, 1, 15), result.EffectiveDate);
        //    Assert.AreEqual(new DateTime(2018, 1, 15, 14, 51, 00), result.ModificationDate);
        //}

        //[TestMethod]
        //public void MapPrivateClass_PrivateCtor_Dynamic()
        //{
        //    var mapper = DelimitedTypeMapper.DefineDynamic(typeof(PrivateCtorClass));

        //    mapper.Int32Property("Id");

        //    string rawData = @"123";
        //    StringReader reader = new StringReader(rawData);
        //    var data = mapper.Read(reader, new DelimitedOptions()
        //    {
        //        IsFirstRecordSchema = false,
        //        RecordSeparator = "\n",
        //        Separator = ",",
        //        Quote = '"'
        //    }).ToArray();

        //    Assert.AreEqual(1, data.Length);
        //    dynamic result = data[0];
        //    Assert.AreEqual(123, result.Id);
        //}

        internal class InternalClass
        {
            internal string Identifier { get; set; }
            internal string Status { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ModificationDate { get; set; }
            internal bool IsInternal = false;

            public void SetInternal()
            {
                IsInternal = true;
            }
        }

        //private class PrivateClass
        //{
        //    public string Identifier { get; set; }
        //    public string Status { get; set; }
        //    public DateTime? EffectiveDate { get; set; }
        //    public DateTime? ModificationDate { get; set; }
        //}

        //private class PrivateCtorClass
        //{
        //    private PrivateCtorClass() { }
        //    public int Id { get; set; }
        //}
    }
}
