import axios from 'axios'
import { useUserStore } from '../helpers/store'
import router from '../router'
import { toast } from './toast'

// let accessToken =
//   localStorageToken != null ? (JSON.parse(localStorageToken) as LoginResponseModel).accessToken : ''

const instance = axios.create({
  baseURL: import.meta.env.VITE_GatewayUrl,
  // headers: { Authorization: `Bearer ${useUserStore().getAccessToken}` },
})

instance.interceptors.request.use((config) => {
  const userStore = useUserStore()
  const token = userStore.getAccessToken

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

instance.interceptors.response.use(
  (response) => response,
  (err) => {
    if (err.response && err.response.status === 401) {
      router.push({ name: 'login' })
      toast.warning(`Unauthenticated user!`)
    }

    return Promise.reject(err)
  },
)

export { instance as axios }
