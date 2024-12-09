/*
 * MIT License
 *
 * Copyright (c) 2024 EndsOfTheEarth
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **/
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

        public static RowValidationResult ValidateRowsInAssembly(Assembly assembly) {

            List<Type> types = new List<Type>();

            RowValidationResult result = new RowValidationResult();

            Type[] assemblyTypes = assembly.GetTypes();

            foreach(Type type in assemblyTypes) {

                if(typeof(ARowRecord).IsAssignableFrom(type) && type != typeof(ARowRecord)) {
                    types.Add(type);
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