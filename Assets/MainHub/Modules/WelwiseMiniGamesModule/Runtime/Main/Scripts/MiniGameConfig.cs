using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using WelwiseMiniGamesModule.Runtime.Shared.Scripts;

namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    [Serializable]
    public class MiniGameConfig
    {
#if !ADDRESSABLES || UNITY_EDITOR
    [field: SerializeField] public MiniGameSerializableComponents Prefab { get; private set; }
#endif
        [field: SerializeField] public AssetReference PrefabReference { get; private set; }
        [field: SerializeField] public MiniGame MiniGame { get; private set; }
        [field: SerializeField] [field: Range(1, 500)] public int Reward { get; private set; } = 100;
    }
}