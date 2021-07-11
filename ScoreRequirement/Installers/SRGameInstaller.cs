using IPA.Logging;
using ScoreRequirement.Configuration;
using ScoreRequirement.UI;
using ScoreRequirement.Managers;
using Zenject;

namespace ScoreRequirement.Installers
{
    internal class SRGameInstaller : Installer
    {
        private readonly Logger _logger;
        private readonly PluginConfig _config;

        internal SRGameInstaller(Logger logger, PluginConfig config)
        {
            _logger = logger;
            _config = config;
        }
        
        public override void InstallBindings()
        {
            Container.BindInstance(_logger).AsSingle();
            Container.BindInstance(_config).AsSingle();
            Container.BindInterfacesAndSelfTo<SRManager>().AsSingle();
        }
    }
}