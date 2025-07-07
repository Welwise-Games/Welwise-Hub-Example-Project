using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Transporting;
using WelwiseChangingNicknameModule.Runtime.Client.Scripts.Services;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Network.Broadcasts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseNicknameSharedModule.Runtime.Client.Scripts;
using WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule;

namespace WelwiseChangingNicknameModule.Runtime.Client.Scripts
{
    public static class NicknameChangingTools
    {
        public static void InitializePlayer(ClientsNicknamesProviderService clientsNicknamesProviderService,
            PlayerNicknameTextController playerNicknameTextController, NetworkConnection networkConnection)
        {
            clientsNicknamesProviderService.ChangedClientNickname +=
                (clientWithNewNameNetworkConnection, nickname) =>
                {
                    if (clientWithNewNameNetworkConnection == networkConnection)
                        playerNicknameTextController.SetNickname(nickname);
                };
        }

        public static void InitializeBot(BotsNicknamesProviderService botsNicknamesProviderService,
            PlayerNicknameTextController botNicknameTextController, int botObjectId)
        {
            botsNicknamesProviderService.ChangedBotNickname +=
                (objectId, nickname) =>
                {
                    if (objectId == botObjectId)
                        botNicknameTextController.SetNickname(nickname);
                };
        }

        public static void Initialize(ClientManager clientManager,
            IClientsNicknamesDataProvider clientsNicknamesDataProvider, IPlayerData playerData,
            SharedClientsNicknamesConfig sharedClientsNicknamesConfig,
            out NicknamesSharedEntryPointData nicknamesSharedEntryPointData)
        {
            var sharedClientsNicknamesProviderService =
                new SharedClientsNicknamesProviderService(sharedClientsNicknamesConfig, clientsNicknamesDataProvider);
            
            var clientsNicknamesProviderService = new ClientsNicknamesProviderService(sharedClientsNicknamesProviderService, 
                clientsNicknamesDataProvider);
            
            clientsNicknamesProviderService.ChangedClientNickname +=
                (networkConnection, nickname) =>
                    TrySavingOwnerNicknameDataForMetaverse(playerData, networkConnection, nickname);
            
            clientsNicknamesDataProvider.AddedNicknameData +=                 
                (networkConnection, data) => TrySavingOwnerNicknameDataForMetaverse(playerData, networkConnection, data.Nickname);
            
            clientManager.RegisterBroadcast<SettingNicknameBroadcastForClient>(SetPlayerNickname);
            
            nicknamesSharedEntryPointData = new NicknamesSharedEntryPointData(clientsNicknamesProviderService);

            void SetPlayerNickname(SettingNicknameBroadcastForClient broadcast, Channel channel)
            {
                clientsNicknamesProviderService.TrySettingClientNickname(broadcast.NicknameOwnerNetworkConnection,
                    broadcast.NewNickname);
            }
        }
        
        private static void TrySavingOwnerNicknameDataForMetaverse(IPlayerData playerData, NetworkConnection networkConnection,
            string nickname) =>
            SetOwnersMetaverseStringData(playerData, networkConnection, nickname);
        
        private static void SetOwnersMetaverseStringData(IPlayerData playerData, NetworkConnection networkConnection,
            string nickname)
        {
            if (!networkConnection.IsOwners() || nickname == playerData.GetPlayerName())
                return;

            playerData.SetPlayerName(nickname);
            playerData.Save();
        }
    }
}