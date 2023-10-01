# HTTP API

::: tip
All example urls in this section require you to have StreamCompanion running, with some song selected.
:::

::: tip
By default all connections to api are restricted to browser requests from local computer (localhost).  
That can be changed by enabling **remote access** in `Web overlay` settings tab.
:::

## WebSocket endpoints

### `tokens`

WebSocket stream of requested tokens, with can be changed at any point by sending message with serialized JArray, containing case sensitive token names

#### Code example

Minimal working JavaScript example looks like this:
@[code js](./apiExamples/minimalWS.js)
if you execute this code and change songs in osu few times, you'll notice that not all tokens are always sent. This is intentional - messages received contain only changed values.  
To have access to all current token values you need to cache them:
@[code js{5,7}](./apiExamples/minimalWSpt2.js)
By default token values are sent as soon as specific plugin updates it, resulting in several messages containing information about single map/state change.  
As this may be sometimes undesirable, SC can be configured to send token updates once all plugins have completed their work by appending `?bulkUpdates=MainPipeline,LiveTokens` to url:
@[code js{1}](./apiExamples/minimalWSpt3.js)

### `outputPatterns`

WebSocket stream of output patterns, which can be changed at any point by sending message with serialized JArray, containing case sensitive output pattern names

This works in same manner as [tokens](#tokens) endpoint. Same code can be reused by replacing endpoint with `ws://localhost:20727/outputPatterns` and provide output pattern names instead of token names.

## HTTP endpoints

### [`json`](http://localhost:20727/json)

All tokens available in StreamCompanion in form of json object.
::: tip
This endpoint should be used only as a tokens reference. Use these via [tokens](#tokens) WebSocket endpoint in actual implementations.  
**Ignoring this will result in unnecessary resource usage**, as live token values are computed only when used.
:::

::: details Example output
@[code json](./apiExamples/exampleSCOutput.json)
:::

### [`backgroundImage`](http://localhost:20727/backgroundImage)

Current map background image

* Base url with no parameters returns map image file as-is without any processing.
* setting `width` or `height` query parameters ensures that at least one of these will be matched while preserving original image aspect ratio. [check it out](http://localhost:20727/backgroundImage?width=500&height=500)
  * in addition, setting `crop=true` disregards image aspect ratio and returns cropped image with specified dimensions, resizing it beforehand if necessary. [check it out](http://localhost:20727/backgroundImage?width=500&height=500&crop=true)
* Set `cache=true` and SC will set the [**Cache-Control**](https://developer.mozilla.org/docs/Web/HTTP/Headers/Cache-Control) response header to instruct the browser to cache background images. When images are repeatedly loaded, it will save a lot of loading time.
  ::: tip cache usage
  When using caching, it is necessary to add some query parameters to distinguish requests. If no other parameters are added, it will result in your browser always reusing the background image of the first loaded map. For example, you can use Id of the current map to create unique urls:

  `/backgroundImage?cache=true&mapId=123456`
  :::

### [`Songs`](http://localhost:20727/Songs/)

View into user osu! Songs folder

- Use [backgroundImage](#backgroundImage) endpoint for getting current map background instead of navigating to it here.
- This is provided mainly for cases where there is need for additional .osu file processing.
  ::: details dev note
  I would argue this sort of thing should end up as new StreamCompanion plugin or as addition to existing one.
  :::

### [`Skins`](http://localhost:20727/Skins/)

View into user osu! Skins folder

- Combine this with `skin` token value to get access to skin assets user is currently using.

### [`overlayList`](http://localhost:20727/overlayList)

List of available overlays (folder paths)  
Mainly for use on [web overlay index page](http://localhost:20727). Folder is considered an overlay if it is contained somewhere in `Files\Web\overlays` and it has `index.html` file.

### [`settings`](http://localhost:20727/settings)

All StreamCompanion settings
