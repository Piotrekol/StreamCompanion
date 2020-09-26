import background from './Background.js';

const app = {
  name: 'App',
  components: {
    Background: background,
  },
  // https://v3.vuejs.org/guide/composition-api-introduction.html#basics-of-composition-api
  setup(props, context) {
    const data = Vue.reactive({
      tokens: {},
      rws: {},
      settings: {
        progressColor: 'yellow',
      },
    });
    //map global _GetToken helper method
    const getToken = (tokenName, decimalPlaces) => _GetToken(data.rws, data.tokens, tokenName, decimalPlaces);

    //use helper _IsInStatus method to update isPlayingOrWatching value as necessary
    let isPlayingOrWatching = Vue.computed(() =>
      _IsInStatus(data.rws, data.tokens, [window.overlay.osuStatus.Playing, window.overlay.osuStatus.ResultsScreen, window.overlay.osuStatus.Watching])
    );

    //map pass percentage
    let mapProgress = Vue.computed(() => ((getToken('time') / (getToken('totaltime') / 1000)) * 100).clamp(0, 100));

    _GetWebOverlaySettings().then((config) => {
      if (config.ChartProgressColor) data.settings.progressColor = config.ChartProgressColor;
    });

    //start websocket connection to SC with some predefined tokens
    data.rws = watchTokens(
      [
        'mapStrains',
        'mapArtistTitle',
        'creator',
        'diffName',
        'mStars',
        'mCS',
        'mAR',
        'mOD',
        'mHP',
        'mBpm',
        'mods',
        'time',
        'totaltime',
        'status',
        'c100',
        'c50',
        'miss',
        'mapsetid',
        'status',
        'md5',
      ],
      (values) => {
        Object.assign(data.tokens, values);
      }
    );

    //return all data & computed vars & methods that we want to use elsewhere in this component
    return {
      data: data.tokens,
      getToken,
      isPlayingOrWatching,
      mapProgress,
      progressColor: Vue.computed(() => data.settings.progressColor),
      
    };
  },
};

export default app;
