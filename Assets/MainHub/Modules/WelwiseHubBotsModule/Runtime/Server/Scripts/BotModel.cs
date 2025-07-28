using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Random = UnityEngine.Random;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotModel
    {
        public InterestPointGroup TargetInterestPointGroup => _lastInterestPointsGroups.LastOrDefault();
        public InterestPointGroup PastTargetInterestPointGroup => _lastInterestPointsGroups.SkipLast(1).LastOrDefault();
        
        public event Action<InterestPointGroup> UpdatedInterestPointGroup;
        public event Action EndedPlayingEmotionTimer;

        private readonly List<InterestPointGroup> _lastInterestPointsGroups = new List<InterestPointGroup>();
        private readonly BotsConfig _botsConfig;
        private readonly Timer _changingInterestPointGroupTimer, _playingEmotionTimer;

        public BotModel(BotsConfig botsConfig, Timer changingInterestPointGroupTimer, Timer playingEmotionTimer)
        {
            _botsConfig = botsConfig;
            _changingInterestPointGroupTimer = changingInterestPointGroupTimer;
            _playingEmotionTimer = playingEmotionTimer;

            _changingInterestPointGroupTimer.Ended += UpdateInterestPointGroup;
            _playingEmotionTimer.Ended += () => EndedPlayingEmotionTimer?.Invoke();

            TryPlayingEmotionTimer();
            UpdateInterestPointGroup();
        }

        public void TryPlayingEmotionTimer() => _playingEmotionTimer.TryStartingCountingTime(Random.Range(
            _botsConfig.MinimalEmotionAnimationWaitingTime,
            _botsConfig.MaximumEmotionAnimationWaitingTime));

        public void UpdateInterestPointGroup()
        {
            var interestPoints = CollectionTools.ParseEnumToList<InterestPointGroup>();
            
            var availableInterestPoints = MustSelectNotLastInterestPoint()
                ? interestPoints.Where(point => point != _lastInterestPointsGroups.Last()).ToList()
                : interestPoints;

            var newInterestPointGroup = availableInterestPoints.GetRandomOrDefault();
            AddInterestPointAndClearDeprecated(newInterestPointGroup);

            UpdatedInterestPointGroup?.Invoke(newInterestPointGroup);
        }

        public bool ShouldInteractWithLastInterestPoint() =>
            (_botsConfig
                .ChanceInteractWithInterestPointConfigs.FirstOrDefault(config => config.Group ==
                    TargetInterestPointGroup)?.Chance ?? 101).UseAsChanceAndGetResult();

        public void StartChangingInterestPointTimer()
        {
            _changingInterestPointGroupTimer.TryStartingCountingTime(Random.Range(_botsConfig.MinimalInterestPointChangingTime,
                _botsConfig.MaximumInterestPointChangingTime));
        }

        private void AddInterestPointAndClearDeprecated(InterestPointGroup interestPointGroup)
        {
            _lastInterestPointsGroups.Add(interestPointGroup);

            if (_lastInterestPointsGroups.Count >= _botsConfig.MaxUniformInterestPointGroupsInRow + 1)
                _lastInterestPointsGroups.Remove(0);
        }

        private bool MustSelectNotLastInterestPoint() =>
            _lastInterestPointsGroups.Count >= _botsConfig.MaxUniformInterestPointGroupsInRow &&
            _lastInterestPointsGroups.GroupBy(group => group).Count().Equals(1);
    }
}