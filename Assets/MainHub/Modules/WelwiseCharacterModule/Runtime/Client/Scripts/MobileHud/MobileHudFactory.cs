using Cysharp.Threading.Tasks;
using WelwiseSharedModule.Runtime.Shared.Scripts;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud
{
    public class MobileHudFactory
    {
        private readonly Container _container = new Container();

        public async UniTask DisposeUIAsync() => await _container.DestroyAndClearAllImplementationsAsync();
        
        public async UniTask<MobileHudController> GetCreatedMobileHudControllerAsync(MobileHudSerializableComponents mobileHudSerializableComponents) =>
            await _container.GetOrRegisterSingleByTypeAsync(async () =>
                new MobileHudController(mobileHudSerializableComponents));
    }
}