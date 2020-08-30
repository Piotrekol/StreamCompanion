import background from './Background.js'

const app = {
  name: 'App',
  data: () => ({
    tokens: {},
    rws: {},
  }),
  components: {
    Background: background
  },
  computed: {
  },
  methods: {
    getToken: function (tokenName,decimalPlaces) { return _GetToken(this.rws, this.tokens, tokenName,decimalPlaces) }
  },
  created: function () {
    //either request all tokens upfront
    //this.rws = watchTokensVue(['backgroundImageLocation', 'MapArtistTitle', 'score', 'circles', 'Md5','c300','c100','c50','PpIfMapEndsNow'], this);
    //or request them later using helper getToken method above
    this.rws = watchTokensVue([], this);
  }
}
export default app;