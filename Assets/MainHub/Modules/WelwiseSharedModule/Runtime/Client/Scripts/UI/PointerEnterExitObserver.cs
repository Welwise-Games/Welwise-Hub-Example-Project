using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WelwiseSharedModule.Runtime.Client.Scripts.UI
{
    public class PointerEnterExitObserver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<PointerEventData> PointerEntered, PointerExited;
        public event Action EnteredWithoutArgs, ExitedWithoutArgs;
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEntered?.Invoke(eventData);
            EnteredWithoutArgs?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExited?.Invoke(eventData);
            ExitedWithoutArgs?.Invoke();
        }
    }
}