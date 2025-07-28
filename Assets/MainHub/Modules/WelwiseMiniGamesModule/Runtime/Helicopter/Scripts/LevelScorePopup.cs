using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using WelwiseMiniGamesModule.Runtime.Shared.Scripts;

namespace WelwiseMiniGamesModule.Runtime.Helicopter.Scripts
{
    public class LevelScorePopup : MonoBehaviour
    {
        public int MaxNeededScore;
        public float TimeBeforeWin = 3;
        public TMP_Text FiniteScoreText;

        [SerializeField] private GotRewardProvider gotRewardProvider;
        private int _score;

        public void DecreaseScore()
        {
            if (_score == 0)
                return;

            _score -= 1;
            FiniteScoreText.text = $"{_score} / {MaxNeededScore}";
        }

        public async void IncreaseScore()
        {
            _score += 1;
            UpdateFiniteScoreText();

            if (_score < MaxNeededScore) return;
            await UniTask.Delay(TimeSpan.FromSeconds(TimeBeforeWin), cancellationToken: destroyCancellationToken);
            if (_score < MaxNeededScore) return;
            _score = 0;
            UpdateFiniteScoreText();
            gotRewardProvider.InvokeGot();
        }

        void Start()
        {
            UpdateFiniteScoreText();
        }

        private void UpdateFiniteScoreText() => FiniteScoreText.text = $"{_score} / {MaxNeededScore}";
    }
}