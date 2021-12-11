using ScoreRequirement.Managers;
using Zenject;

namespace ScoreRequirement.Installers
{
    internal class SRGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SRManager>().AsSingle();
        }
    }
}