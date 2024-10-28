using System;
using System.Collections.Generic;
using System.Reflection;

namespace QueryLite {

    public static class RowValidator {

        public class RowValidationResult {

            public List<string> Messages { get; } = [];
            public List<Type> TypesValidated { get; } = [];

            public bool HasMessages => Messages.Count > 0;
        }
        public static RowValidationResult ValidateRowsInCurrentDomain() {

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> types = new List<Type>();

            RowValidationResult result = new RowValidationResult();

            foreach(Assembly assembly in assemblies) {

                try {

                    Type[] assemblyTypes = assembly.GetTypes();

                    foreach(Type type in assemblyTypes) {

                        if(typeof(ARowRecord).IsAssignableFrom(type) && type != typeof(ARowRecord)) {
                            types.Add(type);
                        }
                    }
                }
                catch(ReflectionTypeLoadException ex) {
                    result.Messages.Add($"Warning: Unable to load the a type from the assembly '{assembly.FullName}'.{Environment.NewLine}{ex}");
                }
            }

            foreach(Type type in types) {
                result.TypesValidated.Add(type);
                ValidateType(type, result.Messages);
            }
            return result;
        }

        public static void ValidateType(Type type, List<string> messages) {

            if(!typeof(ARowRecord).IsAssignableFrom(type) || type == typeof(ARowRecord)) {
                throw new Exception($"{nameof(type)} must be assignable to '{type.FullName}' and not the exact type = '{nameof(ARowRecord)}'");
            }

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach(PropertyInfo property in properties) {

                MethodInfo? getMethod = property.GetGetMethod();

                if(getMethod is null || !getMethod.IsPublic) {
                    messages.Add($"Type: '{type.FullName}', property: '{property.Name}' is missing a public getter");
                }
                else {

                    Type returnType = getMethod.ReturnType;

                    if(!DoesTypeImplementIEquatable(returnType)) {
                        messages.Add($"Type: '{type.FullName}', property: '{property.Name}' return type: '{getMethod.ReturnType.FullName}' does not implement IEquatable<{getMethod.ReturnType.Name}>");
                    }
                }
            }
        }

        private static bool DoesTypeImplementIEquatable(Type returnType) {

            Type[] interfaces = returnType.GetInterfaces();

            foreach(Type inter in interfaces) {

                if(inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEquatable<>)) {
                    Type[] arguments = inter.GetGenericArguments();

                    if(arguments.Length == 1 && arguments[0] == returnType) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}