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
        public string Description { get; } = "Provides current beatmap background image both as token and local file on disk";
        public string Name { get; } = nameof(BackgroundImageProviderPlugin);
        public string Author { get; } = "Piotrekol";
        public string Url { get; }
        public string UpdateUrl { get; }

        private readonly ISaver _saver;
        private readonly ISettings _settings;
        private readonly IContextAwareLogger _logger;
        private Tokens.TokenSetter _tokenSetter;
        private IToken _imageToken;
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
            _imageToken = _tokenSetter("backgroundImage", initialValue);
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
                            if (File.Exists(_saveLocation))
                                File.Delete(_saveLocation);

                            File.Copy(imageLocation, _saveLocation);
                            _imageNameToken.Value = Path.GetFileName(imageLocation);
                            if (_settings.Get<bool>(EnableImageToken))
                                _imageToken.Value = $"data:image/png;base64, {ImageToBase64(imageLocation)}";

                            _imageLocationToken.Value = _lastCopiedFileLocation = imageLocation;
                        }

                        return Task.CompletedTask;
                    }
                }

                if (File.Exists(_saveLocation))
                    File.Delete(_saveLocation);

                _imageToken.Value = null;
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

        private string ImageToBase64(string imageLocation)
        {
            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(imageLocation))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (ExternalException ex)
            {
                var fileInfo = new FileInfo(imageLocation);
                _logger.SetContextData("backgroundImageFilename", fileInfo.Name);
                _logger.SetContextData("backgroundImageSize", fileInfo.Length.ToString());
                _logger.SetContextData("backgroundImageExternalExceptionCode", ex.ErrorCode.ToString());
                _logger.Log(ex, LogLevel.Debug);
                return string.Empty;
            }
        }
    }
}
