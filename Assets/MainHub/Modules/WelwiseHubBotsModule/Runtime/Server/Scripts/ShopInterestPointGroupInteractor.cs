using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network;
using WelwiseChangingClothesModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseHubBotsModule.Runtime.Shared.Scripts;
using WelwisePetsModule.Runtime.Server.Scripts;
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class ShopInterestPointGroupInteractor : IInterestPointGroupInteractor
    {
        public bool IsDestroyInteractionAction => false;

        private readonly SharedBotSerializableComponents _botSerializableComponents;
        private readonly BotModel _botModel;
        private readonly ItemsConfigsProviderService _itemsConfigsProviderService;
        private readonly BotsPetsDataProviderService _botsPetsDataProviderService;
        private readonly PetsConfigProviderService _petsConfigProviderService;
        private readonly BotsNicknamesProviderService _botsNicknamesProviderService;
        private readonly BotsCustomizationDataProviderService _botsCustomizationDataProviderService;
        private readonly ClientsConfigsProviderService _clientsConfigsProviderService;

        private readonly Transform _shopTransform;
        private readonly BotsConfig _botsConfig;

        private readonly float _minimalValue;

        private readonly int _objectId;

        public ShopInterestPointGroupInteractor(ItemsConfigsProviderService itemsConfigsProviderService,
            BotsPetsDataProviderService botsPetsDataProviderService,
            PetsConfigProviderService petsConfigProviderService,
            BotsNicknamesProviderService botsNicknamesProviderService,
            BotsCustomizationDataProviderService botsCustomizationDataProviderService,
            ClientsConfigsProviderService clientsConfigsProviderService,
            SharedBotSerializableComponents botSerializableComponents, float minimalValue, Transform shopTransform,
            BotModel botModel, int objectId, BotsConfig botsConfig)
        {
            _itemsConfigsProviderService = itemsConfigsProviderService;
            _botsPetsDataProviderService = botsPetsDataProviderService;
            _petsConfigProviderService = petsConfigProviderService;
            _botsNicknamesProviderService = botsNicknamesProviderService;
            _botsCustomizationDataProviderService = botsCustomizationDataProviderService;
            _clientsConfigsProviderService = clientsConfigsProviderService;
            _botSerializableComponents = botSerializableComponents;
            _minimalValue = minimalValue;
            _shopTransform = shopTransform;
            _botModel = botModel;
            _objectId = objectId;
            _botsConfig = botsConfig;
        }

        public void Dispose()
        {
        }

        public Vector3? GetDestinationPosition()
        {
            return _shopTransform.transform.position;
        }

        public async UniTask StartInteractionWithInterestPointAsync(Action<bool> changedRunningState)
        {
            var stoppingDistance = _botSerializableComponents.NavMeshAgent.stoppingDistance;

            _botSerializableComponents.NavMeshAgent.stoppingDistance = _minimalValue;

            changedRunningState?.Invoke(true);

            await AsyncTools.WaitUniTaskWithoutCancelledOperationException(UniTask.WaitWhile(() =>
                    _botSerializableComponents.NavMeshAgent.remainingDistance >
                    _botSerializableComponents.NavMeshAgent.stoppingDistance,
                cancellationToken: _botSerializableComponents.destroyCancellationToken));

            if (!_botSerializableComponents)
                return;

            _botSerializableComponents.NavMeshAgent.stoppingDistance = stoppingDistance;

            changedRunningState?.Invoke(false);
        }

        public async UniTask OnEndInteraction()
        {
            TrySettingNickname();
            await TrySettingClothesAndSkinColorAsync();
            await TrySettingBotsPetsAsync();
        }

        private async UniTask TrySettingBotsPetsAsync()
        {
            _botsPetsDataProviderService.TrySettingBotData(_objectId,
                BotsSelectedPetsDataTools.GetRandomSelectedPetsData(_botsConfig.SetBotPetsDataPartChance,
                    await _petsConfigProviderService.GetPetsConfigAsync(),
                    _botsPetsDataProviderService.PetsData[_objectId]));
        }

        private async UniTask TrySettingClothesAndSkinColorAsync()
        {
            _botsCustomizationDataProviderService.TrySettingBotCustomizationData(_objectId,
                BotsCustomizationDataTools.GetRandomCustomizationData(
                    _botsCustomizationDataProviderService.BotsCustomizationData[_objectId],
                    await _clientsConfigsProviderService.GetClientsConfigAsync(),
                    _botsConfig.SetBotCustomizationDataPartChance,
                    await _itemsConfigsProviderService.GetItemsConfigAsync()));
        }

        private void TrySettingNickname()
        {
            if (!_botsConfig.SetBotCustomizationDataPartChance.UseAsChanceAndGetResult()) return;
            var myNickname = _botsNicknamesProviderService.Nicknames[_objectId];
            var newNickname = BotsNicknamesTools.GetRandomNickname(myNickname);
            _botsNicknamesProviderService.TrySettingBotNickname(_objectId, newNickname);
        }
    }
}