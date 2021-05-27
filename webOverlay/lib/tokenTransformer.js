function transformTokens(tokens) {
  const t = tokens;
  return {
    settings: {
      showInterface: t['ingameInterfaceIsEnabled'] || 0,
    },
    menu: {
      state: t['rawStatus'],
      skinFolder: t['skin'],
      gameMode: t['gameMode'],
      isChatEnabled: t['chatIsEnabled'],
      bm: {
        time: {
          firstObj: t['firstHitObjectTime'],
          current: t['time'] * 1000,
          full: t['totaltime'],
          mp3: t['totaltime'],
        },
        id: t['mapid'],
        set: t['mapsetid'],
        md5: t['md5'],
        rankedStatus: t['rankedStatus'],
        metadata: {
          artist: t['artistRoman'],
          title: t['titleRoman'],
          mapper: t['creator'],
          difficulty: t['diffName'],
        },
        stats: {
          AR: t['mAR'],
          CS: t['mCS'],
          OD: t['mOD'],
          HP: t['mHP'],
          SR: t['liveStarRating'],
          BPM: {
            min: t['minBpm'],
            max: t['maxBpm'],
          },
          fullSR: t['mStars'],
          memoryAR: t['AR'],
          memoryCS: t['CS'],
          memoryOD: t['OD'],
          memoryHP: t['HP'],
        },
        path: {
          full: `${t['dir']}\\${t['backgroundImageFileName']}`,
          folder: t['dir'],
          file: t['osuFileName'],
          bg: t['backgroundImageFileName'],
          audio: t['mp3Name'],
        },
      },
      mods: {
        num: t['modsEnum'],
        str: t['mods'].replace(/,/g, ''),
      },
      pp: {
        100: t['osu_mSSPP'],
        99: t['osu_m99PP'],
        98: t['osu_m98PP'],
        97: t['osu_m97PP'],
        96: t['osu_m96PP'],
        95: t['osu_m95PP'],
        strains: t['mapStrains'],
      },
    },
    gameplay: {
      gameMode: t['gameMode'],
      name: t['username'],
      score: t['score'],
      accuracy: t['acc'],
      combo: {
        current: t['combo'],
        max: t['currentMaxCombo'],
      },
      hp: {
        normal: t['playerHP'],
        smooth: t['playerHP'],
      },
      hits: {
        300: t['c300'],
        geki: t['geki'],
        100: t['c100'],
        katu: t['katsu'],
        50: t['c50'],
        0: t['miss'],
        sliderBreaks: t['sliderBreaks'],
        grade: {
          current: window.overlay.osuGrade[t['grade']],
          maxThisPlay: 'A', //TODO: ?
        },
        unstableRate: t['unstableRate'],
        hitErrorArray: t['hitErrors'],
      },
      pp: {
        current: t['ppIfMapEndsNow'],
        fc: t['noChokePp'],
        maxThisPlay: t['ppIfRestFced'],
      },
      rawKeyOverlay: t['keyOverlay'],
      cachedKeyOverlay: null,
      get keyOverlay() {
        return this.cachedKeyOverlay !== null ? this.cachedKeyOverlay : (this.cachedKeyOverlay = convertSCKeyOverlay(this.rawKeyOverlay));
      },
      rawLeaderboard: t['leaderBoardPlayers'],
      rawLeaderboardMainPlayer: t['leaderBoardMainPlayer'],
      cachedLeaderboard: null,
      get leaderboard() {
        return this.cachedLeaderboard !== null
          ? this.cachedLeaderboard
          : (this.cachedLeaderboard = convertSCLeaderBoard(this.rawLeaderboard, this.rawLeaderboardMainPlayer));
      },
      resultsScreen: {
        300: t['c300'],
        100: t['c100'],
        50: t['c50'],
        0: t['miss'],
        geki: t['geki'],
        katu: t['katsu'],
        name: t['username'],
        score: t['score'],
        maxCombo: t['combo'],
        mods: {
          num: t['modsEnum'],
          str: t['mods'].replace(/,/g, ''),
        },
      },
      //TODO: tourney will have to be done at some other time
      tourney: {
        manager: {
          ipcState: 0,
          bestOF: 0,
          teamName: {
            left: '',
            right: '',
          },
          stars: {
            left: 0,
            right: 0,
          },
          bools: {
            scoreVisible: false,
            starsVisible: false,
          },
          chat: null,
          gameplay: {
            score: {
              left: 0,
              right: 0,
            },
          },
        },
        ipcClients: null,
      },
    },
  };
}

