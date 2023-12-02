import{_ as s,r as l,o as i,c as r,a as e,b as t,d as n,e as a}from"./app-6869e31e.js";const d={},c=a('<h1 id="in-game-overlays" tabindex="-1"><a class="header-anchor" href="#in-game-overlays" aria-hidden="true">#</a> In-game overlays</h1><p>Provided as a separate plugins, these can display pretty much anything inside the game itself.</p><p>StreamCompanion has 2 in-game overlay plugins:</p><ul><li><a href="#browser-overlay">Browser overlay</a></li><li><a href="#text-overlay">Text overlay</a></li></ul><h2 id="browser-overlay" tabindex="-1"><a class="header-anchor" href="#browser-overlay" aria-hidden="true">#</a> Browser overlay</h2><p>Used to display any web overlay in-game.</p>',6),u=e("strong",null,"Step 0:",-1),h={href:"https://github.com/Piotrekol/StreamCompanion/releases/latest/",target:"_blank",rel:"noopener noreferrer"},p=e("code",null,"StreamCompanion-browserOverlay.exe",-1),g=e("li",null,[e("strong",null,"Step 1:"),t(" On first startup it will download around 120MB of additional required assets.")],-1),y=e("li",null,[e("strong",null,"Step 2:"),t(" In settings, under "),e("code",null,"Browser overlay"),t(" tab select overlay that you want to display from the list")],-1),m=e("li",null,[e("strong",null,"Step 3:"),t(" Apply recommended canvas settings")],-1),_=e("li",null,[e("strong",null,"Step 4:"),t(" Adjust x/y position values depending on where you want it located on the screen.")],-1),v=e("p",null,[t("Repeat steps 2-5 for as many web overlays you want. Keep in mind however that running too big(total canvas size) overlay will result in a noticeable game performance penalty."),e("br"),e("small",null,"There is an upper limit of 16 ingame overlays.")],-1),f=e("div",{class:"custom-container tip"},[e("p",{class:"custom-container-title"},"TIP: Increase canvas size if your overlay is not fully visible")],-1),b=e("h2",{id:"text-overlay",tabindex:"-1"},[e("a",{class:"header-anchor",href:"#text-overlay","aria-hidden":"true"},"#"),t(" Text overlay")],-1),w=e("p",null,"Used to display simple text content generated by Output patterns. This may be enough if you don't need anything fancy.",-1),x=e("strong",null,"Step 0:",-1),k={href:"https://github.com/Piotrekol/StreamCompanion/releases/latest/",target:"_blank",rel:"noopener noreferrer"},S=e("code",null,"StreamCompanion-textOverlay.exe",-1),T=e("li",null,[e("strong",null,"Step 1:"),t(" In settings, under "),e("code",null,"Output patterns"),t(" tab you'll see new checkbox added under "),e("code",null,"Preview"),t(", controlling which pattern should be displayed in-game.")],-1),I=e("li",null,[e("strong",null,"Step 2:"),t(" Upon checking that, You'll be able to configure basic display options like used font, size or color of the text.")],-1),C=e("li",null,[e("strong",null,"Step 3:"),t(" To see any of your changes reflected in-game you need to save your pattern, change song in osu! and wait up to 5 seconds.")],-1),B=a('<h2 id="troubleshooting" tabindex="-1"><a class="header-anchor" href="#troubleshooting" aria-hidden="true">#</a> Troubleshooting</h2><div class="custom-container warning"><p class="custom-container-title">These overlays work with some, but not all other overlays</p><p>Known <strong>working</strong>:<br> Discord, Steam, Nvidia overlay</p><p>Known <strong>not working</strong> (either doesn&#39;t show anything or crashes osu!):<br> Rivatuner OSD, reshade</p></div><p>General checklist:</p><ul><li>Try restarting your PC.</li><li>Try running BOTH StreamCompanion and osu! as administrator.</li></ul><p>In case overlay still fails to display anything you can check what other applications are trying to interact with osu!. Restart StreamCompanion with logging level set to <code>Trace</code> and start osu! in order to create a list of unknown files loaded in osu! in <code>Files/Logs/</code>.<br> Googling these files individually will most of the time give you a good idea what application it is from. Try disabling some/all of these, and seeing if that changes anything.</p>',5);function O(P,N){const o=l("ExternalLinkIcon");return i(),r("div",null,[c,e("ul",null,[e("li",null,[u,t(" Install it using latest "),e("a",h,[p,n(o)]),t(" or unpack it in plugins folder using portable version.")]),g,y,m,_]),v,f,b,w,e("ul",null,[e("li",null,[x,t(" Install it using latest "),e("a",k,[S,n(o)]),t(" or unpack it in plugins folder using portable version.")]),T,I,C]),B])}const z=s(d,[["render",O],["__file","in-game-overlays.html.vue"]]);export{z as default};
