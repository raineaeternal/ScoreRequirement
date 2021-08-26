using System;
using IPA.Logging;
using ScoreRequirement.Configuration;
using SiraUtil.Services;
using Zenject;

namespace ScoreRequirement.Managers
{
    internal class SRManager : IInitializable, IDisposable
    {
        private readonly Logger _logger;
        private readonly PluginConfig _config;
        private readonly SongController _songController;
        private readonly IScoreController _iScoreController;
        private readonly PauseController _pauseController;
        private readonly IReadonlyBeatmapData _iReadonlyBeatmapData;
        private readonly Submission _submission;
        private AudioTimeSyncController _audioTimeSyncController;
        private readonly RelativeScoreAndImmediateRankCounter _relativeScoreAndImmediateRankCounter;

        private int _currentComboBreaks;
        private int _currentPauses;
        private int _currentMisses;

        public SRManager(Logger logger, PluginConfig config, SongController songController, IScoreController iScoreController, PauseController pauseController, IReadonlyBeatmapData iReadonlyBeatmapData, Submission submission, AudioTimeSyncController audioTimeSyncController, RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRankCounter)
        {
            _config = config;
            _logger = logger;
            _songController = songController;
            _iScoreController = iScoreController;
            _pauseController = pauseController;
            _iReadonlyBeatmapData = iReadonlyBeatmapData;
            _submission = submission;
            _audioTimeSyncController = audioTimeSyncController;
            _relativeScoreAndImmediateRankCounter = relativeScoreAndImmediateRankCounter;
            _submission = submission;
        }

        private void CheckMaxCombo()
        {
            if (!_config.isComboRequirementEnabled) return;
            if (!(_iScoreController.maxCombo > _config.minimumComboCount))
                    _submission?.DisableScoreSubmission("ScoreRequirement", "Combo was too low");
            _logger.Info($"{_iScoreController.maxCombo} of {_config.minimumComboCount} has been reached!");
        }

        private void ComboChanged(int combo)
        {
            if (combo == 0) _currentComboBreaks++;
        }

        private void CheckComboBreaks()
        {
            if (!_config.isComboBreakLimitEnabled) return;
            if (_currentComboBreaks > _config.comboBreakLimit)
                _submission?.DisableScoreSubmission("ScoreRequirement", "Too many combo breaks");
            _logger.Info($"Combo breaks: {_currentComboBreaks}");
        }

        private void DidPause()
        {
            _pauseController.HandleMenuButtonTriggered();
                _currentPauses++;
        }

        private void ScoreChanged()
        {
            if (!_config.isAccRequirementEnabled) return;
            if (_relativeScoreAndImmediateRankCounter.relativeScore < (float) _config.accRequirement / 100f)
                _submission.DisableScoreSubmission("ScoreRequirement", "Accuracy was too low");
            _logger.Info($"Current acc is: {_relativeScoreAndImmediateRankCounter.relativeScore} of the required {_config.accRequirement}"); 
        }

        private void CheckPauses()
        {
            if (!_config.isPauseLimitEnabled) return;
            if (_currentPauses > _config.pauseLimit) 
                _submission?.DisableScoreSubmission("ScoreRequirement", "Too many pauses");
            _logger.Info($"Pauses: {_currentPauses}");
        }

        private void NoteWasMissed(NoteData noteData, int multiplier)
        {
            if (noteData.colorType == ColorType.None) return;
            _currentMisses++;
        }

        private void CheckMisses()
        {
            if (!_config.isMissLimitEnabled) return;
            if (_currentMisses > _config.missLimit) 
                _submission?.DisableScoreSubmission("ScoreRequirement", "Too many misses");
            _logger.Info($"Misses: {_currentMisses}");
        }

        private void SongFinished()
        {
            CheckMaxCombo();
            CheckComboBreaks();
            CheckPauses();
            CheckMisses();
            ScoreChanged();
        }

        public void Initialize()
        {
            _currentComboBreaks = 0;
            _currentPauses = 0;
            _currentMisses = 0;
            
            if (_config.isAccRequirementEnabled) _songController.songDidFinishEvent += ScoreChanged;
            if (_config.isMissLimitEnabled) _iScoreController.noteWasMissedEvent += NoteWasMissed;
            if (_config.isComboBreakLimitEnabled) _iScoreController.comboDidChangeEvent += ComboChanged;
            if (_config.isPauseLimitEnabled) _pauseController.didPauseEvent += DidPause;
            if (_config.isSREnabled) _songController.songDidFinishEvent += SongFinished;
        }

        public void Dispose()
        {
            if (_config.isAccRequirementEnabled) _songController.songDidFinishEvent -= ScoreChanged;
            if (_config.isMissLimitEnabled) _iScoreController.noteWasMissedEvent -= NoteWasMissed;
            if (_config.isComboBreakLimitEnabled) _iScoreController.comboDidChangeEvent -= ComboChanged;
            if (_config.isPauseLimitEnabled) _pauseController.didPauseEvent -= DidPause;
            if (_config.isSREnabled) _songController.songDidFinishEvent -= SongFinished;
        }
    }
}