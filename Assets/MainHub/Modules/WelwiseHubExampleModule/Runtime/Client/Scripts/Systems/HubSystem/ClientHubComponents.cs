using WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class ClientHubComponents
    {
        public readonly ClientHubSerializableComponents SerializableComponents;
        public readonly ShopController ShopController;

        public ClientHubComponents(ClientHubSerializableComponents serializableComponents, ShopController shopController)
        {
            SerializableComponents = serializableComponents;
            ShopController = shopController;
        }
    }
}