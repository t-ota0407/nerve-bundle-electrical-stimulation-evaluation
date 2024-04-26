using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static List<T> Clone<T>(this List<T> list)
    {
        return new List<T>(list);
    }
}
