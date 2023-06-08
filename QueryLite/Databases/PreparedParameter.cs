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
using System.Collections.Generic;
using System.Data.Common;

namespace QueryLite.Databases {

    internal interface IPerparedParameter<PARAMETERS> {

        DbParameter CreateParameter(PARAMETERS parameters);
    }

    internal sealed class PreparedParameterList<PARAMETERS> {

        private readonly List<IPerparedParameter<PARAMETERS>> _list = new List<IPerparedParameter<PARAMETERS>>();

        private int _paramCounter = 0;

        public string GetNextParameterName() {

            string paramName;

            if(ParamNameCache.ParamNames.Length < _paramCounter) {
                paramName = ParamNameCache.ParamNames[_paramCounter];
            }
            else {
                paramName = $"@{_paramCounter}";
            }
            _paramCounter++;
            return paramName;
        }

        public int Count => _list.Count;

        public IPerparedParameter<PARAMETERS> this[int index] => _list[index];

        public void Add(IPerparedParameter<PARAMETERS> parameter) {
            _list.Add(parameter);
        }
    }
    internal sealed class PreparedParameter<PARAMETERS, TYPE> : IPerparedParameter<PARAMETERS> {

        public PreparedParameter(string name, Func<PARAMETERS, TYPE> getValueFunc, CreateParameterDelegate createParameter) {
            Name = name;
            GetValueFunc = getValueFunc;
            _createParameter = createParameter;
        }
        public string Name { get; }
        public Func<PARAMETERS, TYPE> GetValueFunc { get; }
        public CreateParameterDelegate _createParameter { get; }

        DbParameter IPerparedParameter<PARAMETERS>.CreateParameter(PARAMETERS parameters) {
            return _createParameter(name: Name, value: GetValueFunc(parameters));
        }
    }
}