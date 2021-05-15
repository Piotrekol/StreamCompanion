import background from './Background.js';

const app = {
  name: 'App',
  components: {
    Background: background,
  },
  setup(props, context) {
    const data = Vue.reactive({
      tokens: {},
      rws: {},
    });

    const getToken = (tokenName, decimalPlaces) => _GetToken(data.rws, data.tokens, tokenName, decimalPlaces);
    let isMania = Vue.computed(() => getToken('gameMode') === 'OsuMania');
    let isPlayingOrWatching = Vue.computed(() =>
      _IsInStatus(data.rws, data.tokens, [window.overlay.osuStatus.Playing, window.overlay.osuStatus.ResultsScreen, window.overlay.osuStatus.Watching])
    );

    data.rws = watchTokens([], (values) => {
      Object.assign(data.tokens, values);
    });

    let mapTime = Vue.computed(() => {
      let time = getToken('time') * 1000;
      return Math.floor(time / 1000 / 60).pad() + ':' + Math.floor((time / 1000) % 60).pad();
    });

    let mapTimePercent = Vue.computed(() => ((getToken('time') / (getToken('totaltime') / 1000)) * 100).clamp(0, 100));

    const radius = 30;
    const circumference = radius * Math.PI * 2;
    let progressStyle = Vue.computed(() => `stroke-dashoffset: ${(1 - mapTimePercent.value / 100) * circumference}px`);

    return {
      getToken,
      isPlayingOrWatching,
      isMania,
      mapTime,
      mapTimePercent,
      progressStyle,
      osuGrade: window.overlay.osuGrade,
    };
  },
};

export default app;
