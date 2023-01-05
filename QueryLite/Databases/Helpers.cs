using System;
using System.Reflection.Emit;

namespace QueryLite.Databases {

    /// <summary>
    /// This class is used to convert an integer to a generic enum type without causing the integer to be boxed
    /// </summary>
    /// <typeparam name="INTEGER"></typeparam>
    /// <typeparam name="ENUM"></typeparam>
    internal static class IntegerToEnum<INTEGER, ENUM> where INTEGER : struct, IComparable, IFormattable, IConvertible, IComparable<INTEGER>, IEquatable<INTEGER> where ENUM : Enum {

        private static readonly Converter<INTEGER, ENUM> _function;

        static IntegerToEnum() {
            DynamicMethod method = new DynamicMethod(
                name: "",
                returnType: typeof(ENUM),
                parameterTypes: new[] { typeof(INTEGER) },
                restrictedSkipVisibility: true
            );
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ret);
            _function = (Converter<INTEGER, ENUM>)method.CreateDelegate(typeof(Converter<INTEGER, ENUM>));
        }
        public static ENUM Convert(INTEGER value) {
            return _function(value);
        }
    }
}