using UnityEngine;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Tools
{
    public static class StringTools
    {
        public static string GetColored(this string inputString, Color color) =>
            $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{inputString}</color>";

        public static string GetWithoutTags(this string inputString) => inputString; //Regex.Replace(inputString, "<.*?>", string.Empty);

        public static bool IsNullOrEmptyOrWhiteSpace(this string inputString) =>
            string.IsNullOrWhiteSpace(inputString) || string.IsNullOrEmpty(inputString);
    }
}