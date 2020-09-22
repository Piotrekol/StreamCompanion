const background = {
    name: 'backgroundContainer',
    template: `
      <div :style="boxStyle">
          <slot />
      </div>
    `,
    setup(props, context) {
      const data = Vue.reactive({
        tokens: { backgroundImageLocation: '', md5: '', mapsetid: '' },
        backgroundUrl: '',
        backgroundId: Number.MIN_SAFE_INTEGER,
        rws: {},
      });
      const boxStyle = Vue.computed(
        () =>
          `background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0.1), rgba(0, 0, 0, 0.6)),url(${data.backgroundUrl});`
      );
  
      Vue.watch(
        () => data.tokens.backgroundImageLocation,
        () => {
          var currId = (data.backgroundId += 1);
          preloadImage(
            `${window.overlay.config.getUrl()}/backgroundImage?width=700&mapset=${
              data.tokens.mapsetid
            }&dummyData=${encodeURIComponent(data.tokens.md5)}`,
            currId,
            (url, id) => {
              if (data.backgroundId !== id) return;
              data.backgroundUrl = url;
            }
          );
        }
      );
      data.rws = watchTokens(
        ['backgroundImageLocation', 'md5', 'mapsetid'],
        (values) => Object.assign(data.tokens, values)
      );
  
      return {
        boxStyle,
      };
    },
  };
  
  export default background;
  