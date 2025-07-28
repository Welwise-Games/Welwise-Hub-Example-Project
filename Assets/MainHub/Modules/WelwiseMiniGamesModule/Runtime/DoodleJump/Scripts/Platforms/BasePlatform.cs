using UnityEngine;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Game;
using WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Tools;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Platforms
{
    public class BasePlatform : MonoBehaviour
    {
        public float jumpForce;

        public virtual void Init()
        {
            
        }
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            Jump(other.collider);
        }

        protected virtual bool Jump(Collider2D other)
        {
            if (!other.CompareTag("PlayerFeet")) return false;
        
            var player = other.transform.root.GetComponentInChildren<Player>();
            if (player == null) return false;

            var canJump = player.CanJump;
            
            if(canJump) player.Jump(jumpForce);
            return canJump;
        }
    }
}
