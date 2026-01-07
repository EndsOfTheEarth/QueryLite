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
using System.Diagnostics.CodeAnalysis;

namespace QueryLite {

    /// <summary>
    /// A sequence is a collection that must contain at least one item. This is used to
    /// help avoid sql syntax bugs when an 'IN(...)' condition is created using a list with
    /// no items in it.
    /// </summary>
    public sealed class Sequence<ITEM> {

        public ITEM[] Items { get; }

        private Sequence(ITEM[] items) {
            Items = items;
        }

        /// <summary>
        /// Create a sequense with at least one item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Sequence<ITEM> CreateFrom(ITEM item, params ITEM[] items) {
            return new Sequence<ITEM>([item, .. items]);
        }

        /// <summary>
        /// Create a sequence from the list of items if it contains at least one item.
        /// If items is empty this method returns false.
        /// </summary>
        public static bool TryCreateFrom(IEnumerable<ITEM> items, [MaybeNullWhen(false)] out Sequence<ITEM> sequence) {

            if(items.Any()) {
                sequence = new Sequence<ITEM>([.. items]);
                return true;
            }
            sequence = null;
            return false;
        }
    }
}