using UnityEditor;
using UnityEngine;
using WelwiseHubExampleModule.Runtime.Client.ZerinLabs_shaderPack_CartoonSky.Scripts;

#if UNITY_EDITOR

namespace WelwiseHubExampleModule.Runtime.Client.ZerinLabs_shaderPack_CartoonSky.ScriptsEditor
{
    [CustomEditor(typeof(scr_skyboxManipulatorUtility))]
    public class scr_skyboxManipulatorUtility_inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            scr_skyboxManipulatorUtility SkyManipulator = (scr_skyboxManipulatorUtility)target;

            if (GUILayout.Button("Get & Apply Material Data"))
            {
                SkyManipulator.UpdateDataBasedOnMaterial();
            }
        }
    }
}

#endif