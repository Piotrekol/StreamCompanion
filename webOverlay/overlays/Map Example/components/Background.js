const background = {
    name: 'backgroundContainer',
    template: `
    <div :style="boxStyle">
        <slot />
    </div>
  `,
    data: () => ({
        tokens: { 'backgroundImageLocation': '' },
        backgroundUrl: '',
        backgroundId: Number.MIN_SAFE_INTEGER
    }),
    computed: {
        imageDiskLocation: function () {
            return this.tokens.backgroundImageLocation
        },

        boxStyle: function () {
            return `
            background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0.1), rgba(0, 0, 0, 0.6)),url(${this.backgroundUrl});`
        }
    },
    watch: {
        imageDiskLocation() {
            let t = this;
            var currId = t.backgroundId += 1
            preloadImage(`${window.overlay.config.getUrl()}/backgroundImage?width=700&mapset=${t.tokens.mapsetid}&dummyData=${encodeURIComponent(t.tokens.md5)}`, currId, (url, id) => {
                if (t.backgroundId !== id)
                    return;
                t.backgroundUrl = url;
            })
        }
    },
    created: function () {
        watchTokensVue(['backgroundImageLocation', 'md5', 'mapsetid'], this);
    }
}


export default background