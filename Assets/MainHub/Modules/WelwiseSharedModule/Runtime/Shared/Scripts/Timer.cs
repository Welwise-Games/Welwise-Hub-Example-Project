using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Shared.Scripts
{
    public class Timer
    {
        public float AppointedTime => _appointedTime;
        public float RemainingTime => _remainingTime;
        public float PastTime => AppointedTime - RemainingTime;
        public bool IsEnded => AppointedTime > 0 && RemainingTime == 0;
        public bool IsPause { get; private set; }

        #region Events

        public event Action InterruptedIncompleted, Ended;
        public event Action<float> Updated, Ticked, UpdatedOnStartNextSecond, Started, ChangedAppointedTime;

        #endregion

        private float _remainingTime, _appointedTime;
        private bool _isCountDown, _interruptedIncompleteInvokedEarly, _endedInvokedEarly;
        private int _lastSecondUpdateTime;

        private readonly CancellationToken _destroyCancellationToken;
        private CancellationTokenSource _sharedTokenSource;


        public Timer(CancellationToken destroyCancellationToken = default)
        {
            Subscribe();
            _destroyCancellationToken = destroyCancellationToken;
            _sharedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_destroyCancellationToken);
        }

        public void ClearEnded() => Ended = null;

        #region Start

        public static async UniTask TryStartingCountingTime(float time, Action ended, bool isTimeScaled = false,
            CancellationToken token = default)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time), isTimeScaled, PlayerLoopTiming.Update, token);
            ended?.Invoke();
        }

        public void Configure(float appointedTime, float remainingTime)
        {
            TryUpdatingAppointedTime(appointedTime);
            TryUpdatingRemainingTime(remainingTime);
        }

        public void ConfigureAndTryStartingCountingTime(float appointedTime, float remainingTime)
        {
            Configure(appointedTime, remainingTime);
            TryStartingCountingTime(appointedTime, false);
        }

        public void TryStartingCountingTime(float appointedTime, bool shouldStartFromBeginning = true, bool isScaled = false)
        {
            if (_isCountDown)
                return;

            if (IsPause)
            {
                TryUpdatingAppointedTime(appointedTime);
                SetPauseState(false);
                return;
            }

            TryUpdatingAppointedTime(appointedTime, shouldStartFromBeginning);
            TryCountingDownAsync(isScaled: isScaled, shouldStartFromBeginning: shouldStartFromBeginning).Forget();
        }

        public void TryStartingCountingTime(bool shouldStartFromBeginning = true, bool isScaled = false) 
            => TryStartingCountingTime(_appointedTime, shouldStartFromBeginning, isScaled);

        #endregion

        #region Set

        public void Reset() => _remainingTime = _appointedTime;

        public void TryUpdatingAppointedTime(float appointedTime, bool shouldStartFromBeginning = true)
        {
            appointedTime = Mathf.Max(0, appointedTime);
            
            if (ShouldStopCountingImmediately(appointedTime))
            {
                TryStopingCountingImmediately();
                return;
            }

            _appointedTime = appointedTime;
            TryAppointAppointedTimeForRemainingTime(shouldStartFromBeginning);
            ChangedAppointedTime?.Invoke(_appointedTime);
        }

        public void TryUpdatingRemainingTime(float time)
        {
            time = Mathf.Max(0, time);
            
            if (time > _appointedTime) return;

            _remainingTime = time;
                
            if (ShouldStopCountingImmediately(time))
                TryStopingCountingImmediately();
        }

        public void Add(float time) =>
            _appointedTime += time;

        public void IncreaseRemainingTime(float addTime) =>
            _remainingTime += addTime;

        #endregion

        #region Pause

        public void Pause() => SetPauseState(true);
        private void MakeIsPauseFalse() => IsPause = false;

        private void SetPauseState(bool isPause) => IsPause = isPause;

        #endregion
        
        #region Stop

        public bool IsCounting() => _isCountDown;
        public void TryStoppingCountingTime()
        {
            if (!_isCountDown)
                return;

            CancelAndRecreateSharedTokenSource();
            _interruptedIncompleteInvokedEarly = true;
            _isCountDown = false;
            InterruptedIncompleted?.Invoke();
        }

        private void CancelAndRecreateSharedTokenSource()
        {
            _sharedTokenSource.Cancel();
            _sharedTokenSource.Dispose();
            _sharedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_destroyCancellationToken);
        }

        private void TryStopingCountingImmediately()
        {
            if (_isCountDown)
                CancelAndRecreateSharedTokenSource();
        }

        #endregion

        private async UniTaskVoid TryCountingDownAsync(bool isScaled, bool shouldStartFromBeginning = true)
        {
            if (_isCountDown)
                return;
            
            TryAppointAppointedTimeForRemainingTime(shouldStartFromBeginning);

            _isCountDown = true;

            Started?.Invoke(_remainingTime);
            Updated?.Invoke(_remainingTime);
            UpdatedOnStartNextSecond?.Invoke(_remainingTime);
            _interruptedIncompleteInvokedEarly = false;
            _endedInvokedEarly = false;
            _lastSecondUpdateTime = -1;

            while (_remainingTime > 0)
            {
                await UniTask.WaitForFixedUpdate(_sharedTokenSource.Token);
                
                if (_remainingTime == 0)
                    break;
                
                if (_sharedTokenSource.IsCancellationRequested || !_isCountDown)
                {
                    if (_interruptedIncompleteInvokedEarly)
                        return;

                    InterruptedIncompleted?.Invoke();
                    _interruptedIncompleteInvokedEarly = false;
                    return;
                }

                if (IsPause) continue;

                var deltaTime = isScaled ? Time.fixedDeltaTime : Time.fixedUnscaledDeltaTime;
                _remainingTime = Mathf.Max(_remainingTime - deltaTime, 0);

                if (_lastSecondUpdateTime != (int) _remainingTime)
                {
                    UpdatedOnStartNextSecond?.Invoke(_remainingTime);
                    _lastSecondUpdateTime = (int) _remainingTime;
                }

                Updated?.Invoke(_remainingTime);
                Ticked?.Invoke(_remainingTime);
            }

            if (_endedInvokedEarly)
                return;
            
            Ended?.Invoke();
        }

        private void Subscribe()
        {
            Ended += MakeIsPauseFalse;
            Ended += OnEnd;
            InterruptedIncompleted += MakeIsPauseFalse;
        }
        
        private void TryAppointAppointedTimeForRemainingTime(bool shouldStartFromBeginning = true)
        {
            if (shouldStartFromBeginning || _remainingTime > _appointedTime || _remainingTime <= float.Epsilon)
                _remainingTime = _appointedTime;
        }

        private bool ShouldStopCountingImmediately(float appointedOrRemainingTime) 
            => appointedOrRemainingTime <= float.Epsilon && _isCountDown;

        private void OnEnd()
        {
            _remainingTime = 0;
            _isCountDown = false;
            _endedInvokedEarly = true;
            UpdatedOnStartNextSecond?.Invoke(0);
            Updated?.Invoke(0);
        }
    }
}