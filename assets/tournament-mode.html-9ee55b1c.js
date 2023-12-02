import{_ as n,o as e,c as a,e as t}from"./app-6869e31e.js";const s={},o=t('<h1 id="tournament-mode" tabindex="-1"><a class="header-anchor" href="#tournament-mode" aria-hidden="true">#</a> Tournament mode</h1><p>While in tourney mode, StreamCompanion supports reading and processing data from multiple tournament clients.</p><h2 id="enabling-tournament-mode" tabindex="-1"><a class="header-anchor" href="#enabling-tournament-mode" aria-hidden="true">#</a> Enabling tournament mode</h2><ul><li><strong>Step 1:</strong> In <code>settings.ini</code> located in StreamCompanion install directory set <code>TournamentMode</code> to true</li><li><strong>Step 2:</strong> Start and stop StreamCompanion for it to create 2 additional settings in <code>settings.ini</code>: <ul><li><code>TournamentClientCount</code> - number of active player clients</li><li><code>TournamentDataClientId</code> - 0-indexed id of the client used as source of truth for map events</li></ul></li></ul><h2 id="usage" tabindex="-1"><a class="header-anchor" href="#usage" aria-hidden="true">#</a> Usage</h2><p>Usage of StreamCompanion while in tourney mode doesn&#39;t differ from normal mode, with one exception being live token names.<br> Because we are now using multiple clients, we need separate tokens to preserve all this data.<br><strong>Live</strong> tokens name format while in tourney mode are as follows: <code>client_&lt;Id&gt;_&lt;tokenName&gt;</code>.<br> for example: <code>client_0_combo</code>, <code>client_2_ppIfMapEndsNow</code>, <code>client_3_unstableRate</code></p><p>In tournament specific web overlays this can be easily handled by using helper method for retrieving tokens:</p><div class="language-javascript line-numbers-mode" data-ext="js"><pre class="language-javascript"><code><span class="token comment">//Using SC template web overlay as a base:</span>\n<span class="token keyword">const</span> <span class="token function-variable function">getTourneyToken</span> <span class="token operator">=</span> <span class="token punctuation">(</span><span class="token parameter">clientId<span class="token punctuation">,</span> tokenName<span class="token punctuation">,</span> decimalPlaces</span><span class="token punctuation">)</span> <span class="token operator">=&gt;</span> <span class="token function">getToken</span><span class="token punctuation">(</span><span class="token template-string"><span class="token template-punctuation string">`</span><span class="token string">client_</span><span class="token interpolation"><span class="token interpolation-punctuation punctuation">${</span>clientId<span class="token interpolation-punctuation punctuation">}</span></span><span class="token string">_</span><span class="token interpolation"><span class="token interpolation-punctuation punctuation">${</span>tokenName<span class="token interpolation-punctuation punctuation">}</span></span><span class="token template-punctuation string">`</span></span><span class="token punctuation">,</span> decimalPlaces<span class="token punctuation">)</span><span class="token punctuation">;</span>\n</code></pre><div class="line-numbers" aria-hidden="true"><div class="line-number"></div><div class="line-number"></div></div></div>',8),i=[o];function c(p,l){return e(),a("div",null,i)}const d=n(s,[["render",c],["__file","tournament-mode.html.vue"]]);export{d as default};
