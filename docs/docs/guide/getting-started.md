# Getting started

## Prerequisites

* [.NET 5.0.x __x86__ Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/5.0/runtime)
  * [not sure with one to download?](/images/guide/netRuntimeDownload.png)

## Installation steps

1. StreamCompanion can be either installed or used as a portable application. Currently auto-updates are supported only for installed versions, while portable one will only notify you about new updates.  

    __Installer__: Grab latest setup exe from [github releases page][dlLink], and proceed with installation. Windows might complain about setup being unsigned - you can either bypass this window or compile it yourself using [these steps][compileSC].  
    __Portable__: Grab latest portable zip package from [github releases page][dlLink] and unpack it in any non-system folder where you have read&write permissions.

2. On first startup you will be presented with additional setup step, to ensure that essential part of StreamCompanion - with is osu! memory reading - is functioning properly with your system configuration. Select __requested beatmap__ and __exact mods__ in-game and start playing. At that point you can minimize osu!(it will pause), and setup should finish shortly.  

::: tip
If for whatever reason it haven't completed successfully, ensure that when you change beatmap in osu! value highlighted below updates.  
![Memory setup][memoryDebugInfo]  

If it stays on default `---` there is a good chance that SC is getting sandboxed by your antivirus, or osu! is being ran with administrative privileges.
:::

[memoryDebugInfo]: <./images/SCSetupMemoryStatus.png>
[netRuntimeDownload]: <./images/netRuntimeDownload.png>
[compileSC]: </development/SC/#i-just-want-to-compile-it-myself>
[dlLink]: <https://github.com/Piotrekol/StreamCompanion/releases/latest>
