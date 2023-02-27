let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");
let mapid = document.getElementById('mapid');

let bg = document.getElementById("bg");
let title = document.getElementById("title");
let pp = document.getElementById("pp");
let hun = document.getElementById("100");
let fifty = document.getElementById("50");
let miss = document.getElementById("miss");
let progressChart = document.getElementById("progress")


socket.onopen = () => {
    console.log("Successfully Connected");
};

socket.onclose = event => {
    console.log("Socket Closed Connection: ", event);
    socket.send("Client Closed!")
};

socket.onerror = error => {
    console.log("Socket Error: ", error);
};

let tempImg;
let tempTitle;
let tempStrainBase;
let smoothOffset = 2;
let seek;
let fullTime;
let onepart;

socket.onmessage = event => {
    let data = event.data;
    if(tempImg !== data.menu.bm.path.full){
        tempImg = data.menu.bm.path.full
        let img = data.menu.bm.path.full.replace(/#/g,'%23').replace(/%/g,'%25')
        bg.setAttribute('src',`http://${window.overlay.config.host}:${window.overlay.config.port}/Songs/${img}?a=${Math.random(10000)}`)
    }
    if(tempTitle !== data.menu.bm.metadata.artist + ' - ' + data.menu.bm.metadata.title){
        tempTitle = data.menu.bm.metadata.artist + ' - ' + data.menu.bm.metadata.title;
        title.innerHTML = tempTitle
    }
    if(data.gameplay.pp.current != ''){
        pp.innerHTML = Math.round(data.gameplay.pp.current)
    }else{
        pp.innerHTML = 0
    }
    if(data.gameplay.hits[100] > 0){
        hun.innerHTML = data.gameplay.hits[100]
    }else{
        hun.innerHTML = 0
    }
    if(data.gameplay.hits[50] > 0){
        fifty.innerHTML = data.gameplay.hits[50]
    }else{
        fifty.innerHTML = 0
    }
    if(data.gameplay.hits[0] > 0){
        miss.innerHTML = data.gameplay.hits[0]
    }else{
        miss.innerHTML = 0
    }
    if(tempStrainBase != JSON.stringify(data.menu.pp.strains)){
        tempStrainBase = JSON.stringify(data.menu.pp.strains);
        smoothed = smooth(data.menu.pp.strains, smoothOffset);
        config.data.datasets[0].data = smoothed;
        config.data.labels =smoothed;
        configSecond.data.datasets[0].data = smoothed;
        configSecond.data.labels =smoothed;
        window.myLine.update();
        window.myLineSecond.update();
    }
    if(fullTime !== data.menu.bm.time.mp3){
        fullTime = data.menu.bm.time.mp3
        onepart = 500/fullTime
    }
    if(seek !== data.menu.bm.time.current && fullTime !== undefined && fullTime != 0){
        seek = data.menu.bm.time.current;
        progressChart.style.width = onepart*seek+'px'
    }
}

window.onload = function () {
    var ctx = document.getElementById('canvas').getContext('2d');
    window.myLine = new Chart(ctx, config);

    var ctxSecond = document.getElementById('canvasSecond').getContext('2d');
    window.myLineSecond = new Chart(ctxSecond, configSecond);
};

let config = {
    type: 'line',
    data: {
        labels: [],
        datasets: [{
            borderColor: 'rgba(255, 255, 255, 0)',
            backgroundColor: 'rgba(255, 255, 255, 0.2)',
            data: [],
            fill: true,
        }]
    },
    options: {
        tooltips: { enabled: false },
        legend: {
            display: false,
        },
        elements: {
            line: {
                tension: 0.4,
                cubicInterpolationMode: 'monotone'
            },
            point: {
                radius: 0
            }
        },
        responsive: false,
        scales: {
            x: {
                display: false,
            },
            y: {
                display: false,
            }
        }
    }
};

let configSecond = {
    type: 'line',
    data: {
        labels: [],
        datasets: [{
            borderColor: 'rgba(255, 255, 255, 0)',
            backgroundColor: 'rgba(255, 255, 255, 0.9)',
            data: [],
            fill: true,
        }]
    },
    options: {
        tooltips: { enabled: false },
        legend: {
            display: false,
        },
        elements: {
            line: {
                tension: 0.4,
                cubicInterpolationMode: 'monotone'
            },
            point: {
                radius: 0
            }
        },
        responsive: false,
        scales: {
            x: {
                display: false,
            },
            y: {
                display: false,
            }
        }
    }
}