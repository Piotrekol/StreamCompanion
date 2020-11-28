let config = {
  scheme: window.location.protocol.slice(0, -1),
  host: window.location.hostname,
  port: window.location.port,
  getUrl: () => `${config.scheme}://${config.host}:${config.port}`,
  getWs: () => `ws://${config.host}:${config.port}`,
};

let osuStatus = {
  Null: 0,
  Listening: 1,
  Playing: 2,
  Watching: 8,
  Editing: 16,
  ResultsScreen: 32,
};

let rawOsuStatus = {
  Unknown: -2,
  NotRunning: -1,
  MainMenu: 0,
  EditingMap: 1,
  Playing: 2,
  GameShutdownAnimation: 3,
  SongSelectEdit: 4,
  SongSelect: 5,
  ResultsScreen: 7,
  GameStartupAnimation: 10,
  MultiplayerRooms: 11,
  MultiplayerRoom: 12,
  MultiplayerSongSelect: 13,
  MultiplayerResultsscreen: 14,
  OsuDirect: 15,
  RankingTagCoop: 17,
  RankingTeam: 18,
  ProcessingBeatmaps: 19,
  Tourney: 22,
}

let osuGrade = {
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
  rawOsuStatus,
  osuGrade,
  config,
};
