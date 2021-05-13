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
        str: t['mods'].replace(/,/g,''),
      },
      pp: {
        100: t['osu_mSSPP'],
        99: t['osu_m99PP'],
        98: t['osu_m98PP'],
        97: 404, //TODO: add to pp calc
        96: 404, //TODO: add to pp calc
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
        max: t['maxCombo'],
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
      leaderboard: {
        hasLeaderboard: true,
        ourplayer: {
          name: 'Vaxei 2',
          score: 120917,
          combo: 86,
          maxCombo: 86,
          mods: 'HR',
          h300: 45,
          h100: 11,
          h50: 0,
          h0: 0,
          team: 0, //0 - solo, 1 OR 2 is BLUE/RED
          position: 51,
          isPassing: 1,
        },
        slots: [
          {
            //gameplay leaderboard slots. Score order
            name: 'Exarch',
            score: 54862276,
            combo: 0, //only visible in multiplayer or ourplayer in solo
            maxCombo: 1811,
            mods: 'HDHR',
            h300: 1115,
            h100: 25,
            h50: 0,
            h0: 0,
            team: 0,
            position: 1,
            isPassing: 1,
          },
          {
            name: '_Criller',
            score: 52751571,
            combo: 0,
            maxCombo: 1814,
            mods: 'HD',
            h300: 1140,
            h100: 0,
            h50: 0,
            h0: 0,
            team: 0,
            position: 2,
            isPassing: 1,
          },
          {
            name: 'Vaxei 2',
            score: 120917,
            combo: 86,
            maxCombo: 86,
            mods: 'HR',
            h300: 45,
            h100: 11,
            h50: 0,
            h0: 0,
            team: 0,
            position: 51,
            isPassing: 1,
          },
        ],
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
  const tokenNames = ['rawStatus','skin','gameMode','totaltime','mapid','mapsetid','md5','state','artistRoman','titleRoman','creator','diffName','mAR','mCS','mOD','mHP','liveStars','minBpm','maxBpm','mStars','mAR','mCS','mOD','mHP','dir','osuFileName','mp3Name','mods','osu_mSSPP','osu_m99PP','osu_m98PP','osu_m95PP','mapStrains','gameMode','username','score','acc','combo','maxCombo','playerHP','playerHP','c300','geki','c100','katu','c50','miss','sliderBreaks','grade','unstableRate','hitErrors','ppIfMapEndsNow','noChokePp','ppIfRestFced'];
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
