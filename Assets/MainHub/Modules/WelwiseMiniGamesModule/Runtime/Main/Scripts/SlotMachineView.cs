using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    public class SlotMachineView : MonoBehaviour
    {
        [field: SerializeField] public ColliderObserver StartGameColliderObserver { get; private set; }
        [field: SerializeField] public MiniGame MiniGame { get; private set; }
    }
}