using UnityEngine;

namespace WelwiseChangingClothesModule.Runtime.Shared.Scripts
{
    [CreateAssetMenu(fileName = "ClientsConfig", menuName = "ClientsConfig")]
    public class ClientsConfig : ScriptableObject
    {
        [field: SerializeField] [field: Range(0, 1)] public float DefaultPlayerSkinColorValue { get; private set; } = 0.666f;
        [field: SerializeField] [field: Range(0, 1)] public float PlayerDefaultClothesColorValue { get; private set; } = 0f;

        [field: SerializeField] [field: Range(0, 0)] public float PlayerSkinColorMinimumValue { get; private set; } = 0;
        [field: SerializeField] [field: Range(1, 1)] public float PlayerSkinColorMaximumValue { get; private set; } = 1;
        [field: SerializeField] [field: Range(0, 0)] public float PlayerDefaultClothesColorMinimumValue { get; private set; } = 0;
        [field: SerializeField] [field: Range(1, 1)] public float PlayerDefaultClothesColorMaximumValue { get; private set; } = 1;
    }
}