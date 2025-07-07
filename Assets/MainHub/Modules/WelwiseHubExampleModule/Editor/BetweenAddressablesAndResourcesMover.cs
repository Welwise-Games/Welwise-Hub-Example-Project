using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FishNet.Object;
using ParrelSync;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace MainHub.Modules.WelwiseHubExampleModule.Editor
{
    public class BetweenAddressablesAndResourcesMover : EditorWindow
    {
        private const string ModulesPath = "Assets/MainHub/Modules";
        private const string ResourcesPath = "Assets/Resources";
        private const string AddressablesDefineName = "ADDRESSABLES";
        private const string NetworkObjectsLabelName = "NetworkObjects";

        private GUIStyle _buttonStyle;


        [MenuItem("Tools/WelwiseHubExample/Settings")]
        public static void ShowWindow()
        {
            GetWindow<BetweenAddressablesAndResourcesMover>("Welwise Hub Example Settings");
        }

        private GUIStyle GetButtonStyle() =>
            _buttonStyle ??= new GUIStyle(GUI.skin.button)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 50
            };

        private void OnGUI()
        {
            GUILayout.Label("Welwise Hub Example Settings", EditorStyles.boldLabel);

            GUILayout.Space(20);

            GUILayout.BeginVertical();

            if (GUILayout.Button("Remove define and move Welwise addressable assets to resources", GetButtonStyle()))
            {
                RemoveDefineAndMoveAddressablesAssetsToResources();
            }

            GUILayout.Space(15);

            if (GUILayout.Button("Add define and move Welwise resources assets to addressable", GetButtonStyle()))
            {
                AddDefineAndMoveFilesResourcesFilesToAddressables();
            }

            GUILayout.EndVertical();
        }

        public static void RemoveDefineAndMoveAddressablesAssetsToResources()
        {
            DefineSymbolsTools.RemoveDefineSymbol(AddressablesDefineName);

            var foldersNames = GetFoldersNames(ModulesPath);

            if (foldersNames.Length == 0)
                return;

            foreach (var folderName in foldersNames)
                MoveAddressablesToResources($"{ModulesPath}/{folderName}", $"{ResourcesPath}/{folderName}");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                "Welwise resources assets moved from Assets/MainHub/Modules to Assets/Resources and removed from addressables");
        }

        public static void AddDefineAndMoveFilesResourcesFilesToAddressables()
        {
            DefineSymbolsTools.AddDefineSymbol(AddressablesDefineName);

            MoveResourcesAssetsToAddressables(ResourcesPath, "Welwise*", ModulesPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "Welwise addressable assets moved from Assets/Resources to Assets/MainHub/Modules and added to addressables");
        }

        public static void MoveResourcesAssetsToAddressables(string sourcePath, string searchPattern, string targetPath)
        {
            if (!AssetDatabase.IsValidFolder(sourcePath))
            {
                Debug.LogError($"Folder not found: {sourcePath}");
                return;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found!");
                return;
            }

            var allFolders = Directory.GetDirectories(sourcePath, searchPattern, SearchOption.TopDirectoryOnly);

            foreach (var folderPath in allFolders)
            {
                var folderName = folderPath.Split('\\').Last();

                var relativeFolder = folderPath.Substring(sourcePath.Length).TrimStart('/', '\\');
                var targetFolder = Path.Combine(sourcePath, relativeFolder).Replace("\\", "/");

                CreateFoldersForPath(targetFolder);

                var assetPaths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Select(path => path.Replace('\\', '/')).Where(asset => !asset.EndsWith(".meta"));

                var splitFolderName =
                    string.Join(" ", Regex.Matches(folderName, @"([A-Z][a-z]+)")
                        .Select(m => m.Value));

                foreach (var assetPath in assetPaths)
                {
                    var relativeAssetPath = assetPath.Substring(sourcePath.Length).TrimStart('/', '\\');
                    var newAssetPath = Path.Combine(targetPath, relativeAssetPath).Replace("\\", "/");

                    CreateFoldersForPath(newAssetPath);

                    var error = AssetDatabase.MoveAsset(assetPath, newAssetPath);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError($"Error moving asset {assetPath} to {newAssetPath}: {error}");
                    }
                    else
                    {
                        var addressablesFilesPath =
                            GetStrippedPath(newAssetPath, new[] { "/Client/", "/Server/", "/Shared/" }) +
                            "Addressables/";

                        var groupName = GetGroupName(splitFolderName, assetPath);

                        var foundGroup = settings.FindGroup(groupName);

                        var group = foundGroup ?? settings.CreateGroup(groupName, false, false, true,
                            null);

                        if (foundGroup == null)
                        {
                            var schema = group.AddSchema<BundledAssetGroupSchema>();
                            TryMovingSchema(schema, addressablesFilesPath);
                        }

                        var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(newAssetPath), group);

                        entry.SetAddress(Path.GetFileNameWithoutExtension(newAssetPath));

                        TryMovingGroup(group, addressablesFilesPath);
                        
                        var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(newAssetPath);
                        if (asset is GameObject gameObject && gameObject.TryGetComponent<NetworkObject>(out var networkObject))
                            entry.SetLabel(NetworkObjectsLabelName, true, true);
                    }
                }

                AssetDatabase.DeleteAsset(folderPath);
            }
        }

        private static void TryMovingSchema(BundledAssetGroupSchema schema, string addressablesFilesPath)
        {
            string error;
            var schemaAssetPath = AssetDatabase.GetAssetPath(schema);
            var schemaNewPath = addressablesFilesPath +
                                Path.GetFileName(schemaAssetPath);

            if (schemaNewPath != schemaAssetPath)
            {
                CreateFoldersForPath(schemaNewPath);
                error = AssetDatabase.MoveAsset(schemaAssetPath, schemaNewPath);

                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"Error moving asset {schemaAssetPath} to {schemaNewPath}: {error}");
            }
        }

        private static void TryMovingGroup(AddressableAssetGroup group, string addressablesFilesPath)
        {
            string error;
            var groupAssetPath = AssetDatabase.GetAssetPath(group);
            var groupNewPath = addressablesFilesPath + Path.GetFileName(groupAssetPath);

            if (groupAssetPath != groupNewPath)
            {
                CreateFoldersForPath(groupNewPath);
                error = AssetDatabase.MoveAsset(groupAssetPath, groupNewPath);

                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"Error moving asset {groupAssetPath} to {groupNewPath}: {error}");
            }
        }

        private static string GetStrippedPath(string path, string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                var index = path.IndexOf(keyword);

                if (index != -1)
                    return path.Substring(0, index + keyword.Length);
            }

            return path;
        }

        private static string GetGroupName(string splitFolderName, string assetPath) =>
            splitFolderName.Replace("Module", "") +
            GetSecondGroupNamePart(assetPath) +
            " Module Group";

        private static string GetSecondGroupNamePart(string assetPath)
        {
            if (assetPath.Contains("Server"))
                return "Server";
            if (assetPath.Contains("Client"))
                return "Client";
            return "Shared";
        }

        private static string[] GetFoldersNames(string path)
        {
            if (Directory.Exists(path))
                return Directory.GetDirectories(path).Select(Path.GetFileName).ToArray();

            Debug.LogError($"Directory not found: {path}");
            return Array.Empty<string>();
        }

        private static void MoveAddressablesToResources(string sourcePath, string targetPath)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found!");
                return;
            }

            var groups = new List<AddressableAssetGroup>(settings.groups.Where(group => group != null));

            foreach (var group in groups)
            {
                var entries = group.entries;

                var entriesToMove = new List<AddressableAssetEntry>();

                foreach (var entry in entries)
                {
                    var assetPath = entry.AssetPath;
                    if (!assetPath.StartsWith(sourcePath) || assetPath.Contains("Localization")) continue;
                    entriesToMove.Add(entry);
                }

                foreach (var entry in entriesToMove)
                {
                    var assetPath = entry.AssetPath;
                    var relativePath = assetPath.Substring(sourcePath.Length).TrimStart('/', '\\');
                    var newPath = Path.Combine(targetPath, relativePath).Replace("\\", "/");

                    CreateFoldersForPath(newPath);

                    var error = AssetDatabase.MoveAsset(assetPath, newPath);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError($"Error moving asset {assetPath} to {newPath}: {error}");
                        return;
                    }

                    entry.SetAddress(Path.GetFileNameWithoutExtension(newPath));
                }
            }
        }

        private static void CreateFoldersForPath(string assetPath)
        {
            var folderPath = Path.GetDirectoryName(assetPath)?.Replace("\\", "/");

            if (AssetDatabase.IsValidFolder(folderPath))
                return;

            var folders = folderPath?.Split('/');

            var currentPath = folders[0];
            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                Debug.LogError($"Root folder '{currentPath}' does not exist!");
                return;
            }

            for (int i = 1; i < folders.Length; i++)
            {
                var parentPath = currentPath;
                currentPath = Path.Combine(currentPath, folders[i]).Replace("\\", "/");

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    AssetDatabase.CreateFolder(parentPath, folders[i]);
                }
            }
        }
    }
}