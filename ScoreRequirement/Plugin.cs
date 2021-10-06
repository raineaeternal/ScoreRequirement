using Hive.Versioning;
using IPA;
using SiraUtil.Zenject;
using Zenject;
using IPALogger = IPA.Logging.Logger;
using IPA.Config.Stores;
using IPA.Loader;
using JetBrains.Annotations;
using ScoreRequirement.Configuration;
using ScoreRequirement.Installers;
using Config = IPA.Config.Config;

namespace ScoreRequirement
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        private PluginConfig _config;

        [CanBeNull]
        private static PluginMetadata _metadata; 

        public static string Name => _metadata?.Name!;
        public static Version Version => _metadata?.HVersion!;
        
        [Init]
        public void Init(Zenjector zenjector, Config config, IPALogger logger, PluginMetadata pluginMetadata)
        {
            _metadata = pluginMetadata;
            _config = config.Generated<PluginConfig>();
            
            zenjector.OnMenu<SRMenuInstaller>().WithParameters(logger, _config, new UBinder<Plugin, PluginMetadata>(_metadata));
            zenjector.OnGame<SRGameInstaller>().WithParameters(logger, _config).ShortCircuitForMultiplayer().ShortCircuitForTutorial().ShortCircuitForCampaign();
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
