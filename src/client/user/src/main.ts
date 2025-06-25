import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { axios } from './helpers/axios'
import eventBus from './helpers/event-bus'
import Toast, { PluginOptions } from 'vue-toastification'
import 'vue-toastification/dist/index.css'
import { createPinia } from 'pinia'
import piniaPersist from 'pinia-plugin-persistedstate'

const app = createApp(App)
app.config.globalProperties.$axios = axios
app.config.globalProperties.$bus = eventBus

app.use(router)
const pinia = createPinia()
pinia.use(piniaPersist)
app.use(pinia)

const options: PluginOptions = { timeout: 2000 }
app.use(Toast, options)

app.mount('#app')
