using Zenject;
using ScoreRequirement.UI;

namespace ScoreRequirement.Installers
{
	internal class SRMenuInstaller : Installer
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<SRSettingsViewController>().AsSingle();
		}
	}
}