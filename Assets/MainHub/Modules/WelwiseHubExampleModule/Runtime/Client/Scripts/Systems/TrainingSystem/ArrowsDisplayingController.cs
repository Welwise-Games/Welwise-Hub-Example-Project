using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public class ArrowsDisplayingController
    {
        private readonly IObjectPool<ArrowView> _pool;
        private readonly ArrowsDisplayingConfig _config;
        private readonly Transform _arrowsParent;
        private readonly IArrowsPathProvider _arrowsPathProvider;

        private Vector3 _arrowsLocalPosition;

        public ArrowsDisplayingController(Transform arrowsParent,
            IObjectPool<ArrowView> pool, ArrowsDisplayingConfig config,
            IArrowsPathProvider arrowsPathProvider)
        {
            _arrowsParent = arrowsParent;
            _pool = pool;
            _config = config;
            _arrowsPathProvider = arrowsPathProvider;
        }

        public void ReleaseAllArrows() => _pool.ReleaseAllGotObjects();

        public void UpdateArrows()
        {
            var path = _arrowsPathProvider.GetPath();

            float totalPathLength = 0;
            for (var i = 0; i < path.Length - 1; i++)
                totalPathLength += Vector3.Distance(path[i], path[i + 1]);

            var arrowsNumber = 1 + Mathf.FloorToInt(totalPathLength / _config.DistanceBetweenArrows);

            if (arrowsNumber <= 0)
                return;

            var distanceBetweenStartAndCurrentPathPoints = 0f;
            var currentPathPointIndex = 0;

            _pool.ReleaseAllGotObjects();

            _arrowsLocalPosition = new Vector3(_arrowsLocalPosition.x, _arrowsLocalPosition.y,
                (_arrowsLocalPosition.z + _config.ArrowsMovementSpeed * Time.unscaledDeltaTime) %
                _config.ArrowsMovementAmplitude);

            for (var i = 1; i < arrowsNumber; i++)
            {
                var distanceBetweenStartPathPointAndCurrentArrow = _config.DistanceBetweenArrows * i;

                while (currentPathPointIndex < path.Length - 1)
                {
                    var distanceBetweenCurrentAndNextPoints =
                        Vector3.Distance(path[currentPathPointIndex], path[currentPathPointIndex + 1]);

                    if (distanceBetweenStartAndCurrentPathPoints + distanceBetweenCurrentAndNextPoints >=
                        distanceBetweenStartPathPointAndCurrentArrow)
                    {
                        var distanceBetweenCurrentPathPointAndCurrentArrow =
                            distanceBetweenStartPathPointAndCurrentArrow - distanceBetweenStartAndCurrentPathPoints;
                        var pathPartT = distanceBetweenCurrentPathPointAndCurrentArrow /
                                        distanceBetweenCurrentAndNextPoints;
                        var arrowPosition = Vector3.Lerp(path[currentPathPointIndex], path[currentPathPointIndex + 1],
                            pathPartT) + _config.ArrowsOffsetY * Vector3.up;

                        var arrowDirection = (path[currentPathPointIndex + 1] - path[currentPathPointIndex]).normalized;

                        var arrowInstance = _pool.GetProcessed(parent: _arrowsParent);

                        arrowInstance.transform.position = arrowPosition;
                        arrowInstance.ArrowModelGameObject.transform.localPosition = _arrowsLocalPosition;
                        arrowInstance.transform.forward = arrowDirection;
                        break;
                    }

                    distanceBetweenStartAndCurrentPathPoints += distanceBetweenCurrentAndNextPoints;
                    currentPathPointIndex++;
                }
            }
        }
    }
}