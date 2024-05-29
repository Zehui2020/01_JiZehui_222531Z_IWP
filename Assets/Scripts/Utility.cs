using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Utility Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetTopmostParent(Transform child)
    {
        if (child == null)
            return null;

        Transform parent = child;

        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        return parent;
    }

    public List<T> SetListSize<T>(List<T> list, int desiredSize)
    {
        List<T> removedList = new List<T>();

        if (list == null) return removedList;

        if (list.Count > desiredSize)
        {
            int removeCount = list.Count - desiredSize;
            removedList.AddRange(list.GetRange(desiredSize, removeCount));
            list.RemoveRange(desiredSize, removeCount);
        }

        return removedList;
    }
}