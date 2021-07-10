using System;
using ScoreRequirement.Configuration;
using UnityEngine;
using Zenject;
using SiraUtil.Services;

namespace ScoreRequirement.Managers
{
    public class AccuracyCheckManager : MonoBehaviour, IInitializable
    {
        private AudioTimeSyncController _audioTimeSyncController;
        private RelativeScoreAndImmediateRankCounter _relativeScoreAndImmediateRankCounter;
        private PluginConfig _config;
        private Submission _submission;


        [Inject]
        public void Construct(AudioTimeSyncController audioTimeSyncController, RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRankCounter, PluginConfig config, Submission submission)
        {
            _config = config;
            _audioTimeSyncController = audioTimeSyncController;
            _relativeScoreAndImmediateRankCounter = relativeScoreAndImmediateRankCounter;
            _submission = submission;
        }

        public void Initialize()
        {
            
        }

        public void Update()
        {
            if (this._audioTimeSyncController.songTime >= this._audioTimeSyncController.songEndTime - 0.3f) return;
            if (_relativeScoreAndImmediateRankCounter.relativeScore < _config.accRequirement) 
                _submission?.DisableScoreSubmission("ScoreRequirement", "Accuracy was too low");
           
        }
    }
}