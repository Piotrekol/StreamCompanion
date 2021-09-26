# Types project rundown

## StreamCompanionTypes.DataTypes

Contains all base types used by StreamCompanion.

### `Tokens`

Maybe most important of all these, this is central storage class for all tokens in StreamCompanion.  
Tokens from other plugins can be accessed there from `AllTokens` prop.  
Use `Tokens.CreateTokenSetter(pluginName)` to create method for creating and updating tokens.  
Tokens can be lazily-created by providing values as `Lazy<object>`.

### `Beatmap`

Contains most of beatmap-related data. Only thing to note here is that not all props will be filled, notably:

* mapId/mapSetId - these are not provided in old osu diff files(2007~2009?), and should be retrieved using `mapid` and `mapsetid` tokens.
* State - map ranked status, not stored at all in osu diff files, use `rankedStatus` token.
* StarRating - star rating values will be provided only for nomod and currently selected mod combination, in current play mode.

### `ConfigEntry`

Small class for defining key/value setting pairs.  
Creating an instance of this class by itself does nothing. To save your default setting in file you should access it via `ISettings`, by either getting its value `ISettings.Get<T>(configEntry)` or modifying it `ISettings.Add(settingKey, settingValue)`.

### `MapSearchArgs`

Arguments which are/were used for finding current beatmap data.

### `MapSearchResult`

"Final" search object that gets dispatched to consumer plugins. Most important props:

* `BeatmapsFound` - array of beatmap objects returned from search. This is somewhat relic of the past - currently array will contain either 0 or 1 map (in the MSN-based searcher days it used to return multiple, hence the array).
* `Mods` - Raw mods as well as its string representation for user and processing.
* `Action` - What was osu! doing when this event was invoked. (Playing/Watching/Listening...)
* `PlayMode` - What play mode was selected in the song selection screen in osu!.
* `SharedObjects` - Misc objects shared between core plugins.

### StreamCompanionTypes.Interfaces

### `IPlugin`

Base marker interface indicating that current class is an entry point of a StreamCompanion plugin. One assembly can contain multiple of these.

### `IMapDataFinder`

Provides a method for searching beatmap data based on data inside `MapSearchArgs`. Finders will be executed in order, based on `Priority`.

### `IFirstRunControl` & `IFirstRunControlProvider`

Used for creating StreamCompanion first run steps.

### StreamCompanionTypes.Interfaces.Consumers

### `IMapDataConsumer`

Provides a method/hook for executing any plugin-specific actions at the end of events. Map data, all tokens & output patterns are created/lazily-created, and can be freely consumed.

### `IHighFrequencyDataConsumer`

Used for passing frequently updating(live tokens) data around.  
This one can be both implemented if you want to add another output source, and consumed (as `IList<IHighFrequencyDataConsumer>`) if you need to send data.  
Data is sent via MMF(Memory-mapped files), TCP(when enabled in settings) & web socket output patterns endpoint.

### StreamCompanionTypes.Interfaces.Sources

### `ITokensSource`

Provides a method in which you should do all logic related to creating or updating non-live tokens for current map search results.

### `IOsuEventSource`

Provides an event which should get fired whenever your plugin detects new actions in osu!. There should be no cases where you should be using this, unless you are implementing new osu! event source (osu!lazer? :eyes:)

### `IOutputPatternSource`
Provides a method for creating output patterns. At that point map data and all tokens for current event are updated and can be used.

### `ISettingsSource`

Provides a way of creating settings GUI.  
Object returned from `GetUiSettings()` should be a winForm `UserControl`.

### StreamCompanionTypes.Interfaces.Services

Implementations for these interfaces are provided by StreamCompanion, and can be retrieved by specifying these in your plugin constructor.

### `IModParser`

Mods converting utils.

### `ISaver`

Save data in text files. Also provides `SaveDirectory` where custom files should be saved - create directory inside if needed.

### `ISettings`

Used to retrieve and store user settings.

### `ILogger` & `IContextAwareLogger`

Basic level-based logger. Context data set in `IContextAwareLogger.SetContextData` is logged with thrown or logged exceptions.

### `IDifficultyCalculator`

Utility class for calculating basic map stats changes based on mods.

### `Delegates`

Safe way to shutdown or restart StreamCompanion via method call. Request individual methods in your plugin constructor.

### `IMapDataSaver` & `IDatabaseController`

**Not used anymore, will be removed in future versions**. Was used to store beatmaps loaded from osu! database file in local, easily searchable database.

### `IMainWindowModel`

**Will be removed in future versions**. StreamCompanion main window specific interface used for updating data.
