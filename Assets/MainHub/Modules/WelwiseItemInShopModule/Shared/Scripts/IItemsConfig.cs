namespace WelwiseItemInShopModule.Shared.Scripts
{
    public interface IItemsConfig<out TItemConfig> where TItemConfig : IIndexableItemConfig
    {
        int MaxSelectedItemsNumber { get; }
        TItemConfig[] Configs { get; }
    }
}