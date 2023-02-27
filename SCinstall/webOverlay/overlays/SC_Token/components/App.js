const app = {
  name: 'App',
  components: {},
  // https://v3.vuejs.org/guide/composition-api-introduction.html#basics-of-composition-api
  setup(props, context) {
    const data = Vue.reactive({
      tokens: {},
      rws: {},
    });
    //map global _GetToken helper method
    const getToken = (tokenName, decimalPlaces) => _GetToken(data.rws, data.tokens, tokenName, decimalPlaces);

    const urlParams = new URLSearchParams(window.location.search);
    const tokenName = urlParams.get('token');
    const decimals = urlParams.get('decimals');
    
    console.log(urlParams.get('token'));
    //start websocket connection to SC
    data.rws = watchTokens([], (values) => {
      Object.assign(data.tokens, values);
    });
    const baseUrl= window.location.origin+window.location.pathname+"?token=";

    const createExample = (text,token,decimals)=>{
      return {
        text: text,
        token: token,
        decimals: decimals,
        url: `${baseUrl}${token}`+(decimals ? `&decimals=${decimals}` : ``)
      };
    };

    //return all data & computed vars & methods that we want to use elsewhere in this component
    return {
      getToken,
      tokenName,
      decimals,
      baseUrl,
      examples:[
        createExample('text token:','mapArtistTitle'),
        createExample('text token:','mapArtistTitleUnicode'),
        createExample('numeric token(map AR):','mAR'),
        createExample('numeric token, with defined amount of decimals(map AR):','mAR',2),
        createExample('numeric token(map AR):','mAR'),
      ]
    };
  },
};

export default app;
