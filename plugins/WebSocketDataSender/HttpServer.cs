﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using EmbedIO.WebApi;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace WebSocketDataSender
{
    public class HttpServer : IDisposable
    {
        private WebServer _server;
        public HttpServer(string baseUrl, string httpRootPath, ILogger logger, IEnumerable<(string Description, IWebModule Module)> modules)
        {
            _server = CreateWebServer(baseUrl, httpRootPath, logger, modules);
            _server.RunAsync();
        }

        private static WebServer CreateWebServer(string url, string rootPath, ILogger logger,
            IEnumerable<(string Description, IWebModule Module)> modules)
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager()
                .WithModule(new ActionModule("/ping", HttpVerbs.Get, ctx => ctx.SendDataAsync("pong")));

            var modulesList = modules.ToList();
            foreach (var module in modulesList)
            {
                server = server.WithModule(module.Module);
            }
            var endpoints = string.Join(Environment.NewLine, modulesList.Select(x => $"{x.Module.BaseRoute} - {x.Description}"));

#if DEBUG
            //A little hack to grab overlay files directly from git repository
            var newRootPath = Path.Combine(rootPath, "..", "..", "..", "..", "webOverlay");
            if (Directory.Exists(newRootPath))
                rootPath = newRootPath;
#endif

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
    }
}