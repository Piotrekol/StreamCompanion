let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");

let everything = document.getElementById("everything");
let bg = document.getElementById("bg");
let title = document.getElementById("title");
let artist = document.getElementById("artist");
let diff = document.getElementById("diff");
let mapper = document.getElementById("mapper");
let hit = document.getElementById("hits");
let hun = document.getElementById("100");
let fifty = document.getElementById("50");
let miss = document.getElementById("0");
let pp = document.getElementById("pp");
let infoContainer = document.getElementById("infoContainer");
let root = document.documentElement;
let rank = document.getElementById('rank');
let mapRank = document.getElementById("rankedStatus");
let rankedColor = document.getElementById("rankedColor");

let $title = document.getElementsByClassName("title");
let $artist = document.getElementsByClassName("artist");

socket.onopen = () => {
  console.log("Successfully Connected");
};

socket.onclose = (event) => {
  console.log("Socket Closed Connection: ", event);
  socket.send("Client Closed!");
};

socket.onerror = (error) => {
  console.log("Socket Error: ", error);
};

let tempImg;
let tempTitle;
let tempArtist;
let tempDiff;
let tempMapper;
let ppData;
let gameState;
let fullTime;
let onepart;
let seek;
let hdfl;

function hide(el) {
  el.classList.add('hide');
  el.classList.remove('show')
}
function show(el) {
  el.classList.remove('hide');
  el.classList.add('show')
}
function toggleFunction() {
  if(rank.classList.contains("show")){
      hide(rank);
      show($title[0]);
      show($artist[0]);
  } else {
      show(rank);
      hide($title[0]);
      hide($artist[0]);
  }
}
let toggleStatus;

let mapRanking;

socket.onmessage = (event) => {
  let data = event.data, hits = data.gameplay.hits;
  if (data.menu.mods.str.includes("HD") || data.menu.mods.str.includes("FL")) {
    hdfl = true;
  } else hdfl = false;

  if (hits.grade.current == 'SS'){
    if (hdfl == true) {
      rank.style.color = "#D3D3D3";
      rank.style.textShadow = "0 0 0.5rem #D3D3D3";
    } else {
      rank.style.color = "#d6c253";
      rank.style.textShadow = "0 0 0.5rem #d6c253";
    }
  } else if (hits.grade.current == 'S'){
    if (hdfl == true) {
      rank.style.color = "#D3D3D3";
      rank.style.textShadow = "0 0 0.5rem #D3D3D3";
    } else {
      rank.style.color = "#d6c253";
      rank.style.textShadow = "0 0 0.5rem #d6c253";
    }
  } else if (hits.grade.current == 'A'){
    rank.style.color = "#7ed653";
    rank.style.textShadow = "0 0 0.5rem #7ed653";
  } else if (hits.grade.current == 'B'){
    rank.style.color = "#53d4d6";
    rank.style.textShadow = "0 0 0.5rem #53d4d6";
  } else if (hits.grade.current == 'C'){
    rank.style.color = "#d6538e";
    rank.style.textShadow = "0 0 0.5rem #d6538e";
  } else {
    rank.style.color = "#d65353";
    rank.style.textShadow = "0 0 0.5rem #d65353";
  }
  rank.innerHTML = hits.grade.current;
  console.log(hits.grade.current);

  if (tempImg !== data.menu.bm.path.full) {
    tempImg = data.menu.bm.path.full;
    let img = data.menu.bm.path.full.replace(/#/g, "%23").replace(/%/g, "%25");
    bg.setAttribute(
      "src",
      `http://${window.overlay.config.host}:${window.overlay.config.port}/Songs/${img}?a=${Math.random(10000)}`
    );
  }
  if (data.menu.bm.rankedStatus === 7) {
      mapRanking = "";
      rankedColor.className = "LOVED";
  } else if (data.menu.bm.rankedStatus === 4) {
      mapRanking = "";
      rankedColor.className = "RANKED";
  } else if (data.menu.bm.rankedStatus === 5) {
      mapRanking = "";
      rankedColor.className = "QUALIFIED";
  } else {
      mapRanking = ""
      rankedColor.className = "GRAVEYARD";
  }
  mapRank.innerHTML = mapRanking;

  if (gameState !== data.menu.state) {
    gameState = data.menu.state;
    if (gameState === 2) {
      hit.style.transform = "translateY(0)";
      infoContainer.style.transform = "translateY(-1.225rem)";
      toggleStatus = setInterval(toggleFunction, 10000);
    } else {
      hit.style.transform = "translateY(calc(100% - 0.25rem))";
      infoContainer.style.transform = "translate(0)";
      clearInterval(toggleStatus);
      hide(rank);
      show($title[0]);
      show($artist[0]);
      root.style.setProperty("--progress", 0);
    }
  }
  if (fullTime !== data.menu.bm.time.mp3) {
    fullTime = data.menu.bm.time.mp3;
    onepart = 100 / fullTime;
  }
  if (
    gameState === 2 &&
    seek !== data.menu.bm.time.current &&
    fullTime !== undefined &&
    fullTime != 0
  ) {
    seek = data.menu.bm.time.current;
    root.style.setProperty("--progress", onepart * seek + "%");
  }
  if (tempDiff !== data.menu.bm.metadata.difficulty) {
    tempDiff = data.menu.bm.metadata.difficulty;
    diff.innerHTML = tempDiff;
  }
  if (tempMapper !== data.menu.bm.metadata.mapper) {
    tempMapper = data.menu.bm.metadata.mapper;
    mapper.innerHTML = tempMapper;
  }
  if (tempTitle !== data.menu.bm.metadata.title) {
    tempTitle = data.menu.bm.metadata.title;
    title.innerHTML = tempTitle;
  }
  if (tempArtist !== data.menu.bm.metadata.artist) {
    tempArtist = data.menu.bm.metadata.artist;
    artist.innerHTML = tempArtist;
  }
  var widthLimit = everything.getBoundingClientRect().width * 0.6;
  var titleWidth = title.offsetWidth;
  var artistWidth = artist.offsetWidth;

  if (titleWidth>widthLimit) {
    var timeTaken = titleWidth / 24;
    title.style.animationDuration = timeTaken + "s";
    title.className = 'textMarquee';
  } else {
    title.className = ''
  }
  if (artistWidth>widthLimit) {
    var timeTaken = artistWidth / 24;
    artist.style.animationDuration = timeTaken + "s";
    artist.className = 'textMarquee'
  } else {
    artist.className = ''
  }
  if (data.gameplay.pp.current != "") {
    let ppData = data.gameplay.pp.current;
    pp.innerHTML = Math.round(ppData);
  } else {
    pp.innerHTML = 0;
  }


  if (hits[100] > 0) {
    hun.innerHTML = hits[100];
  } else {
    hun.innerHTML = 0;
  }
  if (hits[50] > 0) {
    fifty.innerHTML = hits[50];
  } else {
    fifty.innerHTML = 0;
  }
  if (hits[0] > 0) {
    miss.innerHTML = hits[0];
  } else {
    miss.innerHTML = 0;
  }
};
