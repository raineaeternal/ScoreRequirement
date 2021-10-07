using System;
using IPA.Logging;
using ScoreRequirement.Configuration;
using SiraUtil.Services;

namespace ScoreRequirement.Managers
{
	internal class SRManager : IDisposable
	{
		private readonly Logger _logger;
		private readonly PluginConfig _config;
		private readonly SongController _songController;
		private readonly IScoreController _iScoreController;
		private readonly PauseController _pauseController;
		private readonly Submission _submission;
		private readonly RelativeScoreAndImmediateRankCounter _relativeScoreAndImmediateRankCounter;

		private int currentComboBreaks;
		private int currentPauses;
		private int currentMisses;

		public SRManager(Logger logger, PluginConfig config, SongController songController, IScoreController iScoreController, PauseController pauseController, Submission submission,
			RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRankCounter)
		{
			_config = config;
			_logger = logger;
			_songController = songController;
			_iScoreController = iScoreController;
			_pauseController = pauseController;
			_submission = submission;
			_relativeScoreAndImmediateRankCounter = relativeScoreAndImmediateRankCounter;

			currentComboBreaks = 0;
			currentPauses = 0;
			currentMisses = 0;

			if (!_config.isSREnabled)
			{
				return;
			}

			if (_config.isAccRequirementEnabled) _songController.songDidFinishEvent += ScoreChanged;
			if (_config.isMissLimitEnabled) _iScoreController.noteWasMissedEvent += NoteWasMissed;
			if (_config.isComboBreakLimitEnabled) _iScoreController.comboDidChangeEvent += ComboChanged;
			if (_config.isPauseLimitEnabled) _pauseController.didPauseEvent += PauseChanged;
			
			_songController.songDidFinishEvent += SongFinished;
		}

		public void Dispose()
		{
			if (!_config.isSREnabled)
			{
				return;
			}

			if (_config.isAccRequirementEnabled) _songController.songDidFinishEvent -= ScoreChanged;
			if (_config.isMissLimitEnabled) _iScoreController.noteWasMissedEvent -= NoteWasMissed;
			if (_config.isComboBreakLimitEnabled) _iScoreController.comboDidChangeEvent -= ComboChanged;
			if (_config.isPauseLimitEnabled) _pauseController.didPauseEvent -= PauseChanged;
			
			_songController.songDidFinishEvent -= SongFinished;
		}

		private void SongFinished()
		{
			CheckMaxCombo();
			CheckComboBreaks();
			CheckPauses();
			CheckMisses();
		}

		private void ScoreChanged()
		{
			if (_relativeScoreAndImmediateRankCounter.relativeScore < _config.accRequirement / 100f)
			{
				_submission.DisableScoreSubmission("ScoreRequirement", "Accuracy was too low");
			}

			_logger.Info($"Current acc is: {_relativeScoreAndImmediateRankCounter.relativeScore} of the required {_config.accRequirement}");
		}

		private void NoteWasMissed(NoteData noteData, int multiplier)
		{
			if (noteData.colorType == ColorType.None)
			{
				return;
			}

			currentMisses++;
		}

		private void ComboChanged(int combo)
		{
			if (combo == 0)
			{
				currentComboBreaks++;
			}
		}

		private void PauseChanged()
		{
			currentPauses++;
		}

		private void CheckMaxCombo()
		{
			if (!_config.isComboRequirementEnabled)
			{
				return;
			}

			_logger.Info($"{_iScoreController.maxCombo} of {_config.minimumComboCount} has been reached!");

			if (!(_iScoreController.maxCombo > _config.minimumComboCount))
			{
				_submission?.DisableScoreSubmission("ScoreRequirement", "Combo was too low");
			}
		}
		
		private void CheckComboBreaks()
		{
			if (!_config.isComboBreakLimitEnabled)
			{
				return;
			}

			if (currentComboBreaks > _config.comboBreakLimit)
			{
				_submission?.DisableScoreSubmission("ScoreRequirement", "Too many combo breaks");
			}

			_logger.Info($"Combo breaks: {currentComboBreaks}");
		}
		
		private void CheckPauses()
		{
			if (!_config.isPauseLimitEnabled)
			{
				return;
			}

			if (currentPauses > _config.pauseLimit)
			{
				_submission?.DisableScoreSubmission("ScoreRequirement", "Too many pauses");
			}

			_logger.Info($"Pauses: {currentPauses}");
		}

		private void CheckMisses()
		{
			if (!_config.isMissLimitEnabled)
			{
				return;
			}

			if (currentMisses > _config.missLimit)
			{
				_submission?.DisableScoreSubmission("ScoreRequirement", "Too many misses");
			}

			_logger.Info($"Misses: {currentMisses}");
		}
	}
}
