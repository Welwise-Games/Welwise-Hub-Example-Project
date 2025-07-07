using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using UnityEngine;
using WelwiseChangingNicknameModule.Runtime.Server.Scripts.Services;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Network.Broadcasts;
using WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services;
using WelwiseSharedModule.Runtime.Server.Scripts;

namespace WelwiseChangingNicknameModule.Runtime.Server.Scripts
{
    public static class NicknamesSharedEntryPointTools
    {
        public static void Initialize(SharedClientsNicknamesConfig sharedClientsNicknamesConfig,
            ServerManager serverManager, IVisibleClientsProviderService visibleClientsProviderService,
            IClientsNicknamesDataProvider clientsNicknamesDataProvider,
            out NicknamesSharedEntryPointData nicknamesSharedEntryPointData,
            ICanSetNicknameConditionProvider canSetNicknameConditionProvider = null)
        {
            canSetNicknameConditionProvider ??= new DefaultCanSetNicknamesConditionProvider();
            
            var sharedClientsNicknamesProviderService =
                new SharedClientsNicknamesProviderService(sharedClientsNicknamesConfig, clientsNicknamesDataProvider);
            
            var clientsNicknamesProviderService = new ClientsNicknamesProviderService(
                sharedClientsNicknamesProviderService,
                canSetNicknameConditionProvider);
            
            clientsNicknamesProviderService.ChangedClientNickname += (networkConnection, nickname) => TrySendingForAllPlayersAboutChangingName(networkConnection, nickname, 
                visibleClientsProviderService, serverManager);
            
            serverManager.RegisterBroadcast<SettingNicknameBroadcastForServer>(TrySettingPlayerNameAsync);

            nicknamesSharedEntryPointData = new NicknamesSharedEntryPointData(clientsNicknamesProviderService);
            
            async void TrySettingPlayerNameAsync(NetworkConnection networkConnection,
                SettingNicknameBroadcastForServer broadcast, Channel channel)
            {
                await clientsNicknamesProviderService.TrySettingClientNicknameAsync(networkConnection,
                    broadcast.NewNickname);
            }
        }
        
        private static void TrySendingForAllPlayersAboutChangingName(NetworkConnection changerNetworkConnection,
            string nickname, IVisibleClientsProviderService visibleClientsProviderService, ServerManager serverManager)
        {
            var connectionsForSending =
                visibleClientsProviderService.GetVisibleClientsForClient(changerNetworkConnection).Append(changerNetworkConnection);

            foreach (var clientConnection in connectionsForSending)
            {
                serverManager.Broadcast(clientConnection, new SettingNicknameBroadcastForClient(
                    nickname, changerNetworkConnection));
            }
        }
    }
}