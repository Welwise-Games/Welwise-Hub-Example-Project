using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents;
using WelwiseChatModule.Runtime.Client.Scripts.UI;
using WelwiseClothesSharedModule.Runtime.Client.Scripts;
using WelwiseEmotionsModule.Runtime.Client.Scripts.Animations;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts
{
    public class ClientPlayerComponents
    {
        public readonly ClientPlayerCharacterComponents CharacterComponents;
        public readonly ClientPlayerSerializableComponents SerializableComponents;
        public readonly PlayerChatTextController ChatTextController;
        public readonly PlayerEmotionsComponents PlayerEmotionsComponents;
        public readonly SkinColorChangerController SkinColorChangerController;
        public readonly ColorableClothesViewController ColorableClothesViewController;

        public ClientPlayerComponents(PlayerChatTextController chatTextController,
            SkinColorChangerController skinColorChangerController,
            ColorableClothesViewController colorableClothesViewController,
            PlayerEmotionsComponents playerEmotionsComponents, ClientPlayerCharacterComponents characterComponents,
            ClientPlayerSerializableComponents serializableComponents)
        {
            ChatTextController = chatTextController;
            SkinColorChangerController = skinColorChangerController;
            ColorableClothesViewController = colorableClothesViewController;
            PlayerEmotionsComponents = playerEmotionsComponents;
            CharacterComponents = characterComponents;
            SerializableComponents = serializableComponents;
        }
    }
}