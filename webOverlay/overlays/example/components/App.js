import background from './Background.js'

const app = {
  name: 'App',
  data: () => ({
    tokens: { }
  }),
  components:{
    Background: background
  },
  computed: {
  },
  methods: {
    getToken: function (tokenName) {
      return this.tokens[tokenName] || '';
    }
  },
  created: function () {
    watchTokensVue(['backgroundImageLocation', 'MapArtistTitle', 'score', 'circles', 'Md5','c300','c100','c50','PpIfMapEndsNow'], this);
  }
}
export default app;