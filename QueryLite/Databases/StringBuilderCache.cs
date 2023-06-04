/*
 * MIT License
 *
 * Copyright (c) 2023 EndsOfTheEarth
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
using System.Text;

namespace QueryLite.Databases {

    internal static class StringBuilderCache {

        [ThreadStatic]
        private static StringBuilder? InstanceA;

        [ThreadStatic]
        private static StringBuilder? InstanceB;

        public static StringBuilder Acquire(int capacity = 16) {

            if(Settings.EnableStringBuilderCaching && capacity <= Settings.StringBuilderCacheMaxCharacters) {

                StringBuilder? builder = InstanceA;

                if(builder != null && capacity <= builder.Capacity) {
                    InstanceA = null;
                    builder.Clear();
                    return builder;
                }

                builder = InstanceB;

                if(builder != null && capacity <= builder.Capacity) {
                    InstanceB = null;
                    builder.Clear();
                    return builder;
                }
            }
            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder builder) {

            if(Settings.EnableStringBuilderCaching && builder.Capacity <= Settings.StringBuilderCacheMaxCharacters) {
                InstanceA = InstanceB;
                InstanceB = builder;
            }
        }
        public static string ToStringAndRelease(StringBuilder builder) {
            string result = builder.ToString();
            Release(builder);
            return result;
        }
    }
}