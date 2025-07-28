using TMPro;
using UnityEngine;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Tools;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Game
{
    public class ScoreUI : MonoBehaviour
    {
        public TMP_Text scoreText;
        public ScoreManager scoreManager;

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
        
        private void Update()
        {
            UpdateTextScore();
        }
    
        private void UpdateTextScore()
        {
            scoreText.text = $"{scoreManager.playerScore : 0}";
        }
    }
}
