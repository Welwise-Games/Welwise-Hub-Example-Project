using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.BlockTower.Scripts
{
    public class Block : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [SerializeField] private float WinYpos = 1.5f;
        private LevelController _levelController;

        public void Construct(LevelController levelController)
        {
            _levelController = levelController;
        }
    
        private void OnCollisionEnter2D(Collision2D other) {
            _levelController.PlayBlockSound();
        
            if (transform.localPosition.y < WinYpos) return;
            _levelController.InvokeWon();
            _levelController.Restart();
        }
    }
}
