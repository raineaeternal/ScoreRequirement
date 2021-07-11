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
        private PluginConfig _config;
        
        // This zenjects the MenuInstaller to the BSML Mod Tab
        [Init]
        public void Init(Zenjector zenjector, Config config, IPALogger logger)
        {
            _config = config.Generated<PluginConfig>();
            
            zenjector.OnMenu<SRMenuInstaller>().WithParameters(logger, _config);
            zenjector.OnGame<SRGameInstaller>().WithParameters(logger, _config);
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
