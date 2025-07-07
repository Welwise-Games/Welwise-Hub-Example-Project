using System;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public class EnteredToPortalEventProvider
    {
        public event Action<int> EnteredToPortal;

        public void InvokeOwnerEnteredToPortal(int gameId) => EnteredToPortal?.Invoke(gameId);
    }
}