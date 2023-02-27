let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");

let bg = document.getElementById("bg");
let star = document.getElementById("star");
let pp = document.getElementById("pp");
let hun = document.getElementById("h100");
let fifty = document.getElementById("h50");
let miss = document.getElementById("h0");
let time = document.getElementById("time");

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

let tempState;
let tempImg;
socket.onmessage = event => {
	let data = event.data;
	if (tempState !== data.menu.bm.path.full) {
		tempState = data.menu.bm.path.full
		bg.setAttribute('src', `http://${window.overlay.config.host}:${window.overlay.config.port}/Songs/${data.menu.bm.path.full}?a=${Math.random(10000)}`)
	}
	if (data.menu.bm.time.current > 1000) {
		let seconds = (data.menu.bm.time.current/1000).toFixed(0);
		let minutes = Math.floor(seconds % 3600 / 60).toString();

		if (seconds > 60) {
			time.innerHTML = `${minutes}m ${seconds-(minutes*60)}s`
		} else {
			time.innerHTML = `${seconds}s`
		}
	}
	if (data.gameplay.pp.current != '') {
		let ppData = data.gameplay.pp.current;
		pp.innerHTML = Math.round(ppData) + "pp"
	} else {
		pp.innerHTML = 0 + "pp"
	}
	if (data.menu.bm.stats.SR != '') {
		let SR = data.menu.bm.stats.SR;
		star.innerHTML = SR.toFixed(2)
	} else {
		star.innerHTML = 0
	}
	if (data.gameplay.hits[100] > 0) {
		hun.innerHTML = data.gameplay.hits[100];
	} else {
		hun.innerHTML = 0
	}
	if (data.gameplay.hits[50] > 0) {
		fifty.innerHTML = data.gameplay.hits[50];
	} else {
		fifty.innerHTML = 0
	}
	if (data.gameplay.hits[0] > 0) {
		miss.innerHTML = data.gameplay.hits[0];
	} else {
		miss.innerHTML = 0
	}
}



//Received: '{"menuContainer":{"osuState":2,"bmID":1219126,"bmSetID":575767,"CS":4,"AR":9.5,"OD":8,"HP":6,"bmInfo":"BTS - Not Today [Tomorrow]","bmFolder":"575767 BTS - Not Today","pathToBM":"BTS - Not Today (DeRandom Otaku) [Tomorrow].osu","bmCurrentTime":8861,"bmMinBPM":0,"bmMaxBPM":0},"gameplayContainer":{"300":21,"100":0,"50":0,"miss":0,"accuracy":100,"score":24612,"combo":36,"gameMode":0,"appliedMods":2048,"maxCombo":36,"pp":""}}'