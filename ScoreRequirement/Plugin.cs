using IPA;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;
using IPA.Config.Stores;
using ScoreRequirement.Configuration;
using ScoreRequirement.Installers;
using Config = IPA.Config.Config;

namespace ScoreRequirement
{
	[Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
	public class Plugin
	{
		[Init]
		public void Init(Zenjector zenjector, Config config, IPALogger logger)
		{
			zenjector.UseLogger(logger);
			zenjector.UseMetadataBinder<Plugin>();

			zenjector.Install<SRAppInstaller>(Location.App, config.Generated<PluginConfig>());
			zenjector.Install<SRMenuInstaller>(Location.Menu);
			zenjector.Install<SRGameInstaller>(Location.StandardPlayer);
		}
	}
}