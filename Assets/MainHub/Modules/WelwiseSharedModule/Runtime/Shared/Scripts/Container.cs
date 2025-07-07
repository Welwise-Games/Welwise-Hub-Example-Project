using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Object = UnityEngine.Object;

namespace WelwiseSharedModule.Runtime.Shared.Scripts
{
    public class Container
    {
        private readonly Dictionary<object, object> _implementationsByHash = new Dictionary<object, object>();
        private readonly Dictionary<object, UniTask> _loadingImplementationsByHash = new Dictionary<object, UniTask>();
        private readonly CancellationTokenSource _clearCancellationTokenSource;

        public async UniTask<T> GetOrLoadAndRegisterObjectAsync<T>(string assetId, IAssetLoader assetLoader,
            Func<T, UniTask> loaded = null,
            Func<UniTask> notLoaded = null, bool shouldCreate = true, Transform parent = null,
            bool shouldMakeDontDestroyOnLoad = false,
            Vector3? position = null)
            where T : Object
        {
            var single = await GetSingleByAssetIdAsync<T>(assetId);
            
            var shouldLoad = !single || single is MonoBehaviour monoBehaviour && !monoBehaviour;

            if (!shouldLoad)
            {
                if (notLoaded != null)
                    await notLoaded.Invoke();

                return shouldCreate
                    ? await GetSingleByAssetIdAsync<T>(assetId, position, parent)
                    : await GetSingleByAssetIdAsync<T>(assetId);
            }

            var task = LoadOrInstantiateObjectAsync<T>(assetId, assetLoader, shouldCreate, parent, position);
            
            _loadingImplementationsByHash.Add(assetId, task);
            
            var instance = await task;
            
            if (Application.isEditor && !Application.isPlaying)
            {
                Object.DestroyImmediate(instance is Component component ? component.gameObject : instance);
                return null;
            }

            if (!instance)
                return null;

            if (shouldCreate && shouldMakeDontDestroyOnLoad)
                Object.DontDestroyOnLoad(instance);
            
            if (loaded != null)
                await loaded.Invoke(instance);

            TryRegisteringSingleByAssetId(instance, assetId);
            _loadingImplementationsByHash.Remove(assetId);
            
            return instance;
        }

        public async UniTask DestroyAndClearAllImplementationsAsync()
        {
            (await GetAllLoadedGameObjectsImplementationsAsync()).ForEach(Object.Destroy);
            ClearAllImplementations();
        }

        private async UniTask<HashSet<GameObject>> GetAllLoadedGameObjectsImplementationsAsync()
        {
            await AsyncTools.WaitWhileWithoutSkippingFrame(() => _loadingImplementationsByHash.Count > 0);

            return _implementationsByHash.Values
                .OfType<Component>()
                .Where(implementation => implementation && implementation.gameObject.scene.isLoaded)
                .Select(implementation => implementation.gameObject)
                .ToHashSet();
        }

        private void ClearAllImplementations()
        {
            _implementationsByHash.Clear();
            _loadingImplementationsByHash.Clear();
        }

        public async UniTask<T> GetOrRegisterSingleByTypeAsync<T>(Func<UniTask<T>> implementation) where T : class =>
            IsExistsByType<T>()
                ? GetSingleByType<T>()
                : RegisterAndGetSingleByType(await implementation.Invoke());

        public async UniTask<T> GetOrRegisterSingleByHashAsync<T>(object hash, Func<UniTask<T>> implementation)
            where T : class =>
            IsExistsByHash<T>(hash)
                ? GetSingleByHash<T>(hash)
                : RegisterAndGetSingleByHash(hash, await implementation.Invoke());

        public T GetOrRegisterSingleByHash<T>(object hash, Func<T> implementation) where T : class =>
            IsExistsByHash<T>(hash)
                ? GetSingleByHash<T>(hash)
                : RegisterAndGetSingleByHash(hash, implementation.Invoke());

        public T GetOrRegisterSingleByType<T>(Func<T> implementation) where T : class =>
            IsExistsByType<T>()
                ? GetSingleByType<T>()
                : RegisterAndGetSingleByType(implementation.Invoke());

        public T RegisterAndGetSingleByType<T>(T implementation, Type type = null) =>
            RegisterAndGetSingleByHash(type ?? typeof(T), implementation);

        public T RegisterAndGetSingleByHash<T>(object hash, T implementation)
        {
            _implementationsByHash.AddOrAppoint(hash, implementation);
            return implementation;
        }

        private void TryRegisteringSingleByAssetId<T>(T implementation, string assetId) =>
            _implementationsByHash.AddOrAppoint(assetId, implementation);

        public async UniTask<bool> IsExistsByAssetIdAsync<T>(string assetId) where T : class
        {
            var result = await GetSingleByAssetIdAsync<T>(assetId);
            return result != null;
        }

        public bool IsExistsByType<T>(Type type = null) where T : class =>
            GetSingleByType<T>(type ?? typeof(T)) != null;

        public bool IsExistsByHash<T>(object hash) where T : class =>
            GetSingleByHash<T>(hash) != null;

        public async UniTask<T> GetSingleByAssetIdAsync<T>(string assetId) where T : class
        {
            if (_loadingImplementationsByHash.ContainsKey(assetId))
                await AsyncTools.WaitWhileWithoutSkippingFrame(() =>
                    _loadingImplementationsByHash.ContainsKey(assetId));

            _implementationsByHash.TryGetValue(assetId, out var single);
            return single as T;
        }

        public T GetSingleByHash<T>(object hash) where T : class
        {
            _implementationsByHash.TryGetValue(hash, out var single);
            return single as T;
        }

        public T GetSingleByType<T>(Type type = null) where T : class => GetSingleByHash<T>(type ?? typeof(T));

        public void RemoveSingle<T>() where T : class => _implementationsByHash.Remove(typeof(T));

        private async Task<T> GetSingleByAssetIdAsync<T>(string assetId, Vector3? position, Transform parent)
            where T : Object
        {
            var single = await GetSingleByAssetIdAsync<T>(assetId);

            if (single is ScriptableObject)
                return single;

            if (single is not Component component)
                throw new ArgumentException("Single isn't Component");

            var transform = component.transform;

            if (position.HasValue)
                transform.position = position.Value;

            transform.SetParent(parent);
            return single;
        }

        private async UniTask<T> LoadOrInstantiateObjectAsync<T>(string assetId, IAssetLoader assetLoader, bool shouldCreate, Transform parent,
            Vector3? position) where T : Object =>
            shouldCreate && (typeof(T).IsSubclassOf(typeof(Component)) || typeof(T) == typeof(GameObject))
                ? await AssetProvider.InstantiateAsync<T>(assetId, assetLoader, position, parent: parent)
                : await AssetProvider.LoadAsync<T>(assetId, assetLoader);
    }
}