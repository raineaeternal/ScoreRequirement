using System;
using IPA.Logging;
using ScoreRequirement.Configuration;
using Zenject;

namespace ScoreRequirement.Managers
{
    internal class SRManager : IInitializable, IDisposable
    {
        private readonly Logger _logger;
        private readonly PluginConfig _config;
        private readonly SongController _songController;

        public SRManager(Logger logger, PluginConfig config, SongController songController)
        {
            _config = config;
            _logger = logger;
            _songController = songController;
        }

        public void SongFinished()
        {
            _logger.Info("Level Finished.");
        }
            
        public void Initialize()
        {
            _songController.songDidFinishEvent += SongFinished;
        }

        public void Dispose()
        {
            
        }
    }
}