using System;
using SiraUtil.Tools;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.ViewControllers;
using ScoreRequirement.Configuration;
using ScoreRequirement.Installers;
using Zenject;

namespace ScoreRequirement.UI
{
    public class SRSettingsViewController : IInitializable, IDisposable
    {
        private PluginConfig _config;

        public SRSettingsViewController(PluginConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("ScoreRequirement", "ScoreRequirement.UI.SRSettingsView.bsml", this);
        }

        public void Dispose()
        {
            GameplaySetup.instance.RemoveTab("ScoreRequirement");
        }
    }
}
