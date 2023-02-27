let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");

socket.onopen = () => console.log("Successfully Connected");

socket.onclose = event => {
  console.log("Socket Closed Connection: ", event);
  socket.send("Client Closed!");
};

socket.onerror = error => console.log("Socket Error: ", error);

let pp = document.getElementById("pp");
let h100 = document.getElementById("h100");
let h50 = document.getElementById("h50");
let h0 = document.getElementById("h0");

let $pp = new CountUp('pp', 0, 0, 0, .5, { useEasing: true, useGrouping: true, separator: "", decimal: "." })
let $h100 = new CountUp('h100', 0, 0, 0, .5, { useEasing: true, useGrouping: true, separator: "", decimal: "." })
let $h50 = new CountUp('h50', 0, 0, 0, .5, { useEasing: true, useGrouping: true, separator: "", decimal: "." })
let $h0 = new CountUp('h0', 0, 0, 0, .5, { useEasing: true, useGrouping: true, separator: "", decimal: "." })

function reflow(elt){
  elt.offsetHeight;
}

let shape = document.getElementsByClassName('shape');
let diffClass = document.getElementsByClassName('diff');
let mapperClass = document.getElementsByClassName('mapper');
let secondary_font = document.getElementsByClassName('secondary-font');
let rank = document.getElementById('rank');
let title = document.getElementById('title');
let artist = document.getElementById('artist');
let artist_title = document.getElementById('artist-title');
let mapRank = document.getElementById('mapRank');
let diff = document.getElementById('diff');
let mapper = document.getElementById('mapper');
let bg = document.getElementById('bg');
let container = document.getElementsByClassName('container');
let info = document.getElementsByClassName('info');

let mapName;
let artistName;
let gameState;
let tempImg;
let mapperName;
let diffName;

let params = {
  rank: '',
  mapRank: ''
};

socket.onmessage = event => {
  try {
    let data = event.data, menu = data.menu.bm.metadata, hits = data.gameplay.hits, mods = data.menu.mods.str;

    if (mapName !== menu.title) {
        mapName = menu.title;
        title.innerHTML = mapName;
    }
    if (artistName !== menu.artist) {
        artistName = menu.artist;
        artist.innerHTML = artistName;
    }
    if (tempImg !== data.menu.bm.path.full) {
      tempImg = data.menu.bm.path.full;
      let img = data.menu.bm.path.full.replace(/#/g, "%23").replace(/%/g, "%25");
      bg.setAttribute(
        "src",
        `http://${window.overlay.config.host}:${window.overlay.config.port}/Songs/${img}?a=${Math.random(10000)}`
      );
    }
    if (diffName !== menu.difficulty) {
      diffName = menu.difficulty;
      diff.innerHTML = diffName;
    }
    if (mapperName !== menu.mapper) {
      mapperName = menu.mapper;
      mapper.innerHTML = mapperName;
    }

    var widthLimit = shape[0].getBoundingClientRect().width * 0.9;
    var totalWidth = artist_title.getBoundingClientRect().width;
    if (totalWidth > widthLimit) {
      var speed = totalWidth / 64;
      artist_title.style.animationDuration = speed + "s";
      artist_title.className = "textMarquee";
    } else {
      artist_title.className = "";
    }

    var diffMapperWidthLimit = shape[0].getBoundingClientRect().width / 4;
    var diffWidth = diff.getBoundingClientRect().width + secondary_font[0].getBoundingClientRect().width;
    var mapperWidth = mapper.getBoundingClientRect().width + secondary_font[1].getBoundingClientRect().width;

    if (diffWidth > diffMapperWidthLimit) {
      diffClass[0].classList.add('hide');
    } else {
      diffClass[0].classList.remove('hide');
    }

    if (mapperWidth > diffMapperWidthLimit) {
      mapperClass[0].classList.add('hide');
    } else {
      mapperClass[0].classList.remove('hide');
    }


    if (gameState !== data.menu.state) {
      gameState = data.menu.state;
      if (gameState === 2 || gameState === 7 || gameState === 14) {
        container[0].style.transform = "translateY(50%)";
        container[0].style.bottom = "50%";
        info[0].style.transform = "translateY(-1.8rem)";
      } else {
        container[0].style.transform = "translateY(100%)";
        container[0].style.bottom = "0";
        info[0].style.transform = "translateY(-50%)";
      }
    }
    if (data.menu.bm.rankedStatus === 7) {
        params.mapRank = "";
        mapRank.style.color = "#ff81c5";
    } else if (data.menu.bm.rankedStatus === 4) {
        params.mapRank = "";
        mapRank.style.color = "#80e6ff";
    } else if (data.menu.bm.rankedStatus === 5) {
        params.mapRank = "";
        mapRank.style.color = "#c0e71b";
    } else {
        params.mapRank = "";
        mapRank.style.color = "#929292";
    }

    if (data.gameplay.pp.current != pp.innerHTML) {
      $pp.update(data.gameplay.pp.current);
    }
    if (hits[100] != h100.innerHTML) {
      $h100.update(hits[100]);
      h100.classList.remove('click');
      reflow(h100);
      h100.classList.add('click')
    }
    if (hits[50] != h50.innerHTML) {
      $h50.update(hits[50]);
      h50.classList.remove('click');
      reflow(h50);
      h50.classList.add('click')
    }
    if (hits[0] != h0.innerHTML) {
      $h0.update(hits[0]);
      h0.classList.remove('click');
      reflow(h0);
      h0.classList.add('click')
    }   
        
    if (mods.includes("HD") || mods.includes("FL")) {
      hdfl = true;
    } else hdfl = false;
    if (hits.grade.current === "") {
      params.rank = 'SS'
    } else params.rank = hits.grade.current

    if (params.rank == 'SS'){
      if (hdfl == true) {
        rank.style.color = getComputedStyle(document.documentElement).getPropertyValue('--SHD');
      } else {
        rank.style.color = getComputedStyle(document.documentElement).getPropertyValue('--S');
      }
    } else if (params.rank == 'S'){
      if (hdfl == true) {
        rank.style.color = getComputedStyle(document.documentElement).getPropertyValue('--SHD');
      } else {
        rank.style.color = getComputedStyle(document.documentElement).getPropertyValue('--S');
      }
    } else if (params.rank == 'A'){
      rank.style.color = getComputedStyle(document.documentElement).getPropertyValue('--A');
    } else if (params.rank == 'B'){
      rank.style.color = getComputedStyle(document.documentElement).getPropertyValue('--B');
    } else if (params.rank == 'C'){
      rank.style.color = getComputedStyle(document.documentElement).getPropertyValue('--C');
    } else {
      rank.style.color = getComputedStyle(document.documentElement).getPropertyValue('--D');
    } 
    
    if(rank.innerHTML != params.rank){
        rank.innerHTML = params.rank;
        rank.classList.remove('click');
        reflow(rank);
        rank.classList.add('click')
    }
    if(mapRank.innerHTML != params.mapRank){
        mapRank.innerHTML = params.mapRank;
        mapRank.classList.remove('click');
        reflow(mapRank);
        mapRank.classList.add('click')
    }

  } catch (err) { console.log(err); };
};