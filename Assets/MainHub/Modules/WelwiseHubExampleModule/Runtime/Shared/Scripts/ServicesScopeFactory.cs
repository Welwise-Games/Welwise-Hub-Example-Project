using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts
{
    public class ServicesScopeFactory
    {
        private const string ServicesScopeAssetId =
#if ADDRESSABLES
#if UNITY_SERVER
            "ServerServicesScope";
#else
            "ClientServicesScope";
#endif
#else
#if UNITY_SERVER
            "WelwiseHubExampleModule/Runtime/Server/Loadable/ServerServicesScope";
#else
            "WelwiseHubExampleModule/Runtime/Client/Loadable/Prefabs/ClientServicesScope";
#endif
#endif

        public void CreateServicesScope() => AssetProvider
            .InstantiateAsync<GameObject>(ServicesScopeAssetId, AssetsLoaderTools.GetAssetLoader()).Forget();
    }
}