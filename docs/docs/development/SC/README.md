# Building StreamCompanion

## Prerequisites

* [.NET 5.0.x __x86 SDK__](https://dotnet.microsoft.com/download/dotnet/5.0)
* Optional:
  * [Visual Studio Community](https://visualstudio.microsoft.com/pl/thank-you-downloading-visual-studio/?sku=Community). Not required, but recommended for plugin development.
  * [InnoSetup](https://jrsoftware.org/download.php/is.exe). Used for creating Setup files.

## I just want to compile it myself

* **Step 1:** Install .NET SDK mentioned in [prerequisites](#prerequisites) if you haven't already.

* **Step 2:** Navigate to place where you downloaded full StreamCompanion source code ([here](../gettingSource.md)) and run `buildRelease.cmd` inside.

* **Step 3:** After successful build everything should be ready to use inside `build\Release` directory.

* **Optional Step 4:** Copy in-game overlay plugins from `build\Release_browserOverlay` & `build\Release_unsafe` to `build\Release` to have both text and browser in-game overlay plugins available.

## Building solution in Visual Studio

* **Step 1:** When you first open StreamCompanion solution it might have incorrect configuration selected, leading to solution-wide errors. To fix that you need to change configuration to either `Debug/x86` or `Release/x86`  
![VSError][VSError]
<!--TODO: this will be fixed(and this step removed) whenever SC migrates to using AnyCPU configuration-->

* **Step 2:** Build whole solution by pressing <kbd>CTRL+SHIFT+B</kbd> or right click on top solution node and select `build`.

* **Step 3:** StreamCompanion can be now ran either from `build` folder or from Visual Studio with full debugger capability across plugins if it was built in `Debug` configuration.

[VSError]: <./images/VisualStudioSolutionError.png>
