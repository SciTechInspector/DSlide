using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DSlide
{
    public static class ExtensionUtilities
    {
        public static int FindIndexOfNearestLessOrEqualToKey<TKey, TValue>(this SortedList<TKey, TValue> sortedList, TKey reference) where TKey : IComparable<TKey>
        {
            if (sortedList.Count == 0)
                return -1;

            var comparisonResult = reference.CompareTo(sortedList.Keys[0]);
            if (comparisonResult < 0)
                return -1;
            else if (comparisonResult == 0)
                return 0;

            comparisonResult = reference.CompareTo(sortedList.Keys[sortedList.Count - 1]);
            if (comparisonResult >= 0)
                return sortedList.Count - 1;

            int lower = 0;
            int upper = sortedList.Count - 1;
            int index = (lower + upper) / 2;
            while (lower <= upper)
            {
                Debug.Assert(reference.CompareTo(sortedList.Keys[lower == 0 ? lower : lower - 1]) > 0);
                Debug.Assert(reference.CompareTo(sortedList.Keys[upper == sortedList.Count - 1 ? upper : upper + 1]) < 0);

                comparisonResult = reference.CompareTo(sortedList.Keys[index]);
                if (comparisonResult == 0) { return index; }

                if (comparisonResult < 0)
                {
                    if (lower == index)
                        return lower - 1;

                    upper = index - 1;
                    Debug.Assert(lower <= upper);
                }
                else
                {
                    Debug.Assert(comparisonResult > 0);
                    if (upper == index)
                        return upper;

                    lower = index + 1;
                    Debug.Assert(lower <= upper);
                }
                index = (lower + upper) / 2;
            }

            throw new InvalidOperationException("The list provided was not correctly sorted, make sure that the keys are not modified after being insered in the sorted list.");
        }

        public static bool AreSame(object objA, object objB)
        {
            if (objA == null && objB == null)
                return true;

            if (objA == null || objB == null)
                return false;

            return objA.Equals(objB);
        }
    }
}
