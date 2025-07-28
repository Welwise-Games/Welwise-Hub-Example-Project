using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations
{
    public class EmotionsViewFactory
    {
        private readonly EmotionsViewConfigProviderService _emotionsViewConfigProviderService;

        public EmotionsViewFactory(EmotionsViewConfigProviderService emotionsViewConfigProviderService) => _emotionsViewConfigProviderService = emotionsViewConfigProviderService;

        public async UniTask<ParticlesParentSerializableComponents[]> TryCreatingParticlesParentsAsync(Transform parent, string emotionIndex)
        {
            var emotionsViewConfig = await _emotionsViewConfigProviderService.GetEmotionsViewConfig();
            
            var particlesPrefabs = emotionsViewConfig.Configs
                .FirstOrDefault(config => config.ItemIndex == emotionIndex)?.ParticlesComponentsParentsPrefabs;
            
           return particlesPrefabs?.Where(prefab => prefab != null).Select(prefab =>
            {
                var instance =  Object.Instantiate(prefab, parent);
                Timer.TryStartingCountingTime(emotionsViewConfig.MaxParticlesLifeTime, () => Object.Destroy(instance),
                    false, instance.GetCancellationTokenOnDestroy()).Forget();
                return instance;
            }).ToArray();
        }
    }
}