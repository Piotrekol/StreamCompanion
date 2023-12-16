import{_ as s,r as o,o as r,c as l,a as e,b as t,d as n,w as d,e as c}from"./app-c88ab080.js";const u={},h=c('<h1 id="configuration" tabindex="-1"><a class="header-anchor" href="#configuration" aria-hidden="true">#</a> Configuration</h1><p>Now that you have StreamCompanion up and running, you&#39;ll need to know how to work with the data it produces.<br> StreamCompanion provides several ways of interacting with the data it creates. Each datapoint is delivered using so-called <code>tokens</code> which can contain all sorts of data - usually it&#39;s a number(map AR, pp) or text(map title), but in some cases that can be whole json object(song selection ranking).</p><p>These tokens can be interacted with in 3 ways:</p><ul><li><a href="#output-patterns">Output patterns</a> - Initial, and most basic way to work with the tokens.</li><li><a href="#web-overlays">Web overlays</a> - Use provided <a href="../development/SC/api">api endpoints</a> to interact with the tokens &amp; data on disk.</li><li><a href="../development/SC/creating-a-plugin">Writing your own plugin(s)</a> - Use built-in plugin system to create custom behaviors and new tokens.</li></ul><h2 id="output-patterns" tabindex="-1"><a class="header-anchor" href="#output-patterns" aria-hidden="true">#</a> Output patterns</h2><p>Idea around output patterns is to have some pre-defined text mixed with token data, that should be displayed whenever osu! is doing specific things (playing, watching, idling in main menu etc..). These can be then read from text files created inside <code>Files</code> folder. Display these directly in osu! using in-game overlay plugin or in obs using OBS plugin.</p><h3 id="settings" tabindex="-1"><a class="header-anchor" href="#settings" aria-hidden="true">#</a> Settings</h3><p>Output patterns can be configured in settings, and that&#39;s where you&#39;ll also find ready to use examples. Click on any of these to edit it, or add new one using <code>Add new</code> button. Quick legend:</p><ul><li><code>File/Command name</code> - Filename of the file created on disk, and name of pattern when used via OBS plugin.</li><li><code>Save event</code> - When this pattern should be displaying its value. If the condition is not met, contents of the file are wiped(empty)</li><li><code>Formatting</code> - This is the text that is generated whenever <code>Save event</code> is matched. <code>token</code>s can be used in there by wrapping them inside two exclamation marks <code>!tokenName!</code>. numeric tokens can be formatted globally in <code>tokens preview</code> settings tab or using syntax described below.</li></ul><h3 id="optional-formatting-syntax" tabindex="-1"><a class="header-anchor" href="#optional-formatting-syntax" aria-hidden="true">#</a> Optional formatting syntax</h3><p>There&#39;s also an optional syntax for formatting and doing simple, in-place calculations with numeric tokens: <code>{(token expression) :(number format)}</code>. Some examples:</p>',11),p=e("li",null,[e("code",null,"{unstableRate :0.00}"),t(" - in-place number formating ensuring that it will always have 2 decimal places.")],-1),m=e("li",null,[e("code",null,"{unstableRate :0.##}"),t(" - same as above except it will have up to 2 decimal places instead.")],-1),g=e("code",null,"{min((time*1000/totalTime)*100,100) :0.0}",-1),f={href:"https://github.com/pieterderycke/Jace/wiki/Standard-Functions",target:"_blank",rel:"noopener noreferrer"},w=e("p",null,[e("strong",null,[t("Remember to click "),e("code",null,"Save"),t(" after editing any output pattern value")]),t(". Closing the settings window or navigating to another pattern will discard your changes.")],-1),y=e("h2",{id:"web-overlays",tabindex:"-1"},[e("a",{class:"header-anchor",href:"#web-overlays","aria-hidden":"true"},"#"),t(" Web overlays")],-1),b=e("p",null,[t("These are small web pages, which read data from StreamCompanion "),e("a",{href:"../development/SC/api"},"api endpoints"),t(".")],-1),v={href:"http://localhost:20727/",target:"_blank",rel:"noopener noreferrer"},_=e("li",null,"Browser source url in OBS - make sure to provide big enough width and height in OBS element settings!",-1),k=e("p",null,[t("Files for all overlays are located under "),e("code",null,"Files/Web/overlays/"),t(" folder. That's also where you can modify or create new ones, however HTML, CSS and JavaScript knowledge is required (unless you don't mind tinkering 😃).")],-1),x=e("div",{class:"custom-container warning"},[e("p",{class:"custom-container-title"},"WARNING"),e("p",null,"When editing built-in overlays, make sure to do so on a renamed copy! Otherwise next StreamCompanion update will overwrite all your changes.")],-1);function S(C,T){const a=o("ExternalLinkIcon"),i=o("RouterLink");return r(),l("div",null,[h,e("ul",null,[p,m,e("li",null,[g,t(" - uses time & totalTime tokens, along with built-in min(a,b) function to determine current map completion percentage. Built-in list of functions can be found "),e("a",f,[t("here"),n(a)]),t(".")])]),w,y,b,e("p",null,[t("Several overlays are ready to use at "),e("a",v,[t("overlays index page"),n(a)]),t(" - click on any overlay name that you would like to use and use the page url as either:")]),e("ul",null,[e("li",null,[n(i,{to:"/guide/in-game-overlays.html#browser-overlay"},{default:d(()=>[t("In-game overlay")]),_:1}),t(".")]),_]),k,x])}const O=s(u,[["render",S],["__file","configuration.html.vue"]]);export{O as default};
