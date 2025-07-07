using WelwiseChangingNicknameModule.Runtime.Client.Scripts.Services;

namespace WelwiseChangingNicknameModule.Runtime.Client.Scripts
{
    public class NicknamesSharedEntryPointData
    {
        public readonly ClientsNicknamesProviderService ClientsNicknamesProviderService;

        public NicknamesSharedEntryPointData(ClientsNicknamesProviderService clientsNicknamesProviderService)
        {
            ClientsNicknamesProviderService = clientsNicknamesProviderService;
        }
    }
}