using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ScoreRequirement.Configuration
{
    internal class PluginConfig
    {
        public virtual bool isSREnabled { get; set; }
        public virtual bool isAccRequirementEnabled { get; set; }
        public virtual bool isComboRequirementEnabled { get; set; }
        public virtual bool isPauseLimitEnabled { get; set; }
        public virtual bool isMissLimitEnabled { get; set; }
        public virtual int minimumComboCount { get; set; }
        public virtual int comboBreakLimit { get; set; }
        public virtual int pauseLimit { get; set; }
        public virtual int missLimit { get; set; }
    }
}
