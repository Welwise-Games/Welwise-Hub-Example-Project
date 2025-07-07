using WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts
{
    public class OwnerPlayerComponents
    {
        public readonly OwnerPlayerCharacterComponents CharacterComponents;
        public readonly OwnerPlayerSerializableComponents SerializableComponents;
        public readonly ClientPlayerComponents ClientComponents;

        public OwnerPlayerComponents(OwnerPlayerCharacterComponents characterComponents,
            OwnerPlayerSerializableComponents serializableComponents, ClientPlayerComponents clientComponents)
        {
            CharacterComponents = characterComponents;
            SerializableComponents = serializableComponents;
            ClientComponents = clientComponents;
        }
    }
}