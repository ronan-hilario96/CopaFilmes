import Vue from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify';
import store from './store'
import axios from 'axios'
import router from './router'

Vue.use(axios)

Vue.config.productionTip = false
new Vue({
  vuetify,
  axios,
  store,
  router,
  render: h => h(App)
}).$mount('#app')
