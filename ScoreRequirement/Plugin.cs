using IPA;
using SiraUtil.Zenject;
using Zenject;
using IPALogger = IPA.Logging.Logger;
using IPA.Config.Stores;
using ScoreRequirement.Configuration;
using ScoreRequirement.Installers;
using Config = IPA.Config.Config;

namespace ScoreRequirement
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        // This zenjects the MenuInstaller to the BSML Mod Tab
        [Init]
        public void Init(Zenjector zenjector, Config config, IPALogger logger)
        {
            zenjector.OnApp<SRMenuInstaller>().WithParameters(logger, config.Generated<PluginConfig>());
        }

        [OnEnable]
        public void OnEnable()
        {
        }

        [OnDisable]
        public void OnDisable()
        {
        }
    }
}
