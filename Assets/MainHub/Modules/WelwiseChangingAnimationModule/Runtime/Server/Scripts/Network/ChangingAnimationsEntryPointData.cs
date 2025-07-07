namespace WelwiseChangingAnimationModule.Runtime.Server.Scripts.Network
{
    public class ChangingAnimationsEntryPointData
    {
        public readonly ServerSetPlayersAnimationsPlacesSynchronizer ServerSetPlayersAnimationsPlacesSynchronizer;

        public ChangingAnimationsEntryPointData(
            ServerSetPlayersAnimationsPlacesSynchronizer serverSetPlayersAnimationsPlacesSynchronizer)
        {
            ServerSetPlayersAnimationsPlacesSynchronizer = serverSetPlayersAnimationsPlacesSynchronizer;
        }
    }
}