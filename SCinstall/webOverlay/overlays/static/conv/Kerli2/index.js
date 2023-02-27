let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");

let currentPP = document.getElementById("curentPP");
let ifFC = document.getElementById("PPifFc");
let hun = document.getElementById("hun");
let fifty = document.getElementById("fiv");
let miss = document.getElementById("miss");

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

socket.onmessage = event => {
    let data = event.data;
    if(data.gameplay.pp.current != ''){
        let ppData = data.gameplay.pp.current
        currentPP.innerHTML = Math.round(ppData)
    }else{
        currentPP.innerHTML = 0
    }
    if(data.gameplay.pp.fc != ''){
        let ppData = data.gameplay.pp.fc
        ifFC.innerHTML = Math.round(ppData)
    }else{
        ifFC.innerHTML = 0
    }
    if(data.gameplay.hits[100] > 0){
        hun.innerHTML = `${data.gameplay.hits[100]}<hr>`
    }else{
        hun.innerHTML = `0<hr>`
    }
    if(data.gameplay.hits[50] > 0){
        fifty.innerHTML = `${data.gameplay.hits[50]}<hr>`
    }else{
        fifty.innerHTML = `0<hr>`
    }
    if(data.gameplay.hits[0] > 0){
        miss.innerHTML = `${data.gameplay.hits[0]}<hr>`
    }else{
        miss.innerHTML = `0<hr>`
    }
}
