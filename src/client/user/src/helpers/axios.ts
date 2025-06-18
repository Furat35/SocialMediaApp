import { LoginResponseModel } from '../../../src/shared/models/auth-models/LoginResponseModel'
import axios from 'axios'

let localStorageToken = localStorage.getItem('userInfo')
let accessToken =
  localStorageToken != null ? (JSON.parse(localStorageToken) as LoginResponseModel).accessToken : ''

export default axios.create({
  baseURL: import.meta.env.VITE_GatewayUrl,
  headers: { Authorization: accessToken != '' ? `Bearer ${accessToken}` : '' },
})
