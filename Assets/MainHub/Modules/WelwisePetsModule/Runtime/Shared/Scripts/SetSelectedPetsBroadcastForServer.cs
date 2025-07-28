using System.Collections.Generic;
using FishNet.Broadcast;

namespace WelwisePetsModule.Runtime.Shared.Scripts
{
    public struct SetSelectedPetsBroadcastForServer : IBroadcast
    {
        public List<SelectedPetData> Data { get; set; }

        public SetSelectedPetsBroadcastForServer(List<SelectedPetData> data)
        {
            Data = data;
        }
    }
}