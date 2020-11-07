let config = {
  scheme: window.location.protocol.slice(0, -1),
  host: window.location.hostname,
  port: window.location.port,
  getUrl: () => `${config.scheme}://${config.host}:${config.port}`,
  getWs: () => `ws://${config.host}:${config.port}`,
};

osuStatus = {
  Null: 0,
  Listening: 1,
  Playing: 2,
  Watching: 8,
  Editing: 16,
  ResultsScreen: 32,
};

osuGrade = {
  0: 'SSH',
  1: 'SH',
  2: 'SS',
  3: 'S',
  4: 'A',
  5: 'B',
  6: 'C',
  7: 'D',
  8: 'F',
  9: '',
};

window.overlay = {
  osuStatus,
  osuGrade,
  config,
};
