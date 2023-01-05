using System;
using System.Text;

namespace QueryLite.DbSchema.CodeGeneration {

    public sealed class CodeBuilder {

        private readonly StringBuilder mText = new StringBuilder();

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
                throw new Exception($"{ nameof(indent) } must be >= 0 and <= 100");
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