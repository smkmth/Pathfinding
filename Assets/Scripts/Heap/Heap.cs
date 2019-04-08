using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T :IHeapItem<T>
{
    T[] Items;
    int currentItemCount;

    public Heap(int MaxHeapSize)
    {
        Items = new T[MaxHeapSize]; 

    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        Items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;


    }

    public T RemoveFirst()
    {
        T FirstItem = Items[0];
        currentItemCount--;
        Items[0] = Items[currentItemCount];
        Items[0].HeapIndex = 0;
        SortDown(Items[0]);
        return FirstItem;

    }

    public bool Contains(T item)
    {
        return Equals(Items[item.HeapIndex], item);
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int SwapIndex = 0;
            if (childIndexLeft < currentItemCount)
            {
                SwapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (Items[childIndexLeft].CompareTo(Items[childIndexRight]) < 0)
                    {
                        SwapIndex = childIndexRight;
                    }
                }
                if (item.CompareTo(Items[SwapIndex]) < 0)
                {
                    Swap(item, Items[SwapIndex]);

                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

    } 


    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = Items[parentIndex];
            if (item.CompareTo(parentItem)> 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;

        }

    }

    void Swap(T itemA, T itemB)
    {
        Items[itemA.HeapIndex] = itemB;
        Items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;

    }
}

public interface IHeapItem<T> :IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }

}