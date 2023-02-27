let socket = CreateProxiedReconnectingWebSocket("ws://" + location.host + "/ws");
let mapid = document.getElementById('mapid');

let bg = document.getElementById("bg");
let title = document.getElementById("title");
let diff = document.getElementById("diff");
let cs = document.getElementById("cs");
let ar = document.getElementById("ar");
let od = document.getElementById("od");
let hp = document.getElementById("hp");
let mods = document.getElementById("mods");
let pp = document.getElementById("pp");
let hun = document.getElementById("h100");
let fifty = document.getElementById("h50");
let miss = document.getElementById("h0");
let mapStatus = document.getElementById("mapStatus");
let maskTitleDiff = document.getElementById("maskTitleDiff");
let ppCont = document.getElementById("ppCont");
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

socket.onclose = event => {
    console.log("Socket Closed Connection: ", event);
    socket.send("Client Closed!");
};

socket.onerror = error => {
    console.log("Socket Error: ", error);
};
let tempImg;
let tempCs;
let tempAr;
let tempOd;
let tempHp;
let tempTitle;
let tempDiff;
let tempMods;
let gameState;

socket.onmessage = event => {
    let data = event.data;
    if(tempImg !== data.menu.bm.path.full){
        tempImg = data.menu.bm.path.full;
        data.menu.bm.path.full = data.menu.bm.path.full.replace(/#/g,'%23').replace(/%/g,'%25');
        bg.setAttribute('src',`http://` + location.host + `/Songs/${data.menu.bm.path.full}?a=${Math.random(10000)}`);
    }
	if(gameState !== data.menu.state){
        gameState = data.menu.state;
        if(gameState === 2 || gameState === 7 || gameState === 14){
			// Gameplay, Results Screen, Multiplayer Results Screen
            maskTitleDiff.style.transform = "translateY(0)";
            mapStatus.style.transform = "translateY(0)";
            ppCont.style.transform = "translateY(0)";
            mods.style.transform = "translateY(0)";
            hits.style.transform = "translateY(0)";
        }else{
            maskTitleDiff.style.transform = "translateY(20px)";
            mapStatus.style.transform = "translateY(20px)";
            ppCont.style.transform = "translateY(100px)";
            mods.style.transform = "translateY(100px)";
            hits.style.transform = "translateY(100px)";
        }
    }
	if (data.gameplay.pp.current != '') {
		let ppData = data.gameplay.pp.current;
		pp.innerHTML = Math.round(ppData);
	} else {
		pp.innerHTML = "";
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
    if(tempTitle !== data.menu.bm.metadata.artist + ' - ' + data.menu.bm.metadata.title){
        tempTitle = data.menu.bm.metadata.artist + ' - ' + data.menu.bm.metadata.title;
        title.innerHTML = tempTitle;
		
		if(title.getBoundingClientRect().width >= 300) {
			title.classList.add("over");
		} else {
			title.classList.remove("over");
		}
    }
	if(tempDiff !== '[' + data.menu.bm.metadata.difficulty + ']'){
        tempDiff = '[' + data.menu.bm.metadata.difficulty + ']';
        diff.innerHTML = tempDiff;
    }
    if(data.menu.bm.stats.CS != tempCs){
        tempCs = data.menu.bm.stats.CS;
        cs.innerHTML= `${Math.round(tempCs * 10) / 10}`;
    }
    if(data.menu.bm.stats.AR != tempAr){
        tempAr = data.menu.bm.stats.AR;
        ar.innerHTML= `${Math.round(tempAr * 10) / 10}`;
    }
    if(data.menu.bm.stats.OD != tempOd){
        tempOd = data.menu.bm.stats.OD;
        od.innerHTML= `${Math.round(tempOd * 10) / 10}`;
    }
    if(data.menu.bm.stats.HP != tempHp){
        tempHp = data.menu.bm.stats.HP;
        hp.innerHTML= `${Math.round(tempHp * 10) / 10}`;
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
