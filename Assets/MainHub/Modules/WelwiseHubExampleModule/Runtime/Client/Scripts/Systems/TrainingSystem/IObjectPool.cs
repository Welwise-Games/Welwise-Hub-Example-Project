using System.Collections.Generic;
using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public interface IObjectPool<T> where T : MonoBehaviour
    {
        IReadOnlyList<T> GotObjects { get; }
        public T GetProcessed(Vector3 position = default, Quaternion rotation = default, Transform parent = null, T @object = null);
        public T GetUnprocessed();
        public void TryRelease(T releasingObject);
        void TryReleaseRange(IEnumerable<T> objects);
        void ReleaseAllGotObjects();
    }
}