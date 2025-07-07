using UnityEngine.UI;

namespace WelwiseSharedModule.Runtime.Client.Scripts.UI
{
    public class RaycastTarget : Graphic
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }
    }
}