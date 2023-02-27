let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");
let wrapper = document.getElementById('wrapper');
let ifFcpp = document.getElementsByClassName('ifFcpp')[0];
let progress = document.getElementById("progress");

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

let tempState;
let seek;
let fullTime;
let onepart;

socket.onmessage = event => {
    let data = event.data


    if (data.menu.state !== tempState) {
        tempState = data.menu.state
        

    }
    if (data.gameplay.pp.current !== '' && (tempState === 2 || tempState === 7)) {
        animation.pp.update(data.gameplay.pp.current)
    }
    if(data.menu.pp[100] !== '' && (tempState !== 2 && tempState !== 7)){
        animation.pp.update(data.menu.pp[100])
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
    if(fullTime !== data.menu.bm.time.mp3){
        fullTime = data.menu.bm.time.mp3
        onepart = 178/fullTime
    }
    if(seek !== data.menu.bm.time.current && fullTime !== undefined && fullTime != 0){
        seek = data.menu.bm.time.current;
        progress.style.width = onepart*seek+'px'
    }
}