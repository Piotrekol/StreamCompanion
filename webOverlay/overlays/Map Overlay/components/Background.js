const background = {
  //unique name for this component
  name: 'backgroundContainer',
  //"html" to render instead of original tag (<Background></Background> in this case).
  //Normally this would be in separate template tag(vue SFC) but because we use no build tools we have to use inline string.
  template: `
    <div :style="boxStyle" class="background">
        <slot />
    </div>
  `,
  // https://v3.vuejs.org/guide/composition-api-introduction.html#basics-of-composition-api
  setup(props, context) {
    const data = Vue.reactive({
      tokens: { backgroundImageLocation: '', md5: '', mapsetid: '' },
      backgroundUrl: '',
      backgroundId: Number.MIN_SAFE_INTEGER,
      rws: {},
    });

    //function with will automatically monitor all dependant variables for changes and update its value whenever they change
    //note that these variables have to be reactive(Vue.reactive)/refs(Vue.ref)
    const boxStyle = Vue.computed(() => `background-image:url(${data.backgroundUrl});`);

    //we want to watch and trigger a function whenever data.tokens.backgroundImageLocation changes
    Vue.watch(
      //function returning value with should be watched for changes
      () => data.tokens.backgroundImageLocation,
      //method to execute when value defined above changes
      () => {
        var currId = (data.backgroundId += 1);
        preloadImage(
          `${window.overlay.config.getUrl()}/backgroundImage?width=1000&height=300&mapset=${data.tokens.mapsetid}&dummyData=${encodeURIComponent(data.tokens.md5)}&crop=true`,
          currId,
          (url, id) => {
            if (data.backgroundId !== id) return;
            data.backgroundUrl = url;
          }
        );
      }
    );

    //start websocket connection to SC with some predefined tokens
    data.rws = watchTokens(['backgroundImageLocation', 'md5', 'mapsetid'], (values) => Object.assign(data.tokens, values));

    //return all data & computed vars & methods that we want to use elsewhere in this file or in html template
    return {
      boxStyle,
    };
  },
};

export default background;
