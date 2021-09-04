let ws = new WebSocket('ws://localhost:20727/tokens');
ws.onopen = () => {
  //send token names with should be watched for value changes
  ws.send(JSON.stringify(['rankedStatus', 'titleRoman', 'artistUnicode']));
};
ws.onmessage = (wsEvent) => {
  //do things with data here
  console.log(wsEvent.data);
};
