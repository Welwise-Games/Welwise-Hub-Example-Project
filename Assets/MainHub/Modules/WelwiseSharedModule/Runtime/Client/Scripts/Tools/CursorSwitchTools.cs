using UnityEngine;

namespace WelwiseSharedModule.Runtime.Client.Scripts.Tools
{
    public static class CursorSwitchTools
    {
        public static bool IsCursorEnabled = true;

        public static void TrySwitchingCursor()
        {
            if (DeviceDetectorTools.IsMobile())
                return;
            
            IsCursorEnabled = !IsCursorEnabled;
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public static void TryDisablingCursor()
        {
            if (DeviceDetectorTools.IsMobile())
                return;
            
            IsCursorEnabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void TryEnablingCursor()
        {
            if (DeviceDetectorTools.IsMobile())
                return;
            
            IsCursorEnabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}