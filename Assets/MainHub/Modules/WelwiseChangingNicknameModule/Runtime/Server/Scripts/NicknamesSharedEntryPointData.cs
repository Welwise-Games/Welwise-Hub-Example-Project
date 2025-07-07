using WelwiseChangingNicknameModule.Runtime.Server.Scripts.Services;

namespace WelwiseChangingNicknameModule.Runtime.Server.Scripts
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