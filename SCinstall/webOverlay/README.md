# StreamCompanion web overlays

This repository contains osu! web overlays, ready to be used with [StreamCompanion][SCRepository].  
StreamCompanion API documentation which these overlays are interacting with can be found [here][api].

## Contributing

Anyone should feel free to contribute changes to existing or entirely new overlays.  
When creating new overlays:

* Use either plain JavaScript or browser([dist][vueGlobal]) version of [Vue.js][vue].
* It is recommended(but not mandatory) to use [SC_Template](./overlays/SC_Template) overlay as a starting point (Vue.js & SC connection pre-configured).
* Overlays which require Node.js/other build tools to be built are discouraged, and should be only used with bigger(logic-wise) overlays.

## License

MIT

   [SCRepository]: <https://github.com/Piotrekol/StreamCompanion/>
   [api]: <https://piotrekol.github.io/StreamCompanion/development/SC/api.html>
   [vueGlobal]: <https://unpkg.com/vue@3.x.x/dist/vue.global.js>
   [vue]: <https://v3.vuejs.org/guide>
