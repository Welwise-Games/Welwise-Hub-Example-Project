using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public class TrainingFactory
    {
        private readonly TrainingConfigsProviderService _trainingConfigsProviderService;
        private readonly IAssetLoader _assetLoader;
        
        private readonly Container _container = new Container();

        private const string ArrowViewAssetId = 
#if ADDRESSABLES
        "ArrowView";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/ArrowView";
#endif

        private const string TrainingPopupAssetId = 
#if ADDRESSABLES
        "TrainingPopup";
#else
        "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/TrainingPopup";
#endif

        public TrainingFactory(TrainingConfigsProviderService trainingConfigsProviderService, IAssetLoader assetLoader)
        {
            _trainingConfigsProviderService = trainingConfigsProviderService;
            _assetLoader = assetLoader;
        }

        public void Dispose() => _container.DestroyAndClearAllImplementationsAsync().Forget();

        public async UniTask<TrainingPopupView> GetTrainingPopupViewAsync(Transform parent) =>
            await _container.GetOrLoadAndRegisterObjectAsync<TrainingPopupView>(TrainingPopupAssetId, _assetLoader,
                loaded: async popup => { popup.transform.SetSiblingIndex(popup.transform.parent.childCount - 2); }, parent: parent);

        public async UniTask<ArrowsDisplayingController> GetArrowsDisplayingControllerAsync(Transform playerTransform,
            Transform shopTransform, DataContainer<Transform> arrowsParentContainer)
        {
            return await _container.GetOrRegisterSingleByTypeAsync(async () =>
            {
                var prefab =
                    await _container.GetOrLoadAndRegisterObjectAsync<ArrowView>(ArrowViewAssetId, _assetLoader, shouldCreate: false);

                var arrowsParent = new GameObject("ArrowsParent").transform;
                arrowsParentContainer.Data = arrowsParent;
                return new ArrowsDisplayingController(arrowsParent,
                    new ObjectPool<ArrowView>(arrowsParent, prefab),
                    await _trainingConfigsProviderService.GetArrowsDisplayingConfigAsync(), new ArrowsPathProvider(
                        playerTransform, shopTransform));
            });
        }
    }
}