using System.Collections.Generic;
using UnityEngine;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Platforms;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.ScriptableObjects;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Tools;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Game
{
    public class MapGenerator : MonoBehaviour
    {
        public Transform playerTransform;
        public ScoreManager scoreManager;

        public Transform platformParent;

        public Platform firstPlatform;
        public List<Platform> platforms;
        public List<Item> items;

        public float itemProbability = 0.1f;

        public float overheadDistance = 10f;
        public float maxDistance = 20f;

        private float _yGeneration;
        private float _lastPlatformY;

        private void Update()
        {
            UpdateMap();
        }

        private void UpdateMap()
        {
            var baseY = playerTransform.position.y;
            GeneratePartialMap(baseY, maxDistance);
        }

        private void GeneratePartialMap(float baseY, float maxY)
        {
            if (baseY + overheadDistance <= _yGeneration) return;

            _yGeneration = baseY + overheadDistance;

            var max = _yGeneration + maxY;
            GeneratePlatforms(max);
        }

        private void GeneratePlatforms(float maxY)
        {
            while (_lastPlatformY < maxY)
            {
                GeneratePlatform();
            }
        }

        private void SpawnFirstPlatform()
        {
            var platformPrefab = firstPlatform.prefab;
            Instantiate(platformPrefab, Vector3.zero, Quaternion.identity, platformParent);
        }

        private void GeneratePlatform()
        {
            var difficulty = scoreManager.playerScore / 1000;
            var platformObject = platforms.FindAll(p => p.difficulty <= difficulty).GetRandom();
        
            var platformPrefab = platformObject.prefab;
            var xRange = platformObject.xRange;
            var yRange = platformObject.yRange;
        
            var xRandom = xRange.Random();
            var yRandom = _lastPlatformY + yRange.Random();
        
            var pos = new Vector2(xRandom, yRandom);


            var platform = Instantiate(platformPrefab, pos, Quaternion.identity, platformParent);
            var platformScript = platform.GetComponent<BasePlatform>();

            platformScript.Init();

            var movingPlatform = platform.GetComponent<MovingPlatform>();
            var yPlatformRange = movingPlatform == null ? 0f : movingPlatform.YRange;
            
            _lastPlatformY = yRandom + yPlatformRange;
            
            if (Random.value <= itemProbability && platformObject.hasItem)
            {
                GenerateItem(platform.transform);
            }
        }

        private void GenerateItem(Transform platform)
        {
            var itemObject = items.GetRandom();
            var itemPrefab = itemObject.prefab;
            var xRange = itemObject.xRange;

            var xRandom = xRange.Random();
            var platformPos = platform.position;
            platformPos.x += xRandom;

            Instantiate(itemPrefab, platformPos, Quaternion.identity, platform);
        }

        public void DeletePlatforms()
        {
            foreach (Transform child in platformParent)
            {
                Destroy(child.gameObject);
            }   
        }

        public void ResetMap()
        {
            DeletePlatforms();

            SpawnFirstPlatform();
            
            _lastPlatformY = 0;
            _yGeneration = 0;

            GeneratePlatforms(overheadDistance);
        }
    
    }
}
