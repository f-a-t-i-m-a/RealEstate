using System;
using System.Globalization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using JahanJooy.Common.Util.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.Collections
{
    /// <summary>
    /// Unit tests for Linked-list backend IDictionary implementation. 
    /// Code copied and changed from REVISION 679 of following SVN repository:
    /// https://slog.dk/svn/home/jensen/source/linked_dictionary/trunk/
    /// </summary>
    [TestClass]
    public class LinkedDictionaryTest
    {
        #region Asserts

        public static void AssertDictSubSet<TKey, TValue>(IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> indict)
        {
            Assert.IsTrue(dict.Count <= indict.Count);
            foreach (var entry in dict.ToArray())
            {
                Assert.IsTrue(indict.Contains(entry));
                Assert.IsTrue(indict.Keys.Any(o => entry.Key.Equals(o)));
                Assert.AreEqual(entry.Value, indict[entry.Key]);
                Assert.AreEqual(dict[entry.Key], indict[entry.Key]);
                Assert.IsTrue(indict.Values.Any(o => entry.Value.Equals(o)));
            }
        }

        public static void AssertDictEquals<TKey, TValue>(IDictionary<TKey, TValue> dict1, IDictionary<TKey, TValue> dict2)
        {
            AssertDictSubSet(dict1, dict2);
            AssertDictSubSet(dict2, dict1);
        }

        #endregion // Asserts

        #region Traversal

        public static void TestDictionaryEnumerator<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            var copy = dict.ToDictionary(e => e.Key, e => e.Value);
            AssertDictEquals(dict, copy);
        }

        public static void TestKeys<TKey, TValue>(IDictionary<TKey, TValue> dict) where TValue : class
        {
            var copy = new Dictionary<TKey, object>();
            foreach (TKey key in dict.Keys.ToArray())
            {
                Assert.IsTrue(dict.ContainsKey(key));
                Assert.IsTrue(dict[key] != null);
                copy.Add(key, null); // test for duplicates
            }

            Assert.AreEqual(copy.Count, dict.Keys.Count);
        }

        public static void TestValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            var values = dict.Values.ToArray();
            foreach (var e in dict)
                Assert.IsTrue(values.Any(o => e.Value.Equals(o)));
        }

        public static void TestTraversal<TKey, TValue>(IDictionary<TKey, TValue> dict) where TValue : class
        {
            TestDictionaryEnumerator(dict);
            TestKeys(dict);
            TestValues(dict);
        }

        #endregion // Traversal

        public class DictItem : IComparable
        {
            public readonly int Id;

            public DictItem(int id)
            {
                Id = id;
            }

            protected bool Equals(DictItem other)
            {
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((DictItem) obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }

            public override string ToString()
            {
                return Id.ToString(CultureInfo.InvariantCulture);
            }

            #region IComparable Members

            public int CompareTo(object obj)
            {
                return Id - ((DictItem) obj).Id;
            }

            #endregion
        }

        public void DictionaryOpTest(IDictionary<DictItem, DictItem> dict)
        {
            Assert.AreEqual(dict.Count, 0);
            DictItem[] keys = { new DictItem(3), new DictItem(2), new DictItem(1) };
            DictItem[] values = { new DictItem(-3), new DictItem(-2), new DictItem(-1) };

            IDictionary<DictItem, DictItem> checkDict = new Dictionary<DictItem, DictItem>();

            TestTraversal(dict);

            // ReSharper disable ForCanBeConvertedToForeach
            for (int i = 0; i < keys.Length; ++i)
            {
                checkDict.Add(keys[i], values[i]);
                dict.Add(keys[i], values[i]);
                AssertDictEquals(dict, checkDict);
                TestTraversal(dict);
            }

            for (int i = 0; i < keys.Length; ++i)
            {
                checkDict.Remove(keys[i]);
                dict.Remove(keys[i]);
                AssertDictEquals(dict, checkDict);
                TestTraversal(dict);
            }

            for (int i = 0; i < keys.Length; ++i)
            {
                checkDict[keys[i]] = values[i];
                dict[keys[i]] = values[i];
                AssertDictEquals(dict, checkDict);
                TestTraversal(dict);
            }

            for (int i = 0; i < keys.Length; ++i)
            {
                checkDict[keys[i]] = keys[i];
                dict[keys[i]] = keys[i];
                AssertDictEquals(dict, checkDict);
                TestTraversal(dict);
            }

            TestTraversal(dict);

            for (int i = 0; i < keys.Length; ++i)
            {
                bool threw = false;
                try
                {
                    dict.Add(keys[0], values[0]);
                }
                catch (ArgumentException)
                {
                    threw = true;
                }
                Assert.IsTrue(threw);
            }
            // ReSharper restore ForCanBeConvertedToForeach
        }

        [TestMethod]
        public void UpdateLinkedDictionaryTest()
        {
            IDictionary<DictItem, DictItem> dict = new UpdateLinkedDictionary<DictItem, DictItem>();

            DictItem[] keys = { new DictItem(0), new DictItem(2), new DictItem(1), new DictItem(3), new DictItem(11), new DictItem(8), new DictItem(9) };

            // ReSharper disable ForCanBeConvertedToForeach
            IList<DictItem> mutateKeys = new List<DictItem>();
            for (int i = 0; i < keys.Length; ++i)
            {
                mutateKeys.Add(keys[i]);
                dict.Add(keys[i], null);

                Assert.IsTrue(mutateKeys.SequenceEqual(dict.Keys));
            }
            // ReSharper restore ForCanBeConvertedToForeach

            // removing first
            dict.Remove(mutateKeys[0]);
            mutateKeys.RemoveAt(0);
            Assert.IsTrue(mutateKeys.SequenceEqual(dict.Keys));

            // removing last
            dict.Remove(mutateKeys[mutateKeys.Count - 1]);
            mutateKeys.RemoveAt(mutateKeys.Count - 1);
            Assert.IsTrue(mutateKeys.SequenceEqual(dict.Keys));

            // removing middle
            dict.Remove(mutateKeys[2]);
            mutateKeys.RemoveAt(2);
            Assert.IsTrue(mutateKeys.SequenceEqual(dict.Keys));

            // edit updates
            dict[mutateKeys[2]] = mutateKeys[3];
            mutateKeys.Add(mutateKeys[2]);
            mutateKeys.RemoveAt(2);
            Assert.IsTrue(mutateKeys.SequenceEqual(dict.Keys));
        }

        [TestMethod]
        public void LRUDictinonaryTest()
        {
            IDictionary<object, object> dict = new LRUDictionary<object, object>();

            Assert.AreEqual(dict.Count, 0);
            object[] items = { 0, 1, 2, 3, 4 };
            foreach (object o in items)
                dict.Add(o, o);

            IList keys = dict.Keys.ToList();
            for (int i = 0; i < keys.Count; ++i)
                Assert.AreEqual(keys[i], items[i]);

            Assert.AreEqual(dict[0], 0);
            Assert.AreEqual(dict[3], 3);
            Assert.AreEqual(dict[2], 2);

            keys = dict.Keys.ToList();
            object[] newOrder = { 1, 4, 0, 3, 2 };

            for (int i = 0; i < keys.Count; ++i)
                Assert.AreEqual(keys[i], newOrder[i]);
        }

        [TestMethod]
        public void MRUDictinonaryTest()
        {
            IDictionary<object, object> dict = new MRUDictionary<object, object>();

            Assert.AreEqual(dict.Count, 0);
            object[] items = { 0, 1, 2, 3, 4 };
            foreach (object o in items)
                dict.Add(o, o);

            IList keys = dict.Keys.ToList();
            for (int i = 0; i < keys.Count; ++i)
                Assert.AreEqual(keys[i], items[items.Length - i - 1]);

            Assert.AreEqual(dict[0], 0);
            Assert.AreEqual(dict[3], 3);
            Assert.AreEqual(dict[2], 2);

            keys = dict.Keys.ToList();
            object[] newOrder = { 2, 3, 0, 4, 1 };

            for (int i = 0; i < keys.Count; ++i)
                Assert.AreEqual(keys[i], newOrder[i]);
        }

        [TestMethod]
        public void OpTestForUpdateLinedDictionary()
        {
            DictionaryOpTest(new UpdateLinkedDictionary<DictItem, DictItem>());
        }

        [TestMethod]
        public void OpTestForLRUDictionary()
        {
            DictionaryOpTest(new LRUDictionary<DictItem, DictItem>());
        }

        [TestMethod]
        public void OpTestForMRUDictionary()
        {
            DictionaryOpTest(new MRUDictionary<DictItem, DictItem>());
        }
    }
}