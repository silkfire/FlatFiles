﻿using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class UnusualTypesTester
    {
        [TestMethod]
        public void ShouldRoundTripMaxValues()
        {
            var mapper = getWeirdMapper();
            var thing = new WeirdThing
                        {
                Small = sbyte.MaxValue,
                Big = ushort.MaxValue,
                Bigger = uint.MaxValue,
                Huge = ulong.MaxValue
            };

            var deserialized = roundTrip(mapper, thing);
            AssertEqual(thing, deserialized);
        }

        [TestMethod]
        public void ShouldRoundTripMinValues()
        {
            var mapper = getWeirdMapper();
            var thing = new WeirdThing
                        {
                            Small = sbyte.MinValue,
                            Big = ushort.MinValue,
                            Bigger = uint.MinValue,
                            Huge = ulong.MinValue
                        };

            var deserialized = roundTrip(mapper, thing);

            AssertEqual(thing, deserialized);
        }

        private static IDelimitedTypeMapper<WeirdThing> getWeirdMapper()
        {
            var mapper = DelimitedTypeMapper.Define(static () => new WeirdThing());
            mapper.Property(static x => x.Small);
            mapper.Property(static x => x.Big);
            mapper.Property(static x => x.Bigger);
            mapper.Property(static x => x.Huge);

            return mapper;
        }

        private static WeirdThing roundTrip(IDelimitedTypeMapper<WeirdThing> mapper, WeirdThing thing)
        {
            using (var writer = new StringWriter())
            {
                mapper.Write(writer, [thing]);
                var output = writer.ToString();
                using (var reader = new StringReader(output))
                {
                    var things = mapper.Read(reader).ToArray();
                    Assert.AreEqual(1, things.Length);
                    var deserialized = things.Single();

                    return deserialized;
                }
            }
        }

        private static void AssertEqual(WeirdThing thing1, WeirdThing thing2)
        {
            Assert.AreEqual(thing1.Small, thing2.Small);
            Assert.AreEqual(thing1.Big, thing2.Big);
            Assert.AreEqual(thing1.Bigger, thing2.Bigger);
            Assert.AreEqual(thing1.Huge, thing2.Huge);
        }

        public class WeirdThing
        {
            public sbyte Small { get; set; }

            public ushort Big { get; set; }

            public uint Bigger { get; set; }

            public ulong Huge { get; set; }
        }
    }
}
