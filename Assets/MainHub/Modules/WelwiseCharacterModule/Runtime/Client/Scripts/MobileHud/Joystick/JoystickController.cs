using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WelwiseCharacterModule.Runtime.Client.Scripts.MobileHud.Joystick
{
    public class JoystickController
    {
        public bool IsPointerDown { get; private set; } = false;
        public Vector2 InputAxis { get; private set; }  = Vector2.zero;
        
        private readonly JoystickSerializableComponents _serializableComponents;

        public JoystickController(JoystickSerializableComponents serializableComponents)
        {
            _serializableComponents = serializableComponents;
            
            InitializeJoystick();
            
            new JoystickHighlightersPartsController(serializableComponents.HighlightedPartsSerializableComponents, this);
            
            serializableComponents.MonoBehaviourObserver.Updated += OnUpdate;
            serializableComponents.PointerDragObserver.Draged += OnDrag;
            serializableComponents.PointerUpDownObserver.PointerUpped += OnPointerUp;
            serializableComponents.PointerUpDownObserver.PointerDowned += OnPointerDown;
        }

        private void OnUpdate()
        {
            if (!IsPointerDown && _serializableComponents.CanvasGroup.alpha > 0)
                _serializableComponents.CanvasGroup.alpha -= Time.deltaTime / _serializableComponents.JoystickConfig.FadeDuration;
        }

        private void InitializeJoystick()
        {
            _serializableComponents.JoystickBackground.gameObject.SetActive(!_serializableComponents.JoystickConfig
                .IsDynamic);

            if (!_serializableComponents.JoystickConfig.IsDynamic)
                _serializableComponents.GetComponent<Image>().raycastTarget = false;
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            IsPointerDown = true;
            _serializableComponents.CanvasGroup.alpha = 1f;

            if (_serializableComponents.JoystickConfig.IsDynamic)
            {
                _serializableComponents.JoystickBackground.position = eventData.position;
                _serializableComponents.JoystickBackground.gameObject.SetActive(true);
            }

            OnDrag(eventData);
        }

        private void OnDrag(PointerEventData eventData)
        {
            var direction = eventData.position - (Vector2) _serializableComponents.JoystickBackground.position;
            InputAxis = direction.magnitude > _serializableComponents.JoystickBackground.sizeDelta.x / 2f
                ? direction.normalized
                : direction / (_serializableComponents.JoystickBackground.sizeDelta.x / 2f);

            _serializableComponents.JoystickHandle.anchoredPosition =
                InputAxis * _serializableComponents.JoystickBackground.sizeDelta.x / 2f *
                _serializableComponents.JoystickConfig.HandleRange;
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            IsPointerDown = false;
            InputAxis = Vector2.zero;
            _serializableComponents.JoystickHandle.anchoredPosition = Vector2.zero;

            if (_serializableComponents.JoystickConfig.IsDynamic)
                _serializableComponents.JoystickBackground.gameObject.SetActive(false);
        }
    }
}