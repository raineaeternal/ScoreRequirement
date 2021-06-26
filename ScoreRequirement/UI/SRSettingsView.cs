using SiraUtil.Tools;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomMenuPointers.Configuration;
using CustomMenuPointers.Managers;
using CustomMenuPointers.UI;
using Zenject;

namespace ScoreRequirement.UI
{
    [ViewDefinition("CustomMenuPointers.UI.CMPSettingsView.bsml")]
    [HotReload(RelativePathToLayout = @"..\UI\CMPSettingsView.bsml")]
    public class SRSettingsView : BSMLAutomaticViewController
    {
        private SiraLog _siraLog;
        private PluginConfig _config;

        [Inject]
        internal void Construct(SiraLog siraLog, PluginConfig config)
        {
            _siraLog = siraLog;
            _config = config;
        }
    }
}
