using System.Collections;
using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        public Player player;
    
        public CameraFollow cameraFollow;
        public ScoreManager scoreManager;
        public MapGenerator mapGenerator;

        public ScoreUI scoreUi;
        
        private Vector3 _basePlayerPosition;
        
        private void Start()
        {
            _basePlayerPosition = player.transform.position;
            
            player.deathEvent.AddListener(GameOver);
            
            Restart();
        }

        public void GameOver()
        {
            Restart();
        }
        
        public void Restart()
        {
            scoreUi.Open();
            
            player.transform.position = _basePlayerPosition;
            player.gameObject.SetActive(true);

            scoreManager.ResetScore();
            cameraFollow.ResetCamera();
            mapGenerator.ResetMap();
        }
    }
}
