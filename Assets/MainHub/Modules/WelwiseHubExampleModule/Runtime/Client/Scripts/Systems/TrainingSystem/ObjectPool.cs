using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Object = UnityEngine.Object;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public class ObjectPool<T> : IObjectPool<T> where T : MonoBehaviour
    {
        public IReadOnlyList<T> GotObjects => _gotObjects;

        private readonly HashSet<T> _releasedObjects = new HashSet<T>();
        private readonly List<T> _gotObjects = new List<T>();

        private readonly T _prefab;
        private readonly Action<T> _objectCreated;
        private readonly Action<T> _objectGot;
        private readonly Action<T> _objectReleased;
        private readonly Action _poolInitialized;
        private readonly int _size;
        private Transform _parent;

        public ObjectPool(string parentName, T prefab, Action<T> objectCreated = null,
            Action<T> objectGot = null, Action<T> objectReleased = null, Action poolInitialized = null,
            int size = 100)
        {
            _prefab = prefab;
            _objectCreated = objectCreated;
            _objectGot = objectGot;
            _objectReleased = objectReleased;
            _poolInitialized = poolInitialized;
            _size = size;
            CreateParent(parentName);
            InitializePool();
        }

        public ObjectPool(Transform parent, T prefab, Action<T> objectCreated = null,
            Action<T> objectGot = null, Action<T> objectReleased = null, Action poolInitialized = null,
            int size = 100)
        {
            _prefab = prefab;
            _objectCreated = objectCreated;
            _objectGot = objectGot;
            _objectReleased = objectReleased;
            _poolInitialized = poolInitialized;
            _size = size;
            _parent = parent;

            InitializePool();
        }

        private void InitializePool()
        {
            for (var i = 0; i < _size; i++)
                _releasedObjects.Add(InstantiateObject());

            _poolInitialized?.Invoke();
        }

        public T GetUnprocessed()
        {
            var pooledObject = Dequeue();

            if (pooledObject == null)
                return null;

            pooledObject.gameObject.SetActive(true);
            OnGetCallback(pooledObject);

            return pooledObject;
        }

        public T GetProcessed(Vector3 position = default, Quaternion rotation = default, Transform parent = null,
            T @object = null)
        {
            if (position == default) position = Vector3.zero;
            if (rotation == default) rotation = Quaternion.identity;

            var pooledObject = Dequeue(@object);

            if (pooledObject == null)
                return null;

            var transform = pooledObject.transform;

            transform.position = position;
            transform.rotation = rotation;

            transform.SetParent(parent);

            pooledObject.gameObject.SetActive(true);
            OnGetCallback(pooledObject);

            return pooledObject;
        }

        private T Dequeue(T @object = null)
        {
            if (@object && _releasedObjects.Remove(@object))
                return @object;

            if (_releasedObjects.Count > 0)
            {
                var result = _releasedObjects.First();
                _releasedObjects.Remove(result);
                return result;
            }

            return InstantiateObject();
        }

        public void ReleaseAllGotObjects()
        {
            for (var i = _gotObjects.Count - 1; i >= 0; i--)
                TryRelease(_gotObjects[i]);
        }

        public void TryReleaseRange(IEnumerable<T> objects) => objects.ForEach(TryRelease);

        public void TryRelease(T obj)
        {
            if (_releasedObjects.Contains(obj))
                return;

            _gotObjects.Remove(obj);
            _releasedObjects.Add(obj);
            obj.transform.SetParent(_parent);

            obj.gameObject.SetActive(false);
            OnReleaseCallback(obj);
        }

        private void CreateParent(string parentName) => _parent = new GameObject(parentName + " Parent").transform;

        private T InstantiateObject()
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            OnCreateCallback(instance);
            return instance;
        }

        private void OnCreateCallback(T createdObject) => _objectCreated?.Invoke(createdObject);

        private void OnGetCallback(T givenObject)
        {
            (givenObject as IPoolableObject<T>)?.Construct(this);

            _objectGot?.Invoke(givenObject);
            _gotObjects.Add(givenObject);
        }

        private void OnReleaseCallback(T releasedObject)
        {
            _objectReleased?.Invoke(releasedObject);
        }
    }
}