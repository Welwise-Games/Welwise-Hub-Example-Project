using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Tools
{
    public static class ReadingFileTools
    {
        public static HashSet<string> GetWordsFromFile(string fileName)
        {
            var filePath = GetFilePath(fileName);

            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found at: {filePath}");
                return new HashSet<string>();
            }

            var words = File.ReadAllText(fileName).Split('\n')
                .Where(word => !word.IsNullOrEmptyOrWhiteSpace()).Select(word => word.Replace("\r", "")).ToHashSet();

            if (words.Count == 0)
                Debug.LogError($"Words count in file at {filePath} is zero!");
            
            return words;
        }

        private static string GetFilePath(string fileName) =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", fileName));
    }
}