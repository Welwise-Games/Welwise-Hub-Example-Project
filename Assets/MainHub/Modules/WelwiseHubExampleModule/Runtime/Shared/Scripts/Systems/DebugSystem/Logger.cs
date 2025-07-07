using System;
using System.IO;
using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.DebugSystem
{
    public static class Logger
    {
        private static string _logFilePath;
        private static bool _isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if !UNITY_EDITOR
        string logFolder = Path.Combine(Application.persistentDataPath, "Logs");
        Directory.CreateDirectory(logFolder);
        _logFilePath = Path.Combine(logFolder, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        
        Application.logMessageReceived += HandleLog;
        _isInitialized = true;
        
        Debug.Log("Logger initialized. Path: " + _logFilePath);
#endif
        }

        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!_isInitialized) return;

            string formattedLog = $"[{DateTime.Now:HH:mm:ss}] [{type}] {logString}\n";

            try
            {
                File.AppendAllText(_logFilePath, formattedLog);

                if (type == LogType.Error || type == LogType.Exception)
                {
                    File.AppendAllText(_logFilePath, stackTrace + "\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Logger failed: {e.Message}");
            }
        }

        /// <summary>
        /// Принудительно сохраняет лог (например, перед закрытием приложения)
        /// </summary>
        public static void Flush()
        {
            if (!_isInitialized) return;

            UnityEngine.Debug.Log("Application quit. Log saved to: " + _logFilePath);
            Application.logMessageReceived -= HandleLog;
        }
    }
}