using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI.Window
{
    public class ChatWindow : MonoBehaviour
    {
        [field: SerializeField] public MonoBehaviourObserver PopupMonoBehaviourObserver { get; private set; }
        [field: SerializeField] public MonoBehaviourObserver WindowMonoBehaviourObserver { get; private set; }
        [field: SerializeField] public Button CloseChatButton { get; private set; }
        [field: SerializeField] public Button OpenChatButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI OpenChatKeyCodeText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI CloseChatKeyCodeText { get; private set; }
        [field: SerializeField] public Image OpenChatKeyCodeBackgroundImage { get; private set; }
        [field: SerializeField] public Button SetActiveEmojiParentButton { get; private set; }
        [field: SerializeField] public Popup ChatPopup { get; private set; }
        [field: SerializeField] public TMP_InputField InputField { get; private set; }
        [field: SerializeField] public CanvasGroup[] HidedCanvasGroupsOnSelectChatOnMobile { get; private set; }
        [field: SerializeField] public TextMeshProUGUI PressEnterText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ChatText { get; private set; }
        [field: SerializeField] public Color NicknamesColor { get; private set; }
        [field: SerializeField] public ChatZone StartTargetChatZone { get; private set; }
        [field: SerializeField] public Transform EmojiPanelRootParent { get; private set; }
        [field: SerializeField] public Transform EmojiButtonsParent { get; private set; }
        [field: SerializeField] public RectTransform ScrollRectTransform { get; private set; }
        [field: SerializeField] public RectTransform ChatElementsParentRectTransform { get; private set; }
        [field: SerializeField] public List<EmojiConfig> EmojiConfigs { get; private set; }
        [field: SerializeField] public List<SelectionChatZoneButtonConfig> SelectionChatZoneButtonsConfigs { get; private set; }
        [field: SerializeField] public ChatPopupSettingScaleModeButtonConfig SelectionShortScaleModeButtonConfig { get; private set; }
        [field: SerializeField] public ChatPopupSettingScaleModeButtonConfig SelectionFullScaleModeButtonConfig { get; private set; }
        [field: SerializeField] public Button SettingScaleModeButton { get; private set; }
        [field: SerializeField] public Image SettingScaleModeImage { get; private set; }
    }
}