using CollectionManager.DataTypes;
using CollectionManager.Enums;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Utilities;
using Newtonsoft.Json;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketDataSender
{
    public class MapStatsModule : IPlugin, IOsuEventSource, IMapDataConsumer
    {
        public string Description { get; } = "Map stats web endpoint integration";
        public string Name { get; } = nameof(MapStatsModule);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public EventHandler<IMapSearchArgs> NewOsuEvent { get; set; }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private ManualResetEvent _newMapDataRecieved = new ManualResetEvent(false);
        private IMapSearchResult mapSearchResult = null;
        private readonly ILogger logger;

        public MapStatsModule(ILogger logger)
        {
            this.logger = logger;
        }
        public Task SetNewMapAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            mapSearchResult = map;
            _newMapDataRecieved.Set();
            return Task.CompletedTask;
        }

        public List<(string Description, IWebModule module)> GetModules()
        {
            return new List<(string Description, IWebModule module)>
            {
                ("Calculate token values for given .osu file. Parameters: \"osuFile\" - full .osu file path; \"gamemode\" - gamemode to calculate from 0 to 3, 0(osu!) by default; \"mods\" - mods to calculate with, NM by default; \"tokenNames\" - comma separated list of token names to output, all by default", new ActionModule("/mapStats",HttpVerbs.Get,GetMapStats)),
            };
        }

        private Task GetMapStats(IHttpContext context)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            if (!context.Request.QueryString.ContainsKey("osuFile"))
            {
                context.Response.StatusCode = 422;
                return Task.CompletedTask;
            }

            var filePath = context.Request.QueryString["osuFile"];
            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = 422;
                return Task.CompletedTask;
            }

            var playmode = context.Request.QueryString.ContainsKey("gamemode") ? Convert.ToInt32(context.Request.QueryString["gamemode"]) : 0;
            var rawMods = context.Request.QueryString.ContainsKey("mods") ? context.Request.QueryString["mods"] : "";
            Mods mods = Mods.Omod;
            foreach (var mod in Regex.Split(rawMods, @"([A-Za-z]{2})").Where(s => !string.IsNullOrEmpty(s)))
            {
                if (Enum.TryParse(mod, true, out Mods parsedMod))
                    mods |= parsedMod;
            }

            logger.Log("requesting {0}", LogLevel.Information, filePath);
            NewOsuEvent?.Invoke(this, new MapSearchArgs(nameof(MapStatsModule), OsuEventType.MapChange)
            {
                OsuFilePath = filePath,
                PlayMode = (PlayMode)playmode,
                Status = OsuStatus.Listening,
                Mods = mods,
                MapId = -999,
                MapHash = CalculateMD5(filePath),
                Raw = "dummyRawValue",
            });
            _newMapDataRecieved.Reset();

            //Wait for event to be processed
            if (WaitHandle.WaitAny(new[] { token.WaitHandle, _newMapDataRecieved }, TimeSpan.FromMinutes(5)) == WaitHandle.WaitTimeout || token.IsCancellationRequested)
            {
                context.Response.StatusCode = 202;
                logger.Log("aborted... {0}", LogLevel.Information, filePath);

                return Task.CompletedTask;
            }
            logger.Log("sending... {0}", LogLevel.Information, filePath);

            using var response = context.OpenResponseText();
            context.Response.ContentType = "application/json";

            if (context.Request.QueryString.ContainsKey("tokenNames"))
            {
                var tokenNames = context.Request.QueryString["tokenNames"].Split(",");
                response.Write(JsonConvert.SerializeObject(Tokens.AllTokens.Where(t => tokenNames.Contains(t.Key)).ToDictionary(k => k.Key, v => v.Value.Value)));
            }
            else
            {
                response.Write(JsonConvert.SerializeObject(Tokens.AllTokens.Where(t => (t.Value.Type & TokenType.Live) == 0).ToDictionary(k => k.Key, v => v.Value.Value)));
            }
            return Task.CompletedTask;
        }

        private string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
