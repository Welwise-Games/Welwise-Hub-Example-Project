using System.Collections.Generic;
using UnityEngine;
using WelwiseSharedModule.Runtime.Server.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChangingAnimationModule.Runtime.Server.Scripts
{
    public class SetPlayerAnimationPlaceModelsProviderService
    {
        public IReadOnlyDictionary<IRoom, HashSet<SetPlayerAnimationPlaceModel>> ModelsByRoom => _modelsByRoom;

        private readonly Dictionary<IRoom, HashSet<SetPlayerAnimationPlaceModel>> _modelsByRoom =
            new Dictionary<IRoom, HashSet<SetPlayerAnimationPlaceModel>>();
        
        public void AddModelsByRoom(IRoom room, HashSet<SetPlayerAnimationPlaceModel> models)
        {
            Debug.Log(models.Count);
            _modelsByRoom.AddOrAppoint(room, models);
        }

        public void RemoveModelsByRoom(IRoom room) => _modelsByRoom.Remove(room);
    }
}