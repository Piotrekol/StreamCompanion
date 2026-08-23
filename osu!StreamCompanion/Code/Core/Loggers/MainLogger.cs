using StreamCompanionTypes.DataTypes;
using osu_StreamCompanion.Code.Misc;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace osu_StreamCompanion.Code.Core.Loggers;

internal class MainLogger : IContextAwareLogger
{
    public static MainLogger Instance = new();

    public static ConfigEntry LogsRetentionDays = new("LogsRetentionDays", 14);

    private readonly ConcurrentDictionary<string, string> contextData = new();
    private Settings settings;
    private string logsDirectory;
    private static readonly MessageTemplateParser messageTemplateParser = new();
    private LoggingLevelSwitch levelSwitch;
    private volatile Logger serilogLogger;
    private bool consoleAllocated;
    private readonly object pipelineLock = new();

    private MainLogger()
    { }

    public void Initialize(Settings settings, ISaver saver)
    {
        this.settings = settings;
        logsDirectory = Path.Combine(saver.SaveDirectory, "Logs");
        _ = Directory.CreateDirectory(logsDirectory);
        CleanupLegacyLogs();

        levelSwitch = new LoggingLevelSwitch((LogEventLevel)settings.Get<int>(SettingNames.Instance.LogLevel));

        if (settings.Get<bool>(SettingNames.Instance.Console) && OperatingSystem.IsWindows())
        {
            AllocateConsole();
            consoleAllocated = true;
        }

        serilogLogger = BuildLogger(consoleAllocated);
        settings.SettingUpdated += OnSettingUpdated;
        Serilog.Debugging.SelfLog.Enable(message => Console.Error.WriteLine(message));
    }

    private Logger BuildLogger(bool withConsole)
    {
        LoggerConfiguration configuration = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.File(
                Path.Combine(logsDirectory, "sc.txt"),
                outputTemplate: "{Message:lj}{NewLine}",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                retainedFileTimeLimit: TimeSpan.FromDays(settings.Get<int>(LogsRetentionDays)),
                buffered: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1));

        if (withConsole)
        {
            _ = configuration.WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}");
        }

        return configuration.CreateLogger();
    }

    private void OnSettingUpdated(object sender, SettingUpdated eventArgs)
    {
        if (eventArgs.Name == SettingNames.Instance.LogLevel.Name)
        {
            levelSwitch.MinimumLevel = (LogEventLevel)settings.Get<int>(SettingNames.Instance.LogLevel);
        }
        else if (eventArgs.Name == SettingNames.Instance.Console.Name)
        {
            if (settings.Get<bool>(SettingNames.Instance.Console))
            {
                EnableConsole();
            }
            else
            {
                DisableConsole();
            }
        }
    }

    public void EnableConsole()
    {
        if (consoleAllocated || !OperatingSystem.IsWindows())
        {
            return;
        }

        AllocateConsole();
        RebuildLogger(withConsole: true);
        consoleAllocated = true;
    }

    public void DisableConsole()
    {
        if (!consoleAllocated)
        {
            return;
        }

        RebuildLogger(withConsole: false);
        NativeMethods.FreeConsole();
        consoleAllocated = false;
    }

    [SupportedOSPlatform("windows")]
    private void AllocateConsole()
    {
        _ = NativeMethods.AllocConsole();
        Console.Title = "StreamCompanion logs";
        if (Console.LargestWindowWidth > 0)
        {
            Console.WindowWidth = Console.LargestWindowWidth - Convert.ToInt32(Console.LargestWindowWidth / 3);
        }
    }

    private void RebuildLogger(bool withConsole)
    {
        lock (pipelineLock)
        {
            Logger replacement = BuildLogger(withConsole);
            Logger retired = serilogLogger;
            serilogLogger = replacement;
            retired?.Dispose();
        }
    }

    // TODO(remove after release or two): only cleans pre-Serilog logs.
    private void CleanupLegacyLogs()
    {
        try
        {
            DateTime deleteDateThreshold = DateTime.UtcNow.AddDays(-settings.Get<int>(LogsRetentionDays));
            foreach (string logFilePath in Directory.GetFiles(logsDirectory, "????-??-??.txt"))
            {
                if (File.GetLastWriteTimeUtc(logFilePath) < deleteDateThreshold)
                {
                    File.Delete(logFilePath);
                }
            }
        }
        catch { /* best-effort */ }
    }

    public void Shutdown()
    {
        settings.SettingUpdated -= OnSettingUpdated;
        serilogLogger?.Dispose();
        serilogLogger = null;
    }

    public void Log(object logMessage, LogLevel logLevel, string pluginName, params string[] vals)
    {
        (string message, string prefix) = GetPrefix(logMessage.ToString(), logLevel, pluginName);
        InternalLog(logMessage is Exception ? logMessage : message, logLevel, prefix, vals);
    }

    private (string NewMessage, string Prefix) GetPrefix(string logMessage, LogLevel logLevel, string pluginName)
    {
        string message = logMessage.ToString();
        string prefix = string.Empty;

        while (message.StartsWith(">"))
        {
            prefix += "\t";
            message = message.Substring(1);
        }

        pluginName = pluginName != null ? $" [{pluginName,20}]" : string.Empty;
        prefix = string.Format(@"[{0}] {1:T}{2} - {3}", logLevel.ToString().Substring(0, 3), DateTime.Now, pluginName, prefix);

        return (message, prefix);
    }

    protected void InternalLog(object logMessage, LogLevel logLevel, string messagePrefix, params string[] vals)
    {
        if (logMessage is string message)
        {
            MessageTemplate parsedTemplate = messageTemplateParser.Parse(messagePrefix + message);
            LogEventProperty[] positionalProperties = new LogEventProperty[vals.Length];
         
            for (int valueIndex = 0; valueIndex < vals.Length; valueIndex++)
            {
                positionalProperties[valueIndex] = new LogEventProperty(valueIndex.ToString(), new ScalarValue(vals[valueIndex]));
            }

            serilogLogger?.Write(new LogEvent(DateTimeOffset.Now, (LogEventLevel)(int)logLevel, null, parsedTemplate, positionalProperties));
        }
        else if (logMessage is Exception exception)
        {
            string text = exception.ToString();
            
            if (!contextData.IsEmpty)
            {
                text += Environment.NewLine + string.Join(Environment.NewLine,
                    contextData.Select(pair => $"{pair.Key}: {pair.Value}"));
            }

            serilogLogger?.Write((LogEventLevel)(int)logLevel, exception, "{Message}", text);
        }
        else
        {
            serilogLogger?.Write((LogEventLevel)(int)logLevel, "{Message}", logMessage.ToString());
        }
    }

    public void Log(object logMessage, LogLevel logLevel, params string[] vals)
    {
        (string message, string prefix) = GetPrefix(logMessage.ToString(), logLevel, null);
        InternalLog(logMessage is Exception ? logMessage : message, logLevel, prefix, vals);
    }

    public void SetContextData(string key, string value) => contextData[key] = value;
}
