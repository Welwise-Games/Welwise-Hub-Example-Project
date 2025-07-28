#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Tools
{
    public static class PlayerPrefsTools
    {
        [MenuItem("Tools/PlayerPrefs/DeleteAll")]
        public static void RemoveAllKeysForPlayerPrefs() => PlayerPrefs.DeleteAll();
    }
}
#endif