using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WelwiseMiniGamesModule.Runtime.Shared.Scripts;

namespace WelwiseMiniGamesModule.Runtime.Helicopter.Scripts
{
    public class HelicopterController : MonoBehaviour
    {
        [field: SerializeField] public GotRewardProvider GotRewardProvider { get; private set; }
        [field: SerializeField] public LevelScorePopup LevelScorePopup { get; private set; }
        [field: SerializeField] public Transform DropSpawnPoint { get; private set; }
        [field: SerializeField] public GameObject DropPrefab { get; private set; }
        [field: SerializeField] public Transform ObjectToMove { get; private set; }
        [field: SerializeField] public Transform ObjectToMoveSpawnLeftPos { get; private set; }
        [field: SerializeField] public Transform ObjectToMoveSpawnRightPos { get; private set; }
        [field: SerializeField] public Transform ObjectToRotate { get; private set; }
        [field: SerializeField] public float ForwardForce { get; private set; }
        [field: SerializeField] public float UpForce { get; private set; }
        [field: SerializeField] public float HelicopterSpeed { get; private set; } = 2;

        private float _startHelicopterSpeed;
        private bool _didSpawnOnLeft, _dropped;

        private List<GameObject> _packages = new List<GameObject>();

        void Start()
        {
            _startHelicopterSpeed = HelicopterSpeed;
            Reset();
            ObjectToMove.DOLocalMoveY(ObjectToMove.transform.localPosition.y + 0.25f, .5f).SetLoops(-1, LoopType.Yoyo);

            GotRewardProvider.Got += () =>
            {
                _packages.RemoveAll(package => !package);
                _packages.ForEach(Destroy);
                _packages.Clear();
            };
        }

        private void Update()
        {
            if (ObjectToMove.transform.position.x > ObjectToMoveSpawnRightPos.position.x  || ObjectToMove.transform.position.x < ObjectToMoveSpawnLeftPos.position.x)
                Reset();

            DropSpawnPoint.rotation = ObjectToRotate.rotation;

            ObjectToMove.Translate(HelicopterSpeed * Time.deltaTime * (_didSpawnOnLeft
                ? ObjectToMove.right
                : -ObjectToMove.right));

            if (!Input.GetMouseButtonDown(0) || _dropped) return;

            _dropped = true;
            ObjectToRotate.gameObject.SetActive(false);
            HelicopterSpeed *= 2;
            var package = Instantiate(DropPrefab, DropSpawnPoint.position, Quaternion.identity, transform.parent);
            _packages.RemoveAll(package => !package);
            _packages.Add(package);
            
            var packageRigidbody = package.GetComponent<Rigidbody2D>();

            packageRigidbody.bodyType = RigidbodyType2D.Dynamic;

            packageRigidbody.AddForce(
                package.transform.up * ForwardForce + package.transform.right * (UpForce * (_didSpawnOnLeft ? 1 : -1)),
                ForceMode2D.Impulse);
        }

        private void Reset()
        {
            HelicopterSpeed = _startHelicopterSpeed;
            _dropped = false;
            _didSpawnOnLeft = UnityEngine.Random.Range(0, 2) == 0;
            ObjectToRotate.gameObject.SetActive(true);
            if (_didSpawnOnLeft)
            {
                ObjectToMove.localPosition = ObjectToMoveSpawnLeftPos.localPosition;
                ObjectToMove.localScale = Vector3.one;
            }
            else
            {
                ObjectToMove.localPosition = ObjectToMoveSpawnRightPos.localPosition;
                ObjectToMove.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}