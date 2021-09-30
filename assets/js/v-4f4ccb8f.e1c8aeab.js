"use strict";(self.webpackChunkStreamCompanion_docs=self.webpackChunkStreamCompanion_docs||[]).push([[980],{6856:(e,t,n)=>{n.r(t),n.d(t,{data:()=>a});const a={key:"v-4f4ccb8f",path:"/guide/configuration.html",title:"Configuration",lang:"en-US",frontmatter:{},excerpt:"",headers:[{level:2,title:"Output patterns",slug:"output-patterns",children:[{level:3,title:"Settings",slug:"settings",children:[]},{level:3,title:"Optional formatting syntax",slug:"optional-formatting-syntax",children:[]}]},{level:2,title:"Web overlays",slug:"web-overlays",children:[]}],filePathRelative:"guide/configuration.md",git:{updatedTime:1633016013e3,contributors:[{name:"Piotrekol",email:"4990365+Piotrekol@users.noreply.github.com",commits:1}]}}},3324:(e,t,n)=>{n.r(t),n.d(t,{default:()=>S});var a=n(6252);const i=(0,a.uE)('<h1 id="configuration" tabindex="-1"><a class="header-anchor" href="#configuration" aria-hidden="true">#</a> Configuration</h1><p>Now that you have StreamCompanion up and running, you&#39;ll need to know how to work with the data it produces.<br> StreamCompanion provides several ways of interacting with the data it creates. Each datapoint is delivered using so-called <code>tokens</code> which can contain all sorts of data - usually it&#39;s a number(map AR, pp) or text(map title), but in some cases that can be whole json object(song selection ranking).</p><p>These tokens can be interacted with in 3 ways:</p><ul><li><a href="#output-patterns">Output patterns</a> - Initial, and most basic way to work with the tokens.</li><li><a href="#web-overlays">Web overlays</a> - Use provided <a href="/development/SC/api">api endpoints</a> to interact with the tokens &amp; data on disk.</li><li><a href="/development/SC/creating-plugin">Writing your own plugin(s)</a> - Use built-in plugin system to create custom behaviors and new tokens.</li></ul><h2 id="output-patterns" tabindex="-1"><a class="header-anchor" href="#output-patterns" aria-hidden="true">#</a> Output patterns</h2><p>Idea around output patterns is to have some pre-defined text mixed with token data, that should be displayed whenever osu! is doing specific things (playing, watching, idling in main menu etc..). These can be then read from text files created inside <code>Files</code> folder. Display these directly in osu! using in-game overlay plugin or in obs using OBS plugin.</p><h3 id="settings" tabindex="-1"><a class="header-anchor" href="#settings" aria-hidden="true">#</a> Settings</h3><p>Output patterns can be configured in settings, and that&#39;s where you&#39;ll also find ready to use examples. Click on any of these to edit it, or add new one using <code>Add new</code> button. Quick legend:</p><ul><li><code>File/Command name</code> - Filename of the file created on disk, and name of pattern when used via OBS plugin.</li><li><code>Save event</code> - When this pattern should be displaying its value. If the condition is not met, contents of the file are wiped(empty)</li><li><code>Formatting</code> - This is the text that is generated whenever <code>Save event</code> is matched. <code>token</code>s can be used in there by wrapping them inside two exclamation marks <code>!tokenName!</code>. numeric tokens can be formatted globally in <code>tokens preview</code> settings tab or using syntax described below.</li></ul><h3 id="optional-formatting-syntax" tabindex="-1"><a class="header-anchor" href="#optional-formatting-syntax" aria-hidden="true">#</a> Optional formatting syntax</h3><p>There&#39;s also an optional syntax for formatting and doing simple, in-place calculations with numeric tokens: <code>{(token expression) :(number format)}</code>. Some examples:</p>',11),o=(0,a._)("li",null,[(0,a._)("code",null,"{unstableRate :0.00}"),(0,a.Uk)(" - in-place number formating ensuring that it will always have 2 decimal places.")],-1),l=(0,a._)("li",null,[(0,a._)("code",null,"{unstableRate :0.##}"),(0,a.Uk)(" - same as above except it will have up to 2 decimal places instead.")],-1),r=(0,a._)("code",null,"{min((time*1000/totalTime)*100,100) :0.0}",-1),s=(0,a.Uk)(" - uses time & totalTime tokens, along with built-in min(a,b) function to determine current map completion percentage. Built-in list of functions can be found "),d={href:"https://github.com/pieterderycke/Jace/wiki/Standard-Functions",target:"_blank",rel:"noopener noreferrer"},u=(0,a.Uk)("here"),c=(0,a.Uk)("."),h=(0,a._)("p",null,[(0,a._)("strong",null,[(0,a.Uk)("Remember to click "),(0,a._)("code",null,"Save"),(0,a.Uk)(" after editing any output pattern value")]),(0,a.Uk)(". Closing the settings window or navigating to another pattern will discard your changes.")],-1),p=(0,a._)("h2",{id:"web-overlays",tabindex:"-1"},[(0,a._)("a",{class:"header-anchor",href:"#web-overlays","aria-hidden":"true"},"#"),(0,a.Uk)(" Web overlays")],-1),m=(0,a._)("p",null,[(0,a.Uk)("These are small web pages, which read data from StreamCompanion "),(0,a._)("a",{href:"/development/SC/api"},"api endpoints"),(0,a.Uk)(".")],-1),g=(0,a.Uk)("Several overlays are ready to use at "),f={href:"http://localhost:20727/",target:"_blank",rel:"noopener noreferrer"},w=(0,a.Uk)("overlays index page"),k=(0,a.Uk)(" - click on any overlay name that you would like to use and use the page url as either:"),b=(0,a.Uk)("In-game overlay"),y=(0,a.Uk)("."),v=(0,a._)("li",null,"Browser source url in OBS - make sure to provide big enough width and height in OBS element settings!",-1),_=(0,a._)("p",null,[(0,a.Uk)("Files for all overlays are located under "),(0,a._)("code",null,"Files/Web/overlays/"),(0,a.Uk)(" folder. That's also where you can modify or create new ones, however HTML, CSS and JavaScript knowledge is required (unless you don't mind tinkering 😃).")],-1),x=(0,a._)("div",{class:"custom-container warning"},[(0,a._)("p",{class:"custom-container-title"},"WARNING"),(0,a._)("p",null,"When editing built-in overlays, make sure to do so on a renamed copy! Otherwise next StreamCompanion update will overwrite all your changes.")],-1),S={render:function(e,t){const n=(0,a.up)("OutboundLink"),S=(0,a.up)("RouterLink");return(0,a.wg)(),(0,a.iD)(a.HY,null,[i,(0,a._)("ul",null,[o,l,(0,a._)("li",null,[r,s,(0,a._)("a",d,[u,(0,a.Wm)(n)]),c])]),h,p,m,(0,a._)("p",null,[g,(0,a._)("a",f,[w,(0,a.Wm)(n)]),k]),(0,a._)("ul",null,[(0,a._)("li",null,[(0,a.Wm)(S,{to:"/guide/in-game-overlays.html#browser-overlay"},{default:(0,a.w5)((()=>[b])),_:1}),y]),v]),_,x],64)}}}}]);