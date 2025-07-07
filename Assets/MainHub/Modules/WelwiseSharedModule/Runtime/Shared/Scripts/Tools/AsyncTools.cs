using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Tools
{
    public static class AsyncTools
    {
        public static async UniTask WaitUniTaskWithoutCancelledOperationException(UniTask func)
        {
            try
            {
                await func;
            }
            catch (Exception exception)
            {
                if (exception.IsOperationCanceledException())
                    return;
                
                throw;
            }
        }
        
        public static async UniTask WaitWhileWithoutSkippingFrame(Func<bool> func, CancellationToken cancellationToken = default)
        {
            var result = func.Invoke();

            if (!result)
                return;
            
            await UniTask.WaitWhile(func, cancellationToken: cancellationToken);
        }
    }
}