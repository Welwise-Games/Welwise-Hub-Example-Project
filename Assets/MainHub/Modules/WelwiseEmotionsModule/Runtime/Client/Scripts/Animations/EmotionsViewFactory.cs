using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations
{
    public class EmotionsViewFactory
    {
        private readonly EmotionsViewConfigsProviderService _emotionsViewConfigsProviderService;

        public EmotionsViewFactory(EmotionsViewConfigsProviderService emotionsViewConfigsProviderService) => _emotionsViewConfigsProviderService = emotionsViewConfigsProviderService;

        public async UniTask<ParticlesParentSerializableComponents[]> TryCreatingParticlesParentsAsync(Transform parent, string emotionIndex)
        {
            var emotionsViewConfig = await _emotionsViewConfigsProviderService.GetEmotionsViewConfigAsync();
            
            var particlesPrefabs = emotionsViewConfig.EmotionsConfigs
                .FirstOrDefault(config => config.EmotionIndex == emotionIndex)?.ParticlesComponentsParentsPrefabs;
            
           var a =  particlesPrefabs?.Where(prefab => prefab != null).Select(prefab =>
            {
                var instance =  Object.Instantiate(prefab, parent);
                Timer.TryStartingCountingTime(emotionsViewConfig.MaxParticlesLifeTime, () => Object.Destroy(instance),
                    false, instance.GetCancellationTokenOnDestroy()).Forget();
                return instance;
            }).ToArray();

           return a;
        }
    }
}