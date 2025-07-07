using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using Object = UnityEngine.Object;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    public class ClothesSkinnedMeshRendererController
    {
        public event Action<ItemViewConfig, ItemViewConfig, ColorableClothesSerializableComponents> UpdatedInstance;
        public event Action<ItemViewConfig> RemovedInstance;

        private ItemViewConfig _targetItemViewConfig;
        private bool _shouldEnableInstances = true;

        private readonly List<GameObject> _defaultClothesInstance;
        private readonly List<GameObject> _prefabInstances = new List<GameObject>();
        private readonly Transform _modelTransform;
        private readonly SkinnedMeshRenderer _modelSkinnedMeshRenderer;
        private readonly ClothesFactory _clothesFactory;


        public ClothesSkinnedMeshRendererController(SkinnedMeshRenderer modelSkinnedMeshRenderer,
            Transform modelTransform, ClothesFactory clothesFactory, List<GameObject> defaultClothesInstance)
        {
            _modelSkinnedMeshRenderer = modelSkinnedMeshRenderer;
            _modelTransform = modelTransform;
            _clothesFactory = clothesFactory;
            _defaultClothesInstance = defaultClothesInstance;
        }

        public void SetShouldEnableInstances(bool shouldEnableInstances) => _shouldEnableInstances = shouldEnableInstances;

        public void SetActivePrefabInstances() => _prefabInstances.ForEach(instance => instance.gameObject.SetActive(_shouldEnableInstances));

        public async void UpdateInstanceAsync(ItemViewConfig itemConfig, bool shouldTakeOff)
        {
            shouldTakeOff = shouldTakeOff || itemConfig == null;
            
            if (shouldTakeOff)
            {
                if (_targetItemViewConfig != null)
                    RemovedInstance?.Invoke(_targetItemViewConfig);
             
                _prefabInstances.ForEach(Object.Destroy);
                _prefabInstances.Clear();
                _targetItemViewConfig = null;
            }
            else
            {
                var instance = await GetInstantiatedPrefabsInstancesAsync(_modelSkinnedMeshRenderer, itemConfig, _modelTransform);
                _prefabInstances.ForEach(Object.Destroy);
                _prefabInstances.Clear();
                _prefabInstances.Add(instance);
                SetActivePrefabInstances();
            }
            
            _defaultClothesInstance?.ForEach(instance => instance.SetActive(shouldTakeOff));
        }

        private async UniTask<GameObject> GetInstantiatedPrefabsInstancesAsync(SkinnedMeshRenderer prefabSkinnedMeshRenderer, ItemViewConfig itemConfig,
            Transform parent)
        {
            var instance = await _clothesFactory.GetClothesInstanceAsync(itemConfig, parent);

            var skinnedMeshRenderers = instance.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var skinnedRenderer in skinnedMeshRenderers)
            {
                skinnedRenderer.bones = prefabSkinnedMeshRenderer.bones;
                skinnedRenderer.rootBone = prefabSkinnedMeshRenderer.rootBone;
            }

            UpdatedInstance?.Invoke(_targetItemViewConfig, itemConfig, instance);

            _targetItemViewConfig = itemConfig;

            return instance.gameObject;
        }
    }
}