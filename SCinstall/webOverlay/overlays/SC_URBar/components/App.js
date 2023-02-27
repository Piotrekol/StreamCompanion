import roundurbar from './roundURBar.js';

const app = {
  name: 'App',
  components: { roundurbar },
  // https://v3.vuejs.org/guide/composition-api-introduction.html#basics-of-composition-api
  setup(props, context) {
    const data = Vue.reactive({
      tokens: {},
      rws: {},
    });
    //map global _GetToken helper method
    const getToken = (tokenName, decimalPlaces) => _GetToken(data.rws, data.tokens, tokenName, decimalPlaces);

    //start websocket connection to SC with some predefined tokens
    data.rws = watchTokens([], (values) => {
      Object.assign(data.tokens, values);
    });

    //return all data & computed vars & methods that we want to use elsewhere in this component
    return {
      getToken,
    };
  },
};

export default app;
