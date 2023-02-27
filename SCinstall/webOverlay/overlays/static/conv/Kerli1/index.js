let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");
let mapid = document.getElementById('mapid');

let bg = document.getElementById("bg");
let title = document.getElementById("title");
let cs = document.getElementById("cs");
let ar = document.getElementById("ar");
let od = document.getElementById("od");
let hp = document.getElementById("hp");
let mods = document.getElementById("mods");
const modsImgs = {
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
}

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
let tempCs;
let tempAr;
let tempOd;
let tempHp;
let tempTitle;
let tempMods;

socket.onmessage = event => {
    let data = event.data;
    if(tempImg !== data.menu.bm.path.full){
        tempImg = data.menu.bm.path.full
        data.menu.bm.path.full = data.menu.bm.path.full.replace(/#/g,'%23').replace(/%/g,'%25')
        bg.setAttribute('src',`http://${window.overlay.config.host}:${window.overlay.config.port}/Songs/${data.menu.bm.path.full}?a=${Math.random(10000)}`)
    }
    if(tempTitle !== data.menu.bm.metadata.artist + ' - ' + data.menu.bm.metadata.title){
        tempTitle = data.menu.bm.metadata.artist + ' - ' + data.menu.bm.metadata.title;
        title.innerHTML = tempTitle
    }
    if(data.menu.bm.stats.CS != tempCs){
        tempCs = data.menu.bm.stats.CS
        cs.innerHTML= `CS: ${Math.round(tempCs * 10) / 10} <hr>`
    }
    if(data.menu.bm.stats.AR != tempAr){
        tempAr = data.menu.bm.stats.AR
        ar.innerHTML= `AR: ${Math.round(tempAr * 10) / 10} <hr>`
    }
    if(data.menu.bm.stats.OD != tempOd){
        tempOd = data.menu.bm.stats.OD
        od.innerHTML= `OD: ${Math.round(tempOd * 10) / 10} <hr>`
    }
    if(data.menu.bm.stats.HP != tempHp){
        tempHp = data.menu.bm.stats.HP
        hp.innerHTML= `HP: ${Math.round(tempHp * 10) / 10} <hr>`
    }
    if(tempMods != data.menu.mods.str){
        tempMods = data.menu.mods.str
        if (tempMods == "" || tempMods == "NM"){
            mods.innerHTML = '';
        }
        else{
            mods.innerHTML = '';
            let modsApplied = tempMods.toLowerCase();
            
            if(modsApplied.indexOf('nc') != -1){
                modsApplied = modsApplied.replace('dt','')
            }
            if(modsApplied.indexOf('pf') != -1){
                modsApplied = modsApplied.replace('sd','')
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
}
