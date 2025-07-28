using UnityEditor;

namespace MainHub.Modules.WelwiseHubExampleModule.Editor
{
    public static class DefineSymbolsTools
    {
        public static void AddDefineSymbol(string symbol, BuildTargetGroup? targetGroup = null)
        {
            targetGroup ??= EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup.Value);

            if (!defines.Contains(symbol))
            {
                if (string.IsNullOrEmpty(defines))
                    defines = symbol;
                else
                    defines += ";" + symbol;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup.Value, defines);
                UnityEngine.Debug.Log($"Define symbol '{symbol}' added for {targetGroup}");
            }
            else
            {
                UnityEngine.Debug.Log($"Define symbol '{symbol}' already exists for {targetGroup}");
            }
        }

        public static void RemoveDefineSymbol(string symbol, BuildTargetGroup? targetGroup = null)
        {
            targetGroup ??= EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup.Value);

            if (defines.Contains(symbol))
            {
                var defineList = new System.Collections.Generic.List<string>(defines.Split(';'));
                defineList.Remove(symbol);
                string newDefines = string.Join(";", defineList);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup.Value, newDefines);
                UnityEngine.Debug.Log($"Define symbol '{symbol}' removed for {targetGroup}");
            }
            else
            {
                UnityEngine.Debug.Log($"Define symbol '{symbol}' not found for {targetGroup}");
            }
        }
    }
}