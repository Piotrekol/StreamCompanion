<script>
	import { onMount } from 'svelte';
	let ws;
	let data;
	let dummybg;
	let bgelement;
	let urll;
	import ReconnectingWebSocket from './reconnecting-websocket.js';
	onMount(() => {
		let initdata = [
			"mapStrains",
			"mapArtistTitle",
			"creator",
			"diffName",
			"mStars",
			"mCS",
			"mAR",
			"mOD",
			"mHP",
			"mBpm",
			"mods",
			"time",
			"totaltime",
			"status",
			"c100",
			"c50",
			"miss",
			"mapsetid",
			"status",
			"md5"
		]
		ws = new ReconnectingWebSocket("ws://localhost:28390/tokens", null, {debug: true, reconnectInterval: 3000});
		ws.onopen = function (event) {
			console.log("Connected to websocket.")
			ws.send(JSON.stringify(initdata)); // send init data.
		}
		let lastbg;
		
		ws.onmessage = function (event) {
			data = {...data, ...JSON.parse(event.data)}; // merge data.3
			if (data != null && data.mapsetid != 1 && lastbg != data.mapsetid && dummybg != null) {
				lastbg = data.mapsetid;
				urll = `http://localhost:28390/backgroundImage?width=2000&mapset=${data.mapsetid}&dummyData=${data.md5}`;
				dummybg.src = urll;
				dummybg.onload = function() {
					bgelement.style.backgroundImage = `url("${urll}")`;
				}
			}
		}
	});
</script>



<main>
	{#if data?.mAR != null && data?.mAR != "null"} <!-- delete this pogchamp -->

		<div class="box">
			<img class="dummyimage" bind:this={dummybg} style="width: 0; height: 0; display: none;" alt="Background">
			<div class="background" bind:this={bgelement}>
			</div>
			<div class="move">
				<div class="progress {data?.status == 2 ? 'pg-enabled' : ''}" style={`width: ${(data?.time/data?.totaltime*1000)*1000}px`}></div>
				<div class="title">{data?.mapArtistTitle}</div>
				<div class="diffname">{data?.diffName}
				{#if data?.mods != "" && data?.mods != null && data?.mods != "None"}
					<div class="modcolor">{`+${data.mods.split(",").join("")}`}</div>
				{/if}
				</div>
			</div>
			<div class="move2">
				<div class="attr">
					<div class="attritem"><div class="attrtitle">AR</div>{data.mAR.toFixed(1)}</div>
					<div class="attritem"><div class="attrtitle">CS</div>{data.mCS.toFixed(1)}</div>
					<div class="attritem"><div class="attrtitle">OD</div>{data.mOD.toFixed(1)}</div>
					<div class="attritem"><div class="attrtitle">HP</div>{data.mHP.toFixed(1)}</div>
					<div class="attritem"><div class="attrtitle">BPM</div>{data.mBpm}</div>
					<div class="attritem"><div class="attrtitle">SR</div>{data.mStars.toFixed(2)}</div>
				</div>
			</div>
		</div>
	{/if}
</main>

<style>
	.progress {
		background-color: yellow;
		height: 15px;
		transition: none;
		margin-bottom: 40px;
		opacity: 0;
	}
	.pg-enabled {
		opacity: 0.3;
	}
	.attr {
		margin-top: 30px;
	}
	.attritem {
		display: inline-block;
		background: rgba(0, 0, 0, 0.25);
		margin: 10px;
		padding: 10px;
		border-radius: 0.8rem;
	}
	.attrtitle {
		display: inline-block;
		margin-right: 5px;
		font-weight: bold;
	}
	.box {
		border-radius: 2rem;
		overflow: hidden;
		width: 1000px;
	}
	* {
		transition: all 1s;
		transition-timing-function: ease;
		color: white;
		text-align: center;
		text-shadow: 0px 0px 10px rgba(255, 255, 255, 1), 0px 0px 46px rgba(255, 255, 255, 1), 0px 0px 80px rgba(255, 255, 255, 1);
		font-family: 'Comfortaa';
	}
	.title {
		font-size: 35px;
	}
	.diffname {
		margin-top: 7.5px;
		font-size: 25px;
	}
	.modcolor {
		color: #dbdbdb;
		display: inline-block;
		text-shadow: 0px 0px 10px #dbdbdb, 0px 0px 46px #dbdbdb, 0px 0px 80px #dbdbdb;
	}
	.background {
		transition: all 0.2s;
		filter: brightness(50%);
		width: 1000px;
		height: 300px;
		background-position: center;
		background-repeat: no-repeat;
		background-size: cover;
	}
	.move {
		position: absolute;
		top: 8px;
		width: 1000px;
		border-radius: 2rem; overflow: hidden;
		height: 300px;
	}
	.move2 {
		position: absolute;
		top: 200px;
		width: 1000px;
	}
</style>