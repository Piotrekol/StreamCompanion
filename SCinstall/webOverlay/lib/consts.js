(()=>{let config = {
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
let osuStatusFriendly = {
  Null: { text: 'Null', value: 0 },
  Listening: { text: 'Listening', value: 1 },
  Playing: { text: 'Playing', value: 2 },
  Watching: { text: 'Watching', value: 8 },
  Editing: { text: 'Editing', value: 16 },
  ResultsScreen: { text: 'ResultsScreen', value: 32 },
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
};
let rawOsuStatusFriendly = {
  Unknown: { text: 'Unknown', value: -2 },
  NotRunning: { text: 'NotRunning', value: -1 },
  MainMenu: { text: 'Main Menu', value: 0 },
  EditingMap: { text: 'Editing Map', value: 1 },
  Playing: { text: 'Playing', value: 2 },
  //GameShutdownAnimation: { text: 'GameShutdownAnimation', value: 3 },
  SongSelectEdit: { text: 'Editing beatmap', value: 4 },
  SongSelect: { text: 'Selecting song', value: 5 },
  ResultsScreen: { text: 'Results screen', value: 7 },
  //GameStartupAnimation: { text: 'GameStartupAnimation', value: 10 },
  MultiplayerRooms: { text: 'Multiplayer Room selection', value: 11 },
  MultiplayerRoom: { text: 'Multiplayer lobby', value: 12 },
  MultiplayerSongSelect: { text: 'Multiplayer song selection', value: 13 },
  MultiplayerResultsscreen: { text: 'Multiplayer results screen', value: 14 },
  OsuDirect: { text: 'OsuDirect', value: 15 },
  RankingTagCoop: { text: 'Multiplayer TagCoop results screen', value: 17 },
  RankingTeam: { text: 'Multiplayer Teams results screen', value: 18 },
  ProcessingBeatmaps: { text: 'Processing beatmaps', value: 19 },
  Tourney: { text: 'Tourney manager', value: 22 },
};

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

let scoresType = {
  0: 'Local',
  1: 'Top',
  2: 'Selected mods',
  3: 'Friends',
  4: 'Country',
  10: '', //Unknown
};

window.overlay = {
  osuStatus,
  osuStatusFriendly,
  rawOsuStatus,
  rawOsuStatusFriendly,
  osuGrade,
  scoresType,
  config,
};

})();


