using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.Helicopter.Scripts
{
    public class TriggerStayDetector : MonoBehaviour
    {
        [SerializeField] private LevelScorePopup _levelScorePopup;
    
        void OnTriggerEnter2D(Collider2D other) => _levelScorePopup.IncreaseScore();

        void OnTriggerExit2D(Collider2D other) => _levelScorePopup.DecreaseScore();
    }
}
