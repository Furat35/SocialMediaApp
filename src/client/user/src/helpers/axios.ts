import { LoginResponseModel } from '../../../src/shared/models/auth-models/LoginResponseModel'
import axios from 'axios'
import { useUserStore } from '../helpers/store'

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

export { instance as axios }
