using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseSharedModule.Runtime.Client.Scripts.UI;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.UI
{
    public class UIRootComponents
    {
        public readonly UIRootSerializableComponents SerializableComponents;
        public readonly ErrorTextController ErrorTextController;

        private readonly Dictionary<Transform, bool> _getWasEnableByChild = new Dictionary<Transform, bool>();
        
        public UIRootComponents(UIRootSerializableComponents serializableComponents, ErrorTextController errorTextController)
        {
            SerializableComponents = serializableComponents;
            ErrorTextController = errorTextController;
        }

        public void DisableAllChildrenExcept(GameObject exceptGameObject)
        {
            Enumerable.Range(0, SerializableComponents.transform.childCount).ForEach(i =>
            {
                var child = SerializableComponents.transform.GetChild(i);

                if (child.gameObject == exceptGameObject)
                    return;
                
                var popup = child.GetComponentInChildren<Popup>();
                
                _getWasEnableByChild.AddOrAppoint(child, popup ? popup.IsOpen : child.gameObject.activeSelf);

                if (!popup)
                    child.gameObject.SetActive(false);
                else
                    popup.TryClosing();
            });
        }

        public void EnableEarlyDisabledChildren()
        {
            _getWasEnableByChild.ForEach(pair =>
            {
                if (pair.Value)
                {
                    var popup = pair.Key.GetComponentInChildren<Popup>();
                    
                    if (!popup)
                        pair.Key.gameObject.SetActive(true);
                    else
                        popup.TryOpening();
                }
            });
            
            _getWasEnableByChild.Clear();
        }
    }
}