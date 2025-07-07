using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Tools
{
    public static class CollectionTools
    {
        public static bool CustomSequenceEqual<T>(this IReadOnlyList<T> list1, IReadOnlyList<T> list2, 
            Func<T, T, bool> equalFunc)
        {
            if (list1.Count != list2.Count)
                return false;
            
            return list1.All(list1Element => list2.Any(list2Element => equalFunc.Invoke(list1Element, list2Element))) &&
                   list2.All(list2Element => list1.Any(list1Element => equalFunc.Invoke(list1Element, list2Element)));
        }

        public static T GetRandomOrDefault<T>(this IEnumerable<T> enumerable) =>
            enumerable.ElementAtOrDefault(Random.Range(0, enumerable.Count()));

        public static List<T> ToList<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToList();

        public static T SafeGet<T>(this List<T> list, int index) where T : class => index < 0 || index >= list.Count ? null : list[index];
        public static T SafeGet<T>(this IReadOnlyList<T> list, int index) where T : class => index < 0 || index >= list.Count ? null : list[index];
        public static K AddAndGet<T, K>(this Dictionary<T, K> dictionary, T value, Func<K> getItem)
        {
            if (dictionary.TryGetValue(value, out var item))
                return item;

            item = getItem();
            dictionary.Add(value, item);
            return item;
        }

        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> func) => dictionary.Where(func.Invoke).ForEach(pair => dictionary.Remove(pair.Key));

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> readOnlyDictionary) 
            => readOnlyDictionary?.ToDictionary(dictionary => dictionary.Key, dictionary => dictionary.Value);

        public static void AddOrAppoint<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
        
        public static void AddRangeMissing<T>(this List<T> list, IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                if (!list.Contains(element))
                    list.Add(element);
            }
        }

        public static void AddIfNotNull<T>(this List<T> list, T element)
        {
            if (element != null)
                list.Add(element);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
                action?.Invoke(element);
        }

        public static List<T> GetReversed<T>(this List<T> list)
        {
            list.Reverse();
            return list;
        }

        public static T GetRandom<T>(this List<T> list) => list[Random.Range(0, list.Count)];

        public static Dictionary<TKey, TValue> Zip<TKey, TValue>(this List<TKey> list, List<TValue> otherList) =>
            list.Zip(otherList, (k, v) => new {k, v}).ToDictionary(x => x.k, x => x.v);
    }
}