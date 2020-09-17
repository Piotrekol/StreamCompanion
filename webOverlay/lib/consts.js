//this line is automatically updated on SC startup
let autoConfig = {"Scheme":"http","Host":"localhost","Port":28390};

let config = {
    scheme: window.location.protocol.slice(0, -1),
    host: window.location.hostname,
    port: window.location.port,
    getUrl: () => `${config.scheme}://${config.host}:${config.port}`,
    getWs: () => `ws://${config.host}:${config.port}`,
}

osuStatus = Object.freeze({
    Null: 0,
    Listening: 1,
    Playing: 2,
    Watching: 8,
    Editing: 16,
    ResultsScreen: 32
})

window.overlay = Object.freeze({
    osuStatus, config
})
