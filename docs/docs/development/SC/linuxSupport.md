# State of Linux support

Starting from version v210206.19, StreamCompanion started using .NET 5.0, which made it impossible to run it under any linux system.

There are plans to change this, but no ETA. Support roadmap is as follows:

* Fixes to allow running StreamCompanion as a x64/AnyCpu application.
* Replacing any Windows-only components in base application and its plugins, mainly winForms and WPF.

At that point it should be possible to run StreamCompanion as a native .NET5(6?) application under Linux.
