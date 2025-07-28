using WelwiseSharedModule.Runtime.Client.Scripts.UI;

namespace WelwiseItemInShopModule.Client.Scripts
{
    public interface IItemsViewConfig<out TItemViewConfig> where TItemViewConfig : IItemViewConfig
    {
        TItemViewConfig[] Configs { get; }
        ErrorTextConfig ErrorTextConfig { get; }
    }
}