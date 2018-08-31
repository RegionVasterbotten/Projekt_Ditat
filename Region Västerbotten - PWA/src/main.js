import Vue from 'vue'
import App from './App'
import router from './router'
import Vuetify from 'vuetify'
import 'vuetify/dist/vuetify.min.css'
import './css/style.css'
import {
  routerHistory,
  writeHistory
} from 'vue-router-back-button'
import 'vue-loading-overlay/dist/vue-loading.min.css';
import VueResource from 'vue-resource'
import VeeValidate from 'vee-validate';
import VueImg from 'v-img';
import Tabs from 'vue-tabs-component';


Vue.use(Tabs);
Vue.use(VueImg)
Vue.use(VeeValidate);
Vue.use(VueResource);
Vue.http.options.emulateJSON = true;


Vue.use(Vuetify, {
  theme: {
    primary: '#4CAF50',
    secondary: '#b0bec5',
    accent: '#8c9eff',
    error: '#b71c1c'
  }
})
Vue.use(routerHistory)
router.afterEach(writeHistory)

Vue.config.productionTip = false

import { i18n } from './plugins'

import store from './store';
import {
  mapGetters
} from 'vuex';
/* eslint-disable no-new */
new Vue({
  i18n,
  el: '#app',
  store,
  router,
  template: '<App/>',
  components: {
    App
  }
})
