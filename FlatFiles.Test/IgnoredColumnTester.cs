using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class IgnoredColumnTester
    {
        [TestMethod]
        public void TestIgnoredColumn_HandlePreAndPostProcessing()
        {
            var ignored = new IgnoredColumn
                          {
                ColumnName = "Ignored",
                NullFormatter = NullFormatter.ForValue("NULL"),
                OnParsing = static (_, value) => 
                {
                    Assert.AreEqual("NULL", value);
                    return value;
                },
                OnParsed = static (_, value) =>
                {
                    Assert.IsNull(value);
                    return value;
                },
                OnFormatting = static (_, value) =>
                {
                    Assert.IsNull(value);
                    return value;
                },
                OnFormatted = static (_, value) =>
                {
                    Assert.AreEqual("NULL", value);
                    return value;
                }
            };
            var value = ignored.Parse(null, "NULL");
            Assert.IsNull(value);
            var formatted = ignored.Format(null, value);
            Assert.AreEqual("NULL", formatted);
        }

        [TestMethod]
        public void TestIgnoredMapping_HandlePreAndPostProcessing()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new IgnoredOnly());
            mapper.Ignored()
                .ColumnName("Ignored")
                .NullFormatter(NullFormatter.ForValue("NULL"))
                .OnParsing(static (_, value) =>
                {
                    Assert.AreEqual("NULL", value);
                    return value;
                })
                .OnParsed(static (_, value) =>
                {
                    Assert.IsNull(value);
                    return value;
                })
                .OnFormatting(static (_, value) =>
                {
                    Assert.IsNull(value);
                    return value;
                })
                .OnFormatted(static (_, value) =>
                {
                    Assert.AreEqual("NULL", value);
                    return value;
                });
            var ignored = mapper.GetSchema().ColumnDefinitions["Ignored"];
            var value = ignored.Parse(null, "NULL");
            Assert.IsNull(value);
            var formatted = ignored.Format(null, value);
            Assert.AreEqual("NULL", formatted);
        }

        private class IgnoredOnly
        {
            public string Ignored { get; set; }
        }
    }
}
