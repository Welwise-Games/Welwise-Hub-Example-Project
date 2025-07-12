using System;
using System.Text.RegularExpressions;
using UnityEngine;
using WebGLMobileKeyboardModule.Runtime.Scripts;
using WelwiseChatModule.Runtime.Client.Scripts.Network;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI.Window
{
    public class ChatWindowController
    {
        private ChatZone? _targetChatZone;

        private bool _isSelectedFullScaleMode = true;

        public readonly ChatWindow ChatWindow;
        private readonly ChatsDataProviderService _chatsDataProviderService;
        private readonly ChatConfig _chatConfig;
        private readonly IClientsNicknamesProviderService _clientsNicknamesProviderService;

        private readonly Func<bool> _canSetChatOpenStateFunc;


        public ChatWindowController(ChatWindow window, ChatsDataProviderService chatsDataProviderService,
            ChatFactory chatFactory, ChatConfig chatConfig,
            IClientsNicknamesProviderService clientsNicknamesProviderService, Func<bool> canSetChatOpenStateFunc)
        {
            _chatsDataProviderService = chatsDataProviderService;
            _chatConfig = chatConfig;
            _clientsNicknamesProviderService = clientsNicknamesProviderService;
            _canSetChatOpenStateFunc = canSetChatOpenStateFunc;
            ChatWindow = window;

            window.PopupMonoBehaviourObserver.Updated += SubscribeToSendMessageOrOpenChatIfEnterButtonPressed;

            window.CloseChatButton.onClick.AddListener(() => window.ChatPopup.TryClosing());
            window.OpenChatButton.onClick.AddListener(() => window.ChatPopup.TryOpening());

            window.OpenChatKeyCodeBackgroundImage.gameObject.SetActive(!DeviceDetectorTools.IsMobile());
            window.OpenChatButton.gameObject.SetActive(!window.ChatPopup.IsOpen);

            window.InputField.onSubmit.AddListener(TrySendingMessage);
            
            window.CloseChatKeyCodeText.gameObject.SetActive(!DeviceDetectorTools.IsMobile());

            if (!DeviceDetectorTools.IsMobile())
            {
                window.OpenChatKeyCodeText.text = chatConfig.SetOpenStateChatWindowPopupKeyCode.ToString();
                window.CloseChatKeyCodeText.text = $"[{chatConfig.SetOpenStateChatWindowPopupKeyCode.ToString()}]";
                window.WindowMonoBehaviourObserver.Updated += SetPopupOpenStateIfButtonPressed;
            }
            else
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                window.InputField.InitializeInputFieldForMobileKeyboard(true, false, "white", "rgba(0, 0, 0, 0.3)", "63px", null, "25px", 
                    "182px", "55px", "none", "absolute", "1px solid white", "16px");   

                window.InputField.onSelect.AddListener(_ =>
                {
                    window.HidedCanvasGroupsOnSelectChatOnMobile.ForEach(group => group.alpha = 0);
                });
                window.InputField.onDeselect.AddListener(_ =>
                {
                    window.HidedCanvasGroupsOnSelectChatOnMobile.ForEach(group => group.alpha = 1);
                });
#endif
            }

            window.ChatPopup.ChangedOpenState += isOpen => window.OpenChatButton.gameObject.SetActive(!isOpen);

            TryUpdatingTargetChatZoneAndUpdateChatText(window.StartTargetChatZone);

            window.InputField.onSelect.AddListener(text => UpdateInputFieldPressEnterText(true));
            window.InputField.onDeselect.AddListener(text => UpdateInputFieldPressEnterText(false));

            UpdateInputFieldPressEnterText(false);

            _chatsDataProviderService.AddedInitializationMessagesData +=
                () => UpdateChatText(_targetChatZone.Value);
            chatsDataProviderService.AddedMessageData +=
                (networkConnection, chatZone, message) => UpdateChatText(chatZone);

            CreateAndSubscribeEmojiButtonsAsync(window, chatFactory);
            SubscribeSetActiveEmojiParentButton(window);
            SubscribeSelectionChatZoneButtons();
            window.SettingScaleModeButton.onClick.AddListener(SetScaleMode);
        }

        private async void UpdateInputFieldPressEnterText(bool isSelected) =>
            ChatWindow.PressEnterText.text =
                await LocalizationTools.GetLocalizedStringAsync(
                    LocalizationTablesHolder.ChatWindow,
                    isSelected
                        ? LocalizationKeysHolder.InputFieldTextOnSelect
                        : DeviceDetectorTools.IsMobile()
                            ? LocalizationKeysHolder.InputFieldTextOnDeselectOnMobile
                            : LocalizationKeysHolder.InputFieldTextOnDeselectOnPC);

        private void SetPopupOpenStateIfButtonPressed()
        {
            if (!Input.GetKeyDown(_chatConfig.SetOpenStateChatWindowPopupKeyCode) || ChatWindow.InputField.isFocused)
                return;

            ChatWindow.ChatPopup.TrySettingOpenState();
        }

        private void SubscribeToSendMessageOrOpenChatIfEnterButtonPressed()
        {
            if (!Input.GetKeyDown(_chatConfig.SelectInputFieldOrSendMessageKeyCode) || !_canSetChatOpenStateFunc.Invoke())
                return;

            if (DeviceDetectorTools.IsMobile())
            {
                ChatWindow.InputField.onSubmit?.Invoke(ChatWindow.InputField.text);
            }
            else if (!ChatWindow.InputField.isFocused)
            {
                ChatWindow.InputField.ActivateInputField();
            }
        }

        private void SetScaleMode()
        {
            _isSelectedFullScaleMode = !_isSelectedFullScaleMode;

            var config = _isSelectedFullScaleMode
                ? ChatWindow.SelectionFullScaleModeButtonConfig
                : ChatWindow.SelectionShortScaleModeButtonConfig;

            var imageScale = ChatWindow.SettingScaleModeImage.transform.localScale;

            ChatWindow.SettingScaleModeImage.transform.localScale =
                new Vector3(imageScale.x, Mathf.Abs(imageScale.y) * (_isSelectedFullScaleMode ? 1 : -1),
                    imageScale.z);

            var anchoredPosition = ChatWindow.ChatElementsParentRectTransform.anchoredPosition;
            ChatWindow.ChatElementsParentRectTransform.anchoredPosition =
                new Vector2(anchoredPosition.x, config.PositionY);
            ChatWindow.ScrollRectTransform.sizeDelta =
                new Vector2(ChatWindow.ScrollRectTransform.sizeDelta.x, config.Height);
        }

        private void SubscribeSelectionChatZoneButtons()
        {
            ChatWindow.SelectionChatZoneButtonsConfigs.ForEach(button =>
                button.Button.onClick.AddListener(() => TryUpdatingTargetChatZoneAndUpdateChatText(button.Zone)));
        }

        private void TryUpdatingTargetChatZoneAndUpdateChatText(ChatZone chatZone)
        {
            if (_targetChatZone == chatZone)
                return;

            _targetChatZone = chatZone;
            UpdateChatText(_targetChatZone.Value);
        }

        private static void SubscribeSetActiveEmojiParentButton(ChatWindow window)
        {
            window.SetActiveEmojiParentButton.onClick.AddListener(() =>
                window.EmojiPanelRootParent.gameObject.SetActive(!window.EmojiPanelRootParent.gameObject.activeSelf));
        }

        private async void CreateAndSubscribeEmojiButtonsAsync(ChatWindow window, ChatFactory chatFactory)
        {
            foreach (var emojiConfig in window.EmojiConfigs)
            {
                var button = await chatFactory.GetEmojiButtonAsync(window.EmojiButtonsParent);
                button.image.sprite = emojiConfig.Sprite;
                button.onClick.AddListener(() => AddEmojiToText(emojiConfig));
            }
        }

        private void AddEmojiToText(EmojiConfig emojiConfig)
        {
            var emojiText = $"<sprite={emojiConfig.Index.ToString()}>";

#if UNITY_WEBGL && !UNITY_EDITOR
            if (DeviceDetectorTools.IsMobile())
            {
                MobileWebGLKeyboardTools.InsertTextAtCursor(ChatWindow.InputField, emojiText);
                return;
            }
#endif

            InsertTextAndAppointCaretPosition(emojiText);
        }

        private void InsertTextAndAppointCaretPosition(string newElementText)
        {
            ChatWindow.InputField.ActivateInputField();

            var startVisualCaretPosition = ChatWindow.InputField.caretPosition;

            int resultCaretPosition = 0, currentVisualCaretPosition = 0;

            var regex = new Regex(@"<sprite=\d+>");
            var matches = regex.Matches(ChatWindow.InputField.text);

            while (resultCaretPosition < ChatWindow.InputField.text.Length &&
                   currentVisualCaretPosition < startVisualCaretPosition)
            {
                var matched = false;
                foreach (Match match in matches)
                {
                    if (match.Index != resultCaretPosition) continue;
                    currentVisualCaretPosition += 1;
                    resultCaretPosition += match.Length;
                    matched = true;
                    break;
                }

                if (matched) continue;
                currentVisualCaretPosition += 1;
                resultCaretPosition += 1;
            }

            ChatWindow.InputField.text = ChatWindow.InputField.text.Insert(resultCaretPosition, newElementText);
            ChatWindow.InputField.caretPosition = startVisualCaretPosition + 1;
            ChatWindow.InputField.selectionFocusPosition = ChatWindow.InputField.caretPosition;
            ChatWindow.InputField.selectionAnchorPosition = ChatWindow.InputField.caretPosition;
        }

        private void UpdateChatText(ChatZone chatZone)
        {
            if (_targetChatZone != chatZone)
                return;

            var containsChatZoneMessagesData =
                _chatsDataProviderService.DataByChatZone.TryGetValue(chatZone, out var messages);

            if (!containsChatZoneMessagesData || messages == null)
            {
                ChatWindow.ChatText.text = "";
                return;
            }

            var chatContent = "";

            messages.ForEach(chatMessageData =>
                chatContent +=
                    $"\n{$"[{chatMessageData.AuthorNickname}]:".GetColored(ChatWindow.NicknamesColor)} {chatMessageData.Content}");

            ChatWindow.ChatText.text = chatContent;
        }

        private void TrySendingMessage(string text)
        {
            _chatsDataProviderService.TryProcessingAndAddingMessageData(_targetChatZone.Value,
                new ChatMessageData(text, _clientsNicknamesProviderService.Nicknames.GetOwners()),
                out var successfully, SharedNetworkTools.OwnerConnection);

            ChatWindow.InputField.text = "";
            ChatWindow.InputField.ActivateInputField();
        }
    }
}