using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.UI
{
    public class UIRootComponents
    {
        public readonly UIRootSerializableComponents SerializableComponents;
        public readonly ErrorTextController ErrorTextController;

        public UIRootComponents(UIRootSerializableComponents serializableComponents, ErrorTextController errorTextController)
        {
            SerializableComponents = serializableComponents;
            ErrorTextController = errorTextController;
        }
    }
}