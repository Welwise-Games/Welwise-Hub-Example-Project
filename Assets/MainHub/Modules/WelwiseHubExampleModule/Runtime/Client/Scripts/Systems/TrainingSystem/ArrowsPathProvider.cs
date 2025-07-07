using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public class ArrowsPathProvider : IArrowsPathProvider
    {
        private readonly Transform _playerTransform;
        private readonly Transform _goalTransform;

        public ArrowsPathProvider(Transform playerTransform, Transform goalTransform)
        {
            _playerTransform = playerTransform;
            _goalTransform = goalTransform;
        }

        public Vector3[] GetPath() => new Vector3[] { _playerTransform.position, _goalTransform.position };
    }
}