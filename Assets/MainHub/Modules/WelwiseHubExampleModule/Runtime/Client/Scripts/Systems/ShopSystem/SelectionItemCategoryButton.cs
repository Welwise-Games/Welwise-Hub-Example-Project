using UnityEngine;
using UnityEngine.UI;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class SelectionItemCategoryButton : MonoBehaviour
    {
        [field: SerializeField] public ItemCategory ItemCategory { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }
    }
}