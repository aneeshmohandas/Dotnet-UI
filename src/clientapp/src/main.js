import Vue from "vue";
import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import router from "./router";
import "vuetify/dist/vuetify.min.css";
import axios from "axios";
import appHub from "./appHub";
import dayjs from "dayjs";
import velocity      from 'velocity-animate'
var relativeTime = require('dayjs/plugin/relativeTime')
dayjs.extend(relativeTime)

// import "xterm/css/xterm.css";
// import "xterm/lib/xterm.js";
import Notifications from "vue-notification";

import store from './store'
Vue.use(Notifications, { velocity });

var siteName = "http://localhost:3000";
// var siteName = "";
Vue.prototype.$httpClient = axios;
Vue.prototype.$siteName = siteName;
Vue.prototype.$dayjs = dayjs;

Vue.config.productionTip = false;
Vue.use(appHub);
new Vue({
  vuetify,
  router,
  store,
  render: (h) => h(App)
}).$mount("#app");
