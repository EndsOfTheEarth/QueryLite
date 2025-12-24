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