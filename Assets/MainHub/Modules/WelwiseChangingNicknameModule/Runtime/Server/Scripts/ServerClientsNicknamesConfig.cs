using UnityEngine;

namespace WelwiseChangingNicknameModule.Runtime.Server.Scripts
{
    [CreateAssetMenu(fileName = "ServerClientsNicknamesConfig", menuName = "WelwiseNicknameModule/ServerClientsNicknamesConfig")]
    public class ServerClientsNicknamesConfig : ScriptableObject
    {
        [field: SerializeField] public string DefaultNickname { get; private set; } = "Ghost";
    }
}