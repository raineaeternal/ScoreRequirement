using IPA.Logging;
using ScoreRequirement.Configuration;
using Zenject;
using SiraUtil;
using ScoreRequirement.UI;

namespace ScoreRequirement.Installers
{
    internal class SRMenuInstaller : Installer
    {
        private readonly Logger _logger;
        private readonly PluginConfig _config;

        internal SRMenuInstaller(Logger logger, PluginConfig config)
        {
            _logger = logger;
            _config = config;
        }
        
        public override void InstallBindings()
            {
                Container.BindInterfacesAndSelfTo<SRSettingsViewController>().AsSingle();
                Container.BindInstance(_logger).AsSingle();
                Container.BindInstance(_config).AsSingle();
            }
        }
    }
