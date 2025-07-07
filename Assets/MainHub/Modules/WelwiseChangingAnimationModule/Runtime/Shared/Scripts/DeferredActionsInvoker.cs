using System;
using System.Collections.Generic;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts
{
    public class DeferredActionsInvoker
    {
        private readonly Dictionary<object, Action> _subscribedActions = new Dictionary<object, Action>();

        private readonly DeferredActionInvoker _deferredActionInvoker;

        public DeferredActionsInvoker(DeferredActionInvoker deferredActionInvoker)
        {
            _deferredActionInvoker = deferredActionInvoker;

            _deferredActionInvoker.Action += InvokeSubscribedActionsAndClearDictionary;
        }

        public void UpdateActionAndInvokeActionInEndOfFrame(object actionId, Action action)
        {
            _subscribedActions.AddOrAppoint(actionId, action);
            _deferredActionInvoker.WaitEndFrameAndInvokeAction();
        }

        private void InvokeSubscribedActionsAndClearDictionary()
        {
            _subscribedActions.ForEach(action => action.Value?.Invoke());
            _subscribedActions.Clear();
        }
    }
}