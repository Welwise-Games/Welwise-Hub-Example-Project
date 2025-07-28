using UnityEngine;

namespace WelwiseChangingNicknameModule.Runtime.Server.Scripts
{
    [CreateAssetMenu(menuName = "WelwiseNicknameModule/ServerClientsNicknamesConfig")]
    public class ServerClientsNicknamesConfig : ScriptableObject
    {
        [field: SerializeField] public string DefaultNickname { get; private set; } = "Ghost";
    }
}