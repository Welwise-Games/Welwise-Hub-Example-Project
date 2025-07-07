using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem;
using WelwiseSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.Localization;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public static class TrainingTools
    {
        private const string OpenedShopPopupHubSavingsVariableName = "OpenedShopPopup";
        private const string EnteredPortalHubSavingsVariableName = "EnteredPortal";

        public static void Initialize(
            ClientsConnectionTrackingServiceForClient clientsConnectionTrackingServiceForClient,
            IAssetLoader assetLoader, out TrainingEntryPointData trainingEntryPointData)
        {
            var trainingConfigsProviderService = new TrainingConfigsProviderService(assetLoader);
            var trainingFactory = new TrainingFactory(trainingConfigsProviderService, assetLoader);

            clientsConnectionTrackingServiceForClient.OwnerDisconnected +=
                trainingFactory.Dispose;

            trainingEntryPointData = new TrainingEntryPointData(trainingFactory, trainingConfigsProviderService);
        }

        public static async UniTask InitializeTrainingProcessAsync(IPlayerData playerData,
            ShopPopupController shopPopupController,
            Transform playerTransform, Transform shopTransform, IPlatformNavigation platformNavigation,
            EnteredToPortalEventProvider enteredToPortalEventProvider, TrainingFactory trainingFactory,
            Transform trainingPopupParent,
            ClientsConnectionTrackingServiceForClient clientsConnectionTrackingServiceForClient)
        {
            if (!playerData.GameData.GetBool(EnteredPortalHubSavingsVariableName))
            {
                await EnableGoToPortalTrainingAsync(trainingFactory, playerData,
                    enteredToPortalEventProvider);
            }

            if (!playerData.GameData.GetBool(OpenedShopPopupHubSavingsVariableName))
            {
                await EnableGoToShopTrainingAsync(trainingFactory, shopTransform, playerTransform, shopPopupController,
                    playerData, trainingPopupParent, clientsConnectionTrackingServiceForClient);
            }
        }

        private static async UniTask EnableGoToPortalTrainingAsync(TrainingFactory trainingFactory,
            IPlayerData playerData, EnteredToPortalEventProvider enteredToPortalEventProvider)
        {
            enteredToPortalEventProvider.EnteredToPortal += id =>
            {
                playerData.GameData.SetBool(EnteredPortalHubSavingsVariableName, true);
                playerData.Save();

                if (playerData.GameData.GetBool(OpenedShopPopupHubSavingsVariableName))
                    trainingFactory.Dispose();
            };
        }

        private static async UniTask EnableGoToShopTrainingAsync(TrainingFactory trainingFactory,
            Transform shopTransform,
            Transform playerTransform, ShopPopupController shopPopupController, IPlayerData playerData,
            Transform trainingPopupParent, ClientsConnectionTrackingServiceForClient clientsConnectionTrackingServiceForClient)
        {
            var arrowsParentContainer = new DataContainer<Transform>();
            var arrowsController = await trainingFactory.GetArrowsDisplayingControllerAsync(
                playerTransform, shopTransform, arrowsParentContainer);

            var trainingPopupView = await trainingFactory.GetTrainingPopupViewAsync(trainingPopupParent);

            trainingPopupView.DoText.text = await LocalizationTools.GetLocalizedStringAsync(
                LocalizationTablesHolder.TrainingPopup,
                LocalizationKeysHolder.SetPlayerClothes);

            arrowsParentContainer.Data.gameObject.GetOrAddComponent<MonoBehaviourObserver>().Updated +=
                arrowsController.UpdateArrows;

            clientsConnectionTrackingServiceForClient.OwnerDisconnected += ReleaseAllArrowsAndUnsubscribe;

            void ReleaseAllArrowsAndUnsubscribe()
            {
                Object.Destroy(arrowsParentContainer.Data.gameObject);
                clientsConnectionTrackingServiceForClient.OwnerDisconnected -= ReleaseAllArrowsAndUnsubscribe;
            }
            
            shopPopupController.ShopPopup.Popup.Opened += async () =>
            {
                playerData.GameData.SetBool(OpenedShopPopupHubSavingsVariableName, true);
                playerData.Save();

                Object.Destroy(arrowsParentContainer.Data.gameObject);

                if (!playerData.GameData.GetBool(EnteredPortalHubSavingsVariableName))
                {
                    trainingPopupView.DoText.text = await LocalizationTools.GetLocalizedStringAsync(
                        LocalizationTablesHolder.TrainingPopup,
                        LocalizationKeysHolder.GoToGameThroughPortal);
                }
                else
                    trainingFactory.Dispose();
            };
        }
    }
}