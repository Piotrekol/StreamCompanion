# Creating plugins

::: tip
This is written in a tutorial-like style. Skip to next section for raw documentation.
:::

## Basic template

Create new .net standard project in Visual Studio or using command line:

```sh
dotnet new classlib -f netstandard2.1
```

Open csproj file using Visual Studio and double click on your project name to open its configuration. Inside of it add reference to newest [StreamCompanionTypes nuget package](https://www.nuget.org/packages/StreamCompanionTypes/):
@[code xml{5,7-9}](./apiExamples/plugin/1.csproj)

Rename `Class1` that got created by default to some meaningful name(`MyPlugin`) and implement base `IPlugin` interface:
@[code csharp](./apiExamples/plugin/1.cs)
At this point this project could get compiled and ran by StreamCompanion, but what is the point if it does nothing? Lets make it log something at startup:
@[code csharp{2,3,13-16}](./apiExamples/plugin/2.cs)
Our plugin now requests `ILogger` **Service** at startup from StreamCompanion and uses it to log our message.  
Lets see it in action: Build whole solution, copy everything from `bin\Debug\netstandard2.1` in solution folder to StreamCompanion `plugins` folder. It should be loaded along with log message logged.  
That's cool and all but this copying and manual running will get old and annoying really quick - so we need to automate things a bit.

## Testing enviroment

Create an empty folder with 2 directories inside:

* `newTestPlugin` - folder with your plugin project, its name doesn't matter.
* `SCInstall` - folder with installed/portable SC. Existing installation can be just copied over.

Add `OutputPath` to your project configuration:
@[code xml{6}](./apiExamples/plugin/2.csproj)
Inside your solution folder create `Properties` folder with `launchSettings.json` inside and populate it with:
@[code json{3}](./apiExamples/plugin/launchSettings.json)
Replace `newTestPlugin` with name of your project(not class name!)  

With that done, your plugin can be now easily tested and debugged without ever leaving Visual Studio - Start debugging (Debug->Start debugging at the top menu) to test any changes.

::: tip
Project with everything mentioned so far can be found [here](/misc/PluginProject.zip) and can be used as a template. Remember to change namespace and plugin class name!
:::

## Interacting with events

`CreateTokensAsync`(from [`ITokensSource`](#itokenssource)) & `SetNewMapAsync`(from [`IMapDataConsumer`](#imapdataconsumer)) are 2 hooks you'll most likely want to use. Code below demonstrates how to:

* request multiple services from SC and store these for later use (lines 19-27)
* create&update tokens (lines 31-41).
* store persistent settings between runs (lines 22 and 38).
* use final event data(tokens/map search result) (lines 43-55).

@[code csharp](./apiExamples/plugin/3.cs)

For more understanding when these methods are executed proceed to next section.
