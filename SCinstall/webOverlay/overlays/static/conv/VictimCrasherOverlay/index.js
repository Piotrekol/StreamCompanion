let socket = CreateProxiedReconnectingWebSocket("ws://" + location.host + "/ws");
let mapid = document.getElementById('mapid');

let playername = document.getElementById("playername");
let mods = document.getElementById("mods");
let pp = document.getElementById("pp");
let combo = document.getElementById("combo");
let score = document.getElementById("score");
let accuracy = document.getElementById("accuracy");
let hun = document.getElementById("h100");
let fifty = document.getElementById("h50");
let miss = document.getElementById("h0");
let ur = document.getElementById("ur");
let top_cont = document.getElementById("top");
let bottom_cont = document.getElementById("bottom");
let hpbar = document.getElementById("hpbar");
let progressChart = document.getElementById("progress");
let strainGraph = document.getElementById("strainGraph");
const modsImgs = {
    'nm': './static/nomod.png',
    'ez': './static/easy.png',
    'nf': './static/nofail.png',
    'ht': './static/halftime.png',
    'hr': './static/hardrock.png',
    'sd': './static/suddendeath.png',
    'pf': './static/perfect.png',
    'dt': './static/doubletime.png',
    'nc': './static/nightcore.png',
    'hd': './static/hidden.png',
    'fl': './static/flashlight.png',
    'rx': './static/relax.png',
    'ap': './static/autopilot.png',
    'so': './static/spunout.png',
    'at': './static/autoplay.png',
    'cn': './static/cinema.png',
    'v2': './static/v2.png',
    'tp': './static/target.png',
};

socket.onopen = () => {
    console.log("Successfully Connected");
};

let animation = {
    acc:  new CountUp('accuracy', 0, 0, 2, .2, {useEasing: true, useGrouping: true,   separator: " ", decimal: "." }),
    ur:  new CountUp('ur', 0, 0, 2, .2, {useEasing: true, useGrouping: true,   separator: " ", decimal: "." }),
    score:  new CountUp('score', 0, 0, 0, .2, {useEasing: true, useGrouping: true,   separator: " ", decimal: "." }),
}

socket.onclose = event => {
    console.log("Socket Closed Connection: ", event);
    socket.send("Client Closed!");
};

socket.onerror = error => {
    console.log("Socket Error: ", error);
};
let tempPlayername;
let tempMods;
let gameState;
let tempStrainBase;
let smoothOffset = 2;
let seek;
let fullTime;

socket.onmessage = event => {
    let data = event.data;
	if(gameState !== data.menu.state){
        gameState = data.menu.state;
        if(gameState === 2){
			// Gameplay
            top_cont.style.transform = "translateY(0)";
            hpbar.style.transform = "translateY(0)";
            bottom_cont.style.transform = "translateY(0)";
            strainGraph.style.transform = "translateY(0)";
        }else{
            top_cont.style.transform = "translateY(-500px)";
            hpbar.style.transform = "translateY(-500px)";
            bottom_cont.style.transform = "translateY(500px)";
            strainGraph.style.transform = "translateY(500px)";
			ur.innerHTML = 0;
        }
    }
	if(tempStrainBase != JSON.stringify(data.menu.pp.strains)){
		tempStrainBase = JSON.stringify(data.menu.pp.strains);
		smoothed = smooth(data.menu.pp.strains, smoothOffset);
		config.data.datasets[0].data = smoothed;
		config.data.labels = smoothed;
		configSecond.data.datasets[0].data = smoothed;
		configSecond.data.labels = smoothed;
		window.myLine.update();
		window.myLineSecond.update();
	}
	if(fullTime !== data.menu.bm.time.mp3){
		fullTime = data.menu.bm.time.mp3;
		onepart = 1400/fullTime;
	}
	if(seek !== data.menu.bm.time.current && fullTime !== undefined && fullTime != 0){
		seek = data.menu.bm.time.current;
		progressChart.style.width = onepart*seek+'px';
	}
	if(data.gameplay.name != '') {
		playername.innerHTML = data.gameplay.name;
	}
	if (data.gameplay.pp.current != '') {
		let ppData = data.gameplay.pp.current;
		pp.innerHTML = Math.round(ppData);
	} else {
		pp.innerHTML = "";
	}
	if (data.gameplay.hp.smooth > 0) {
		hpbar.style.width = (data.gameplay.hp.normal / 200) * 1300 + "px";
	} else {
		hpbar.style.width = 1300;
	}
	if (data.gameplay.score > 0) {
		animation.score.update(data.gameplay.score);
	} else {
		animation.score.update(data.gameplay.score);
	}
	if (data.gameplay.accuracy > 0) {
		animation.acc.update(data.gameplay.accuracy);
	} else {
		animation.acc.update(0)
	}
	if (data.gameplay.combo.current != '') {
		let comboData = data.gameplay.combo.current;
		combo.innerHTML = comboData;
	} else {
		combo.innerHTML = "";
	}
	if (data.gameplay.hits[100] > 0) {
		hun.innerHTML = data.gameplay.hits[100];
	} else {
		hun.innerHTML = 0;
	}
	if (data.gameplay.hits[50] > 0) {
		fifty.innerHTML = data.gameplay.hits[50];
	} else {
		fifty.innerHTML = 0;
	}
	if (data.gameplay.hits[0] > 0) {
		miss.innerHTML = data.gameplay.hits[0];
	} else {
		miss.innerHTML = 0;
	}
	if (data.gameplay.hits.unstableRate > 0) {
		animation.ur.update(data.gameplay.hits.unstableRate);
	} else {
		animation.ur.update(0);
	}
    if(tempMods != data.menu.mods.str){
        tempMods = data.menu.mods.str;
        if (tempMods == ""){
           tempMods = 'NM';
        }
		mods.innerHTML = '';
		let modsApplied = tempMods.toLowerCase();
		
		if(modsApplied.indexOf('nc') != -1){
			modsApplied = modsApplied.replace('dt','');
		}
		if(modsApplied.indexOf('pf') != -1){
			modsApplied = modsApplied.replace('sd','');
		}
		let modsArr = modsApplied.match(/.{1,2}/g);
		for(let i = 0; i < modsArr.length; i++){
			let mod = document.createElement('div');
			mod.setAttribute('class','mod');
			let modImg = document.createElement('img');
			modImg.setAttribute('src', modsImgs[modsArr[i]]);
			mod.appendChild(modImg);
			mods.appendChild(mod);
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
            backgroundColor: 'rgba(255, 255, 255, 0.4)',
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
