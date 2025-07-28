using UnityEngine;

namespace WelwiseMiniGamesModule.Runtime.FlappyBird.Scripts
{
    public class Player : MonoBehaviour
    {
        private Vector3 direction;
        public float gravity = -9.8f;
        public int strength = 5;

        private SpriteRenderer spriteRenderer;
        public Sprite[] spriteArr;
        private int spriteIndex;

        public void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                direction = Vector3.up * strength;
            }
            direction.y += gravity * Time.deltaTime;
            transform.position += direction * Time.deltaTime;
        }

        public void Start()
        {
            InvokeRepeating(nameof(ChangeRenderSprite), 0.15f, 0.15f);
        }

        public void ChangeRenderSprite()
        {
            spriteIndex++;

            if (spriteIndex >= spriteArr.Length)
            {
                spriteIndex = 0;
            }
            if (direction.y > 0)
            {
                spriteRenderer.sprite = spriteArr[spriteIndex];
            }
            else
            {
                spriteRenderer.sprite = spriteArr[0];
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Obstacle"))
            {
                FindObjectOfType<GameManager>().ReplayGame();
            }
            else if (other.gameObject.CompareTag("ScoreZone"))
            {
                FindObjectOfType<GameManager>().IncreaseScore();
            }
        }
    }
}
