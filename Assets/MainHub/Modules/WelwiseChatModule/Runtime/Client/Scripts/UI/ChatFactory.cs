using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WelwiseChatModule.Runtime.Client.Scripts.Network;
using WelwiseChatModule.Runtime.Client.Scripts.UI.Window;
using WelwiseChatModule.Runtime.Shared.Scripts.Network;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Object = UnityEngine.Object;

namespace WelwiseChatModule.Runtime.Client.Scripts.UI
{
    public class ChatFactory
    {
        public event Action<ChatWindowController> CreatedChatWindowController;

        private readonly ChatsDataProviderService _chatsDataProviderService;
        private readonly IClientsNicknamesProviderService _clientsNicknamesProviderService;

        private readonly IAssetLoader _assetLoader;
        private readonly Container _container = new Container();

        private const string ChatWindowAssetId =
#if ADDRESSABLES
            "ChatWindow";
#else
        LoadablePath + "ChatWindow";
#endif
        private const string EmojiButtonAssetId =
#if ADDRESSABLES
            "EmojiButton";
#else
       LoadablePath + "EmojiButton";
#endif
        private const string ChatConfigAssetId =
#if ADDRESSABLES
            "ChatConfig";
#else
        LoadablePath + "ChatConfig";
#endif

#if !ADDRESSABLES
        private const string LoadablePath = "WelwiseChatModule/Runtime/Client/Loadable/";
#endif

        public ChatFactory(ChatsDataProviderService chatsDataProviderService,
            IClientsNicknamesProviderService clientsNicknamesProviderService, IAssetLoader assetLoader)
        {
            _chatsDataProviderService = chatsDataProviderService;
            _clientsNicknamesProviderService = clientsNicknamesProviderService;
            _assetLoader = assetLoader;
        }

        public async UniTask DisposeUIAsync()
            => await _container.DestroyAndClearAllImplementationsAsync();

        public async UniTask<PlayerChatTextController> GetChatTextControllerAsync(
            PlayerChatTextSerializableComponents textSerializableComponents,
            Camera mainCamera) =>
            new(await GetChatConfigAsync(), textSerializableComponents, mainCamera);

        public async UniTask<ChatWindowController> GetChatWindowControllerAsync(Transform windowParent,
            Func<bool> canSetChanOpenStateFunc) =>
            await _container.GetControllerAsync<ChatWindowController, ChatWindow>(ChatWindowAssetId, _assetLoader,
                async popup
                    =>
                {
                    var chatWindowController =
                        _container.RegisterAndGetSingleByType(new ChatWindowController(popup, _chatsDataProviderService,
                            this, await GetChatConfigAsync(), _clientsNicknamesProviderService,
                            canSetChanOpenStateFunc));

                    CreatedChatWindowController?.Invoke(chatWindowController);
                },
                parent: windowParent);

        public async UniTask<Button> GetEmojiButtonAsync(Transform parent)
        {
            var prefab =
                await _container.GetOrLoadAndRegisterObjectAsync<Button>(EmojiButtonAssetId, _assetLoader,
                    shouldCreate: false);
            return Object.Instantiate(prefab, parent);
        }

        private async UniTask<ChatConfig> GetChatConfigAsync() =>
            await _container.GetOrLoadAndRegisterObjectAsync<ChatConfig>(ChatConfigAssetId, _assetLoader);
    }
}