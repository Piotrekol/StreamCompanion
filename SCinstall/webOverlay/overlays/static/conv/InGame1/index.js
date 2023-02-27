let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");
let wrapper = document.getElementById('wrapper');
let ifFcpp = document.getElementsByClassName('ifFcpp')[0];

socket.onopen = () => console.log("Successfully Connected");
socket.onclose = event => {
    console.log("Socket Closed Connection: ", event);
    socket.send("Client Closed!");
};
socket.onerror = error => console.log("Socket Error: ", error);

let animation = {
    pp: new CountUp('pp', 0, 0, 0, 0.5, { decimalPlaces: 2, useEasing: true, useGrouping: false, separator: " ", decimal: "." }),
    ifFcpp: new CountUp('ifFcpp', 0, 0, 0, 0.5, { decimalPlaces: 2, useEasing: true, useGrouping: false, separator: " ", decimal: "." }),
    hun: new CountUp('hun', 0, 0, 0, 0.5, { decimalPlaces: 2, useEasing: true, useGrouping: false, separator: " ", decimal: "." }),
    fiv: new CountUp('fiv', 0, 0, 0, 0.5, { decimalPlaces: 2, useEasing: true, useGrouping: false, separator: " ", decimal: "." }),
    miss: new CountUp('miss', 0, 0, 0, 0.5, { decimalPlaces: 2, useEasing: true, useGrouping: false, separator: " ", decimal: "." }),
};

let tempState

socket.onmessage = event => {
    let data = event.data

    if (data.menu.state !== tempState) {
        tempState = data.menu.state
        if (tempState !== 2) {
            wrapper.style.transform = "translateX(-110%)"
        } else {
            wrapper.style.transform = "translateX(0)"
        }

    }
    if (data.gameplay.pp.current !== '' && data.gameplay.pp.current !== 0) {
        animation.pp.update(data.gameplay.pp.current)
    } else {
        animation.pp.update(0)
    }
    if (data.gameplay.pp.fc !== '' && data.gameplay.pp.fc !== 0) {
        animation.ifFcpp.update(data.gameplay.pp.fc)
    } else {
        animation.ifFcpp.update(0)
    }
    if (data.gameplay.hits[100] > 0) {
        animation.hun.update(data.gameplay.hits[100])
    } else {
        animation.hun.update(0)
    }
    if (data.gameplay.hits[50] > 0) {
        animation.fiv.update(data.gameplay.hits[50])
    } else {
        animation.fiv.update(0)
    }
    if (data.gameplay.hits[0] > 0) {
        animation.miss.update(data.gameplay.hits[0])
    } else {
        animation.miss.update(0)
    }

    if (data.gameplay.hits[0] > 0  ||  data.gameplay.hits.sliderBreaks > 0) {
        ifFcpp.style.opacity = 1
    } else {
        ifFcpp.style.opacity = 0
    }
}