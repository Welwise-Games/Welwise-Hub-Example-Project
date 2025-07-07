using System;
using System.Collections.Generic;
using System.Linq;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using Random = UnityEngine.Random;

namespace WelwiseHubBotsModule.Runtime.Server.Scripts
{
    public class BotBehaviourModel
    {
        public InterestPointGroup TargetInterestPointGroup => _lastInterestPointsGroups.LastOrDefault();
        
        public event Action<InterestPointGroup> UpdatedInterestPointGroup;
        public event Action EndedPlayingEmotionTimer;

        private readonly List<InterestPointGroup> _lastInterestPointsGroups = new List<InterestPointGroup>();
        private readonly BotsConfig _botsConfig;
        private readonly Timer _changingInterestPointGroupTimer, _playingEmotionTimer;

        public BotBehaviourModel(BotsConfig botsConfig, Timer changingInterestPointGroupTimer, Timer playingEmotionTimer)
        {
            _botsConfig = botsConfig;
            _changingInterestPointGroupTimer = changingInterestPointGroupTimer;
            _playingEmotionTimer = playingEmotionTimer;

            _changingInterestPointGroupTimer.Ended += UpdateInterestPointGroup;
            _playingEmotionTimer.Ended += () => EndedPlayingEmotionTimer?.Invoke();

            TryPlayingEmotionTimer();
            UpdateInterestPointGroup();
        }

        public float GetSetDataPartChance() => _botsConfig.SetBotCustomizationDataPartChance;

        public void TryPlayingEmotionTimer() => _playingEmotionTimer.TryStartingCountingTime(Random.Range(
            _botsConfig.MinimalEmotionAnimationWaitingTime,
            _botsConfig.MaximumEmotionAnimationWaitingTime));

        public void UpdateInterestPointGroup()
        {
            var interestPoints = CollectionTools.ToList<InterestPointGroup>();
            
            var availableInterestPoints = MustSelectNotLastInterestPoint()
                ? interestPoints.Where(point => point != _lastInterestPointsGroups.Last()).ToList()
                : interestPoints;

            var newInterestPointGroup = availableInterestPoints[Random.Range(0, availableInterestPoints.Count)];
            AddInterestPointAndClearDeprecated(newInterestPointGroup);

            UpdatedInterestPointGroup?.Invoke(newInterestPointGroup);
        }

        public bool ShouldSetCustomizationDataPart() =>
            Random.Range(0f, 100) <= _botsConfig.SetBotCustomizationDataPartChance;

        public bool ShouldInteractWithLastInterestPoint() =>
            Random.Range(0f, 100f) <= (_botsConfig
                .ChanceInteractWithInterestPointConfigs.FirstOrDefault(config => config.Group ==
                    TargetInterestPointGroup)?.Chance ?? 101);

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