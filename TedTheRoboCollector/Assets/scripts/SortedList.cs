using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// A sorted list
/// </summary>
public class SortedList<T> where T:IComparable
{
    List<T> items = new List<T>();

    // used in Add method
    List<T> tempList = new List<T>();
	
    #region Constructors

    /// <summary>
    /// No argument constructor
    /// </summary>
    public SortedList()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of items in the list
    /// </summary>
    /// <value>number of items in the list</value>
    public int Count
    {
        get { return items.Count; }
    }
	
    /// <summary>
    /// Gets the item in the array at the given index
    /// This property allows access using [ and ]
    /// </summary>
    /// <param name="index">index of item</param>
    /// <returns>item at the given index</returns>
    public T this[int index]
    {
        get { return items[index]; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds the given item to the list
    /// </summary>
    /// <param name="item">item</param>
    public void Add(T itemToAdd)
    {
        // get number of items already in list
        int count = items.Count; // O(1)

        if (count == 0)
        {
            items.Add(itemToAdd);
        } 
        else
        {
            // in worst case scenario, we will be inserting at end of list
            int index = count; // O(1)

            // iterate over items and identify insertion index for new item
            for (int i = 0; i < count; i++) // O(n)
            {
                T item = items[i]; // O(1)
                if (itemToAdd.CompareTo(item) < 0) // O(1)
                {
                    // if itemToAdd is greater than item, we want to insert
                    // itemToAdd at index of item
                    index = i;
                    break;
                }
            }

            if (index >= count)
            {
                items.Add(itemToAdd); // O(n)
            }
            else
            {
                // save all items beyond the insertion index in a temporary list
                tempList = items.GetRange(index, count - index); // O(n)

                // remove all items beyond the insertion point
                items.RemoveRange(index, count - index); // O(n)

                // add the new item at the end of the list
                items.Add(itemToAdd); // O(n)

                // add back in all the items in the temporary list
                items.AddRange(tempList); // O(n)
            }

            // be nice and clear the temp list
            tempList.Clear(); // O(n)
        }
    }

    /// <summary>
    /// Removes the item at the given index from the list
    /// </summary>
    /// <param name="index">index</param>
    public void RemoveAt(int index)
    {
        // add your implementation below
        items.RemoveAt(index);
    }

    /// <summary>
    /// Determines the index of the given item using binary search
    /// </summary>
    /// <param name="item">the item to find</param>
    /// <returns>the index of the item or -1 if it's not found</returns>
    public int IndexOf(T item)
    {
        int lowerBound = 0;
        int upperBound = items.Count - 1;
        int location = -1;

        // if last item is still the closest (which it might well be)
        // skip the searching
        bool skip = false;
        T lastValue = items[upperBound];
        if (lastValue.CompareTo(item) == 0)
        {
            location = upperBound;
            skip = true;
        }

        if (!skip)
        {
            // loop until found value or exhausted array
            while ((location == -1) &&
                (lowerBound <= upperBound))
            {
                // find the middle
                int middleLocation = lowerBound + (upperBound - lowerBound) / 2;
                T middleValue = items[middleLocation];

                // check for match
                if (middleValue.CompareTo(item) == 0)
                {
                    location = middleLocation;
                }
                else
                {
                    // split data set to search appropriate side
                    if (middleValue.CompareTo(item) > 0) // middleValue is smaller than item
                    {
                        upperBound = middleLocation - 1;
                    }
                    else // middleValue is smaller than item
                    {
                        lowerBound = middleLocation + 1;
                    }
                }
            }
        }

        return location;
    }

    /// <summary>
    /// Sorts the list
    /// </summary>
    public void Sort()
    {
        items.Sort();
    }

    #endregion
}