function convertSCKeyOverlay(rawKeyOverlay) {
  let keys = JSON.parse(rawKeyOverlay || '{}');

  return {
    k1: {
      isPressed: keys.K1Pressed,
      count: keys.K1Count,
    },
    k2: {
      isPressed: keys.K2Pressed,
      count: keys.K2Count,
    },
    m1: {
      isPressed: keys.M1Pressed,
      count: keys.M1Count,
    },
    m2: {
      isPressed: keys.M2Pressed,
      count: keys.M2Count,
    },
  };
}

function convertSCLeaderBoard(rawPlayers, rawMainPlayer) {
  let players = JSON.parse(rawPlayers || '[]');
  let mainPlayer = JSON.parse(rawMainPlayer || '{}');

  return {
    hasLeaderboard: players.length > 0,
    isVisible: mainPlayer.IsLeaderboardVisible || false,
    ourplayer: convertSCPlayerSlot(mainPlayer),
    slots: players.map((p) => convertSCPlayerSlot(p)),
  };
}

function convertSCPlayerSlot(player) {
  return {
    name: player.Username,
    score: player.Score,
    combo: player.Combo,
    maxCombo: player.MaxCombo,
    mods: player.Mods ? player.Mods.Value : 0, //TODO: this should be returning mod string instead of enum
    h300: player.Hit300,
    h100: player.Hit100,
    h50: player.Hit50,
    h0: player.HitMiss,
    team: player.Team,
    position: player.Position,
    isPassing: player.IsPassing,
  };
}

function CreateProxiedReconnectingWebSocket(url) {
  const tokensCache = {};

  let proxy = {
    //onopen,
    //onclose,
    //onerror,
    //onmessage,
  };
  const tokenNames = [
    'acc',
    'AR',
    'artistRoman',
    'backgroundImageFileName',
    'c100',
    'c300',
    'c50',
    'chatIsEnabled',
    'combo',
    'creator',
    'CS',
    'currentMaxCombo',
    'diffName',
    'dir',
    'firstHitObjectTime',
    'gameMode',
    'geki',
    'grade',
    'hitErrors',
    'HP',
    'ingameInterfaceIsEnabled',
    'katsu',
    'keyOverlay',
    'leaderBoardMainPlayer',
    'leaderBoardPlayers',
    'liveStarRating',
    'mapid',
    'mapsetid',
    'mapStrains',
    'mAR',
    'maxBpm',
    'mCS',
    'md5',
    'mHP',
    'minBpm',
    'miss',
    'mOD',
    'mods',
    'modsEnum',
    'mp3Name',
    'mStars',
    'noChokePp',
    'OD',
    'osu_m95PP',
    'osu_m96PP',
    'osu_m97PP',
    'osu_m98PP',
    'osu_m99PP',
    'osu_mSSPP',
    'osuFileName',
    'playerHP',
    'ppIfMapEndsNow',
    'ppIfRestFced',
    'rankedStatus',
    'rawStatus',
    'score',
    'skin',
    'sliderBreaks',
    'time',
    'titleRoman',
    'totaltime',
    'unstableRate',
    'username',
  ];

  let rws = watchTokens(tokenNames, (values) => {
    Object.assign(tokensCache, values);
    proxy.onmessage({ data: transformTokens(tokensCache) });
  });

  let origOnOpen = rws.onopen;

  rws.onopen = (ev) => {
    origOnOpen(ev);
    proxy.onopen(ev);
  };
  rws.onclose = (ev) => proxy.onclose(ev);
  rws.onerror = (ev) => proxy.onerror(ev);
  //rws.onmessage = (ev) => proxy.onmessage(ev);

  return proxy;
}
