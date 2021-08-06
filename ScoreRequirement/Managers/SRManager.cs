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
        private Submission _submission;
        private AudioTimeSyncController _audioTimeSyncController;
        private RelativeScoreAndImmediateRankCounter _relativeScoreAndImmediateRankCounter;

        private int currentComboBreaks;
        private int currentPauses;
        private int currentMisses;

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

        public void CheckMaxCombo()
        { 
            if (!_config.isComboRequirementEnabled) return;
            if (!(_iScoreController.maxCombo > _config.minimumComboCount))
                    _submission?.DisableScoreSubmission("ScoreRequirement", "Combo was too low");
            _logger.Info($"{_iScoreController.maxCombo} of {_config.minimumComboCount} has been reached!");
        }

        private void ComboChanged(int combo)
        {
            if (combo == 0) currentComboBreaks++;
        }

        private void CheckComboBreaks()
        {
            if (!_config.isComboBreakLimitEnabled) return;
            if (currentComboBreaks > _config.comboBreakLimit) _submission?.DisableScoreSubmission("ScoreRequirement", "Too many combo breaks");
            _logger.Info($"Combo breaks: {currentComboBreaks}");
        }
        
        private void SongFinished()
        {
            CheckMaxCombo();
            CheckComboBreaks();
            CheckPauses();
            CheckMisses();
        }
        
        private void PauseChanged()
        {
            currentPauses++;
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
            if (!_config.isMissLimitEnabled) return;
            if (currentPauses > _config.pauseLimit) _submission?.DisableScoreSubmission("ScoreRequirement", "Too many pauses");
            _logger.Info($"Pauses: {currentPauses}");
        }

        private void NoteWasMissed(NoteData noteData, int multiplier)
        {
            if (noteData.colorType == ColorType.None) return;
            currentMisses++;
        }

        private void CheckMisses()
        {
            if (!_config.isMissLimitEnabled) return;
            if (currentMisses > _config.missLimit) 
                _submission?.DisableScoreSubmission("ScoreRequirement", "Too many misses");
            _logger.Info($"Misses: {currentMisses}");
        }

        public void Initialize()
        {
            currentComboBreaks = 0;
            currentPauses = 0;
            currentMisses = 0;

            if (_config.isSREnabled) _songController.songDidFinishEvent += SongFinished;
            if (_config.isAccRequirementEnabled) _songController.songDidFinishEvent += ScoreChanged;
            if (_config.isMissLimitEnabled) _iScoreController.noteWasMissedEvent += NoteWasMissed;
            if (_config.isComboBreakLimitEnabled) _iScoreController.comboDidChangeEvent += ComboChanged;
            if (_config.isPauseLimitEnabled) _pauseController.didPauseEvent += PauseChanged;
        }

        public void Dispose()
        {
            if (_config.isSREnabled) _songController.songDidFinishEvent -= SongFinished;
            if (_config.isAccRequirementEnabled) _songController.songDidFinishEvent -= ScoreChanged;
            if (_config.isMissLimitEnabled) _iScoreController.noteWasMissedEvent -= NoteWasMissed;
            if (_config.isComboBreakLimitEnabled) _iScoreController.comboDidChangeEvent -= ComboChanged;
            if (_config.isPauseLimitEnabled) _pauseController.didPauseEvent -= PauseChanged;
        }
    }
}