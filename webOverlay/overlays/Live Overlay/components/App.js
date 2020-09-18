import background from './Background.js'
import lineChart from './LineChart.js'

const app = {
  name: 'App',
  data: () => ({
    tokens: {},
    rws: {},
    chart: {},
    settings: {},

  }),
  components: {
    Background: background,
    Linechart: lineChart,
  },
  computed: {
    totalTime() {
      let time = this.getToken('totaltime');
      return Math.round((time / 1000) / 60).pad() + ":" + Math.round((time / 1000) % 60).pad();
    },
    isPlaying() { return _IsPlaying(this.rws, this.tokens) },
    mapStrains() { return Object.entries(this.tokens.mapStrains || {}); },
    ppValue() {
      if (this.isPlaying)
        return this.getToken('ppIfMapEndsNow', 1)
      if (this.overlaySettings.simulatePPWhenListening)
        return this.getToken('simulatedPp', 1)
      return 0;
    },
    mapProgress() {
      return this.getToken('time') / (this.getToken('totaltime') / 1000);
    },
    overlaySettings() {
      if (Object.keys(this.settings).length === 0)
        return {};
      let s = this.settings;

      return {
        backgroundColor: this.convertSettingsColor(s.ChartColor),
        chartProgressColor: this.convertSettingsColor(s.ChartProgressColor),
        imageDimColor: this.convertSettingsColor(s.ImageDimColor),
        artistTextColor: this.convertSettingsColor(s.ArtistTextColor),
        titleTextColor: this.convertSettingsColor(s.TitleTextColor),
        ppBackgroundColor: this.convertSettingsColor(s.PpBackgroundColor),
        hit100BackgroundColor: this.convertSettingsColor(s.Hit100BackgroundColor),
        hit50BackgroundColor: this.convertSettingsColor(s.Hit50BackgroundColor),
        hitMissBackgroundColor: this.convertSettingsColor(s.HitMissBackgroundColor),
        yAxesFontColor: s.HideChartLegend ? 'transparent' : 'white',

        simulatePPWhenListening: s.SimulatePPWhenListening,
        hideDiffText: s.HideDiffText,
        hideMapStats: s.HideMapStats,
        hideChartLegend: s.HideChartLegend,

        chartHeight: s.ChartHeight
      };
    },
    progressChartSettings() {
      return {
        backgroundColor: this.overlaySettings.chartProgressColor,
        yAxesFontColor: 'transparent'
      }
    },
    chartStyle() {
      if (Object.keys(this.overlaySettings).length === 0)
        return `height:200px`;
      return `height:${this.overlaySettings.chartHeight}px;`
    },
    progressChartStyle() {
      return `clip-path: inset(0px ${100 - this.mapProgress * 100}% 0px 0px);`
    },
    hitsStyle() {
      if (!this.overlaySettings.ppBackgroundColor)
        return ``;

      let { ppBackgroundColor: pp, hit100BackgroundColor: h100,
        hit50BackgroundColor: h50, hitMissBackgroundColor: hMiss } = this.overlaySettings;
      return `background: linear-gradient(to right, ${pp},${pp},${h100},${h100},${h50},${h50},${hMiss},${hMiss});`;
    }
  },
  methods: {
    getToken: function (tokenName, decimalPlaces) { return _GetToken(this.rws, this.tokens, tokenName, decimalPlaces) },
    convertSettingsColor(color) {
      //#argb => #rgba
      return color;//`#${color.slice(3)}${color.slice(1, 3)}`;
    },
    getWebOverlaySettings() {
      let t = this;
      return fetch(`${config.getUrl()}/settings`)
        .then(response => response.json())
        .then(data => {
          if (data.WebOverlay_Config)
            mergeObjects(t, t.settings, JSON.parse(data.WebOverlay_Config));
          else
            t.settings = {}
        });
    }
  },
  created: function () {
    //either request all tokens upfront
    this.rws = watchTokensVue(['mapStrains'], this);
    //or request them later using helper getToken method above
    //this.rws = watchTokensVue([], this);
    let t = this;

    this.getWebOverlaySettings().then(() => {
      if (t.overlaySettings.EditMode)
        setInterval(t.getWebOverlaySettings, 500)
    });
  }
}
export default app;