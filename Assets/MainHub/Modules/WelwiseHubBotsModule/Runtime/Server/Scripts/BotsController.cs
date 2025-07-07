using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotsController
    {
        private readonly BotsFactory _botsFactory;
        private readonly IRoom _targetRoom;
        private readonly Transform[] _portalsTransforms;
        private readonly Transform _shopTransform;
        private readonly Transform _sceneRootTransform;
        private readonly IAssetLoader _assetLoader;

        public BotsController(BotsFactory botsFactory, IRoom targetRoom, Transform[] portalsTransforms, Transform shopTransform,
            BotsConfig botsConfig, Transform sceneRootTransform, IAssetLoader assetLoader)
        {
            _botsFactory = botsFactory;
            _targetRoom = targetRoom;
            _portalsTransforms = portalsTransforms;
            _shopTransform = shopTransform;
            _sceneRootTransform = sceneRootTransform;
            _assetLoader = assetLoader;

            for (var i = 0; i < botsConfig.MaximumBotsNumber; i++)
                 SpawnBot(botsConfig);
        }

        private async void SpawnBot(BotsConfig botsConfig)
        {
            var botController = await _botsFactory.GetInitializedBotControllerAsync(_targetRoom, _portalsTransforms, _shopTransform, _sceneRootTransform.gameObject.scene, _assetLoader);
            
            botController.EnteredPortal += () =>
            {
                Timer.TryStartingCountingTime(Random.Range(botsConfig.MinimalBotRespawnTime, botsConfig.MaximumBotRespawnTime), () => SpawnBot(botsConfig),
                    false, _sceneRootTransform.gameObject.GetCancellationTokenOnDestroy()).Forget();
            };

            //botController.EnteredInPortal += _ => 
        }
    }
}