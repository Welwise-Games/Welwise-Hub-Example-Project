using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Tools
{
    public static class ContainerTools
    {
        public static async UniTask<TController> GetControllerAsync<TController, TView>(this Container container,
            string viewAssetId, IAssetLoader assetLoader,
            Func<TView, UniTask> created,
            bool shouldMakeDontDestroyOnLoad = false, Transform parent = null)
            where TView : MonoBehaviour where TController : class
        {
            var viewInstance = await container.GetSingleByAssetIdAsync<TView>(viewAssetId);
            
            if (!viewInstance)
            {
                await container.GetOrLoadAndRegisterObjectAsync(viewAssetId, assetLoader, created, shouldCreate: true,
                    parent: parent, shouldMakeDontDestroyOnLoad: shouldMakeDontDestroyOnLoad);
            }

            return container.GetSingleByType<TController>();
        }
    }
}