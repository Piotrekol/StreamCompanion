let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");
let mapid = document.getElementById('mapid');


let bg = document.getElementById("bg");
let title = document.getElementById("title");
let bmlink = document.getElementById("bmlink");
let progressChart = document.getElementById("progress")
let currentPP = document.getElementById("ppCurent");
let ifFC = document.getElementById("ppIfFc");
let pp = document.getElementById("pp");

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
let tempLink;
let tempStrainBase;
let smoothOffset = 2;
let seek;
let fullTime;
let onepart;
let tempState;

function getRandomInt(max) {
    return Math.floor(Math.random() * Math.floor(max));
  }

socket.onmessage = event => {
    let data = event.data;
    if(tempImg !== data.menu.bm.path.full){
        tempImg = data.menu.bm.path.full
        data.menu.bm.path.full = data.menu.bm.path.full.replace(/#/g,'%23').replace(/%/g,'%25')
        bg.setAttribute('src',`http://${window.overlay.config.host}:${window.overlay.config.port}/Songs/${data.menu.bm.path.full}?a=${Math.random(10000)}`)
    }
    if(tempTitle !== `♪ ${data.menu.bm.metadata.artist} - ${data.menu.bm.metadata.title} [${data.menu.bm.metadata.difficulty}] ☆ ${data.menu.bm.stats['fullSR']} Mapset by ${data.menu.bm.metadata.mapper}` ){
        tempTitle = `♪ ${data.menu.bm.metadata.artist} - ${data.menu.bm.metadata.title} [${data.menu.bm.metadata.difficulty}] ☆ ${data.menu.bm.stats['fullSR']} Mapset by ${data.menu.bm.metadata.mapper}`;
        title.innerHTML = tempTitle;
    } 
    if(tempLink !== data.menu.bm.id){
        tempLink = data.menu.bm.id;
        bmlink.innerHTML = 'https://osu.ppy.sh/b/' + tempLink;
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
        onepart = 1920/fullTime
    }
    if(seek !== data.menu.bm.time.current && fullTime !== undefined && fullTime != 0){
        seek = data.menu.bm.time.current;
        progressChart.style.width = onepart*seek+'px'
    }
    if(data.gameplay.pp.current != ''){
        let ppData = data.gameplay.pp.current
        currentPP.innerHTML = Math.round(ppData)
    }else{
        currentPP.innerHTML = 0
    }
    if(data.gameplay.pp.fc != ''){
        let ppData = data.gameplay.pp.fc
        ifFC.innerHTML = Math.round(ppData)
    }else if (tempState == 1){
        ifFC.innerHTML = data.menu.pp[100]
    }else {
        ifFC.innerHTML = 0
    }
    if(tempState !== data.menu.state){
        tempState = data.menu.state;
        if(tempState == 2 || tempState == 7 || tempState == 1){
            pp.style.bottom = 210+'px'
            pp.style.color = 'rgba(199, 199, 199, 0.8);'
        }
        else{
            pp.style.bottom = 100+'px'
            pp.style.color = 'rgba(199, 199, 199, 0);'
        }
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
