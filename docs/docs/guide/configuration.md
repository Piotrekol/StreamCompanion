# Configuration

Now that you have StreamCompanion up and running, you'll need to know how to work with the data it produces.  
StreamCompanion provides several ways of interacting with the data it creates. Each datapoint is delivered using so-called `tokens` with can contain all sorts of data - usually it's a number(map AR, pp) or text(map title), but in some cases that can be whole json object(song selection ranking).  

These tokens can be interacted with in 3 ways:

* [Output patterns](#output-patterns) - Initial, and most basic way to work with the tokens.
* [Web overlays](#web-overlays) - Use provided [api endpoints](/development/SC/api) to interact with the tokens & data on disk.
* [Writing your own plugin(s)](/development/SC/creating-plugin) - Use built-in plugin system to create custom behaviors and new tokens.

## Output patterns

Idea around output patterns is to have some pre-defined text mixed with token data, that should be displayed whenever osu! is doing specific things (playing, watching, idling in main menu etc..). These can be then read from text files created inside `Files` folder. Display these directly in osu! using in-game overlay plugin or in obs using OBS plugin.

### Settings

Output patterns can be configured in settings, and that's where you'll also find ready to use examples. Click on any of these to edit it, or add new one using `Add new` button.
Quick legend:  

* `File/Command name` - Filename of the file created on disk, and name of pattern when used via OBS plugin.
* `Save event` - When this pattern should be displaying its value. If the condition is not met, contents of the file are wiped(empty)
* `Formatting` - This is the text that is generated whenever `Save event` is matched. `token`s can be used in there by wrapping them inside two exclamation marks `!tokenName!`. numeric tokens can be formatted globally in `tokens preview` settings tab or using syntax described below.  

### Optional formatting syntax

There's also an optional syntax for formatting and doing simple, in-place calculations with numeric tokens: `{(token expression) :(number format)}`. Some examples:

* `{unstableRate :0.00}` - in-place number formating ensuring that it will always have 2 decimal places.
* `{unstableRate :0.##}` - same as above except it will have up to 2 decimal places instead.
* `{min((time*1000/totalTime)*100,100) :0.0}` - uses time & totalTime tokens, along with built-in min(a,b) function to determine current map completion percentage. Built-in list of functions can be found [here](https://github.com/pieterderycke/Jace/wiki/Standard-Functions).

**Remember to click `Save` after editing any output pattern value**. Closing the settings window or navigating to another pattern will discard your changes.

## Web overlays

These are small web pages, which read data from StreamCompanion [api endpoints](/development/SC/api).

Several overlays are ready to use at [overlays index page](http://localhost:20727/) - click on any overlay name that you would like to use and use the page url as either:

* [In-game overlay](./in-game-overlays.md#browser-overlay).
* Browser source url in OBS - make sure to provide big enough width and height in OBS element settings!

Files for all overlays are located under `Files/Web/overlays/` folder. That's also where you can modify or create new ones, however HTML, CSS and JavaScript knowledge is required (unless you don't mind tinkering :)).  

::: warning
When editing built-in overlays, make sure to do so on a renamed copy! Otherwise next StreamCompanion update will overwrite all your changes.
:::
