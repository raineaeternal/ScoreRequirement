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
        private readonly AccuracyCheckManager _accuracyCheckManager;
        private Submission _submission;
        
        private int currentComboBreaks;
        private int currentPauses;
        private int currentMisses;

        public SRManager(Logger logger, PluginConfig config, SongController songController, IScoreController iScoreController, PauseController pauseController, IReadonlyBeatmapData iReadonlyBeatmapData, AccuracyCheckManager accuracyCheckManager, Submission submission)
        {
            _config = config;
            _logger = logger;
            _songController = songController;
            _iScoreController = iScoreController;
            _pauseController = pauseController;
            _iReadonlyBeatmapData = iReadonlyBeatmapData;
            _accuracyCheckManager = accuracyCheckManager;
            _submission = submission;
        }

        public void CheckMaxCombo()
        { 
            if (!_config.isComboRequirementEnabled) return;
                _logger.Info($"{_iScoreController.maxCombo} of {_config.minimumComboCount} has been reached!");
                if (!(_iScoreController.maxCombo > _config.minimumComboCount))
                    _submission?.DisableScoreSubmission("ScoreRequirement", "Combo was too low");
        }

        private void ComboChanged(int combo)
        {
            if (combo == 0) currentComboBreaks++;
        }

        private void CheckComboBreaks()
        {
            _logger.Info($"Combo breaks: {currentComboBreaks}");
            if (currentComboBreaks > _config.comboBreakLimit) _submission?.DisableScoreSubmission("ScoreRequirement", "Too many combo breaks");
        }
        
        private void SongFinished()
        {
            CheckMaxCombo();
            CheckComboBreaks();
            CheckPauses();
            CheckMisses();
            _accuracyCheckManager.Update();

            _logger.Info($"Is Score Submission disabled: {isScoreSubmissionDisabled}");
        }
        
        private void PauseChanged()
        {
            currentPauses++;
        }

        public void CheckPauses()
        {
            _logger.Info($"Pauses: {currentPauses}");
            if (currentPauses > _config.pauseLimit) _submission?.DisableScoreSubmission("ScoreRequirement", "Too many pauses");
        }
        
        public void NoteWasMissed(NoteData noteData, int multiplier)
        {
            if (noteData.colorType == ColorType.None) return;
            currentMisses++;
        }

        public void CheckMisses()
        {
            _logger.Info($"Misses: {currentMisses}");
            if (currentMisses > _config.missLimit) _submission?.DisableScoreSubmission("ScoreRequirement", "Too many misses");
        }

        public void Initialize()
        {
            currentComboBreaks = 0;
            currentPauses = 0;
            currentMisses = 0;

            if(!_config.isSREnabled) return;
            _songController.songDidFinishEvent += SongFinished;

            if(_config.isAccRequirementEnabled) _accuracyCheckManager.Update();
            if (_config.isMissLimitEnabled) _iScoreController.noteWasMissedEvent += NoteWasMissed;
            if(_config.isComboBreakLimitEnabled) _iScoreController.comboDidChangeEvent += ComboChanged;
            if (_config.isPauseLimitEnabled) _pauseController.didPauseEvent += PauseChanged;
        }

        public void Dispose()
        {
            
        }
    }
}