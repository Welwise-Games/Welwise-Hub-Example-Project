using UnityEngine;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents
{
    public class ClientPlayerCharacterSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Transform AnimatorChildrenParent { get; private set; }
    }
}