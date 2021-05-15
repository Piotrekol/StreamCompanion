import background from './Background.js';
import lineChart from './LineChart.js';

const app = {
  name: 'App',
  components: {
    Background: background,
    Linechart: lineChart,
  },

  setup(props, context) {
    const data = Vue.reactive({
      tokens: {},
      rws: {},
      settings: {},
    });

    const getToken = (tokenName, decimalPlaces) => _GetToken(data.rws, data.tokens, tokenName, decimalPlaces);
    //either request all tokens upfront by filling their names in array
    //or request them later using helper getToken method above
    data.rws = watchTokens(['mapStrains'], (values) => {
      Object.assign(data.tokens, values);
    });

    const getWebOverlaySettings = () =>
      fetch(`${window.overlay.config.getUrl()}/settings`)
        .then((response) => response.json())
        .then((responseData) => JSON.parse(responseData.WebOverlay_Config));

    getWebOverlaySettings().then((config) => {
      Object.assign(data.settings, config);
    });
    let mapStrains = Vue.computed(() => Object.entries(data.tokens.mapStrains || {}));
    let isMania = Vue.computed(() => getToken('gameMode') === 'OsuMania');
    let isPlayingOrWatching = Vue.computed(() =>
      _IsInStatus(data.rws, data.tokens, [window.overlay.osuStatus.Playing, window.overlay.osuStatus.ResultsScreen, window.overlay.osuStatus.Watching])
    );

    let ppValue = Vue.computed(() => {
      if (isPlayingOrWatching.value) return getToken('ppIfMapEndsNow', 1);
      if (data.settings.SimulatePPWhenListening) return getToken('simulatedPp', 1);
      return 0;
    });
    let mapProgress = Vue.computed(() => getToken('time') / (getToken('totaltime') / 1000));

    return {
      getToken,

      data,

      isPlayingOrWatching,
      isMania,
      mapStrains,
      ppValue,
      mapProgress,
    };
  },
  computed: {
    overlaySettings() {
      if (Object.keys(this.data.settings).length === 0) return {};
      let s = this.data.settings;

      return {
        backgroundColor: s.ChartColor,
        chartProgressColor: s.ChartProgressColor,
        imageDimColor: s.ImageDimColor,
        artistTextColor: s.ArtistTextColor,
        titleTextColor: s.TitleTextColor,
        ppBackgroundColor: s.PpBackgroundColor,
        hit100BackgroundColor: s.Hit100BackgroundColor,
        hit50BackgroundColor: s.Hit50BackgroundColor,
        hitMissBackgroundColor: s.HitMissBackgroundColor,
        yAxesFontColor: s.HideChartLegend ? 'transparent' : 'white',

        simulatePPWhenListening: s.SimulatePPWhenListening,
        hideDiffText: s.HideDiffText,
        hideMapStats: s.HideMapStats,
        hideChartLegend: s.HideChartLegend,

        chartHeight: s.ChartHeight,
      };
    },
    progressChartSettings() {
      return {
        backgroundColor: this.overlaySettings.chartProgressColor,
        yAxesFontColor: 'transparent',
      };
    },
    chartStyle() {
      if (Object.keys(this.overlaySettings).length === 0) return `height:200px`;
      return `height:${this.overlaySettings.chartHeight}px;`;
    },
    progressChartStyle() {
      return `clip-path: inset(0px ${100 - this.mapProgress * 100}% 0px 0px);`;
    },
    hitsStyle() {
      if (!this.overlaySettings.ppBackgroundColor) return ``;

      let { ppBackgroundColor: pp, hit100BackgroundColor: h100, hit50BackgroundColor: h50, hitMissBackgroundColor: hMiss } = this.overlaySettings;
      return `background: linear-gradient(to right, ${pp},${pp},${h100},${h100},${h50},${h50},${hMiss},${hMiss});`;
    },
  },
};
export default app;
