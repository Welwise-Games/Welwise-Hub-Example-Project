using FishNet;
using FishNet.Connection;
using UnityEngine;

namespace WelwiseChangingAnimationModule.Runtime.Server.Scripts
{
    public class SetPlayerAnimationPlaceModel
    {
        public NetworkConnection PlaceOwnerPlayerConnection { get; private set; }
        public int? PlaceOwnerBotObjectId { get; private set; }
        public bool IsPlaceOwnerBot => PlaceOwnerBotObjectId.HasValue;
        public bool IsPlaceBusy => IsPlaceOwnerBot || PlaceOwnerPlayerConnection != null;
        public int Id { get; }
        public readonly Vector3 Position;

        public SetPlayerAnimationPlaceModel(int id, Vector3 position)
        {
            Id = id;
            Position = position;
        }

        public void SetPlaceOwnerPlayer(NetworkConnection placeOwner) => PlaceOwnerPlayerConnection = placeOwner;

        public void UpdatePlaceOwnerBotObjectId(int? objectId)
            => PlaceOwnerBotObjectId = objectId;
    }
}