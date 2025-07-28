using System;
using FishNet.Object;
using UnityEngine;

namespace Modules.WelwiseMiniGamesModule.Runtime.Main.Scripts
{
    public class SlotMachineController
    {
        private readonly SlotMachineView _slotMachineView;
        public event Action<MiniGame> StartedMiniGame;

        public SlotMachineController(SlotMachineView slotMachineView)
        {
            _slotMachineView = slotMachineView;
            slotMachineView.StartGameColliderObserver.Entered += TryInvokingStartedMiniGame;
        }

        private void TryInvokingStartedMiniGame(Collider collider)
        {
            if (collider.TryGetComponent<NetworkObject>(out var networkObject) && networkObject.IsOwner)
                StartedMiniGame?.Invoke(_slotMachineView.MiniGame);
        }
    }
}