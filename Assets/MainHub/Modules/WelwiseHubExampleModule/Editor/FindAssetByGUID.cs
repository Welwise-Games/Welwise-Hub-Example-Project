using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace MainHub.Modules.WelwiseHubExampleModule.Editor
{
    public class FindAssetByGUID : EditorWindow
    {
        private string guidToFind = "";
        private Object foundAsset = null;

        [MenuItem("Tools/Find Asset by GUID")]
        public static void ShowWindow()
        {
            GetWindow<FindAssetByGUID>("Find Asset by GUID");
        }

        void OnGUI()
        {
            GUILayout.Label("Enter GUID:", EditorStyles.boldLabel);
            guidToFind = EditorGUILayout.TextField("GUID", guidToFind);

            if (GUILayout.Button("Find Asset"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guidToFind);
                if (!string.IsNullOrEmpty(path))
                {
                    foundAsset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                    if (foundAsset != null)
                    {
                        EditorGUIUtility.PingObject(foundAsset);  // Выделяет ассет в Project View
                    }
                    else
                    {
                        Debug.LogWarning("Asset with GUID " + guidToFind + " not found (but path exists).");
                    }
                }
                else
                {
                    foundAsset = null;
                    Debug.LogWarning("Asset with GUID " + guidToFind + " not found.");
                }
            }

            if (foundAsset != null)
            {
                EditorGUILayout.ObjectField("Found Asset", foundAsset, typeof(Object), false);
            }
        }
    }
}
#endif