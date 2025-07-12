using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite.Databases;
using QueryLite.Databases.SqlServer;
using QueryLite.Utility;
using QueryLiteTest.Tables;
using System;

namespace QueryLiteTest.Tests {

    [TestClass]
    public class TypeMappingTests {

        [TestMethod]
        public void TestCreateParameterDelegateTypes() {

            TestCreateParameterDelegateTypes(new SqlServerParameterMapper());
            TestCreateParameterDelegateTypes(new PostgreSqlParameterMapper());
        }

        private static void TestCreateParameterDelegateTypes(IPreparedParameterMapper mapper) {

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(Guid)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(Guid?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(short)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(short?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(int)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(int?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(long)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(long?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(string)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(bool)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(bool?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(decimal)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(decimal?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(DateTime)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(DateTime?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(DateTimeOffset)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(DateTimeOffset?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(DateOnly)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(DateOnly?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(TimeOnly)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(TimeOnly?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(float)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(float?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(double)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(double?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(Bit)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(Bit?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomGuid)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomGuid?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomShort)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomShort?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomInt)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomInt?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomLong)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomLong?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomString)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomString?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomBool)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomBool?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDecimal)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDecimal?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDateTime)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDateTime?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDateTimeOffset)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDateTimeOffset?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDateOnly)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDateOnly?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomTimeOnly)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomTimeOnly?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomFloat)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomFloat?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDouble)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(CustomDouble?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(GuidKey<TestingType>)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(GuidKey<TestingType>?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(ShortKey<TestingType>)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(ShortKey<TestingType>?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(IntKey<TestingType>)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(IntKey<TestingType>?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(LongKey<TestingType>)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(LongKey<TestingType>?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(StringKey<TestingType>)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(StringKey<TestingType>?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(BoolValue<TestingType>)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(BoolValue<TestingType>?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(Bit)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(Bit?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(UShortEnum)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(UShortEnum?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(ShortEnum)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(ShortEnum?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(UIntEnum)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(UIntEnum?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(IntEnum)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(IntEnum?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(ULongEnum)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(ULongEnum?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(LongEnum)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(LongEnum?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(SByteEnum)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(SByteEnum?)));

            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(ByteEnum)));
            Assert.IsNotNull(mapper.GetCreateParameterDelegate(typeof(ByteEnum?)));
        }

        private class TestingType { }

        private enum UShortEnum : ushort {
            A, B, C
        }
        private enum ShortEnum : short {
            A, B, C
        }
        private enum UIntEnum : uint {
            A, B, C
        }
        private enum IntEnum : int {
            A, B, C
        }
        private enum ULongEnum : ulong {
            A, B, C
        }
        private enum LongEnum : long {
            A, B, C
        }

        private enum SByteEnum : sbyte {
            A, B, C
        }
        private enum ByteEnum : byte {
            A, B, C
        }
    }
}