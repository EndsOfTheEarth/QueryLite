using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite.Databases;
using QueryLite.Databases.PostgreSql;
using QueryLite.Databases.SqlServer;
using QueryLite.Utility;
using QueryLiteTest.Tables;
using System;
using System.Collections.Generic;

namespace QueryLiteTest.Tests {

    [TestClass]
    public class TypeMappingTests {

        [TestMethod]
        public void SqlServerCreateParameterDelegateTypes() {

            TestCreateParameterDelegateTypes(new SqlServerParameterMapper());
        }

        [TestMethod]
        public void PostgreSqlCreateParameterDelegateTypes() {

            TestCreateParameterDelegateTypes(new PostgreSqlParameterMapper());
        }

        private static void TestCreateParameterDelegateTypes(IPreparedParameterMapper mapper) {

            List<Type> types = GetTypes();

            foreach(Type type in types) {
                Assert.IsNotNull(mapper.GetCreateParameterDelegate(type));
            }            
        }

        [TestMethod]
        public void TestSqlServerTypeMappings() {

            List<Type> types = GetTypes();

            foreach(Type type in types) {
                SqlServerSqlTypeMappings.GetDbType(type);
            }
        }

        [TestMethod]
        public void TestPostgreSqlTypeMappings() {

            List<Type> types = GetTypes();

            foreach(Type type in types) {
                PostgreSqlTypeMappings.GetNpgsqlDbType(type);
            }
        }


        private static List<Type> GetTypes() {

            List<Type> list = new List<Type>() {
                typeof(Guid),
                typeof(Guid?),

                typeof(short),
                typeof(short?),

                typeof(int),
                typeof(int?),

                typeof(long),
                typeof(long?),

                typeof(string),

                typeof(bool),
                typeof(bool?),

                typeof(decimal),
                typeof(decimal?),

                typeof(DateTime),
                typeof(DateTime?),

                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),

                typeof(DateOnly),
                typeof(DateOnly?),

                typeof(TimeOnly),
                typeof(TimeOnly?),

                typeof(float),
                typeof(float?),

                typeof(double),
                typeof(double?),

                typeof(Bit),
                typeof(Bit?),

                typeof(CustomGuid),
                typeof(CustomGuid?),

                typeof(CustomShort),
                typeof(CustomShort?),

                typeof(CustomInt),
                typeof(CustomInt?),

                typeof(CustomLong),
                typeof(CustomLong?),

                typeof(CustomString),
                typeof(CustomString?),

                typeof(CustomBool),
                typeof(CustomBool?),

                typeof(CustomDecimal),
                typeof(CustomDecimal?),

                typeof(CustomDateTime),
                typeof(CustomDateTime?),

                typeof(CustomDateTimeOffset),
                typeof(CustomDateTimeOffset?),

                typeof(CustomDateOnly),
                typeof(CustomDateOnly?),

                typeof(CustomTimeOnly),
                typeof(CustomTimeOnly?),

                typeof(CustomFloat),
                typeof(CustomFloat?),

                typeof(CustomDouble),
                typeof(CustomDouble?),

                typeof(GuidKey<TestingType>),
                typeof(GuidKey<TestingType>?),

                typeof(ShortKey<TestingType>),
                typeof(ShortKey<TestingType>?),

                typeof(IntKey<TestingType>),
                typeof(IntKey<TestingType>?),

                typeof(LongKey<TestingType>),
                typeof(LongKey<TestingType>?),

                typeof(StringKey<TestingType>),
                typeof(StringKey<TestingType>?),

                typeof(BoolValue<TestingType>),
                typeof(BoolValue<TestingType>?),

                typeof(Bit),
                typeof(Bit?),

                typeof(UShortEnum),
                typeof(UShortEnum?),

                typeof(ShortEnum),
                typeof(ShortEnum?),

                typeof(UIntEnum),
                typeof(UIntEnum?),

                typeof(IntEnum),
                typeof(IntEnum?),

                typeof(ULongEnum),
                typeof(ULongEnum?),

                typeof(LongEnum),
                typeof(LongEnum?),

                typeof(SByteEnum),
                typeof(SByteEnum?),

                typeof(ByteEnum),
                typeof(ByteEnum?)
            };
            return list;
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