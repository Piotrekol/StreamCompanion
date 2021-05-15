Number.prototype.pad = function (size) {
    var s = String(this);
    while (s.length < (size || 2)) { s = "0" + s; }
    return s;
}
Number.prototype.clamp = function(min, max) {
    return Math.min(Math.max(this, min), max);
  };
  
function mergeObjects(vueThis, target, source) {
    for (const [key, value] of Object.entries(source)) {
        if (target.hasOwnProperty(key))
            target[key] = value;
        else
            vueThis.$set(target, key, value);
    }
}
function preloadImage(url, id, cb) {
    let img = new Image();
    img.onload = () => cb(url, id);
    img.src = url;
}

function watchTokens(tokenList, onTokensUpdated) {
    let rws = new ReconnectingWebSocket(`${window.overlay.config.getWs()}/tokens`, null, {
        automaticOpen: false,
        reconnectInterval: 3000
    });
    rws.watchedTokens = tokenList;

    rws.onopen = () => {
        rws.send(JSON.stringify(rws.watchedTokens))
    };
    rws.onmessage = (eventData) => {
        onTokensUpdated(JSON.parse(eventData.data));
    };

    rws.AddTokens = (tokens) => {
        rws.watchedTokens = [...new Set([...rws.watchedTokens, ...tokens])];
        if (rws.readyState === 1)
            rws.send(JSON.stringify(rws.watchedTokens))
    }
    rws.open();
    return rws;
}

function watchTokensVue(tokenList, vueThis) {
    return watchTokens(tokenList, (tokens) => {
        mergeObjects(vueThis, vueThis.tokens, tokens);
    });
}
function _GetToken(rws, tokens, tokenName, decimalPlaces) {
    tokens[tokenName];//hack to inform vue3 that we are dependant on tokens object values
    if (tokens.hasOwnProperty(tokenName)) {
        if (decimalPlaces !== undefined && decimalPlaces !== null)
            return Number(tokens[tokenName]).toFixed(decimalPlaces);
        return tokens[tokenName];
    }
    if (rws.watchedTokens.indexOf(tokenName) === -1)
        rws.AddTokens([tokenName]);
    return '';
}

function _GetWebOverlaySettings() {
  return fetch(`${window.overlay.config.getUrl()}/settings`)
    .then((response) => response.json())
    .then((responseData) => JSON.parse(responseData.WebOverlay_Config));
}

function _IsInStatus(rws, tokens, osuStatuses) {
    if (Array.isArray(osuStatuses))
        return osuStatuses.indexOf(_GetToken(rws, tokens, 'status')) > -1;

    return _GetToken(rws, tokens, 'status') == osuStatuses
}

function _IsPlaying(rws, tokens) {
    return _IsInStatus(rws, tokens, [window.overlay.osuStatus.Playing, window.overlay.osuStatus.ResultsScreen]);
}
function _IsWatching(rws, tokens) {
    return _IsInStatus(rws, tokens, window.overlay.osuStatus.Watching);
}