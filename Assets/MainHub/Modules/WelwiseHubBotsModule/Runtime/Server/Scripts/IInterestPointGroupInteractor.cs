using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public interface IInterestPointGroupInteractor : IDisposable
    {
        bool IsDestroyInteractionAction { get; }
        Vector3? GetDestinationPosition();
        UniTask StartInteractionWithInterestPointAsync(Action<bool> changedRunningState);
        UniTask OnEndInteraction();
    }
}