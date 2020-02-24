using System;
using System.IO;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace BackgroundImageProvider
{
    public class BackgroundImageProviderPlugin : IPlugin, ITokensProvider
    {
        public string Description { get; } = "Provides current beatmap background image both as token and local file on disk";
        public string Name { get; } = nameof(BackgroundImageProviderPlugin);
        public string Author { get; } = "Piotrekol";
        public string Url { get; }
        public string UpdateUrl { get; }

        private readonly ISaver _saver;
        private readonly ISettingsHandler _settings;
        private Tokens.TokenSetter _tokenSetter;
        private IToken _imageToken;
        private string _saveLocation;
        private string _lastCopiedFileLocation;

        public BackgroundImageProviderPlugin(ISaver saver, ISettingsHandler settings)
        {
            _saver = saver;
            _settings = settings;
            _saveLocation = Path.Combine(_saver.SaveDirectory, "BG.png");
            _tokenSetter = Tokens.CreateTokenSetter(Name);
            _imageToken = _tokenSetter("backgroundImage", null);

        }

        protected void InternalCreateTokens(MapSearchResult map, int retryCount)
        {
            try
            {
                if (map.FoundBeatmaps)
                {
                    var imageLocation = map.BeatmapsFound[0].GetImageLocation(_settings);
                    if (_lastCopiedFileLocation != imageLocation && !string.IsNullOrEmpty(imageLocation))
                    {
                        if (File.Exists(_saveLocation))
                            File.Delete(_saveLocation);

                        File.Copy(imageLocation, _saveLocation);
                        _imageToken.Value = $"data:image/png;base64, {ImageToBase64(imageLocation)}";
                        _lastCopiedFileLocation = imageLocation;
                        return;
                    }
                }

                if (File.Exists(_saveLocation))
                    File.Delete(_saveLocation);

                _imageToken.Value = null;
                _lastCopiedFileLocation = null;
            }
            catch (IOException)
            {
                if (retryCount < 5)
                    InternalCreateTokens(map, ++retryCount);
            }
        }
        public void CreateTokens(MapSearchResult map)
        {
            InternalCreateTokens(map, 0);
        }

        private string ImageToBase64(string imageLocation)
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
    }
}
