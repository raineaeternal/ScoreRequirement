using ScoreRequirement.Configuration;
using Zenject;

namespace ScoreRequirement.Installers
{
	internal class SRAppInstaller : Installer
	{
		private readonly PluginConfig _config;

		public SRAppInstaller(PluginConfig config)
		{
			_config = config;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(_config);
		}
	}
}