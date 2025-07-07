using System.Collections.Generic;
using WelwiseSharedModule.Runtime.Server.Scripts;

namespace WelwiseChangingAnimationModule.Runtime.Server.Scripts
{
    public class SetPlayerAnimationPlaceModelsProviderService
    {
        public IReadOnlyDictionary<IRoom, HashSet<SetPlayerAnimationPlaceModel>> ModelsByRoom => _modelsByRoom;

        private readonly Dictionary<IRoom, HashSet<SetPlayerAnimationPlaceModel>> _modelsByRoom =
            new Dictionary<IRoom, HashSet<SetPlayerAnimationPlaceModel>>();
        
        public void AddModelsByRoom(IRoom room, HashSet<SetPlayerAnimationPlaceModel> models)
        {
            _modelsByRoom.Remove(room);
            _modelsByRoom.Add(room, models);
        }

        public void RemoveModelsByRoom(IRoom room) => _modelsByRoom.Remove(room);
    }
}