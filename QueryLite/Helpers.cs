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
using System.Diagnostics.CodeAnalysis;

namespace QueryLite {

    public static class Helpers {

        /// <summary>
        /// Escape string text for use in an sql query
        /// </summary>
        /// <param name="text">Text to be escaped</param>
        /// <returns></returns>
        public static string EscapeForSql(string text) {
            return text.Replace("'", "''");
        }
    }

    /// <summary>
    /// Composite dictionary key
    /// </summary>
    /// <typeparam name="K1"></typeparam>
    /// <typeparam name="K2"></typeparam>
    internal readonly struct Key<K1, K2> : IEquatable<Key<K1, K2>> where K1 : notnull, IEquatable<K1> where K2 : notnull, IEquatable<K2> {

        public Key(K1 key1, K2 key2) {
            Key1 = key1;
            Key2 = key2;
        }
        public K1 Key1 { get; }
        public K2 Key2 { get; }

        public override bool Equals([NotNullWhen(true)] object? obj) {

            if(obj is Key<K1, K2> other) {
                return Key1.Equals(other.Key1) && Key2.Equals(other.Key2);
            }
            return false;
        }
        public bool Equals(Key<K1, K2> other) {
            return Key1.Equals(other.Key1) && Key2.Equals(other.Key2);
        }
        public override int GetHashCode() {
            return HashCode.Combine(Key1, Key2);
        }
    }

    /// <summary>
    /// Composite dictionary key
    /// </summary>
    /// <typeparam name="K1"></typeparam>
    /// <typeparam name="K2"></typeparam>
    /// <typeparam name="K3"></typeparam>
    internal readonly struct Key<K1, K2, K3> : IEquatable<Key<K1, K2, K3>> where K1 : notnull, IEquatable<K1> where K2 : notnull, IEquatable<K2> where K3 : notnull, IEquatable<K3> {

        public Key(K1 key1, K2 key2, K3 key3) {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
        }
        public K1 Key1 { get; }
        public K2 Key2 { get; }
        public K3 Key3 { get; }

        public override bool Equals([NotNullWhen(true)] object? obj) {

            if(obj is Key<K1, K2, K3> other) {
                return Key1.Equals(other.Key1) && Key2.Equals(other.Key2) && Key3.Equals(other.Key3);
            }
            return false;
        }
        public bool Equals(Key<K1, K2, K3> other) {
            return Key1.Equals(other.Key1) && Key2.Equals(other.Key2) && Key3.Equals(other.Key3);
        }
        public override int GetHashCode() {
            return HashCode.Combine(Key1, Key2, Key3);
        }
    }
}