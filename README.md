
### ![StreamCompanion](images/logo.png)
 **StreamCompanion** is a program directed towards osu! streamers, with functions such as a *pp counter* and *map information*

### Functions 
  - Grabbing song title **anywhere** you are in osu!  
  - A simplistic map and PP web displays
  - Customizable map outputs
  - Key counter - how many times you clicked these 2 osu! keys since...(beginning of the stream?)
  - Play/Retry counter
  - Mods display
  - [osu!Post][osuPost] integration
  - Magic(and more)

### Help!
StreamCompanion has a wiki explaining most of its functionality. [Give it a read][wiki] if you are having problems or wish to know what SC can do.

Any contributions / constructive criticism is appreciated 

### Built in web overlays support
After starting StreamCompanion navigate to http://localhost:28390/ to see default web overlays. All overlays live in `Files/Web/overlays/` folder and you're free to create/edit these as you see fit.  
Default `Live Overlay` can be configured to some extent in settings `Visualizer` tab without having to tinker with js/css.
![WebOverlayPreview](images/webOverlay.png)

## Plugins
StreamCompanion supports and treats plugins as first-class citizen - [majority of its functionality is written using this system](./plugins)  
If you wish to create plugin/use SC for something and have no clue where to start - create a Github issue or ask on [Discord](https://discord.gg/N854wYZ)

## License
This software is licensed under MIT. You can find the full text of the license [here][license].

   [license]: <https://github.com/Piotrekol/StreamCompanion/blob/master/LICENSE>
   [osuPost]: <https://osu.ppy.sh/forum/t/164486>
   [wiki]: <https://github.com/Piotrekol/StreamCompanion/wiki>
