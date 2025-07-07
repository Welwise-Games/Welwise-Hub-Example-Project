using UnityEngine;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts
{
    [CreateAssetMenu(fileName = "SharedClientsNicknamesConfig", menuName = "WelwiseNicknameModule/SharedClientsNicknamesConfig")]
    public class SharedClientsNicknamesConfig : ScriptableObject
    {
        [field: SerializeField] [field: Range(1, 32)] public int MinimalNicknameLength { get; private set; } = 1;
        [field: SerializeField] [field: Range(2, 32)] public int MaximumNicknameLength { get; private set; } = 2;
    }
}