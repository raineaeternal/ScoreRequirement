using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SiraUtil.Tools;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using HMUI;
using IPA.Config.Data;
using JetBrains.Annotations;
using ScoreRequirement.Configuration;
using ScoreRequirement.Managers;
using ScoreRequirement.UI.Components;
using UnityEngine;
using Zenject;

namespace ScoreRequirement.UI
{
    public class SRSettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private PluginConfig _config;
        private LevelCollectionNavigationController _levelCollectionNavigationController;
        private StandardLevelDetailViewController _levelDetail;

        public event PropertyChangedEventHandler PropertyChanged;

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
            return (acc / 100).ToString("P1");
        }

        [UIComponent("comboSlider")] 
        private SliderSetting comboSlider;

        [UIValue("srEnabled")]
        private bool SREnabled
        {
            get => _config.isSREnabled;
            set
            {
                _config.isSREnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.isSREnabled)));
            }
        }
        
        [UIValue("comboRequirement")]
        private bool ComboReqEnabled
        {
            get => _config.isComboRequirementEnabled;
            set
            {
                _config.isComboRequirementEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.isComboRequirementEnabled)));
            }
        }
        
        [UIValue("breakLimit")]
        private bool BreakLimitEnabled
        {
            get => _config.isComboBreakLimitEnabled;
            set
            {
                _config.isComboBreakLimitEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.isComboBreakLimitEnabled)));
            } 
        }
        
        [UIValue("accRequirement")]
        private bool AccReqEnabled
        {
            get => _config.isAccRequirementEnabled;
            set
            {
                _config.isAccRequirementEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.isAccRequirementEnabled)));
            } 
    }
        
        [UIValue("pauseLimit")]
        private bool PauseLimitEnabled
        {
            get => _config.isPauseLimitEnabled;
            set
            {
                _config.isPauseLimitEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.isPauseLimitEnabled)));
            } 
        }
        
        [UIValue("missCount")]
        private bool MissCountEnabled
        {
            get => _config.isMissLimitEnabled;
            set
            {
                _config.isMissLimitEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.isMissLimitEnabled)));
            } 
        }
        
        [UIValue("comboRequirementSlider")]
        private int ComboRequirement
        {
            get => _config.minimumComboCount;
            set
            {
                _config.minimumComboCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.minimumComboCount)));
            } 
        }
        
        [UIValue("comboBreakLimitSlider")]
        private int ComboBreakLimit
        {
            get => _config.comboBreakLimit;
            set
            {
                _config.comboBreakLimit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.comboBreakLimit)));
            } 
        }
        
        [UIValue("accRequirementSlider")]
        private float AccRequirement
        {
            get => _config.accRequirement;
            set
            {
                _config.accRequirement = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.accRequirement)));
            } 
        }
        
        [UIValue("pauseLimitSlider")]
        private int PauseLimit
        {
            get => _config.pauseLimit;
            set
            {
                _config.pauseLimit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.pauseLimit)));
            }
        }
        
        [UIValue("maxMissCountSlider")]
        private int MaxMissCount
        {
            get => _config.missLimit;
            set
            {
                _config.missLimit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_config.missLimit)));
            } 
        }
        
        [UIComponent("left-button")]
        protected readonly RectTransform leftButton;
        
        [UIComponent("rightButton")]
        protected readonly RectTransform rightButton;

        [UIComponent("accRequirementSlider")] 
        protected readonly SliderSetting accSlider;
        
        [UIComponent("accRequirementSlider")] 
        protected readonly SliderSetting breakSlider;
        
        [UIComponent("maxMissCount")] 
        protected readonly SliderSetting _comboSlider;
        
        [UIComponent("pauseLimitSlider")] 
        protected readonly SliderSetting pauseSlider;
        
        [UIComponent("missSlider")] 
        protected readonly SliderSetting missSlider;
        
        public static readonly int universalInt = 1;
        public static readonly float accStep = 0.01f;

        [UIAction("#post-parse")]
        protected void PostParse()
        {
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), accSlider, accStep);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), _comboSlider, universalInt);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), pauseSlider, universalInt);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), breakSlider, universalInt);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), missSlider, universalInt);
            GameObject.Destroy(leftButton.gameObject);
            GameObject.Destroy(rightButton.gameObject);
        }
    }
}
