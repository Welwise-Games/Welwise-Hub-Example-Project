using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Systems.DebugSystem
{
    public class InGameConsole : MonoBehaviour
    {
        [Header("Settings")] 
        public KeyCode toggleKey = KeyCode.F10;
        public int maxLines = 100;
        public float consoleWidth = 600f;
        public float consoleHeight = 300f;

        private bool _isVisible;
        private string _inputText = "";
        private Vector2 _scrollPosition;
        private readonly List<string> _logMessages = new();
        private readonly StringBuilder _logBuilder = new();

        private void Awake()
        {
            Application.logMessageReceived += HandleLog;
            DontDestroyOnLoad(gameObject); // Консоль будет работать между сценами
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(toggleKey))
            {
                _isVisible = !_isVisible;
            }
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            string color = type switch
            {
                LogType.Error => "red",
                LogType.Exception => "red",
                LogType.Warning => "yellow",
                _ => "white"
            };

            _logBuilder.Clear();
            _logBuilder.AppendLine($"<color={color}>{message}</color>");

            if (type == LogType.Error || type == LogType.Exception)
            {
                _logBuilder.AppendLine($"<color=gray>{stackTrace}</color>");
            }

            _logMessages.Add(_logBuilder.ToString());

            // Ограничение количества сообщений
            if (_logMessages.Count > maxLines)
            {
                _logMessages.RemoveAt(0);
            }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;

            // Стиль для консоли
            GUIStyle consoleStyle = new(GUI.skin.box)
            {
                fontSize = 14,
                richText = true
            };

            // Рисуем консоль
            GUILayout.BeginArea(new Rect(10, 10, consoleWidth, consoleHeight), consoleStyle);

            // Прокручиваемый лог
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(consoleHeight - 50));
            foreach (string log in _logMessages)
            {
                GUILayout.Label(log, consoleStyle);
            }

            GUILayout.EndScrollView();

            // Поле ввода команд
            GUILayout.BeginHorizontal();
            _inputText = GUILayout.TextField(_inputText, GUILayout.Width(consoleWidth - 80));
            if (GUILayout.Button("Send", GUILayout.Width(80)))
            {
                ExecuteCommand(_inputText);
                _inputText = "";
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void ExecuteCommand(string command)
        {
            // Пример обработки команд
            switch (command.ToLower())
            {
                case "clear":
                    _logMessages.Clear();
                    break;
                case "help":
                    _logMessages.Add("Available commands: clear, help, time");
                    break;
                case "time":
                    _logMessages.Add($"Current time: {System.DateTime.Now:HH:mm:ss}");
                    break;
                default:
                    _logMessages.Add($"Unknown command: '{command}'");
                    break;
            }
        }
    }
}