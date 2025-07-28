using UnityEngine;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.ScriptableObjects;
using WelwiseMiniGamesModule.Runtime.Shared.Scripts;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Game
{
    public class ScoreManager : MonoBehaviour
    {
        public Transform player;
        public float playerScore;
        public float rewardByEveryYPoints = 100;
        public GotRewardProvider GotRewardProvider;

        private float _playerMaxY;
        private float _lastYPointWhenGetReward;
        private void Start()
        {
            ResetScore();
        }

        private void Update()
        {
            UpdateScore();
        }

        private void UpdateScore()
        {
            var playerPos = player.position;
            if (playerPos.y <= _playerMaxY) return;
        
            _playerMaxY = playerPos.y;
            
            playerScore = _playerMaxY * 10;
            
            if (Mathf.Abs(_lastYPointWhenGetReward - playerScore) >= rewardByEveryYPoints)
            {
                _lastYPointWhenGetReward += rewardByEveryYPoints;
                GotRewardProvider.InvokeGot();
            }

        }

        public void ResetScore()
        {
            _playerMaxY = 0;
            playerScore = 0f;
            _lastYPointWhenGetReward = 0;
        }
    }
}
