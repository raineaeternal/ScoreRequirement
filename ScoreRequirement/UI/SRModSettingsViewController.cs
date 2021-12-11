using System;
using System.ComponentModel;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using IPA.Loader;
using ScoreRequirement.Configuration;
using SiraUtil.Zenject;
using Zenject;

namespace ScoreRequirement.UI
{
    internal class SRModSettingsViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; 
        
        private readonly PluginConfig _config;
        private readonly PluginMetadata _metadata;

        public SRModSettingsViewController(PluginConfig config, UBinder<Plugin, PluginMetadata> metadata)
        {
            _config = config;
            _metadata = metadata.Value;
        }

        public void Initialize()
        {
            BSMLSettings.instance.AddSettingsMenu("ScoreRequirement", "ScoreRequirement.UI.BSML.ModSettingsView.bsml", this);
        }

        public void Dispose()
        {
            if (BSMLSettings.instance != null) BSMLSettings.instance.RemoveSettingsMenu("ScoreRequirement");
        }

        [UIValue("accStepValue")]
        public float AccStepValue
        {
            get => _config.accStep;
            set
            {
                _config.accStep = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccStepValue)));
            }
        }

        [UIValue("accStepText")]
        internal string AccStepText => $"How much do you want it to step at a time? \n Default is 0.01";
        
        [UIValue("metadata")] 
        internal string MetadataName => $"{_metadata.Name} | {_metadata.HVersion}";
    }
}