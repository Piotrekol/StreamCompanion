function GetToken(tokenName,tokens,rws){
  if(tokens.hasOwnProperty(tokenName))
    return tokens[tokenName];
  
  rws.AddToken(tokenName);
  return '';
}
function transformTokens(tokens, rws) {
  const t = tokens;
  
  return {
    settings: {
      get showInterface(){ return GetToken('ingameInterfaceIsEnabled',tokens,rws)},
    },
    menu: {
      get state(){ return GetToken('rawStatus',tokens,rws)},
      get skinFolder(){ return GetToken('skin',tokens,rws)},
      get gameMode(){ return GetToken('gameMode',tokens,rws)},
      get isChatEnabled(){ return GetToken('chatIsEnabled',tokens,rws)},
      bm: {
        time: {
          get firstObj(){ return GetToken('firstHitObjectTime',tokens,rws)},
          get current(){ return GetToken('time',tokens,rws) * 1000},
          get full(){ return GetToken('totaltime',tokens,rws)},
          get mp3(){ return GetToken('totalAudioTime',tokens,rws)},
        },
        get id(){ return GetToken('mapid',tokens,rws)},
        get set(){ return GetToken('mapsetid',tokens,rws)},
        get md5(){ return GetToken('md5',tokens,rws)},
        get rankedStatus(){ return GetToken('rankedStatus',tokens,rws)},
        metadata: {
          get artist(){ return GetToken('artistRoman',tokens,rws)},
          get title(){ return GetToken('titleRoman',tokens,rws)},
          get mapper(){ return GetToken('creator',tokens,rws)},
          get difficulty(){ return GetToken('diffName',tokens,rws)},
        },
        stats: {
          get AR(){ return GetToken('mAR',tokens,rws)},
          get CS(){ return GetToken('mCS',tokens,rws)},
          get OD(){ return GetToken('mOD',tokens,rws)},
          get HP(){ return GetToken('mHP',tokens,rws)},
          get SR(){ return GetToken('liveStarRating',tokens,rws)},
          BPM: {
            get min(){ return GetToken('minBpm',tokens,rws)},
            get max(){ return GetToken('maxBpm',tokens,rws)},
          },
          get fullSR(){ return GetToken('mStars',tokens,rws)},
          get memoryAR(){ return GetToken('AR',tokens,rws)},
          get memoryCS(){ return GetToken('CS',tokens,rws)},
          get memoryOD(){ return GetToken('OD',tokens,rws)},
          get memoryHP(){ return GetToken('HP',tokens,rws)},
        },
        path: {
          get full(){ return GetToken('dir',tokens,rws)+'\\'+GetToken('backgroundImageFileName',tokens,rws)},
          get folder(){ return GetToken('dir',tokens,rws)},
          get file(){ return GetToken('osuFileName',tokens,rws)},
          get bg(){ return GetToken('backgroundImageFileName',tokens,rws)},
          get audio(){ return GetToken('mp3Name',tokens,rws)},
        },
      },
      mods: {
        get num(){ return GetToken('modsEnum',tokens,rws)},
        get str(){ let mods = GetToken('mods',tokens,rws); return mods === 'None' ? 'NM' : mods.replace(/,/g, '').replace(/SV2/g, 'v2')},
      },
      pp: {
        get 100(){ return GetToken('osu_mSSPP',tokens,rws)},
        get 99(){ return GetToken('osu_m99PP',tokens,rws)},
        get 98(){ return GetToken('osu_m98PP',tokens,rws)},
        get 97(){ return GetToken('osu_m97PP',tokens,rws)},
        get 96(){ return GetToken('osu_m96PP',tokens,rws)},
        get 95(){ return GetToken('osu_m95PP',tokens,rws)},
        get strains(){ return GetToken('mapStrains',tokens,rws)},
      },
    },
    gameplay: {
      get gameMode(){ return GetToken('gameMode',tokens,rws)},
      get name(){ return GetToken('username',tokens,rws)},
      get score(){ return GetToken('score',tokens,rws)},
      get accuracy(){ return GetToken('acc',tokens,rws)},
      combo: {
        get current(){ return GetToken('combo',tokens,rws)},
        get max(){ return GetToken('currentMaxCombo',tokens,rws)},
      },
      hp: {
        get normal(){ return GetToken('playerHp',tokens,rws)},
        get smooth(){ return GetToken('playerHpSmooth',tokens,rws)},
      },
      hits: {
        get 300(){ return GetToken('c300',tokens,rws)},
        get geki(){ return GetToken('geki',tokens,rws)},
        get 100(){ return GetToken('c100',tokens,rws)},
        get katu(){ return GetToken('katsu',tokens,rws)},
        get 50(){ return GetToken('c50',tokens,rws)},
        get 0(){ return GetToken('miss',tokens,rws)},
        get sliderBreaks(){ return GetToken('sliderBreaks',tokens,rws)},
        grade: {
          get current(){ return window.overlay.osuGrade[GetToken('grade',tokens,rws)]},
          get maxThisPlay(){ return window.overlay.osuGrade[GetToken('maxGrade',tokens,rws)]},
        },
        get unstableRate(){ return GetToken('unstableRate',tokens,rws)},
        get hitErrorArray(){ return GetToken('hitErrors',tokens,rws)},
      },
      pp: {
        get current(){ return GetToken('ppIfMapEndsNow',tokens,rws)},
        get fc(){ return GetToken('noChokePp',tokens,rws)},
        get maxThisPlay(){ return GetToken('ppIfRestFced',tokens,rws)},
      },
      get rawKeyOverlay(){ return GetToken('keyOverlay',tokens,rws)},
      cachedKeyOverlay: null,
      get keyOverlay() {
        return this.cachedKeyOverlay !== null ? this.cachedKeyOverlay : (this.cachedKeyOverlay = convertSCKeyOverlay(this.rawKeyOverlay));
      },
      get rawLeaderboard(){ return GetToken('leaderBoardPlayers',tokens,rws)},
      get rawLeaderboardMainPlayer(){ return GetToken('leaderBoardMainPlayer',tokens,rws)},
      cachedLeaderboard: null,
      get leaderboard() {
        return this.cachedLeaderboard !== null
          ? this.cachedLeaderboard
          : (this.cachedLeaderboard = convertSCLeaderBoard(this.rawLeaderboard, this.rawLeaderboardMainPlayer));
      },
      resultsScreen: {
        get 300(){ return GetToken('c300',tokens,rws)},
        get 100(){ return GetToken('c100',tokens,rws)},
        get 50(){ return GetToken('c50',tokens,rws)},
        get 0(){ return GetToken('miss',tokens,rws)},
        get geki(){ return GetToken('geki',tokens,rws)},
        get katu(){ return GetToken('katsu',tokens,rws)},
        get name(){ return GetToken('username',tokens,rws)},
        get score(){ return GetToken('score',tokens,rws)},
        get maxCombo(){ return GetToken('combo',tokens,rws)},
        mods: {
          get num(){ return GetToken('modsEnum',tokens,rws)},
          get str(){ return GetToken('mods',tokens,rws).replace(/,/g, '')},
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
function ApplyConstChanges(){
  //gosu doesn't provide HD/FL info in grades
  window.overlay.osuGrade[0] = `SS`;
  window.overlay.osuGrade[1] = `S`;
}
function CreateProxiedReconnectingWebSocket(url) {
  ApplyConstChanges();
  const tokensCache = {};

  let proxy = {
    //onopen,
    //onclose,
    //onerror,
    //onmessage,
  };

  //Requesting only basic tokens to bootstrap everything on first map event
  const tokenNames = [
    'md5',
    'modsEnum',
    'gameMode',
    'mapid',
    'mapsetid',
    'username',
  ];

  let rws = watchTokens(tokenNames, (values) => {
    Object.assign(tokensCache, values);
    proxy.onmessage({ data: transformTokens(tokensCache, rws) });
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
