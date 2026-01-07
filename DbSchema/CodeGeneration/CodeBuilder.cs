/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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

namespace QueryLite.DbSchema.CodeGeneration {

    public sealed class CodeBuilder {

        private readonly StringBuilder mText = new();

        private readonly string mIndent;

        public int Length => mText.Length;

        public CodeBuilder(string indent = "    ") {

            mIndent = indent;

            if(string.IsNullOrEmpty(mIndent)) {
                mIndent = "    ";
            }
        }
        public CodeBuilder Indent(int indent) {

            if(indent < 0 || indent > 100) {
                throw new Exception($"{nameof(indent)} must be >= 0 and <= 100");
            }

            for(int index = 0; index < indent; index++) {
                mText.Append(mIndent);
            }
            return this;
        }
        public CodeBuilder Append(string pValue) {
            mText.Append(pValue);
            return this;
        }

        public CodeBuilder EndLine() {
            mText.Append(Environment.NewLine);
            return this;
        }
        public override string ToString() {
            return mText.ToString();
        }
    }
}

namespace DbSchema.CodeGeneration {

    public static class Str {

        public static string FirstLetterUpperCase(this string value) {

            if(value.Length == 0) {
                return value;
            }
            if(!char.IsUpper(value[0]) && value.Length > 1) {
                char c = char.ToUpper(value[0]);
                value = string.Concat(new Span<char>(ref c), value.AsSpan(1));
            }
            return value;
        }
        public static string FirstLetterLowerCase(this string value) {

            if(value.Length == 0) {
                return value;
            }
            if(!char.IsLower(value[0]) && value.Length > 1) {
                char c = char.ToLower(value[0]);
                value = string.Concat(new Span<char>(ref c), value.AsSpan(1));
            }
            return value;
        }
    }
}