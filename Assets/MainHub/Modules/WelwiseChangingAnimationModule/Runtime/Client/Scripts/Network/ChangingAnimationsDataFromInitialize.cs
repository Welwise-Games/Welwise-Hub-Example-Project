using WelwiseChangingAnimationModule.Runtime.Client.Scripts.Events;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts.Network
{
    public class ChangingAnimationsDataFromInitialize
    {
        public readonly SetPlayerAnimationButtonControllersProviderService
            SetPlayerAnimationButtonControllersProviderService;

        public ChangingAnimationsDataFromInitialize(
            SetPlayerAnimationButtonControllersProviderService setPlayerAnimationButtonControllersProviderService)
        {
            SetPlayerAnimationButtonControllersProviderService = setPlayerAnimationButtonControllersProviderService;
        }
    }
}