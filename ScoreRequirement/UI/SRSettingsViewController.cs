using System;
using SiraUtil.Tools;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using HMUI;
using IPA.Config.Data;
using ScoreRequirement.Configuration;
using ScoreRequirement.Managers;

using Zenject;

namespace ScoreRequirement.UI
{
    public class SRSettingsViewController : IInitializable, IDisposable
    {
        private PluginConfig _config;
        private LevelCollectionNavigationController _levelCollectionNavigationController;
        private StandardLevelDetailViewController _levelDetail;

        public SRSettingsViewController(PluginConfig config, LevelCollectionNavigationController levelCollectionNavigationController, StandardLevelDetailViewController levelDetail)
        {
            _config = config;
            _levelDetail = levelDetail;
            _levelCollectionNavigationController = levelCollectionNavigationController;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("ScoreRequirement", "ScoreRequirement.UI.SRSettingsView.bsml", this);
            _levelCollectionNavigationController.didChangeDifficultyBeatmapEvent -= LevelCollectionNavigationControllerOndidChangeDifficultyBeatmapEvent;
            _levelCollectionNavigationController.didChangeDifficultyBeatmapEvent += LevelCollectionNavigationControllerOndidChangeDifficultyBeatmapEvent;
            _levelCollectionNavigationController.didChangeLevelDetailContentEvent += DidChangeBeatmap;
        }

        private void DidChangeBeatmap(LevelCollectionNavigationController navigationController, StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType != StandardLevelDetailViewController.ContentType.OwnedAndReady) return;
                comboSlider.slider.maxValue = navigationController.selectedDifficultyBeatmap.beatmapData.cuttableNotesType;
                comboSlider.Value = 0;
        }
        
        private void LevelCollectionNavigationControllerOndidChangeDifficultyBeatmapEvent(LevelCollectionNavigationController _, IDifficultyBeatmap beatMap)
        {
            comboSlider.slider.maxValue = beatMap.beatmapData.cuttableNotesType;
                comboSlider.Value = 0;
        }

        public void Dispose()
        {
            GameplaySetup.instance.RemoveTab("ScoreRequirement");
            _levelCollectionNavigationController.didChangeDifficultyBeatmapEvent -= LevelCollectionNavigationControllerOndidChangeDifficultyBeatmapEvent;
        }

        [UIAction("maxAcc")]
        public string MaxAcc(float acc)
        {
            return (acc / 100f).ToString("P");
        }

        [UIComponent("comboSlider")] 
        private SliderSetting comboSlider;

        [UIValue("srEnabled")]
        private bool SREnabled
        {
            get => _config.isSREnabled;
            set => _config.isSREnabled = value;
        }
        
        [UIValue("comboRequirement")]
        private bool ComboReqEnabled
        {
            get => _config.isComboRequirementEnabled;
            set => _config.isComboRequirementEnabled = value;
        }
        
        [UIValue("breakLimit")]
        private bool BreakLimitEnabled
        {
            get => _config.isComboBreakLimitEnabled;
            set => _config.isComboBreakLimitEnabled = value;
        }
        
        [UIValue("accRequirement")]
        private bool AccReqEnabled
        {
            get => _config.isAccRequirementEnabled;
            set => _config.isAccRequirementEnabled = value;
        }
        
        [UIValue("pauseLimit")]
        private bool PauseLimitEnabled
        {
            get => _config.isPauseLimitEnabled;
            set => _config.isPauseLimitEnabled = value;
        }
        
        [UIValue("missCount")]
        private bool MissCountEnabled
        {
            get => _config.isMissLimitEnabled;
            set => _config.isMissLimitEnabled = value;
        }
        
        [UIValue("comboRequirementSlider")]
        private int ComboRequirement
        {
            get => _config.minimumComboCount;
            set => _config.minimumComboCount = value;
        }
        
        [UIValue("comboBreakLimitSlider")]
        private int ComboBreakLimit
        {
            get => _config.comboBreakLimit;
            set => _config.comboBreakLimit = value;
        }
        
        [UIValue("accRequirementSlider")]
        private float AccRequirement
        {
            get => _config.accRequirement;
            set => _config.accRequirement = value;
        }
        
        [UIValue("pauseLimitSlider")]
        private int PauseLimit
        {
            get => _config.pauseLimit;
            set => _config.pauseLimit = value;
        }
        
        [UIValue("maxMissCountSlider")]
        private int MaxMissCount
        {
            get => _config.missLimit;
            set => _config.missLimit = value;
        }
    }
}
