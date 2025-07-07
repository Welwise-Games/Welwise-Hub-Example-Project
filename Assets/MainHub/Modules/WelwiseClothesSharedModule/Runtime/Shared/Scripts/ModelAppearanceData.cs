using System;

namespace WelwiseClothesSharedModule.Runtime.Shared.Scripts
{
    [Serializable]
    public class ModelAppearanceData
    {
        public float DefaultClothesEmissionColor { get; set; }

        public float SkinColor { get; set; }

        public ModelAppearanceData() { }
        
        public ModelAppearanceData(float defaultClothesEmissionColor, float skinColor)
        {
            DefaultClothesEmissionColor = defaultClothesEmissionColor;
            SkinColor = skinColor;
        }
    }
}