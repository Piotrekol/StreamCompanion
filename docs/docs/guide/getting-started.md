# Getting started

## Prerequisites

* [.NET 6.0.x __x64__ Desktop Runtime](https://aka.ms/dotnet/6.0/windowsdesktop-runtime-win-x64.exe)

## Installation

StreamCompanion can be either installed or used as a portable application. Currently auto-updates are supported only for installed versions, while portable one will only notify you about new updates.  

__Installer__: Grab latest setup exe from [github releases page][dlLink], and proceed with installation. Windows might complain about setup being unsigned - you can either bypass this window or compile it yourself using [these steps][compileSC].  
__Portable__: Grab latest portable zip package from [github releases page][dlLink] and unpack it in any non-system folder where you have read&write permissions.

::: warning
Do not run either osu! or StreamCompanion as administrator. Doing so will result in unexpected behavior (like SC not detecting any songs or ingame overlays not displaying anything)
:::

[compileSC]: </development/SC/#i-just-want-to-compile-it-myself>
[dlLink]: <https://github.com/Piotrekol/StreamCompanion/releases/latest>
