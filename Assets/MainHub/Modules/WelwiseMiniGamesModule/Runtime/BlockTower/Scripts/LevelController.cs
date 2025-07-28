using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwiseMiniGamesModule.Runtime.Shared.Scripts;
using Random = UnityEngine.Random;

namespace WelwiseMiniGamesModule.Runtime.BlockTower.Scripts
{
    public class LevelController : MonoBehaviour
    {
        public AudioSource AudioSourceFX;
        public Sprite[] BlockSprites;
        [SerializeField] private GotRewardProvider gotRewardProvider;
        [SerializeField] private Block blockPrefab;
        [SerializeField] private Transform blockHolder;

        private Block currentBlock = null;
        private Rigidbody2D currentRigidbody;

        private Vector2 blockStartPosition = new Vector2(0f, 4f);


        private float startBlockSpeed = 8f;
        private float blockSpeed = 8f;
        private float blockSpeedIncrement = 0.5f;
        private int blockDirection = 1;
        private float xLimit = 5;
        private float timeBetweenRounds = 1f;

        private List<GameObject> _blocks = new List<GameObject>();

        // Variables to handle the game state.
        private int startingLives = 3;

        public AudioClip BlockClip;

        public void PlayBlockSound()
        {
            AudioSourceFX.PlayOneShot(BlockClip);
        }

        public void InvokeWon() => gotRewardProvider.InvokeGot();

        // Start is called before the first frame update
        void Start()
        {
            AudioSourceFX.volume = PlayerPrefs.GetInt("FXMusic");
            Restart();
            SpawnNewBlock();
        }

        public void Restart()
        {
            _blocks.RemoveAll(block => !block);
            _blocks.ForEach(Destroy);
            _blocks.Clear();
            blockSpeed = startBlockSpeed;
        }

        private void SpawnNewBlock()
        {
            // Create a block with the desired properties.
            currentBlock = Instantiate(blockPrefab, blockHolder);
            _blocks.RemoveAll(block => !block);
            _blocks.Add(currentBlock.gameObject);
            currentBlock.Construct(this);
            currentBlock.transform.position = blockStartPosition;
            int rnd = Random.Range(0, BlockSprites.Length);
            currentBlock.GetComponentInChildren<SpriteRenderer>().sprite = BlockSprites[rnd];
            currentRigidbody = currentBlock.GetComponent<Rigidbody2D>();
            // Increase the block speed each time to make it harder.
            blockSpeed += blockSpeedIncrement;
        }

        private async void DelayedSpawn()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeBetweenRounds), cancellationToken: destroyCancellationToken);
            SpawnNewBlock();
        }

        // Update is called once per frame
        void Update()
        {
            // If we have a waiting block, move it about.
            if (currentBlock)
            {
                float moveAmount = Time.deltaTime * blockSpeed * blockDirection;
                currentBlock.transform.position += new Vector3(moveAmount, 0, 0);
                // If we've gone as far as we want, reverse direction.
                if (Mathf.Abs(currentBlock.transform.position.x) > xLimit)
                {
                    // Set it to the limit so it doesn't go further.
                    currentBlock.transform.position =
                        new Vector3(blockDirection * xLimit, currentBlock.transform.position.y, 0);
                    blockDirection = -blockDirection;
                }

                // If we press space drop the block.
                if (Input.GetMouseButtonDown(0))
                {
                    // Stop it moving.
                    currentBlock = null;
                    // Activate the RigidBody to enable gravity to drop it.
                    currentRigidbody.simulated = true;
                    // Spawn the next block.
                    DelayedSpawn();
                }
            }
        }
    }
}