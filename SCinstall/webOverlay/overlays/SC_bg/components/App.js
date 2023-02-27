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
      settings: {},
    });

    const getToken = (tokenName, decimalPlaces) => _GetToken(data.rws, data.tokens, tokenName, decimalPlaces);
    data.rws = watchTokens([], (values) => {
      Object.assign(data.tokens, values);
    });

    return {
      getToken,
      data,
    };
  }
};
export default app;
