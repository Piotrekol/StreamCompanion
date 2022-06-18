"use strict";(self.webpackChunkStreamCompanion_docs=self.webpackChunkStreamCompanion_docs||[]).push([[65],{565:(n,e,a)=>{a.r(e),a.d(e,{data:()=>t});const t={key:"v-485265c4",path:"/guide/tournament-mode.html",title:"Tournament mode",lang:"en-US",frontmatter:{},excerpt:"",headers:[{level:2,title:"Enabling tournament mode",slug:"enabling-tournament-mode",children:[]},{level:2,title:"Usage",slug:"usage",children:[]}],filePathRelative:"guide/tournament-mode.md",git:{updatedTime:1655574808e3,contributors:[{name:"Piotr Partyka",email:"4990365+Piotrekol@users.noreply.github.com",commits:1}]}}},239:(n,e,a)=>{a.r(e),a.d(e,{default:()=>o});const t=(0,a(6252).uE)('<h1 id="tournament-mode" tabindex="-1"><a class="header-anchor" href="#tournament-mode" aria-hidden="true">#</a> Tournament mode</h1><p>While in tourney mode, StreamCompanion supports reading and processing data from multiple tournament clients.</p><h2 id="enabling-tournament-mode" tabindex="-1"><a class="header-anchor" href="#enabling-tournament-mode" aria-hidden="true">#</a> Enabling tournament mode</h2><ul><li><strong>Step 1:</strong> In <code>settings.ini</code> located in StreamCompanion install directory set <code>TournamentMode</code> to true</li><li><strong>Step 2:</strong> Start and stop StreamCompanion for it to create 2 additional settings in <code>settings.ini</code>: <ul><li><code>TournamentClientCount</code> - number of active player clients</li><li><code>TournamentDataClientId</code> - 0-indexed id of the client used as source of truth for map events</li></ul></li></ul><h2 id="usage" tabindex="-1"><a class="header-anchor" href="#usage" aria-hidden="true">#</a> Usage</h2><p>Usage of StreamCompanion while in tourney mode doesn&#39;t differ from normal mode, with one exception being live token names.<br> Because we are now using multiple clients, we need separate tokens to preserve all this data.<br><strong>Live</strong> tokens name format while in tourney mode are as follows: <code>client_&lt;Id&gt;_&lt;tokenName&gt;</code>.<br> for example: <code>client_0_combo</code>, <code>client_2_ppIfMapEndsNow</code>, <code>client_3_unstableRate</code></p><p>In tournament specific web overlays this can be easily handled by using helper method for retrieving tokens:</p><div class="language-javascript ext-js line-numbers-mode"><pre class="language-javascript"><code><span class="token comment">//Using SC template web overlay as a base:</span>\n<span class="token keyword">const</span> <span class="token function-variable function">getTourneyToken</span> <span class="token operator">=</span> <span class="token punctuation">(</span><span class="token parameter">clientId<span class="token punctuation">,</span> tokenName<span class="token punctuation">,</span> decimalPlaces</span><span class="token punctuation">)</span> <span class="token operator">=&gt;</span> <span class="token function">getToken</span><span class="token punctuation">(</span><span class="token template-string"><span class="token template-punctuation string">`</span><span class="token string">client_</span><span class="token interpolation"><span class="token interpolation-punctuation punctuation">${</span>clientId<span class="token interpolation-punctuation punctuation">}</span></span><span class="token string">_</span><span class="token interpolation"><span class="token interpolation-punctuation punctuation">${</span>tokenName<span class="token interpolation-punctuation punctuation">}</span></span><span class="token template-punctuation string">`</span></span><span class="token punctuation">,</span> decimalPlaces<span class="token punctuation">)</span><span class="token punctuation">;</span>\n</code></pre><div class="line-numbers"><span class="line-number">1</span><br><span class="line-number">2</span><br></div></div>',8),s={},o=(0,a(3744).Z)(s,[["render",function(n,e){return t}]])},3744:(n,e)=>{e.Z=(n,e)=>{for(const[a,t]of e)n[a]=t;return n}}}]);