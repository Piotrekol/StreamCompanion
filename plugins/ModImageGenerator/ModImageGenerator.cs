﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ModImageGenerator.API;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace ModImageGenerator
{
    class ModImageGenerator : IPlugin, ITokensProvider, ISettingsProvider
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private ISettingsHandler _settings;
        private ISaver _saver;
        ImageDeployer _imageDeployer;
        ImageGenerator _imageGenerator;
        private ModImageGeneratorSettings _modImageGeneratorSettings;
        private ILogger _logger;


        public string Description { get; } = "";
        public string Name { get; } = nameof(ModImageGenerator);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public ModImageGenerator(ILogger logger, ISaver saver, ISettingsHandler settings)
        {
            _logger = logger;
            _saver = saver;
            _settings = settings;

            var imagesFolderName = "Images";
            _imageDeployer = new ImageDeployer(Path.Combine(_saver.SaveDirectory, imagesFolderName));
            _imageDeployer.DeployImages();
            _imageDeployer.CreateReadMe();
            _imageGenerator = new ImageGenerator(_settings, @"Images");
            _imageGenerator.SetSaveHandle(_saver);
        }

        public void CreateTokens(MapSearchResult map)
        {
            if (_settings.Get<bool>(_names.EnableModImages))
            {
                if (map.FoundBeatmaps)
                {
                    var fullPathOfCreatedImage = Path.Combine(_saver.SaveDirectory, "ModImage.png");
                    string mods = map.Mods?.ShownMods ?? "";
                    using (Bitmap img = _imageGenerator.GenerateImage(mods.Split(',')))
                    {
                        try
                        {
                            img.Save(fullPathOfCreatedImage, ImageFormat.Png);
                        }
                        catch (ExternalException e)
                        {
                            _logger.Log("Image file save fail: {0} {1}", LogLevel.Error, e.ErrorCode.ToString(), e.Message ?? "null");
                        }
                    }
                }
            }
        }

        public string SettingGroup { get; } = "Mod Image";

        public void Free()
        {
            _modImageGeneratorSettings.Dispose();
        }

        public UserControl GetUiSettings()
        {
            if (_modImageGeneratorSettings == null || _modImageGeneratorSettings.IsDisposed)
            {
                _modImageGeneratorSettings = new ModImageGeneratorSettings(_settings, _imageGenerator.GenerateImage);
            }
            return _modImageGeneratorSettings;
        }
    }
}
