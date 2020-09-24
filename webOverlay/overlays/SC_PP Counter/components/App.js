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

    const getToken = (tokenName, decimalPlaces) =>
      _GetToken(data.rws, data.tokens, tokenName, decimalPlaces);
    let isPlaying = Vue.computed(() => _IsPlaying(data.rws, data.tokens));
    //either request all tokens upfront by filling their names in array
    //or request them later using helper getToken method above
    data.rws = watchTokens([], (values) => {
      Object.assign(data.tokens, values);
    });

    return {
      getToken,
      isPlaying,
    };
  },
};

export default app;
