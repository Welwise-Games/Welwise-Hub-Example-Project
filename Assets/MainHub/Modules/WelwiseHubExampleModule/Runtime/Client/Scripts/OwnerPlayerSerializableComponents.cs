using TMPro;
using UnityEngine;
using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts
{
    public class OwnerPlayerSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public OwnerPlayerCharacterSerializableComponents CharacterSerializableComponents { get; private set; }
        [field: SerializeField] public ClientPlayerSerializableComponents ClientSerializableComponents { get; private set; }
    }
}