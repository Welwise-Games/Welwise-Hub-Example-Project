using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseCharacterModule.Runtime.Client.Scripts.InputServices;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

namespace MainHub.Modules.WelwiseCharacterModule.Runtime.Client.Scripts.PlayerComponents
{
    public class CursorController
    {
        private readonly List<Func<bool>> _canSwitchCursorFuncs = new List<Func<bool>>();
        private readonly IShouldSwitchCursorProvider _shouldSwitchCursorProvider;

        public CursorController(IShouldSwitchCursorProvider shouldSwitchCursorProvider, MonoBehaviourObserver monoBehaviourObserver)
        {
            _shouldSwitchCursorProvider = shouldSwitchCursorProvider;

            monoBehaviourObserver.Updated += TrySwitchingCursor;
        }

        public void AddCanSwitchCursorFunc(Func<bool> func) => _canSwitchCursorFuncs.Add(func);
        
        private void TrySwitchingCursor()
        {
            if (_canSwitchCursorFuncs.All(@func => @func.Invoke()) && _shouldSwitchCursorProvider.ShouldSwitchCursor())
                CursorSwitchTools.TrySwitchingCursor();
        }
    }
}