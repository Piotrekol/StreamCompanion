let ws = new WebSocket('ws://localhost:20727/tokens');
ws.onopen = () => {
  ws.send(JSON.stringify(['rankedStatus', 'titleRoman', 'artistUnicode']));
};
let cache = {};
ws.onmessage = (wsEvent) => {
  Object.assign(cache, JSON.parse(wsEvent.data));
  //cache will always contain up to date token values
  console.log(cache);
};
