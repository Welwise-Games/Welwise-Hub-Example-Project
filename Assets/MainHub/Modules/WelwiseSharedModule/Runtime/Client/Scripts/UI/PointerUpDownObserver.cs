using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WelwiseSharedModule.Runtime.Client.Scripts.UI
{
    public class PointerUpDownObserver: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<PointerEventData> PointerUpped, PointerDowned;
        public event Action PointerUppedWithoutArgs, PointerDownedWithoutArgs;
        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUpped?.Invoke(eventData);
            PointerUppedWithoutArgs?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDowned?.Invoke(eventData);
            PointerDownedWithoutArgs?.Invoke();
        }
    }
}