using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Salesforce.Sample.SmartSyncExplorer.utilities
{
    public class SortedObservableCollection<T> : ObservableCollection<T> where T : IComparable<T>
    {
        protected override void InsertItem(int index, T item)
        {
            lock (Items)
            {
                if (this.Count == 0)
                {
                    base.InsertItem(0, item);
                    return;
                }
                if (base.Contains(item))
                {
                    var dupes = (from i in Items where i.Equals(item) select i).ToList();
                    foreach (var n in dupes)
                    {
                        Remove(n);
                    }
                }
                index = Compare(item, 0, this.Count - 1);

                base.InsertItem(index, item);
            }
        }

        private int Compare(T item, int lowIndex, int highIndex)
        {
            int compareIndex = (lowIndex + highIndex) / 2;

            if (compareIndex == 0)
            {
                return SearchIndexByIteration(lowIndex, highIndex, item);
            }

            int result = item.CompareTo(this[compareIndex]);

            if (result < 0)
            {   //item precedes indexed obj in the sort order

                if ((lowIndex + compareIndex) < 100 || compareIndex == (lowIndex + compareIndex) / 2)
                {
                    return SearchIndexByIteration(lowIndex, compareIndex, item);
                }

                return Compare(item, lowIndex, compareIndex);
            }

            if (result > 0)
            {   //item follows indexed obj in the sort order

                if ((compareIndex + highIndex) < 100 || compareIndex == (compareIndex + highIndex) / 2)
                {
                    return SearchIndexByIteration(compareIndex, highIndex, item);
                }

                return Compare(item, compareIndex, highIndex);
            }

            return compareIndex;
        }

        /// <summary>
        /// Iterates through sequence of the collection from low to high index
        /// and returns the index where to insert the new item
        /// </summary>
        private int SearchIndexByIteration(int lowIndex, int highIndex, T item)
        {
            for (int i = lowIndex; i <= highIndex; i++)
            {
                if (item.CompareTo(this[i]) < 0)
                {
                    return i;
                }
            }
            return this.Count;
        }

        /// <summary>
        /// Adds the item to collection by ignoring the index
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            this.InsertItem(index, item);
        }
    }
}
