﻿using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the Int32Column class.
    /// </summary>
    [TestClass]
    public class Int32ColumnTester
    {
        /// <summary>
        /// An exception should be thrown if name is blank.
        /// </summary>
        [TestMethod]
        public void TestCtor_NameBlank_Throws()
        {
            Assert.ThrowsException<ArgumentException>(static () => new Int32Column("    "));
        }

        /// <summary>
        /// If someone tries to pass a name that contains leading or trailing whitespace, it will be trimmed.
        /// The name will also be made lower case.
        /// </summary>
        [TestMethod]
        public void TestCtor_SetsName_Trimmed()
        {
            var column = new Int32Column(" Name   ");
            Assert.AreEqual("Name", column.ColumnName);
        }

        /// <summary>
        /// If the value is blank and the field is not required, null will be returned.
        /// </summary>
        [TestMethod]
        public void TestParse_ValueBlank_NullReturned()
        {
            var column = new Int32Column("count");
            var actual = (int?)column.Parse(null, "    ");
            int? expected = null;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If the FormatProvider is null , Parse will use the currrent culture.
        /// </summary>
        [TestMethod]
        public void TestParse_FormatProviderNull_UsesCurrentCulture()
        {
            var column = new Int32Column("count")
                         {
                             FormatProvider = null
                         };
            var actual = (int)column.Parse(null, "  -123 ");
            var expected = -123;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If the FormatProvider is provided, Parse will use the given provider.
        /// </summary>
        [TestMethod]
        public void TestParse_FormatProviderProvided_UsesProvider()
        {
            var column = new Int32Column("count")
                         {
                             FormatProvider = CultureInfo.CurrentCulture
                         };
            var actual = (int)column.Parse(null, "  -123 ");
            var expected = -123;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// An exception should be thrown if trying to parse a null when not nullable.
        /// </summary>
        [TestMethod]
        public void TestParse_NotNullable_NullValue_Throws()
        {
            var column = new Int32Column("count")
                         {
                             IsNullable = false,
                             DefaultValue = DefaultValue.Disabled()
                         };
            Assert.ThrowsException<InvalidCastException>(() => column.Parse(null, string.Empty));
        }

        /// <summary>
        /// A replacement value should be provided when trying to parse a null when not nullable.
        /// </summary>
        [TestMethod]
        public void TestParse_NotNullable_NullValue_DefaultProvided()
        {
            var column = new Int32Column("count")
                         {
                             IsNullable = false,
                             DefaultValue = DefaultValue.Use(0)
                         };
            var value = (int)column.Parse(null, string.Empty);
            Assert.AreEqual(0, value, "A default was not provided.");
        }
    }
}
