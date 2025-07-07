using System.Collections.Generic;
using System.Linq;
using WelwiseChangingAnimationModule.Runtime.Client.Scripts.SetPlayerAnimationsButton;

namespace WelwiseChangingAnimationModule.Runtime.Client.Scripts
{
    public class SetPlayerAnimationButtonControllersProviderService
    {
        public IReadOnlyDictionary<int, SetPlayerAnimationPlaceController> ControllersById => _controllersById;

        private Dictionary<int, SetPlayerAnimationPlaceController> _controllersById =
            new Dictionary<int, SetPlayerAnimationPlaceController>();

        public void AppointControllers(HashSet<SetPlayerAnimationPlaceController> controllers) =>
            _controllersById = controllers.ToDictionary(controller => controller.Id, controller => controller);

        public void ClearControllers() => _controllersById.Clear();
    }
}