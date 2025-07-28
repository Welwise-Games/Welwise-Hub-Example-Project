using TMPro;
using UnityEngine;
using WelwiseMiniGamesModule.Runtime.Shared.Scripts;

namespace WelwiseMiniGamesModule.Runtime.FlappyBird.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private int score;
        private Pipes[] pipes;

        [Header("Player")]
        public GameObject Player;

        public GotRewardProvider GotRewardProvider;
    

        [Header("Overlay")]
        public GameObject scoreObject;
        private TextMeshProUGUI scoreText;

        void Awake()
        {
            scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        
            Player.SetActive(false);
            ResetScore();
            ReplayGame();
        }

        public void ReplayGame()
        {
            ResetScore();
            scoreObject.SetActive(true);
            Player.SetActive(true);
            Player.transform.position = new Vector3(0f,0f,1f);

            pipes = FindObjectsOfType<Pipes>();

            for (int i=0; i < pipes.Length; i++)
            {
                Destroy(pipes[i].gameObject);
            }
        }

        public void IncreaseScore()
        {
            score++;
            GotRewardProvider.InvokeGot();
            scoreText.text = score.ToString();
        }

        public void ResetScore()
        {
            score = 0;
            scoreText.text = score.ToString();
        }
    }
}
