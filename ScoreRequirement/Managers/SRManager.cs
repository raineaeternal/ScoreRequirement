using System;
using IPA.Logging;
using ScoreRequirement.Configuration;
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

        //Uses SiraUtil for disabling Score Submission
        private bool isScoreSubmissionDisabled;
        private int currentComboBreaks;
        private int currentPauses;
        private int currentMisses;

        public SRManager(Logger logger, PluginConfig config, SongController songController, IScoreController iScoreController, PauseController pauseController, IReadonlyBeatmapData iReadonlyBeatmapData)
        {
            _config = config;
            _logger = logger;
            _songController = songController;
            _iScoreController = iScoreController;
            _pauseController = pauseController;
            _iReadonlyBeatmapData = iReadonlyBeatmapData;
        }

        public void CheckMaxCombo()
        { 
            if (!_config.isComboRequirementEnabled) return;
                _logger.Info($"{_iScoreController.maxCombo} of {_config.minimumComboCount} has been reached!");
                if (!(_iScoreController.maxCombo > _config.minimumComboCount))
                    isScoreSubmissionDisabled = true;
        }

        private void ComboChanged(int combo)
        {
            if (combo == 0) currentComboBreaks++;
        }

        private void CheckComboBreaks()
        {
            _logger.Info($"Combo breaks: {currentComboBreaks}");
            if (currentComboBreaks > _config.comboBreakLimit) isScoreSubmissionDisabled = true;
        }
        
        public void SongFinished()
        {
            CheckMaxCombo();
            CheckComboBreaks();
            CheckPauses();
            CheckMisses();
            
            _logger.Info($"Is Score Submission disabled: {isScoreSubmissionDisabled}");
        }
        
        private void PauseChanged()
        {
            currentPauses++;
        }

        public void CheckPauses()
        {
            _logger.Info($"Pauses: {currentPauses}");
            if (currentPauses > _config.pauseLimit) isScoreSubmissionDisabled = true;
        }
        
        public void NoteWasMissed(NoteData noteData, int multiplier)
        {
            if (noteData.colorType == ColorType.None) return;
            currentMisses++;
        }

        public void CheckMisses()
        {
            _logger.Info($"Misses: {currentMisses}");
            if (currentMisses > _config.missLimit) isScoreSubmissionDisabled = true;
        }
        
        public void Initialize()
        {
            currentComboBreaks = 0;
            currentPauses = 0;
            currentMisses = 0;
            
            _iScoreController
            
            if(!_config.isSREnabled) return;
            _songController.songDidFinishEvent += SongFinished;

            if (_config.isMissLimitEnabled) _iScoreController.noteWasMissedEvent += NoteWasMissed;
            if(_config.isComboBreakLimitEnabled) _iScoreController.comboDidChangeEvent += ComboChanged;
            if (_config.isPauseLimitEnabled) _pauseController.didPauseEvent += PauseChanged;
        }

        public void Dispose()
        {
            
        }
    }
}