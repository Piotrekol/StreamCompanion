function transformTokens(tokens) {
  const t = tokens;
  return {
    menu: {
      state: t['rawStatus'],
      skinFolder: t['skin'],
      gameMode: t['gameMode'],
      isChatEnabled: 0, //TODO: missing value in SC
      bm: {
        time: {
          firstObj: 6963, //TODO: expose in SC
          current: 21984, //TODO: expose in SC
          full: t['totaltime'],
          mp3: 269836,
        },
        id: t['mapid'],
        set: t['mapsetid'],
        md5: t['md5'],
        rankedStatus: t['state'],
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
          SR: t['liveStars'],
          BPM: {
            min: t['minBpm'],
            max: t['maxBpm'],
          },
          fullSR: t['mStars'],
          memoryAR: t['mAR'], //TODO: what is diference between these and non memory values above?
          memoryCS: t['mCS'], //^
          memoryOD: t['mOD'], //^
          memoryHP: t['mHP'], //^
        },
        path: {
          full: '509341 REOL - YoiYoi Kokon\\bg.jpg', //TODO: expose image name alone then do dir+'\\'+imagename
          folder: t['dir'],
          file: t['osuFileName'],
          bg: 'bg.jpg', //TODO: expose imagename
          audio: t['mp3Name'],
        },
      },
      mods: {
        num: 16, //TODO: expose raw number value
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
        200: 6,
        geki: t['geki'],
        100: t['c100'],
        katu: t['katu'],
        50: t['c50'],
        0: t['miss'],
        sliderBreaks: t['sliderBreaks'],
        grade: {
          current: osuGrade[t['grade']],
          maxThisPlay: 'A', //TODO: ?
        },
        unstableRate: t['unstableRate'],
        hitErrorArray: t['hitErrors'],
      },
      pp: {
        current: t['ppIfMapEndsNow'],
        fc: t['noChokePp'], //TODO: not sure if this is same value
        maxThisPlay: t['ppIfRestFced'],
      },
      keyOverlay: {
        //TODO: keyOverlay
        k1: {
          isPressed: true,
          count: 5,
        },
        k2: {
          isPressed: false,
          count: 1,
        },
        m1: {
          isPressed: false,
          count: 0,
        },
        m2: {
          isPressed: false,
          count: 0,
        },
      },
      leaderboard: {
        hasLeaderboard: true,
        isVisible: true,
        ourplayer: {
          name: 'Piotrekol',
          score: 1470,
          combo: 6,
          maxCombo: 6,
          mods: 'NM',
          h300: 3,
          h100: 0,
          h50: 0,
          h0: 0,
          team: 0,
          position: 3,
          isPassing: 1,
        },
        slots: [
          {
            name: 'Piotrekol',
            score: 9783580,
            combo: 0,
            maxCombo: 500,
            mods: 'NM',
            h300: 1185,
            h100: 45,
            h50: 1,
            h0: 10,
            team: 0,
            position: 1,
            isPassing: 1,
          },
          {
            name: 'Piotrekol',
            score: 8652960,
            combo: 0,
            maxCombo: 484,
            mods: 'NM',
            h300: 1167,
            h100: 52,
            h50: 0,
            h0: 22,
            team: 0,
            position: 2,
            isPassing: 1,
          },
          {
            name: 'Piotrekol',
            score: 1470,
            combo: 6,
            maxCombo: 6,
            mods: 'NM',
            h300: 3,
            h100: 0,
            h50: 0,
            h0: 0,
            team: 0,
            position: 3,
            isPassing: 1,
          },
        ],
      },
      resultsScreen: {
        300: t['c300'],
        100: t['c100'],
        50: t['c50'],
        0: t['miss'],
        geki: t['geki'],
        katu: t['katu'],
        name: t['username'],
        score: t['score'],
        maxCombo: t['combo'],
        mods: {
          num: 0,
          str: t['mods'].replace(/,/g, ''),
        },
      },
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

function CreateProxiedReconnectingWebSocket(url) {
  const tokensCache = {};

  let proxy = {
    //onopen,
    //onclose,
    //onerror,
    //onmessage,
  };
  const tokenNames = [
    'rawStatus',
    'skin',
    'gameMode',
    'totaltime',
    'mapid',
    'mapsetid',
    'md5',
    'state',
    'artistRoman',
    'titleRoman',
    'creator',
    'diffName',
    'mAR',
    'mCS',
    'mOD',
    'mHP',
    'liveStars',
    'minBpm',
    'maxBpm',
    'mStars',
    'mAR',
    'mCS',
    'mOD',
    'mHP',
    'dir',
    'osuFileName',
    'mp3Name',
    'mods',
    'osu_mSSPP',
    'osu_m99PP',
    'osu_m98PP',
    'osu_m95PP',
    'mapStrains',
    'gameMode',
    'username',
    'score',
    'acc',
    'combo',
    'maxCombo',
    'playerHP',
    'playerHP',
    'c300',
    'geki',
    'c100',
    'katu',
    'c50',
    'miss',
    'sliderBreaks',
    'grade',
    'unstableRate',
    'hitErrors',
    'ppIfMapEndsNow',
    'noChokePp',
    'ppIfRestFced',
  ];
  let rws = watchTokens(tokenNames, (values) => {
    Object.assign(tokensCache, values);
    proxy.onmessage({ data: JSON.stringify(transformTokens(tokensCache)) });
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
