using System;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public class EnteredToPortalEventProvider
    {
        public event Action<string> EnteredToPortal;

        public void InvokeOwnerEnteredToPortal(string gameId) => EnteredToPortal?.Invoke(gameId);
    }
}