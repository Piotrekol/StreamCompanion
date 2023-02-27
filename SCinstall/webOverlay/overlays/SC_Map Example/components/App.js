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
    //either request all tokens upfront by filling their names in array
    //or request them later using helper getToken method above
    data.rws = watchTokens([], (values) => {
      Object.assign(data.tokens, values);
    });
    
    const totalTime = Vue.computed(() => {
      let time = getToken('totaltime');
      return (
        Math.floor(time / 1000 / 60).pad() +
        ':' +
        Math.floor((time / 1000) % 60).pad()
      );
    });

    return {
      getToken,

      totalTime,
    };
  },
};

export default app;
