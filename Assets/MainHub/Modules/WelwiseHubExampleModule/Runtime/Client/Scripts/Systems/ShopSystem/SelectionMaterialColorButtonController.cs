using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.ShopSystem
{
    public class SelectionMaterialColorButtonController
    {
        private bool _isSelected;

        private readonly SelectionMaterialColorButtonView _selectionMaterialColorButtonView;

        public SelectionMaterialColorButtonController(SelectionMaterialColorButtonView selectionMaterialColorButtonView)
        {
            _selectionMaterialColorButtonView = selectionMaterialColorButtonView;
        }

        public void ChangeSelectionModeAndUpdateView(bool isSelected, Color color)
        {
            _selectionMaterialColorButtonView.ColorImage.color = color;
            _selectionMaterialColorButtonView.SelectedImage.gameObject.SetActive(isSelected);
        }
    }
}