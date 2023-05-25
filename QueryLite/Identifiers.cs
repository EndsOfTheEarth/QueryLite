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
using MessagePack;
using System;
using System.Text.Json.Serialization;

namespace QueryLite {

    public interface IKeyValue {

        object GetValueAsObject();
    }

    public interface IGuidType {
        
        Guid Value { get; }
    }
    public interface IStringType {
        
        string Value { get; }
    }
    public interface IInt16Type {
        
        short Value { get; }
    }
    public interface IInt32Type {
    
        int Value { get; }
    }
    public interface IInt64Type {
    
        long Value { get; }
    }
    public interface IBoolType {
    
        bool Value { get; }
    }

    public readonly struct GuidKey<TYPE> : IKeyValue, IGuidType {

        [Key(0)]
        [JsonPropertyName("Value")]
        [JsonInclude]
        public Guid Value { get; init; }

        object IKeyValue.GetValueAsObject() {
            return Value;
        }

        public GuidKey() { }
        public GuidKey(Guid value) {
            Value = value;
        }

        public static GuidKey<TYPE> ValueOf(Guid value) {
            return new GuidKey<TYPE>(value);
        }

        public static Guid? ToGuid(GuidKey<TYPE>? key) {
            return key?.Value;
        }

        public static GuidKey<TYPE> Parse(string text) {
            return new GuidKey<TYPE>(Guid.Parse(text));
        }

        [IgnoreMember]
        [JsonIgnore]
        public Type DataType => typeof(TYPE);

        public static GuidKey<TYPE> NotSet { get; } = new GuidKey<TYPE>(Guid.Empty);

        [IgnoreMember]
        [JsonIgnore]
        public bool IsValid {
            get { return Value != Guid.Empty; }
        }

        public static bool operator ==(GuidKey<TYPE>? pA, GuidKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(GuidKey<TYPE>? pA, GuidKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is GuidKey<TYPE> identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct StringKey<TYPE> : IKeyValue, IStringType {

        [Key(0)]
        [JsonPropertyName("Value")]
        [JsonInclude]
        public string Value { get; init; }

        object IKeyValue.GetValueAsObject() {
            return Value;
        }

        public StringKey(string value) {
            Value = value;
        }

        public static StringKey<TYPE> ValueOf(string value) {
            return new StringKey<TYPE>(value);
        }

        [IgnoreMember]
        [JsonIgnore]
        public Type DataType => typeof(TYPE);

        public static StringKey<TYPE> NotSet { get; } = new StringKey<TYPE>(string.Empty);

        [IgnoreMember]
        [JsonIgnore]
        public bool IsValid {
            get { return !string.IsNullOrEmpty(Value); }
        }
        public static bool operator ==(StringKey<TYPE>? pA, StringKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(StringKey<TYPE>? pA, StringKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is StringKey<TYPE> identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value ?? string.Empty;
        }
    }

    public readonly struct ShortKey<TYPE> : IKeyValue, IInt16Type {

        [Key(0)]
        [JsonPropertyName("Value")]
        [JsonInclude]
        public short Value { get; init; }

        object IKeyValue.GetValueAsObject() {
            return Value;
        }

        public ShortKey(short value) {
            Value = value;
        }

        public static ShortKey<TYPE> ValueOf(short value) {
            return new ShortKey<TYPE>(value);
        }

        public static short? ToShort(ShortKey<TYPE>? key) {
            return key?.Value;
        }

        public static ShortKey<TYPE> Parse(string text) {
            return new ShortKey<TYPE>(short.Parse(text));
        }

        [IgnoreMember]
        [JsonIgnore]
        public Type DataType => typeof(TYPE);

        public static ShortKey<TYPE> NotSet { get; } = new ShortKey<TYPE>(0);

        [IgnoreMember]
        [JsonIgnore]
        public bool IsValid {
            get { return Value > 0; }
        }

        public static bool operator ==(ShortKey<TYPE>? pA, ShortKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(ShortKey<TYPE>? pA, ShortKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is ShortKey<TYPE> identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct IntKey<TYPE> : IKeyValue, IInt32Type {

        [Key(0)]
        [JsonPropertyName("Value")]
        [JsonInclude]
        public int Value { get; init; }

        object IKeyValue.GetValueAsObject() {
            return Value;
        }

        public IntKey(int value) {
            Value = value;
        }

        public static IntKey<TYPE> ValueOf(int value) {
            return new IntKey<TYPE>(value);
        }

        public static int? ToInt(IntKey<TYPE>? key) {
            return key?.Value;
        }

        public static IntKey<TYPE> Parse(string text) {
            return new IntKey<TYPE>(int.Parse(text));
        }

        [IgnoreMember]
        [JsonIgnore]
        public Type DataType => typeof(TYPE);

        public static IntKey<TYPE> NotSet { get; } = new IntKey<TYPE>(0);

        [IgnoreMember]
        [JsonIgnore]
        public bool IsValid {
            get { return Value > 0; }
        }

        public static bool operator ==(IntKey<TYPE>? pA, IntKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(IntKey<TYPE>? pA, IntKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is IntKey<TYPE> identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct LongKey<TYPE> : IKeyValue, IInt64Type {

        [Key(0)]
        [JsonPropertyName("Value")]
        [JsonInclude]
        public long Value { get; init; }

        object IKeyValue.GetValueAsObject() {
            return Value;
        }

        public LongKey(long value) {
            Value = value;
        }

        public static LongKey<TYPE> ValueOf(long value) {
            return new LongKey<TYPE>(value);
        }
        public static long? ToLong(LongKey<TYPE>? key) {
            return key?.Value;
        }

        public static LongKey<TYPE> Parse(string text) {
            return new LongKey<TYPE>(long.Parse(text));
        }

        [IgnoreMember]
        [JsonIgnore]
        public Type DataType => typeof(TYPE);

        public static LongKey<TYPE> NotSet { get; } = new LongKey<TYPE>(0);

        [IgnoreMember]
        [JsonIgnore]
        public bool IsValid {
            get { return Value > 0; }
        }

        public static bool operator ==(LongKey<TYPE>? pA, LongKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(LongKey<TYPE>? pA, LongKey<TYPE>? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is LongKey<TYPE> identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct BoolValue<TYPE> : IKeyValue, IBoolType {

        [Key(0)]
        [JsonPropertyName("Value")]
        [JsonInclude]
        public bool Value { get; init; }

        object IKeyValue.GetValueAsObject() {
            return Value;
        }

        public BoolValue(bool value) {
            Value = value;
        }

        public static BoolValue<TYPE> ValueOf(bool value) {
            return new BoolValue<TYPE>(value);
        }

        public static bool? ToBool(BoolValue<TYPE>? key) {
            return key?.Value;
        }

        [IgnoreMember]
        [JsonIgnore]
        public Type DataType => typeof(TYPE);

        public static bool operator ==(BoolValue<TYPE>? pA, BoolValue<TYPE>? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(BoolValue<TYPE>? pA, BoolValue<TYPE>? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is BoolValue<TYPE> value) {
                return Value.CompareTo(value.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct Bit {

        public readonly static Bit TRUE = new Bit(true);
        public readonly static Bit FALSE = new Bit(false);

        public bool Value { get; }

        public Bit(bool value) {
            Value = value;
        }
        
        public static Bit ValueOf(bool value) => value ? TRUE : FALSE;

        public override string ToString() {
            return Value ? "1" : "0";
        }
    }
}