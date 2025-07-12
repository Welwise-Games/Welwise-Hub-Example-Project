using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WebGLMobileKeyboardModule.Runtime.Scripts
{
    public static class MobileWebGLKeyboardTools
    {
        private static TMP_InputField _lastFocusedInputField,  _targetInputField;
        private static MobileKeyboardEventsObserver _mobileKeyboardEventsObserver;

        private static readonly HashSet<TMP_InputField> _subscribedInputFields = new HashSet<TMP_InputField>();

#if !UNITY_EDITOR && UNITY_WEBGL
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            var instance = new GameObject("MobileKeyboardEventsObserver");
            Object.DontDestroyOnLoad(instance);
            _mobileKeyboardEventsObserver = instance.AddComponent<MobileKeyboardEventsObserver>();
            
            _mobileKeyboardEventsObserver.Destroyed += () =>
            {
                _mobileKeyboardEventsObserver = null;
                _lastFocusedInputField = null;
                _subscribedInputFields.Clear();
            };
            
            _mobileKeyboardEventsObserver.ChangedValue += (text) =>
                DoActionOnFocusedInputField(inputField =>
                    inputField.text = text);

            _mobileKeyboardEventsObserver.Submitted += () =>
                DoActionOnFocusedInputField(inputField =>
                    inputField.onSubmit?.Invoke(inputField.text));
        }

        private static void DoActionOnFocusedInputField(Action<TMP_InputField> action)
        {
            _subscribedInputFields.RemoveWhere(field => !field);

            if (_targetInputField)
            {
                action?.Invoke(_targetInputField);
                return;
            }
                
            var focusedSubscribedInputField =
                _subscribedInputFields.FirstOrDefault(inputField => inputField.isFocused);

            if (focusedSubscribedInputField)
            {
                action?.Invoke(focusedSubscribedInputField);
                _lastFocusedInputField = focusedSubscribedInputField;
                return;
            }

            if (_lastFocusedInputField)
                action?.Invoke(_lastFocusedInputField);
        }
#endif

        public static void InitializeInputFieldForMobileKeyboard(this TMP_InputField inputField,
            bool shouldReplaceEmojiTextToImage = true, bool shouldCloseKeyboardAfterSubmit = false, string color = null,
            string backgroundColor = null, string top = null, string bottom = null,
            string left = null, string width = null, string height = null, string transform = null, string position = null,
            string border = null, string fontSize = null)
        {
            if (!_mobileKeyboardEventsObserver || _subscribedInputFields.Contains(inputField))
                return;

            inputField.customCaretColor = true;
            inputField.caretColor = new Color(0, 0, 0, 0);

            inputField.onSelect.AddListener(_ =>
            {
                OpenKeyboard(shouldReplaceEmojiTextToImage, shouldCloseKeyboardAfterSubmit, inputField.text, color,
                    backgroundColor, top, bottom, left,
                    width, height, transform, position, border, fontSize);
                _lastFocusedInputField = inputField;
            });
            
            inputField.onDeselect.AddListener(_ => CloseKeyboard());

            _subscribedInputFields.Add(inputField);
        }

        public static void InsertTextAtCursor(TMP_InputField targetInputField, string text,
            bool shouldReplaceEmojiTextToImage = true)
        {
            _targetInputField = targetInputField;
            InsertTextAtCursor(text, shouldReplaceEmojiTextToImage);
            _targetInputField = null;
        }

        [DllImport("__Internal")]
        private static extern void InsertTextAtCursor(string text, bool shouldReplaceEmojiTextToImage = true);

        [DllImport("__Internal")]
        private static extern void OpenKeyboard(bool shouldReplaceEmojiTextToImage, bool shouldCloseKeyboardAfterSubmit,
            string startText = null,
            string color = null,
            string backgroundColor = null, string top = null, string bottom = null,
            string left = null, string width = null, string height = null, string transform = null, string position = null,
            string border = null, string fontSize = null);

        [DllImport("__Internal")]
        private static extern void CloseKeyboard();
    }
}