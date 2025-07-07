using UnityEditor;

namespace MainHub.Modules.WelwiseHubExampleModule.Editor
{
    public static class DefineSymbolsTools
    {
        public static void AddDefineSymbol(string symbol)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (!defines.Contains(symbol))
            {
                if (string.IsNullOrEmpty(defines))
                    defines = symbol;
                else
                    defines += ";" + symbol;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
                UnityEngine.Debug.Log($"Define symbol '{symbol}' added.");
            }
            else
            {
                UnityEngine.Debug.Log($"Define symbol '{symbol}' already exists.");
            }
        }

        public static void RemoveDefineSymbol(string symbol)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (defines.Contains(symbol))
            {
                var defineList = new System.Collections.Generic.List<string>(defines.Split(';'));
                defineList.Remove(symbol);
                string newDefines = string.Join(";", defineList);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newDefines);
                UnityEngine.Debug.Log($"Define symbol '{symbol}' removed.");
            }
            else
            {
                UnityEngine.Debug.Log($"Define symbol '{symbol}' not found.");
            }
        }
    }
}