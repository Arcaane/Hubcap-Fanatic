using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StringExtension {
    /// <summary>
    /// Check if string contains a stirng key
    /// </summary>
    /// <param name="source"></param>
    /// <param name="toCheck"></param>
    /// <param name="comp"></param>
    /// <returns></returns>
    public static bool Contains(this string source, string toCheck, StringComparison comp) => source?.IndexOf(toCheck, comp) >= 0;

    /// <summary>
    /// CompareTag Extension to check if the target contains a certain tag or a list of tags
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tags"></param>
    /// <param name="ANDLogic"></param>
    /// <returns></returns>
    public static bool CompareTags(this GameObject gameObject, IEnumerable<string> tags) => tags.Any(gameObject.CompareTag);
}