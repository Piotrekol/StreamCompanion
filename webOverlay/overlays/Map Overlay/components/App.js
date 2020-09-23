import background from './Background.js';

const app = {
  name: 'App',
  components: {
    Background: background,
  },
  // https://v3.vuejs.org/guide/composition-api-introduction.html#basics-of-composition-api
  setup(props, context) {
    const data = Vue.reactive({
      tokens: {},
      rws: {},
    });
    //map global _GetToken helper method
    const getToken = (tokenName, decimalPlaces) => _GetToken(data.rws, data.tokens, tokenName, decimalPlaces);

    //use helper _IsPlaying method to update isPlaying value as necessary
    let isPlaying = Vue.computed(() => _IsPlaying(data.rws, data.tokens));
    
    //map pass percentage 
    let mapProgress = Vue.computed(
      () => ((getToken('time') / (getToken('totaltime') / 1000))*100).clamp(0,100)
    );
    //start websocket connection to SC with some predefined tokens
    data.rws = watchTokens(
      [
        'mapStrains',
        'mapArtistTitle',
        'creator',
        'diffName',
        'mStars',
        'mCS',
        'mAR',
        'mOD',
        'mHP',
        'mBpm',
        'mods',
        'time',
        'totaltime',
        'status',
        'c100',
        'c50',
        'miss',
        'mapsetid',
        'status',
        'md5',
      ],
      (values) => {
        Object.assign(data.tokens, values);
      }
    );

    //return all data & computed vars & methods that we want to use elsewhere in this component
    return {
      data: data.tokens,
      mapProgress,
      getToken,
      isPlaying,
    };
  },
};

export default app;
