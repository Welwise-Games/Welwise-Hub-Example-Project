using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WelwiseSharedModule.Runtime.Client.Scripts.UI
{
    public class PointerDragObserver : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<PointerEventData> BeganDrag, Draged, EndedDrag;
        public event Action BeganDragWithoutArgs, DragedWithoutArgs, EndedDragWithoutArgs;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            BeganDrag?.Invoke(eventData);
            BeganDragWithoutArgs?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Draged?.Invoke(eventData);
            DragedWithoutArgs?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndedDrag?.Invoke(eventData);
            EndedDragWithoutArgs?.Invoke();
        }
    }
}