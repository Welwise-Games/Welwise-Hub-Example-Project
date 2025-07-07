using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public interface IPoolableObject<T> where T : MonoBehaviour
    {
        void Construct(ObjectPool<T> pool);
    }
}