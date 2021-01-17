using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using EmbedIO.WebApi;
using StreamCompanionTypes.Enums;
using Swan.Logging;
using ILogger = StreamCompanionTypes.Interfaces.Services.ILogger;
using LogLevel = Swan.Logging.LogLevel;

namespace WebSocketDataSender
{
    public class HttpServer : IDisposable
    {
        private WebServer _server;
        public HttpServer(string baseUrl, string httpRootPath, ILogger logger, IEnumerable<(string Description, IWebModule Module)> modules)
        {
            _server = CreateWebServer(baseUrl, httpRootPath, logger, modules);
        }

        public Task RunAsync() => _server.RunAsync();

        private static WebServer CreateWebServer(string url, string rootPath, ILogger logger,
            IEnumerable<(string Description, IWebModule Module)> modules)
        {
            Logger.NoLogging();
            Logger.RegisterLogger(new HttpLogger(logger));
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager()
                .WithCors()
                .WithModule(new ActionModule("/ping", HttpVerbs.Get, ctx => ctx.SendDataAsync("pong")));


            var modulesList = modules.ToList();
            foreach (var module in modulesList)
            {
                server = server.WithModule(module.Module);
            }
            var endpoints = string.Join(Environment.NewLine, modulesList.Select(x => $"{x.Module.BaseRoute} - {x.Description}"));

            server
                .WithModule(new ActionModule("/help", HttpVerbs.Any, ctx =>
            {
                ctx.Response.StatusCode = 404;
                return ctx.SendStringAsync($"Usable endpoints:{Environment.NewLine}{endpoints}", "text", Encoding.Default);
            }))
                .WithStaticFolder("/", rootPath, true, m => m.WithoutContentCaching().WithDirectoryLister(DirectoryLister.Html));
            
            return server;
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
        private class HttpLogger : Swan.Logging.ILogger
        {
            private readonly ILogger _logger;

            public HttpLogger(ILogger logger)
            {
                _logger = logger;
            }
            public void Log(LogMessageReceivedEventArgs logEvent)
            {
                var loglevel = logEvent.MessageType switch
                {
                    LogLevel.Error => StreamCompanionTypes.Enums.LogLevel.Error,
                    LogLevel.Fatal => StreamCompanionTypes.Enums.LogLevel.Critical,
                    _ => StreamCompanionTypes.Enums.LogLevel.Debug
                };

                if (logEvent.Exception != null)
                    _logger.Log(logEvent.Exception, loglevel);
                else
                    _logger.Log(logEvent.Message, loglevel);
            }

            public LogLevel LogLevel { get; } = LogLevel.Info;

            public void Dispose()
            {

            }

        }
    }
}