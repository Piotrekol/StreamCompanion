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
    let rws = new ReconnectingWebSocket("ws://localhost:28390/tokens", null, {
        automaticOpen: false,
    });
    rws.onopen = () => {
        rws.send(JSON.stringify(tokenList))
    };
    rws.onmessage = (eventData) => {
        onTokensUpdated(JSON.parse(eventData.data));
    };

    rws.open();
    return rws;
}

function watchTokensVue(tokenList, vueThis) {
    return watchTokens(tokenList, (tokens) => {
        mergeObjects(vueThis, vueThis.tokens, tokens);
    });
}