using FishNet.Broadcast;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Services.Data;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts
{
    public struct LoginBroadcast : IBroadcast
    {
        public readonly ClientData ClientData;

        public LoginBroadcast(ClientData clientData)
        {
            ClientData = clientData;
        }
    }
}