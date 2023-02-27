let socket = CreateProxiedReconnectingWebSocket("ws://${window.overlay.config.host}:${window.overlay.config.port}/ws");
// Variables
let mainContainer = document.getElementById("main");
let line = document.getElementById("line");
let gameContainer = document.getElementById("game-container");
let songContainer = document.getElementById("song-container");
let sliderBreak = document.getElementById("sb");
let ppFC = document.getElementById("ppfc");
let rank = document.getElementById("rank");
// Functions
function setRankStyle(text, color, shadow) {
	rank.innerHTML = text;
	rank.style.color = color;
	rank.style.textShadow = shadow;
}
socket.onopen = () => console.log("Successfully Connected");
socket.onclose = event => {
	console.log("Socket Closed Connection: ", event);
	socket.send("Client Closed!");
};
socket.onerror = error => console.log("Socket Error: ", error);
let pp = new CountUp('pp', 0, 0, 0, .5, {
	useEasing: true
	, useGrouping: true
	, separator: " "
	, decimal: "."
});
let fc = new CountUp('ppfc', 0, 0, 0, .5, {
	useEasing: true
	, useGrouping: true
	, separator: " "
	, decimal: "."
});
let h100 = new CountUp('h100', 0, 0, 0, .5, {
	useEasing: true
	, useGrouping: true
	, separator: ""
	, decimal: "."
});
let h50 = new CountUp('h50', 0, 0, 0, .5, {
	useEasing: true
	, useGrouping: true
	, separator: ""
	, decimal: "."
});
let h0 = new CountUp('h0', 0, 0, 0, .5, {
	useEasing: true
	, useGrouping: true
	, separator: ""
	, decimal: "."
});
let ss = new CountUp('ss', 0, 0, 0, .5, {
	useEasing: true
	, useGrouping: true
	, separator: " "
	, decimal: "."
});
let sb = new CountUp('sb', 0, 0, 0, .5, {
	useEasing: true
	, useGrouping: true
	, separator: " "
	, decimal: "."
});
socket.onmessage = event => {
	try {
		let data = event.data
			, menu = data.menu
			, play = data.gameplay
			, hitGrade = data.gameplay.hits.grade.current
			, hdfl = (data.menu.mods.str.includes("HD") || data.menu.mods.str.includes("FL") ? true : false)
			, tempGrade = ""
			, tempColor = ""
			, tempShadow = "";
		// Rank Check
		function rankCheck() {
			switch (hitGrade) {
                case "SS":
                    tempGrade = hitGrade;
                    tempColor = (hdfl ? "#e0e0e0" : "#d6c253");
                    tempShadow = (hdfl ? "0 0 0.5rem #e0e0e0" : "0 0 0.5rem #d6c253");
                    break;
                case "S":
                    tempGrade = hitGrade;
                    tempColor = (hdfl ? "#e0e0e0" : "#d6c253");
                    tempShadow = (hdfl ? "0 0 0.5rem #e0e0e0" : "0 0 0.5rem #d6c253");
                    break;
                case "A":
                    tempGrade = hitGrade;
                    tempColor = "#7ed653";
                    tempShadow = "0 0 0.5rem #7ed653";
                    break;
                case "B":
                    tempGrade = hitGrade;
                    tempColor = "#53d4d6";
                    tempShadow = "0 0 0.5rem #53d4d6";
                    break;
                case "C":
                    tempGrade = hitGrade;
                    tempColor = "#d6538e";
                    tempShadow = "0 0 0.5rem #d6538e";
                    break;
                case "D":
                    tempGrade = hitGrade;
                    tempColor = "#f04848";
                    tempShadow = "0 0 0.5rem #f04848";
                    break;
                default:
                    tempGrade = "SS";
                    tempColor = (hdfl ? "#ffffff" : "#d6c253");
                    tempShadow = (hdfl ? "0 0 0.5rem #ffffff" : "0 0 0.5rem #d6c253");;
                    break;
			}
		}
		//Game State Check
		switch (menu.state) {
            case 7:
            case 14:
            case 2:
                //Main
                mainContainer.style.opacity = "1";
                // Rank
                rankCheck();
                setRankStyle(tempGrade, tempColor, tempShadow);
                //Box
                document.documentElement.style.setProperty('--width', ` 500px`);
                // Line
                document.documentElement.style.setProperty('--progress', ` ${(menu.bm.time.current / menu.bm.time.mp3 * 100).toFixed(2)}%`);
                line.style.cssText = "transition: transform 500ms ease, opacity 20ms ease, width 500ms ease;";
                line.style.transform = "translate(0px, 5px)"
                line.style.opacity = "1"
                // Game Container
                gameContainer.style.top = "0";
                // Song Container
                songContainer.style.top = "100px";
                // Sliderbreak
                if (play.hits.sliderBreaks >= 1) {
                    sb.update(play.hits.sliderBreaks);
                    sliderBreak.style.transform = "scale(1)";
                    sliderBreak.style.opacity = "1";
                } else {
                    sliderBreak.style.transform = "scale(0)";
                    sliderBreak.style.opacity = "0";
                }
                // PP FC
                if (play.hits.sliderBreaks >= 1 || play.hits[0] >= 1) {
                    fc.update(play.pp.fc);
                    ppFC.style.transform = "scale(1)";
                    ppFC.style.opacity = "1";
                } else {
                    ppFC.style.transform = "scale(0)";
                    ppFC.style.opacity = "0";
                }
                // Set Only in Gameplay
                pp.update(play.pp.current);
                h100.update(play.hits[100]);
                h50.update(play.hits[50]);
                h0.update(play.hits[0]);
                break;
            case 0:
                //Main
                mainContainer.style.opacity = "0";
                break;
            default:
                //Main
                mainContainer.style.opacity = "1";
                // Rank
                setRankStyle("", tempColor, tempShadow);
                //Box
                document.documentElement.style.setProperty('--width', ` 300px`);
                //Line
                document.documentElement.style.setProperty('--progress', ` 100%`);
                line.style.cssText = "transition: transform 500ms ease, opacity 20ms ease, width 300ms ease;";
                line.style.transform = "translate(0px, 5px)"
                line.style.opacity = "1"
                // Game Container
                gameContainer.style.top = "-100px";
                //Song Container
                songContainer.style.top = "0";
                ss.update(menu.pp["100"]);
                // Sliderbreak
                sliderBreak.style.transform = "scale(0)";
                sliderBreak.style.opacity = "0";
                // PP FC
                ppFC.style.transform = "scale(0)";
                ppFC.style.opacity = "0";
                break;
		}
	} catch (err) {
		console.log(err);
	};
};
