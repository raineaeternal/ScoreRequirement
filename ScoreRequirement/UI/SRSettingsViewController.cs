using System;
using System.ComponentModel;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using IPA.Loader;
using ScoreRequirement.Configuration;
using SiraUtil.Zenject;
using Zenject;

namespace ScoreRequirement.UI
{
    internal class SRSettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly PluginConfig _config;
        private readonly LevelCollectionNavigationController _levelCollectionNavigationController;
        private readonly PluginMetadata _metadata;

        public event PropertyChangedEventHandler PropertyChanged;

        public SRSettingsViewController(PluginConfig config, LevelCollectionNavigationController levelCollectionNavigationController, UBinder<Plugin, PluginMetadata> metadata)
        {
            _config = config;
            _metadata = metadata.Value;
            _levelCollectionNavigationController = levelCollectionNavigationController;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab(_metadata.Name, "ScoreRequirement.UI.BSML.SRSettingsView.bsml", this);

            _levelCollectionNavigationController.didChangeDifficultyBeatmapEvent -= LevelCollectionNavigationControllerOndidChangeDifficultyBeatmapEvent;
            _levelCollectionNavigationController.didChangeDifficultyBeatmapEvent += LevelCollectionNavigationControllerOndidChangeDifficultyBeatmapEvent;
            _levelCollectionNavigationController.didChangeLevelDetailContentEvent += DidChangeBeatmap;
        }

        private void DidChangeBeatmap(LevelCollectionNavigationController navigationController, StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType != StandardLevelDetailViewController.ContentType.OwnedAndReady) return;
                // comboSlider.slider.maxValue = navigationController.selectedDifficultyBeatmap.beatmapData.cuttableNotesCount;
                ComboRequirement = 0;
        }
        
        private void LevelCollectionNavigationControllerOndidChangeDifficultyBeatmapEvent(LevelCollectionNavigationController _, IDifficultyBeatmap beatMap)
        {
            // comboSlider.slider.maxValue = beatMap.beatmapData.cuttableNotesCount;
                ComboRequirement = 0;
        }

        public void Dispose()
        {
            if (GameplaySetup.instance != null)
                GameplaySetup.instance.RemoveTab("ScoreRequirement");
            _levelCollectionNavigationController.didChangeDifficultyBeatmapEvent -= LevelCollectionNavigationControllerOndidChangeDifficultyBeatmapEvent;
        }

        [UIAction("maxAcc")]
        public string MaxAcc(float acc)
        {
            return (acc / 100).ToString("P");
        }

        #region EnabledToggles

        [UIValue("srEnabled")]
        private bool SREnabled
        {
            get => _config.isSREnabled;
            set
            {
                _config.isSREnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SREnabled)));
            }
        }
        
        [UIValue("comboRequirementEnabled")]
        private bool ComboReqEnabled
        {
            get => _config.isComboRequirementEnabled;
            set
            {
                _config.isComboRequirementEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComboReqEnabled)));
            }
        }
        
        [UIValue("breakLimitEnabled")]
        private bool BreakLimitEnabled
        {
            get => _config.isComboBreakLimitEnabled;
            set
            {
                _config.isComboBreakLimitEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BreakLimitEnabled)));
            } 
        }
        
        [UIValue("accRequirementEnabled")]
        private bool AccReqEnabled
        {
            get => _config.isAccRequirementEnabled;
            set
            {
                _config.isAccRequirementEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccReqEnabled)));
            } 
    }
        
        [UIValue("pauseLimitEnabled")]
        private bool PauseLimitEnabled
        {
            get => _config.isPauseLimitEnabled;
            set
            {
                _config.isPauseLimitEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PauseLimitEnabled)));
            } 
        }
        
        [UIValue("missLimitEnabled")]
        private bool MissLimitEnabled
        {
            get => _config.isMissLimitEnabled;
            set
            {
                _config.isMissLimitEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MissLimitEnabled)));
            } 
        }
        
        #endregion

        #region Values

        [UIValue("metadata")] 
        internal string MetadataName => $"{_metadata.Name} | {_metadata.HVersion}";
        
        [UIValue("comboRequirement")]
        private int ComboRequirement
        {
            get => _config.minimumComboCount;
            set
            {
                _config.minimumComboCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComboRequirement)));
            } 
        }
        
        [UIValue("comboBreakLimit")]
        private int ComboBreakLimit
        {
            get => _config.comboBreakLimit;
            set
            {
                _config.comboBreakLimit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComboBreakLimit)));
            } 
        }
        
        [UIValue("accRequirement")]
        private float AccRequirement
        {
            get => _config.accRequirement;
            set
            {
                _config.accRequirement = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccRequirement)));
            } 
        }
        
        [UIValue("pauseLimit")]
        private int PauseLimit
        {
            get => _config.pauseLimit;
            set
            {
                _config.pauseLimit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PauseLimit)));
            }
        }
        
        [UIValue("maxMissCount")]
        private int MaxMissCount
        {
            get => _config.missLimit;
            set
            {
                _config.missLimit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxMissCount)));
            } 
        }
        #endregion
    }
}
