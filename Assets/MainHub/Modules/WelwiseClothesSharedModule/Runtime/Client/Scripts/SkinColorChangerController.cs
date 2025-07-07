using UnityEngine;
using WelwiseClothesSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseClothesSharedModule.Runtime.Client.Scripts
{
    public class SkinColorChangerController
    {
        private readonly SkinColorChangerSerializableComponents _serializableComponents;
        
        private static readonly int _materialCurrentDistancePropertyId = Shader.PropertyToID("_CurrentDistance");
        private static readonly int _materialEmissionColorPropertyId = Shader.PropertyToID("_EmissionHue");
        private static readonly int _materialColorPropertyId = Shader.PropertyToID("_Hue");

        public SkinColorChangerController(SkinColorChangerSerializableComponents serializableComponents, ModelAppearanceData modelAppearanceData)
        {
            _serializableComponents = serializableComponents;
            ApplyMaterialToAllRenderers(serializableComponents.BodyMaterial, serializableComponents.ArmsMaterial);
            
            SetSkinColor(modelAppearanceData.SkinColor);
            SetDefaultClothesEmissionColor(modelAppearanceData.DefaultClothesEmissionColor);
        }

        public void SetDefaultClothesEmissionColorAndSkinColor(float color, float emissionColor)
        {
            SetSkinColor(color);
            SetDefaultClothesEmissionColor(emissionColor);
        }
        
        private static float GetSkinColorValue(float colorValue) => (colorValue + 0.45f) % 1f;
        public void SetSkinColor(float colorValue) => SetColor(GetSkinColorValue(colorValue), false);
        public void SetDefaultClothesEmissionColor(float emissionColor) => SetColor(emissionColor, true);

        public void SetBodyEnabledMode(bool isFirstCamera) 
            => _serializableComponents.BodyRenderers.ForEach(renderer => renderer.enabled = !isFirstCamera);

        public void SetArmsEnabledMode(bool isFirstCamera) 
            => _serializableComponents.ArmsRenderers.ForEach(renderer => renderer.enabled = isFirstCamera);

        public void SetSkinFade(float distance) 
            => _serializableComponents.BodyRenderers.ForEach(renderer => renderer.material.SetFloat(_materialCurrentDistancePropertyId, distance));

        private void ApplyMaterialToAllRenderers(Material bodyMaterial, Material armsMaterial)
        {
            foreach (var renderer in _serializableComponents.ArmsRenderers)
            {
                var newMaterial = Object.Instantiate(armsMaterial);
                renderer.material = newMaterial;
                renderer.enabled = false;
            }

            foreach (var renderer in _serializableComponents.BodyRenderers)
            {
                var newMaterial = Object.Instantiate(bodyMaterial);
                renderer.material = newMaterial;
            }
        }

        private void SetColor(float color, bool isDefaultClothesEmissionColor)
        {
            var propertyID = isDefaultClothesEmissionColor ? _materialEmissionColorPropertyId : _materialColorPropertyId;
            
            _serializableComponents.ArmsRenderers.ForEach(renderer => renderer.material.SetFloat(propertyID, color));
            _serializableComponents.BodyRenderers.ForEach(renderer => renderer.material.SetFloat(propertyID, color));
        }
    }
}