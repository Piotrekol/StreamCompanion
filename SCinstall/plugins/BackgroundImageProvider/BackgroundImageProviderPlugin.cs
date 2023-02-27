using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanion.Common;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace BackgroundImageProvider
{
    public class BackgroundImageProviderPlugin : IPlugin, ITokensSource
    {
        public string Description { get; } = "Provides current beatmap background image as local file on disk";
        public string Name { get; } = nameof(BackgroundImageProviderPlugin);
        public string Author { get; } = "Piotrekol";
        public string Url { get; }
        public string UpdateUrl { get; }

        private readonly ISaver _saver;
        private readonly ISettings _settings;
        private readonly IContextAwareLogger _logger;
        private Tokens.TokenSetter _tokenSetter;
        private IToken _imageNameToken;
        private IToken _imageLocationToken;
        private string _saveLocation;
        private string _lastCopiedFileLocation;
        public static ConfigEntry EnableImageToken = new ConfigEntry($"{nameof(BackgroundImageProviderPlugin)}_EnableImageToken", false);
        public BackgroundImageProviderPlugin(ISaver saver, ISettings settings, IContextAwareLogger logger)
        {
            _saver = saver;
            _settings = settings;
            _logger = logger;
            _saveLocation = Path.Combine(_saver.SaveDirectory, "BG.png");
            _tokenSetter = Tokens.CreateTokenSetter(Name);
            var initialValue = settings.Get<bool>(EnableImageToken)
                ? null
                : $"Disabled, enable it in configuration manually under {EnableImageToken.Name}";
            _imageLocationToken = _tokenSetter("backgroundImageLocation", string.Empty);
            _imageNameToken = _tokenSetter("backgroundImageFileName", string.Empty);
        }

        protected Task InternalCreateTokens(IMapSearchResult map, CancellationToken cancellationToken, int retryCount)
        {
            try
            {
                if (map.BeatmapsFound.Any())
                {
                    var imageLocation = map.BeatmapsFound[0].GetImageLocation(_settings);
                    if (!string.IsNullOrEmpty(imageLocation))
                    {
                        if (_lastCopiedFileLocation != imageLocation)
                        {
                            _imageNameToken.Value = Path.GetFileName(imageLocation);
                            _imageLocationToken.Value = _lastCopiedFileLocation = imageLocation;
                            if (_settings.Get<bool>(EnableImageToken))
                            {
                                if (File.Exists(_saveLocation))
                                    File.Delete(_saveLocation);

                                File.Copy(imageLocation, _saveLocation);
                            }
                        }

                        return Task.CompletedTask;
                    }
                }

                if (_settings.Get<bool>(EnableImageToken) && File.Exists(_saveLocation))
                    File.Delete(_saveLocation);

                _lastCopiedFileLocation = null;
                _imageLocationToken.Value = string.Empty;
            }
            catch (IOException)
            {
                if (retryCount < 5)
                    return InternalCreateTokens(map, cancellationToken, ++retryCount);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.Log($"Could not save background image at \"{_saveLocation}\" (UnauthorizedAccessException)",LogLevel.Warning);
            }

            return Task.CompletedTask;
        }
        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            return InternalCreateTokens(map, cancellationToken, 0);
        }
    }
}
